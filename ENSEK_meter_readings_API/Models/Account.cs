using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ENSEK_meter_readings_API.Models
{
    public class Account
    {
       [Key]
        public int AccountId { get; set; }
        [MaxLength(15,ErrorMessage ="First name must be 15 characters long")]
        public String FirstName { get; set; }

        [MaxLength(15,ErrorMessage ="Last Name must be 15 characters long")]
        public String LastName { get; set; }

        public ICollection<MeterReading> meterReadings { get; set; }
    }
}
