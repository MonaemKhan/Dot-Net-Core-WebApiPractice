using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassObjectMapping
{
    public class Medicine
    {
        [MapColumn("MedicineId")]
        public int Id { get; set; }

        [MapColumn("MedicineName")]
        public string Name { get; set; }

        [MapColumn("Strength")]
        public string Strength { get; set; }

        [MapColumn("CompanyName")]
        public string Company { get; set; }
    }

    public class MedicineEntity
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string Strength { get; set; }
        public string Company { get; set; }
        public string BatchNo { get; set; }
    }

}
