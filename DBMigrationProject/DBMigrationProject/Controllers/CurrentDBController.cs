using Dapper;
using DBMigrationProject.Classes;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DBMigrationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentDBController(IConfiguration configuration, ILogger<CurrentDBController> logger, IWebHostEnvironment env) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<CurrentDBController> _logger = logger;
        private readonly IWebHostEnvironment _env = env;
        private string _currentDBConnection { get => _configuration.GetValue<string>("ConnectionStrings:CurrentDBConnection"); }
        private string _migratedDBConnection { get => _configuration.GetValue<string>("ConnectionStrings:MigrationConnection"); }


        [HttpGet]
        public IActionResult GetCurrentDB()
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
                foreach (var tn in tableName.Skip(0))
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
                    await saveDataAsJson(i, tables);
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

        private async Task saveDataAsJson(int i, List<TableInfo> tables)
        {
            try
            {
                // Save the tables list as a JSON file in the wwwroot/CurrentDbData folder
                var uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName = "TABLE_INFORMATION.json";
                var filePath = Path.Combine(uploadFolder, fileName);

                string fileName1 = "countTableInsert.json";
                var filePath1 = Path.Combine(uploadFolder, fileName1);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(tables, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("export-table-data")]
        public async Task<IActionResult> ExportTableData()
        {
            try
            {
                var uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData");
                string fileName = "TABLE_INFORMATION.json";
                var filePath = Path.Combine(uploadFolder, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Table information file not found.");
                }
                var jsonData = System.IO.File.ReadAllText(filePath);
                var tables = JsonSerializer.Deserialize<List<TableInfo>>(jsonData);
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
                        await saveTableAsJson(result.ToList(), table.TableName);
                    }
                    catch (Exception ex)
                    {
                        errorId = errorId + 1;
                        await saveErrorLogAsJson(errorId, table.TableName, ex.Message);
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

        [HttpGet]
        [Route("Dummy")]
        public async Task<IActionResult> GetTableInfo()
        {
            try
            {
                var uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData");
                string fileName = "TABLE_INFORMATION.json";
                var filePath = Path.Combine(uploadFolder, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Table information file not found.");
                }
                var jsonData = System.IO.File.ReadAllText(filePath);
                var tables = JsonSerializer.Deserialize<List<TableInfo>>(jsonData);

                TableInfo table = tables.Where(x => x.TableName == "SYS_APP_CONFIG").First();
                var columns = table.Columns.Select(x => x.ColumnName).ToList();
                var Dbcolumns = string.Join(", ", columns.Select(k => $"{k}"));

                string query = "Select * from SYS_APP_CONFIG";
                using var _dbContext = new OracleConnection(_currentDBConnection);
                await _dbContext.OpenAsync();

                var result = await _dbContext.QueryAsync(query);
                await saveTableAsJson(result.ToList(), table.TableName);


                uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData/TableData");
                fileName = $"{table.TableName}.json";
                filePath = Path.Combine(uploadFolder, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Table information file not found.");
                }
                jsonData = System.IO.File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<List<IDictionary<string, object>>>(jsonData);

                string insertQuery = "";
                foreach (IDictionary<string, object> item in data)
                {
                    var values = string.Join(", ", item.Values.Select(k =>
                    {
                        if (k is DateTime dt)
                        {
                            var dtString = dt.ToString("dd-MMM-yyyy");
                            return $"'{dtString}'";
                        }

                        // Try parse if string
                        if (DateTime.TryParse(k?.ToString(), out DateTime parsedDate))
                        {
                            var dtString = parsedDate.ToString("dd-MMM-yyyy");
                            return $"'{dtString}'";
                        }

                        return $"'{k}'";
                    }));
                    insertQuery = $"INSERT INTO {table.TableName} ({Dbcolumns}) VALUES ({values})";
                }

                return Ok(insertQuery);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reading table information: {ex.Message}");
            }
        }

        private async Task saveTableAsJson(List<object> data, string TableName)
        {
            try
            {
                // Save the tables data list as a JSON file in the wwwroot/CurrentDbData/TableData folder
                var uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData/TableData");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName = $"{TableName}.json";
                var filePath = Path.Combine(uploadFolder, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task saveErrorLogAsJson(int count, string TableName, string message)
        {
            try
            {
                // Save the tables data list as a JSON file in the wwwroot/CurrentDbData folder
                
                var uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                string fileName = $"ErrorLog.json";
                var filePath = Path.Combine(uploadFolder, fileName);

                List<ErrorLog> errorLogs = new List<ErrorLog>();

                try
                {
                    var jsonData = System.IO.File.ReadAllText(filePath);
                    errorLogs = JsonSerializer.Deserialize<List<ErrorLog>>(jsonData);
                }
                catch {
                    errorLogs = new List<ErrorLog>();
                }

                errorLogs.Add(new ErrorLog
                {
                    Id = count,
                    TableName = TableName,
                    ErrorMessage = message
                });
                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(errorLogs, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
