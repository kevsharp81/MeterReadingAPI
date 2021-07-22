using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;
using System.IO;
using ENSEK_meter_readings_API.CORE.DAL;
using ENSEK_meter_readings_API.Models;
using Microsoft.AspNetCore.Http;
using CsvHelper;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;
namespace ENSEK_meter_readings_API.CORE.BAL
{
    public class MeterReadingsProcessingLayer: IMeterReadingsProcessingLayer
    {
        private IReadingsDataRepository _dataRepository;
        private ILogger<MeterReadingsProcessingLayer> _logger;
        public MeterReadingsProcessingLayer(IReadingsDataRepository dataRepository)
        {
            this._dataRepository = dataRepository;
        }
        public DataTable ProcessMeterReadings(String CSVFileName)
        {
            DataTable processedMeterReadings = BuildCSVStatusTableStructure();

            return ReadCSVFile(CSVFileName, processedMeterReadings);
        }

        public DataTable ReadCSVFile(String CSVFileName,DataTable processedMeterReadings)
        {
            using (StreamReader streamReader = new StreamReader(CSVFileName))
            {

                using(var csv = new CsvReader(streamReader,CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while(csv.Read())
                    {
                        var meterReading = new MeterReading
                        {
                            AccountFk = csv.GetField<int>("AccountId"),
                            MeterReadingDateTime = ParseCSVDate(csv.GetField("MeterReadingDateTime")),
                            MeterReadingValue = csv.GetField("MeterReadValue")
                            
                        };

                        processedMeterReadings = ProcessSingleCSVRow(meterReading, processedMeterReadings);
                    }
                }

            }

            //summerising the meter readings
            processedMeterReadings = SummeriseMeterReadings(processedMeterReadings);

            return processedMeterReadings;
        }
        
        public DataTable ProcessSingleCSVRow(MeterReading meterReading,DataTable processedMeterReadings)
        {
            int newMeterReadingID = CreateMeterReadingInRepository(meterReading);            
            String status = (newMeterReadingID > 0) ? "Success" : "Failed";

            processedMeterReadings = CreateRowInProcessedMeterReadings(status, meterReading, processedMeterReadings);

            return processedMeterReadings;
        }

        public int CreateMeterReadingInRepository(MeterReading meterReading)
        {
            return _dataRepository.AddMeterReading(meterReading);
        }

        public DataTable CreateRowInProcessedMeterReadings(String status, MeterReading meterReading,DataTable processedMeterReadings)
        {
            DataRow dataRow = processedMeterReadings.NewRow();
            dataRow["UploadStatus"] = status;
            dataRow["AccountNumber"] = meterReading.AccountFk;
            dataRow["MeterReadingDateTime"] = meterReading.MeterReadingDateTime;
            dataRow["MeterReadingValue"] = meterReading.MeterReadingValue;
            processedMeterReadings.Rows.Add(dataRow);
            return processedMeterReadings;
        }

        public DataTable BuildCSVStatusTableStructure()
        {
            DataTable processedMeterReadings = new DataTable("ProcessedMeterReadings");

            processedMeterReadings.Columns.Add("UploadStatus", typeof(String));
            processedMeterReadings.Columns.Add("AccountNumber", typeof(int));
            processedMeterReadings.Columns.Add("MeterReadingDateTime", typeof(DateTime));
            processedMeterReadings.Columns.Add("MeterReadingValue", typeof(String));

            return processedMeterReadings;
        }

        public String StoreCSVFile(IFormFile file)
        {
            String pathAndFileName = "";

            try
            {
                if (file != null && file.Length > 0 && IsCsvFile(file.FileName))
                {
                    var path = Path.Combine(
                      Directory.GetCurrentDirectory(), "dataSources",
                      file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    
                    pathAndFileName = path;
                }
            }
            catch(Exception e)
            {
                _logger.LogError("Error occured StoreCSVFile error= " + e.ToString());
            }
            

            return pathAndFileName;
        }

        public DateTime ParseCSVDate(string CSVDateTime)
        {
            return DateTime.ParseExact(CSVDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        }


        public byte[] Base64ToByteArray(String Base64)
        {
            return Convert.FromBase64String(Base64);
        }

        public Boolean IsCsvFile(String fileName)
        {
            return (Path.GetExtension(fileName).ToLower() == ".csv");
        }

        public DataTable SummeriseMeterReadings(DataTable meterReadings)
        {
            DataTable summaryMeterReadings = BuildSummaryTableStructure();

            int totalNumberOfReadings = meterReadings.Rows.Count;
            int failedReadingsCount = meterReadings.Select("UploadStatus = 'Failed'").Length;

            summaryMeterReadings.Rows.Add("Failed", failedReadingsCount);
            summaryMeterReadings.Rows.Add("Success", totalNumberOfReadings - failedReadingsCount);

            return summaryMeterReadings;
        }

        private DataTable BuildSummaryTableStructure()
        {
            DataTable summaryTable = new DataTable("MeterReadingsSummary");
            summaryTable.Columns.Add("Status", typeof(String));
            summaryTable.Columns.Add("Count", typeof(int));

            return summaryTable;
        }

    }
}
