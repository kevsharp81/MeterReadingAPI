using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ENSEK_meter_readings_API.Models;
using System.Text.RegularExpressions;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;
namespace ENSEK_meter_readings_API.CORE.DAL
{
    public class ReadingsDataRepository:IReadingsDataRepository
    {
        private EnsekContext _context;
        private ILogger<ReadingsDataRepository> _logger;

        public ReadingsDataRepository(EnsekContext context,ILogger<ReadingsDataRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public int AddMeterReading(MeterReading meterReading)
        {
            var meterReadingID = 0;

            try
            {
                var accountFK = meterReading.AccountFk;
                var dateTime = meterReading.MeterReadingDateTime;
                var meterReadingValue = meterReading.MeterReadingValue;

                if (FindAccountByID(accountFK) != null && !MeterReadingExists(accountFK, dateTime) && ValidReadingValue(meterReadingValue))
                {
                    _context.MeterReading.Add(meterReading);
                    _context.SaveChanges();

                }

                meterReadingID = meterReading.ReadingID; 
            }
            catch(Exception e)
            {
                _logger.LogError("Error occurred AddMeterReading error = " + e.ToString());
            }

            return meterReadingID;
        }

        public Boolean EditMeterReading(MeterReading meterReading)
        {
            Boolean updated = false;

            try
            {
                var accountFK = meterReading.AccountFk;
                var meterReadingValue = meterReading.MeterReadingValue;

                if (FindAccountByID(accountFK) != null && ValidReadingValue(meterReadingValue))
                {
                    var existingReading = GetExistingMeterReadingByID(meterReading.ReadingID);

                    if (existingReading != null)
                    {
                        existingReading.MeterReadingDateTime = meterReading.MeterReadingDateTime;
                        existingReading.MeterReadingValue = meterReading.MeterReadingValue;
                        _context.MeterReading.Update(existingReading);
                        _context.SaveChanges();

                        updated = true;
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError("Error occurred EditMeterReading error = " + e.ToString());
            }

            

            return updated;

        }

        public Boolean DeleteMeterReading(MeterReading meterReading)
        {
            Boolean deleted = false;

            try
            {
                var existingReading = GetExistingMeterReadingByID(meterReading.ReadingID);

                if (existingReading != null)
                {
                    _context.MeterReading.Remove(existingReading);
                    _context.SaveChanges();

                    deleted = true;
                }
            }
            catch(Exception e)
            {
                _logger.LogError("Error occurred DeleteMeterReading error = " + e.ToString());
            }            

            return deleted;
        }

        public Account FindAccountByID(int id)
        {
            Account account = null;

            try
            {
                account = _context.Account.FirstOrDefault(a => a.AccountId == id);
            }
            catch(Exception e)
            {
                _logger.LogError("Error occurred FindAccountByID error = " + e.ToString());
            }
            return account;
        }

        public Boolean MeterReadingExists(int AccountID,DateTime dateTime)
        {
            Boolean exists = true;

            try
            {
                exists = (_context.MeterReading.FirstOrDefault(m => m.AccountFk == AccountID && m.MeterReadingDateTime == dateTime) != null);
            }
            catch(Exception e)
            {
                _logger.LogError("Error occurred MeterReadingExists error = " + e.ToString());
            }
            return exists;
        }

        public MeterReading GetExistingMeterReadingByID(int ReadingID)
        {
            MeterReading meterReading = null;

            try
            {
                meterReading = _context.MeterReading.FirstOrDefault(m => m.ReadingID == ReadingID);
            }
            catch(Exception e)
            {
                _logger.LogError("Error occurred GetExistingMeterReadingByID error = " + e.ToString());
            }

            return meterReading;
        }

        public Boolean ValidReadingValue(string readingValue)
        {
            return (readingValue.Length == 5 && MeterReadingNumeric(readingValue));
        }

        public Boolean MeterReadingNumeric(string readingValue)
        {
            var regex = new Regex(@"([0-9])");

            return regex.IsMatch(readingValue);
        }
    }
}
