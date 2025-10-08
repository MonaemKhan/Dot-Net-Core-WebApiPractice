using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassObjectMapping
{
    public static class DataTableObjectClass
    {
        public static DataTable DataTableObject()
        {
            // Create DataTable
            DataTable dt = new DataTable("Medicine");

            // Define columns
            dt.Columns.Add("MedicineId", typeof(int));
            dt.Columns.Add("MedicineName", typeof(string));
            dt.Columns.Add("Strength", typeof(string));
            dt.Columns.Add("CompanyName", typeof(string));

            // Add rows
            dt.Rows.Add(1, "Paracetamol", "500mg", "Square Pharma");
            dt.Rows.Add(2, "Amoxicillin", "250mg", "Beximco Pharma");
            dt.Rows.Add(3, "Cetrizine", "10mg", "ACI Limited");

            return dt;
        }
    }
}
