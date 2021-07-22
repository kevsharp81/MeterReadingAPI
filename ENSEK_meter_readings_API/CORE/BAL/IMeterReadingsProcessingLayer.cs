using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Http;
using ENSEK_meter_readings_API.Models;
namespace ENSEK_meter_readings_API.CORE.BAL
{
    public interface IMeterReadingsProcessingLayer
    {
        public DataTable ProcessMeterReadings(String CSVFileName);

        public DataTable ReadCSVFile(String CSVFileName, DataTable processedMeterReadings);

        public DataTable ProcessSingleCSVRow(MeterReading meterReading, DataTable processedMeterReadings);

        public DataTable BuildCSVStatusTableStructure();

        public String StoreCSVFile(IFormFile file);

        public DataTable SummeriseMeterReadings(DataTable meterReadings);
    }
}
