using CLassAtrName;
using System.Reflection;
using System.Text.Json;

//var type = typeof(Medicine);
//var classAttr = type.GetCustomAttribute<MapStoreProcedure>();
//Console.WriteLine($"Class Attribute SpName: {classAttr.SpName}");
//foreach (var prop in type.GetProperties())
//{
//    var attr = prop.GetCustomAttribute<MapColumn>();
//    if (attr != null)
//    {
//        Console.WriteLine($"Property: {prop.Name}, Column: {attr.ColumnName}, outputparam: {attr.OutputParam}");
//    }
//}

LoginModelDTO login = new LoginModelDTO
{
    UserId = "testuser",
    Password = "password123"
};

var data = login.getSerarchEntity();

List<string> outputParam = new List<string>();

var className = data.GetType();
var classAttr = className.GetCustomAttribute<MapStoreProcedure>();
var spName = classAttr.SpName;
foreach (var obj in className.GetProperties())
{
    var val = obj.GetValue(data);
    var propAttr = obj.GetCustomAttribute<MapColumn>();
    if(propAttr != null && propAttr.OutputParam)
    {
        outputParam.Add(propAttr.ColumnName);
    }
}

foreach (var obj in className.GetProperties())
{
    var propAttr = obj.GetCustomAttribute<MapColumn>();
    if (propAttr != null)
    {
        if (outputParam.Contains(propAttr.ColumnName))
        {
            obj.SetValue(data, "Output Value");
        }
    }
}

Console.WriteLine(JsonSerializer.Serialize(data));

    
