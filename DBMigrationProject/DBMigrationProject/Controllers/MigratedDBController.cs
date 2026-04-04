using Dapper;
using DBMigrationProject.Classes;
using DBMigrationProject.Service;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Text.Json;

namespace DBMigrationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MigratedDBController(IConfiguration configuration,
        ILogger<MigratedDBController> logger,
        CommonMethods com) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<MigratedDBController> _logger = logger;
        private readonly CommonMethods _com = com;
        private string _migratedDBConnection { get => _configuration.GetValue<string>("ConnectionStrings:MigrationConnection"); }


        [HttpGet]
        public IActionResult GetDBName()
        {
            if (string.IsNullOrEmpty(_migratedDBConnection))
            {
                return NotFound("CurrentDB configuration is not set.");
            }
            return Ok(_migratedDBConnection);
        }

        [HttpGet]
        [Route("test-connection")]
        public async Task<IActionResult> Test()
        {
            try
            {
                using var _dbContext = new OracleConnection(_migratedDBConnection);
                await _dbContext.OpenAsync();
                return Ok("Connected");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error connecting to CurrentDB: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("import-db")]
        public async Task<IActionResult> ImportDB()
        {
            try
            {
                var tables = await _com.getTableInfos();
                _logger.LogInformation($"Found {tables.Count} tables to import.");

                using var _dbContext = new OracleConnection(_migratedDBConnection);
                await _dbContext.OpenAsync();

                int i = 0;
                foreach (var table in tables)
                {
                    i++;
                    _logger.LogInformation($"--------{i}. Importing table: {table.TableName}");

                    #region create table 
                    string createQuery = $"CREATE TABLE {table.TableName} ( ";
                    var typesWithLength = new[] { "NVARCHAR2", "VARCHAR2", "NUMBER" };
                    foreach (var column in table.Columns)
                    {
                        createQuery += $"{column.ColumnName} {column.DataType}";
                        if (column.DataLength.HasValue && typesWithLength.Contains(column.DataType?.ToUpper()))
                        {
                            createQuery += $"({column.DataLength.Value})";
                        }
                        if (column.DefaultValue != null)
                        {
                            createQuery += $" DEFAULT {column.DefaultValue}";
                        }
                        if (column.IsNullable == false)
                        {
                            createQuery += " NOT NULL";
                        }
                        createQuery += ", ";
                    }

                    createQuery = createQuery.TrimEnd(',', ' ') + " )";
                    _logger.LogInformation($"\r\t{createQuery}");
                    try
                    {
                        await _dbContext.QueryAsync(createQuery);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error creating table: {table.TableName}. Error: {ex.Message}");
                        await _com.saveErrorLogs(i, "TableCreationError", table.TableName, createQuery, ex.Message);
                    }
                    _logger.LogInformation($"--------{i}.2 Table created: {table.TableName}");
                    #endregion

                    #region importing data
                    _logger.LogInformation($"--------{i}.3 Importing data for table: {table.TableName}");
                    var columns = table.Columns.Select(x => x.ColumnName).ToList();
                    var Dbcolumns = string.Join(", ", columns.Select(k => $"{k}"));

                    var data = await _com.getColumsData(table.TableName);
                    _logger.LogInformation($"{i}.4 Found {data.Count} records to import for table: {table.TableName}");

                    int j = 0;
                    int errorCount = 0;
                    foreach (IDictionary<string, object> item in data)
                    {
                        j++;
                        _logger.LogInformation($"--------{i}.{j}. Processing record for table: {table.TableName}");
                        string insertQuery = "";
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
                        _logger.LogInformation($"\r\t{insertQuery}");
                        try
                        {
                            using var _dbContext = new OracleConnection(_migratedDBConnection);
                            await _dbContext.OpenAsync();

                            await _dbContext.QueryAsync(insertQuery);
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            _logger.LogError($"Error processing record for table: {table.TableName}. Error: {ex.Message}");
                            await _com.saveErrorLogs(errorCount, "InsertError", table.TableName, insertQuery, ex.Message);
                        }
                    }
                    #endregion 


                    _logger.LogInformation($"-------- -- END --- {i}. Table imported: {table.TableName}");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reading table information: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("import-tables")]
        public async Task<IActionResult> ImportTables()
        {
            try
            {
                var tables = await _com.getTableInfos();
                _logger.LogInformation($"Found {tables.Count} tables to import.");
                int i = 0;
                foreach (var table in tables)
                {
                    i++;
                    _logger.LogInformation($"--------{i}. Importing table: {table.TableName}");
                    
                    string createQuery = $"CREATE TABLE {table.TableName} ( ";
                    var typesWithLength = new[] { "NVARCHAR2", "VARCHAR2", "NUMBER" };
                    foreach (var column in table.Columns)
                    {
                        createQuery += $"{column.ColumnName} {column.DataType}";
                        if (column.DataLength.HasValue && typesWithLength.Contains(column.DataType?.ToUpper()))
                        {
                            createQuery += $"({column.DataLength.Value})";
                        }
                        if(column.DefaultValue != null)
                        {
                            createQuery += $" DEFAULT {column.DefaultValue}";
                        }
                        if (column.IsNullable == false)
                        {
                            createQuery += " NOT NULL";
                        }
                        createQuery += ", ";
                    }

                    createQuery = createQuery.TrimEnd(',', ' ') + " )";
                    _logger.LogInformation($"\r\t{createQuery}");
                    try
                    {
                        using var _dbContext = new OracleConnection(_migratedDBConnection);
                        await _dbContext.OpenAsync();

                        await _dbContext.QueryAsync(createQuery);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error creating table: {table.TableName}. Error: {ex.Message}");
                        await _com.saveErrorLogs(i,"TableCreationError", table.TableName, createQuery, ex.Message);
                    }
                    _logger.LogInformation($"-------- -- END --- {i}. Table imported: {table.TableName}");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reading table information: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("import-table-data")]
        public async Task<IActionResult> ImportTableData()
        {
            try
            {
                var tables = await _com.getTableInfos();

                _logger.LogInformation($"Found {tables.Count} tables to import.");
                int i = 0;
                foreach (var table in tables)
                {
                    i++;
                    _logger.LogInformation($"--------{i}. Importing data for table: {table.TableName}");
                    var columns = table.Columns.Select(x => x.ColumnName).ToList();
                    var Dbcolumns = string.Join(", ", columns.Select(k => $"{k}"));
                                       
                    var data = await _com.getColumsData(table.TableName);
                    _logger.LogInformation($"Found {data.Count} records to import for table: {table.TableName}");

                    int j = 0;
                    int errorCount = 0;
                    foreach (IDictionary<string, object> item in data)
                    {
                        j++;
                        _logger.LogInformation($"--------{i}.{j}. Processing record for table: {table.TableName}");
                        string insertQuery = "";
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
                        _logger.LogInformation($"\r\t{insertQuery}");
                        try
                        {
                            using var _dbContext = new OracleConnection(_migratedDBConnection);
                            await _dbContext.OpenAsync();

                            await _dbContext.QueryAsync(insertQuery);
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            _logger.LogError($"Error processing record for table: {table.TableName}. Error: {ex.Message}");
                            await _com.saveErrorLogs(errorCount,"InsertError", table.TableName, insertQuery, ex.Message);
                        }
                    }

                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reading table information: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("import-table-contraint")]
        public async Task<IActionResult> ImportTableConstraints()
        {
            try
            {
                var tables = await _com.getTableInfos();

                _logger.LogInformation($"Found {tables.Count} tables to import.");
                int i = 0;
                foreach (var table in tables)
                {
                    i++;
                    _logger.LogInformation($"--------{i}. Accessing contraint for table: {table.TableName}");
                    
                    int j = 0;
                    int errorCount = 0;
                    int PK = 0;
                    int FK = 0;
                    int UK = 0;
                    int CC = 0;
                    foreach (var constraint in table.Constraints)
                    {
                        j++;
                        _logger.LogInformation($"--------{i}.{j}. Processing constraint: {constraint.ConstraintName} for table: {table.TableName}");
                        
                        string columns = string.Join(", ", constraint.ColumnsDetails.Select(c => c.ColumnName));
                        string contrainName = "";
                        string constraintType = "";

                        #region get constrain name based on type
                        if (constraint.ConstraintType == "P")
                        {
                            contrainName = $"PK_{table.TableName}";
                            if(PK > 0)
                            {
                                contrainName += $"_{PK.ToString().PadLeft(2,'0')}";
                            }
                            PK++;
                            constraintType = "PRIMARY KEY";
                        }
                        else if (constraint.ConstraintType == "U")
                        {
                            contrainName = $"UK_{table.TableName}";
                            if (UK > 0)
                            {
                                contrainName += $"_{UK.ToString().PadLeft(2, '0')}";
                            }
                            UK++;
                            constraintType = "UNIQUE";
                        }
                        else if (constraint.ConstraintType == "R")
                        {
                            contrainName = $"FK_{table.TableName}";
                            if (FK > 0)
                            {
                                contrainName += $"_{FK.ToString().PadLeft(2, '0')}";
                            }
                            FK++;
                            constraintType = "FOREIGN KEY";
                        }
                        else if (constraint.ConstraintType == "C")
                        {
                            contrainName = $"CC_{table.TableName}";
                            if (CC > 0)
                            {
                                contrainName += $"_{CC.ToString().PadLeft(2, '0')}";
                            }
                            CC++;
                            constraintType = "CHECK";
                        }
                        else
                        {
                            contrainName = "";
                            constraintType = "";
                        }
                        #endregion

                        string constraintQuery = $"ALTER TABLE {table.TableName} " +
                                $"ADD CONSTRAINT {contrainName} " +
                                $"{constraintType} " +
                                $"({columns}))";
                        if(constraint.ConstraintType == "R")
                        {
                            var refTable = constraint.ColumnsDetails.FirstOrDefault();
                            var refColumn = string.Join(", ", constraint.ColumnsDetails.Select(c => c.ColumnName));
                            constraintQuery = constraintQuery + $" references {refTable.ParentTable} ({refColumn})";
                        }else if(constraint.ConstraintType == "C")
                        {
                            var condition = constraint.ColumnsDetails.FirstOrDefault()?.Condition;
                            if (!string.IsNullOrEmpty(condition))
                            {
                                constraintQuery = constraintQuery + $" CHECK ({condition})";
                            }
                        }

                        _logger.LogInformation($"\r\t{constraintQuery}");
                        try
                        {
                            using var _dbContext = new OracleConnection(_migratedDBConnection);
                            await _dbContext.OpenAsync();
                            await _dbContext.QueryAsync(constraintQuery);
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            _logger.LogError($"Error processing constraint: {constraint.ConstraintName} for table: {table.TableName}. Error: {ex.Message}");
                            await _com.saveErrorLogs(errorCount,"ConstraintError", table.TableName, constraintQuery, ex.Message);
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reading table information: {ex.Message}");
            }
        }

    }
}
