using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.IO;
using System.Globalization;
using ENSEK_meter_readings_API.Models;
namespace ENSEK_meter_readings_API.Models
{
    public class EnsekContext : DbContext
    {
        public EnsekContext(DbContextOptions options):base(options)
        {

        }
       

       protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var path = Path.Combine(
                      Directory.GetCurrentDirectory(), "dataSources",
                      "Test_Accounts.csv");
            using(var reader = new StreamReader(path))
            {
                using(var csv = new CsvReader(reader,CultureInfo.InvariantCulture))
                {
                    var seedRecords = new List<Account>();
                    csv.Read();
                    csv.ReadHeader();

                    while(csv.Read())
                    {
                        var seedRecord = new Account
                        {
                            AccountId = csv.GetField<int>("AccountId"),
                            FirstName = csv.GetField("FirstName"),
                            LastName = csv.GetField("LastName")
                        };

                        seedRecords.Add(seedRecord);
                    }

                    if(seedRecords.Count>0)
                    {
                        modelBuilder.Entity<Account>().HasData(seedRecords);
                    }


                }
            }
        }
       
        public DbSet<Account> Account { get; set; }
        public DbSet<MeterReading> MeterReading { get; set; }
    }

}
