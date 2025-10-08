using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLassAtrName
{
    [MapStoreProcedure("Test")]
    public class Medicine
    {
        [MapColumn("MedicineId")]
        public int Id { get; set; }

        [MapColumn("MedicineName")]
        public string Name { get; set; }

        [MapColumn("Strength")]
        public string Strength { get; set; }

        [MapColumn("ErrorMsg",true)]
        public string ErrorMsg { get; set; }
    }

}
