using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLassAtrName
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapColumn : Attribute
    {
        public string ColumnName { get; }
        public bool OutputParam { get; }
        public MapColumn(string columnName,bool outputParam = false)
        {
            ColumnName = columnName;
            OutputParam = outputParam;
        }
    }

    public class MapStoreProcedure : Attribute
    {
        public string SpName { get; }
        public MapStoreProcedure(string spName)
        {
            SpName = spName;
        }
    }

}
