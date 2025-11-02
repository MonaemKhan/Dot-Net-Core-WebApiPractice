using AutoMapper;
using ClassObjectMapping;
using Microsoft.Extensions.Logging.Abstractions;
using System.Data;


internal class Program
{
    private static void Main(string[] args)
    {
        DataTable table = DataTableObjectClass.DataTableObject();
        var props = typeof(Medicine).GetProperties();

        var medList = new List<Medicine>();
        foreach (DataRow row in table.Rows)
        {
            var med = new Medicine();
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttributes(typeof(MapColumn), false)
                               .FirstOrDefault() as MapColumn;

                if (attr != null && table.Columns.Contains(attr.ColumnName))
                {
                    object value = row[attr.ColumnName];
                    if (value != DBNull.Value)
                    {
                        prop.SetValue(med, Convert.ChangeType(value, prop.PropertyType));
                    }
                }
            }

            medList.Add(med);
        }

        // AutoMapper configuration MUST be non-static lambda
        var config = new MapperConfiguration((cfg => cfg.AddProfile<MappingProfile>()), new NullLoggerFactory());

        // Validate mapping configuration - this will throw if invalid
        config.AssertConfigurationIsValid();

        var mapper = config.CreateMapper();

        List<MedicineEntity> dtoList = mapper.Map<List<MedicineEntity>>(medList);

        Console.ReadKey();
    }
}