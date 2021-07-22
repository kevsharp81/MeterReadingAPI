using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using ENSEK_meter_readings_API.CORE.BAL;
using ENSEK_meter_readings_API.CORE.DAL;
using ENSEK_meter_readings_API.Models;
namespace ENSEK_meter_readings_API.Controllers
{
    
    [ApiController]
    public class MeterReadingsController : ControllerBase
    {
        private IMeterReadingsProcessingLayer _meterReadingsProcessingLayer;

        public MeterReadingsController(IMeterReadingsProcessingLayer meterReadingsProcessingLayer)
        {
            _meterReadingsProcessingLayer = meterReadingsProcessingLayer;
        }

        [HttpPost("meter-reading-uploads")]
        public ActionResult<DataTable> ProcessMeterReadings(IFormFile file)
        {
            String CSVPathAndName = _meterReadingsProcessingLayer.StoreCSVFile(file);

            if(CSVPathAndName !=null && CSVPathAndName.Trim()!="")
            {
                DataTable ProcessedMeterReadings = _meterReadingsProcessingLayer.ProcessMeterReadings(CSVPathAndName);
                return Ok(ProcessedMeterReadings);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("API-online")]
        public ActionResult APIOnline()
        {
            return Ok();
        }
    }
}
