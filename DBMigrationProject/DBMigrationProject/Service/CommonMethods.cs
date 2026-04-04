using DBMigrationProject.Classes;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DBMigrationProject.Service
{
    public class CommonMethods(IWebHostEnvironment env)
    {
        private readonly IWebHostEnvironment _env = env;
        public async Task<List<TableInfo>> getTableInfos()
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData");
            string fileName = "TABLE_INFORMATION.json";
            var filePath = Path.Combine(uploadFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Table information file not found.");
            }
            var jsonData = System.IO.File.ReadAllText(filePath);
            var tableInfos = JsonSerializer.Deserialize<List<TableInfo>>(jsonData);
            if (tableInfos == null)
            {
                throw new Exception("Failed to deserialize table information.");
            }
            return tableInfos;
        }

        public async Task<List<IDictionary<string, object>>> getColumsData(string TableName)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData/TableData");
            var fileName = $"{TableName}.json";
            var filePath = Path.Combine(uploadFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return new List<IDictionary<string, object>>();
            }
            var jsonData = System.IO.File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<List<IDictionary<string, object>>>(jsonData);
            return data;
        }

        public async Task saveErrorLogs(int count, string file, string TableName, string query, string message)
        {
            try
            {
                // Save the tables data list as a JSON file in the wwwroot/CurrentDbData folder

                var uploadFolder = Path.Combine(_env.WebRootPath, "CurrentDbData");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                string fileName = $"{file}.json";
                var filePath = Path.Combine(uploadFolder, fileName);

                List<ErrorLogMigration> errorLogs = new List<ErrorLogMigration>();

                try
                {
                    var jsonData = System.IO.File.ReadAllText(filePath);
                    errorLogs = JsonSerializer.Deserialize<List<ErrorLogMigration>>(jsonData);
                }
                catch
                {
                    errorLogs = new List<ErrorLogMigration>();
                }

                errorLogs.Add(new ErrorLogMigration
                {
                    Id = count,
                    TableName = TableName,
                    ErrorMessage = message,
                    Query = query
                });
                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(errorLogs, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveTableInfo(List<TableInfo> tables)
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

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(tables, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task saveTableData(List<object> data, string TableName)
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
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
