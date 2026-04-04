using Dapper;
using DBMigrationProject.Classes;
using DBMigrationProject.Service;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace DBMigrationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentDBController(IConfiguration configuration,
        ILogger<CurrentDBController> logger,
        CommonMethods com) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<CurrentDBController> _logger = logger;
        private readonly CommonMethods _com = com;
        private string _currentDBConnection { get => _configuration.GetValue<string>("ConnectionStrings:CurrentDBConnection"); }

        [HttpGet]
        public IActionResult GetDBName()
        {
            if (string.IsNullOrEmpty(_currentDBConnection))
            {
                return NotFound("CurrentDB configuration is not set.");
            }
            return Ok(_currentDBConnection);
        }

        [HttpGet]
        [Route("test-connection")]
        public async Task<IActionResult> Test()
        {
            try
            {
                using var _dbContext = new OracleConnection(_currentDBConnection);
                await _dbContext.OpenAsync();
                return Ok("Connected");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error connecting to CurrentDB: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("export-db")]
        public async Task<IActionResult> ExportDB()
        {
            try
            {
                List<TableInfo> tables = new List<TableInfo>();

                using var _dbContext = new OracleConnection(_currentDBConnection);
                await _dbContext.OpenAsync();

                // get all the available tables in the current database except the ones with BAK, TEST, TMP and __EFMigrationsHistory in their names
                string Tablequery = "SELECT table_name" +
                    "  FROM user_tables" +
                    " WHERE table_name NOT LIKE '%BAK%'" +
                    "   AND table_name NOT LIKE '__EFMigrationsHistory'" +
                    "   AND table_name NOT LIKE '%TEST%'" +
                    "   AND table_name NOT LIKE '%TMP%'" +
                    "   AND table_name NOT LIKE '%TEMP%'" +
                    "   AND table_name NOT LIKE '%OLD%'" +
                    "   AND table_name NOT LIKE '%AUDIT%'" +
                    " ORDER BY table_name";

                var tableName = await _dbContext.QueryAsync<TabelClass>(Tablequery);

                _logger.LogInformation($"Total tables found: {tableName.Count()}");
                int i = 0;
                // now get the columns & contraint for each table and add them to the list of tables
                int errorId = 0;
                foreach (var tn in tableName)
                {
                    i = i + 1;
                    TableInfo tableInfo = new TableInfo
                    {
                        Id = i,
                        TableName = tn.table_name,
                        Columns = new List<ColumnInfo>(),
                        Constraints = new List<ConstraintInfo>()
                    };
                    _logger.LogInformation($"-------- Start Process for Table: {tableInfo.TableName} -------- Time : {DateTimeOffset.Now}");

                    #region get columns for the table
                    string ColumnQuery = "SELECT column_name, data_type, data_length, nullable, data_default" +
                        "  FROM user_tab_columns t" +
                        $" WHERE table_name = '{tn.table_name}'" +
                        " ORDER BY table_name, column_id";

                    var tablesColums = await _dbContext.QueryAsync<ColumnClass>(ColumnQuery);

                    _logger.LogInformation($"{i}.1 Total columns found for table {tn.table_name}: {tablesColums.Count()}");
                    _logger.LogInformation($"{i}.2 Mapping columns for table {tn.table_name} to ColumnInfo objects");
                    foreach (var tc in tablesColums)
                    {
                        tableInfo.Columns.Add(new ColumnInfo
                        {
                            ColumnName = tc.column_name,
                            DataType = tc.data_type,
                            DataLength = tc.data_length,
                            IsNullable = tc.nullable == "Y" ? true : false,
                            DefaultValue = tc.data_default
                        });
                    }
                    _logger.LogInformation($"{i}.3 Columns for table {tn.table_name} imported successfully. Total columns: {tablesColums.Count()}");
                    #endregion

                    _logger.LogInformation($"{i}.4 Now fetching constraints for table {tn.table_name}");

                    // query to get the  key constraints for the table
                    #region query to get the  key constraints for the table
                    string ConstraintQuery = "SELECT DISTINCT ac.constraint_type, ac.constraint_name" +
                        "  FROM user_constraints ac" +
                        "  JOIN user_cons_columns acc" +
                        "    ON ac.constraint_name = acc.constraint_name" +
                        " WHERE  ac.status = 'ENABLED'" +
                        "   AND ac.validated = 'VALIDATED' " +
                        $"  AND ac.table_name = '{tn.table_name}'" +
                        "   AND ac.constraint_name NOT LIKE '%SYS_%'" +
                        " ORDER BY ac.constraint_name";
                    var constraints = await _dbContext.QueryAsync<ConstraintClass>(ConstraintQuery);
                    _logger.LogInformation($"{i}.5 Total constraints found for table {tn.table_name}: {constraints.Count()}");
                    foreach (var tc in constraints)
                    {
                        string constraintQuery = "";
                        ConstraintInfo constraintInfo = new ConstraintInfo
                        {
                            ConstraintName = tc.constraint_name,
                            ConstraintType = tc.constraint_type,
                            ColumnsDetails = new List<ConstraintColumnInfo>()
                        };

                        if (tc.constraint_type == "P" || tc.constraint_type == "U")
                        {
                            constraintQuery = "SELECT " +
                                "   acc.column_name, " +
                                "   acc.position, " +
                                "   NULL AS parrent_table, " +
                                "   NULL  AS parent_column, " +
                                "   NULL AS condition" +
                                "  FROM user_constraints ac" +
                                " JOIN user_cons_columns acc" +
                                "   ON ac.constraint_name = acc.constraint_name " +
                                $" WHERE ac.constraint_name = '{tc.constraint_name}'" +
                                " ORDER BY position";
                        }
                        else if (tc.constraint_type == "R")
                        {
                            constraintQuery = "SELECT " +
                                "   child.column_name  AS column_name," +
                                "   child.position     AS position," +
                                "   parent.table_name  AS parrent_table," +
                                "   parent.column_name AS parent_column," +
                                "   NULL               AS condition" +
                                "  FROM user_constraints fk" +
                                "  JOIN user_cons_columns child" +
                                "    ON fk.constraint_name = child.constraint_name" +
                                "  JOIN user_constraints pk" +
                                "   ON fk.r_constraint_name = pk.constraint_name" +
                                "  JOIN user_cons_columns PARENT" +
                                "    ON pk.constraint_name = parent.constraint_name " +
                                "  AND child.position = parent.position" +
                                $" WHERE fk.CONSTRAINT_NAME = '{tc.constraint_name}'" +
                                " ORDER BY child.POSITION";
                        }
                        else if (tc.constraint_type == "C")
                        {
                            constraintQuery = "SELECT " +
                                "   acc.column_name, " +
                                "   acc.position, " +
                                "   NULL AS parrent_table, " +
                                "   NULL  AS parent_column, " +
                                "   ac.search_condition AS condition" +
                                "  FROM user_constraints ac" +
                                " JOIN user_cons_columns acc" +
                                "   ON ac.constraint_name = acc.constraint_name " +
                                $" WHERE ac.constraint_name = '{tc.constraint_name}'" +
                                " ORDER BY position";
                        }

                        var constraintColumns = await _dbContext.QueryAsync<ConstraintColumnClass>(constraintQuery);
                        foreach (var cc in constraintColumns)
                        {
                            constraintInfo.ColumnsDetails.Add(new ConstraintColumnInfo
                            {
                                ColumnName = cc.column_name,
                                Position = cc.position,
                                ParentTable = cc.parrent_table,
                                ParentColumn = cc.parent_column,
                                Condition = cc.condition?.Replace("\n", "").Replace("\r", "").Trim() // Remove newlines and trim whitespace
                            });
                        }
                        tableInfo.Constraints.Add(constraintInfo);
                        _logger.LogInformation($"{i}.5.1 parsing constrain complete of {tc.constraint_name} ({tc.constraint_type}) ");
                    }
                    _logger.LogInformation($"{i}.6 Constraints for table {tn.table_name} imported successfully. Total constraints: {constraints.Count()}");
                    #endregion

                    tables.Add(tableInfo);
                    await _com.SaveTableInfo(tables);

                    #region Save each table data as a separate JSON file
                    _logger.LogInformation($"{i}.7 Now fetching data for table {tn.table_name} to save as JSON file");
                    try
                    {
                        string query = $"Select * from {tn.table_name}";
                        var result = await _dbContext.QueryAsync(query);
                        if (result != null)
                            await _com.saveTableData(result.ToList(), tn.table_name);
                    }
                    catch (Exception ex)
                    {
                        errorId = errorId + 1;
                        await _com.saveErrorLogs(errorId, "DataInserError", tn.table_name, "", ex.Message);
                        _logger.LogError($"Error processing table {tn.table_name}: {ex.Message}");
                    }
                    _logger.LogInformation($"-------------- End Processing Data for table: {tn.table_name} -------------- ");
                    #endregion

                    _logger.LogInformation($"-------- END PROCESS FOR TABLE : {tableInfo.TableName} -------- Time : {DateTimeOffset.Now}");
                }

                _logger.LogInformation($"All tables Export finished");

                return Ok("Successfully Added data to the table");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to CurrentDB: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("export-table")]
        public async Task<IActionResult> ExportTable()
        {
            try
            {
                List<TableInfo> tables = new List<TableInfo>();

                using var _dbContext = new OracleConnection(_currentDBConnection);
                await _dbContext.OpenAsync();

                // get all the available tables in the current database except the ones with BAK, TEST, TMP and __EFMigrationsHistory in their names
                string Tablequery = "SELECT table_name" +
                    "  FROM user_tables" +
                    " WHERE table_name NOT LIKE '%BAK%'" +
                    "   AND table_name NOT LIKE '__EFMigrationsHistory'" +
                    "   AND table_name NOT LIKE '%TEST%'" +
                    "   AND table_name NOT LIKE '%TMP%'" +
                    "   AND table_name NOT LIKE '%TEMP%'" +
                    "   AND table_name NOT LIKE '%OLD%'" +
                    "   AND table_name NOT LIKE '%AUDIT%'" +
                    " ORDER BY table_name";

                var tableName = await _dbContext.QueryAsync<TabelClass>(Tablequery);

                _logger.LogInformation($"Total tables found: {tableName.Count()}");
                int i = 0;
                // now get the columns & contraint for each table and add them to the list of tables
                foreach (var tn in tableName)
                {
                    i = i + 1;
                    TableInfo tableInfo = new TableInfo
                    {
                        Id = i,
                        TableName = tn.table_name,
                        Columns = new List<ColumnInfo>(),
                        Constraints = new List<ConstraintInfo>()
                    };
                    _logger.LogInformation($"-------- Start Process for Table: {tableInfo.TableName} -------- Time : {DateTimeOffset.Now}");

                    #region get columns for the table
                    string ColumnQuery = "SELECT column_name, data_type, data_length, nullable, data_default" +
                        "  FROM user_tab_columns t" +
                        $" WHERE table_name = '{tn.table_name}'" +
                        " ORDER BY table_name, column_id";

                    var tablesColums = await _dbContext.QueryAsync<ColumnClass>(ColumnQuery);

                    _logger.LogInformation($"{i}.1 Total columns found for table {tn.table_name}: {tablesColums.Count()}");
                    _logger.LogInformation($"{i}.2 Mapping columns for table {tn.table_name} to ColumnInfo objects");
                    foreach (var tc in tablesColums)
                    {
                        tableInfo.Columns.Add(new ColumnInfo
                        {
                            ColumnName = tc.column_name,
                            DataType = tc.data_type,
                            DataLength = tc.data_length,
                            IsNullable = tc.nullable == "Y" ? true : false,
                            DefaultValue = tc.data_default
                        });
                    }
                    _logger.LogInformation($"{i}.3 Columns for table {tn.table_name} imported successfully. Total columns: {tablesColums.Count()}");
                    #endregion

                    _logger.LogInformation($"{i}.4 Now fetching constraints for table {tn.table_name}");

                    // query to get the  key constraints for the table
                    #region query to get the  key constraints for the table
                    string ConstraintQuery = "SELECT DISTINCT ac.constraint_type, ac.constraint_name" +
                        "  FROM user_constraints ac" +
                        "  JOIN user_cons_columns acc" +
                        "    ON ac.constraint_name = acc.constraint_name" +
                        " WHERE  ac.status = 'ENABLED'" +
                        "   AND ac.validated = 'VALIDATED' " +
                        $"  AND ac.table_name = '{tn.table_name}'" +
                        "   AND ac.constraint_name NOT LIKE '%SYS_%'" +
                        " ORDER BY ac.constraint_name";
                    var constraints = await _dbContext.QueryAsync<ConstraintClass>(ConstraintQuery);
                    _logger.LogInformation($"{i}.5 Total constraints found for table {tn.table_name}: {constraints.Count()}");
                    foreach (var tc in constraints)
                    {
                        string constraintQuery = "";
                        ConstraintInfo constraintInfo = new ConstraintInfo
                        {
                            ConstraintName = tc.constraint_name,
                            ConstraintType = tc.constraint_type,
                            ColumnsDetails = new List<ConstraintColumnInfo>()
                        };

                        if (tc.constraint_type == "P" || tc.constraint_type == "U")
                        {
                            constraintQuery = "SELECT " +
                                "   acc.column_name, " +
                                "   acc.position, " +
                                "   NULL AS parrent_table, " +
                                "   NULL  AS parent_column, " +
                                "   NULL AS condition" +
                                "  FROM user_constraints ac" +
                                " JOIN user_cons_columns acc" +
                                "   ON ac.constraint_name = acc.constraint_name " +
                                $" WHERE ac.constraint_name = '{tc.constraint_name}'" +
                                " ORDER BY position";
                        }
                        else if (tc.constraint_type == "R")
                        {
                            constraintQuery = "SELECT " +
                                "   child.column_name  AS column_name," +
                                "   child.position     AS position," +
                                "   parent.table_name  AS parrent_table," +
                                "   parent.column_name AS parent_column," +
                                "   NULL               AS condition" +
                                "  FROM user_constraints fk" +
                                "  JOIN user_cons_columns child" +
                                "    ON fk.constraint_name = child.constraint_name" +
                                "  JOIN user_constraints pk" +
                                "   ON fk.r_constraint_name = pk.constraint_name" +
                                "  JOIN user_cons_columns PARENT" +
                                "    ON pk.constraint_name = parent.constraint_name " +
                                "  AND child.position = parent.position" +
                                $" WHERE fk.CONSTRAINT_NAME = '{tc.constraint_name}'" +
                                " ORDER BY child.POSITION";
                        }
                        else if (tc.constraint_type == "C")
                        {
                            constraintQuery = "SELECT " +
                                "   acc.column_name, " +
                                "   acc.position, " +
                                "   NULL AS parrent_table, " +
                                "   NULL  AS parent_column, " +
                                "   ac.search_condition AS condition" +
                                "  FROM user_constraints ac" +
                                " JOIN user_cons_columns acc" +
                                "   ON ac.constraint_name = acc.constraint_name " +
                                $" WHERE ac.constraint_name = '{tc.constraint_name}'" +
                                " ORDER BY position";
                        }

                        var constraintColumns = await _dbContext.QueryAsync<ConstraintColumnClass>(constraintQuery);
                        foreach (var cc in constraintColumns)
                        {
                            constraintInfo.ColumnsDetails.Add(new ConstraintColumnInfo
                            {
                                ColumnName = cc.column_name,
                                Position = cc.position,
                                ParentTable = cc.parrent_table,
                                ParentColumn = cc.parent_column,
                                Condition = cc.condition?.Replace("\n", "").Replace("\r", "").Trim() // Remove newlines and trim whitespace
                            });
                        }
                        tableInfo.Constraints.Add(constraintInfo);
                        _logger.LogInformation($"{i}.5.1 parsing constrain complete of {tc.constraint_name} ({tc.constraint_type}) ");
                    }
                    _logger.LogInformation($"{i}.6 Constraints for table {tn.table_name} imported successfully. Total constraints: {constraints.Count()}");
                    #endregion

                    tables.Add(tableInfo);
                    await _com.SaveTableInfo(tables);
                    _logger.LogInformation($"-------- END PROCESS FOR TABLE : {tableInfo.TableName} -------- Time : {DateTimeOffset.Now}");
                }

                _logger.LogInformation($"Total tables imported finished\n Start Save Process as Json");

                return Ok("Successfully Added data to the table");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to CurrentDB: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("export-table-data")]
        public async Task<IActionResult> ExportTableData()
        {
            try
            {
                var tables = await _com.getTableInfos();
                _logger.LogInformation($"Total tables found in JSON file: {tables.Count()}");
                int i = 0;
                int errorId = 0;
                foreach (var table in tables)
                {
                    i = i + 1;
                    _logger.LogInformation($"-------------- {i}. Processing table: {table.TableName} -------------- ");
                    try
                    {
                        string query = $"Select * from {table.TableName}";
                        using var _dbContext = new OracleConnection(_currentDBConnection);
                        await _dbContext.OpenAsync();
                        var result = await _dbContext.QueryAsync(query);
                        await _com.saveTableData(result.ToList(), table.TableName);
                    }
                    catch (Exception ex)
                    {
                        errorId = errorId + 1;
                        await _com.saveErrorLogs(errorId, "DataInserError", table.TableName, "", ex.Message);
                        _logger.LogError($"Error processing table {table.TableName}: {ex.Message}");
                    }
                    _logger.LogInformation($"-------------- End Processing table: {table.TableName} -------------- ");
                }
                _logger.LogInformation($"All tables data imported successfully and saved as JSON files in the wwwroot/CurrentDbData/TableData folder");
                _logger.LogInformation($"Total tables processed: {i}; error {errorId}");
                return Ok("Save Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reading table data: {ex.Message}");
            }
        }
    }
}
