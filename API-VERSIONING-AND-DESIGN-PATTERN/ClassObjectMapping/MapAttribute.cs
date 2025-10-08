using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassObjectMapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapColumn :Attribute
    {
        public string ColumnName { get; }

        public MapColumn(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
