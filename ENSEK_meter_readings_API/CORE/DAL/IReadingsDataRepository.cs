using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ENSEK_meter_readings_API.Models;
namespace ENSEK_meter_readings_API.CORE.DAL
{
   public interface IReadingsDataRepository
    {
        public int AddMeterReading(MeterReading meterReading);
        public Boolean EditMeterReading(MeterReading meterReading);

        public Boolean DeleteMeterReading(MeterReading meterReading);

        public Account FindAccountByID(int id);
    }
}
