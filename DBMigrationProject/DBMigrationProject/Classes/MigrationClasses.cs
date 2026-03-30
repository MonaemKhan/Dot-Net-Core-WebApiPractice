namespace DBMigrationProject.Classes
{
    public class TabelClass
    {
        public string table_name { get; set; }
    }
    public class ColumnClass
    {
        public string column_name { get; set; }
        public string data_type { get; set; }
        public int? data_length { get; set; }
        public string nullable { get; set; }
        public string data_default { get; set; }
    }
    public class ConstraintClass
    {
        public string constraint_name { get; set; }
        public string constraint_type { get; set; }
    }
    public class ConstraintColumnClass
    {
        public string column_name { get; set; }
        public int? position { get; set; }
        public string parrent_table { get; set; }
        public string parent_column { get; set; }
        public string condition { get; set; }
    }

    public class TableInfo
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public List<ConstraintInfo> Constraints { get; set; }
    }

    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int? DataLength { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
    }
    public class ConstraintInfo
    {
        public string ConstraintName { get; set; }
        public string ConstraintType { get; set; }
        public List<ConstraintColumnInfo> ColumnsDetails { get; set; }
    }

    public class ConstraintColumnInfo
    {
        public string ColumnName { get; set; }
        public int? Position { get; set; }
        public string ParentTable { get; set; }
        public string ParentColumn { get; set; }
        public string Condition { get; set; }
    }

    public class ErrorLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
