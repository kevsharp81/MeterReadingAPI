using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENSEK_meter_readings_API.Models
{
    public class MeterReading
    {
        [Key]
        public int ReadingID { get; set; }

        public int AccountFk { get; set; }

        [ForeignKey("AccountFk")]
        public Account Account { get; set; }
        

        public DateTime MeterReadingDateTime { get; set; }

        public String MeterReadingValue { get; set; }
    }
}
