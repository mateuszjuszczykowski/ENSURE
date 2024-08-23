using Microsoft.AspNetCore.Mvc;
using DATABASE_library;
using DATABASE_library.Models.Data;
using Newtonsoft.Json;

namespace API_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly DbHandler _dbHandler;

        public AggregationController(DbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        [HttpGet("GetAggregatedPowerData")]
        public async Task<IActionResult> GetAggregatedPowerData(string deviceId, DateTime start, DateTime end, AggregationType aggregation)
        {
            var data = _dbHandler.GetData("DATA", deviceId, start.ToUniversalTime(), end.ToUniversalTime());
            var values = data.Select(d => new AggregatedDataModel { Timestamp = d.Timestamp, Value = d.Power }).ToList();
            var result = AggregateData(values, aggregation).ToList();
            var response = JsonConvert.SerializeObject(result);
            return Ok(response);
        }

        [HttpGet("GetAggregatedCostData")]
        public async Task<IActionResult> GetAggregatedCostData(string deviceId, DateTime start, DateTime end, AggregationType aggregation, double costPerKWh)
        {
            var data = _dbHandler.GetData("DATA", deviceId, start.ToUniversalTime(), end.ToUniversalTime());
            var values = data.Select(d => new AggregatedDataModel { Timestamp = d.Timestamp, Value = d.Power * costPerKWh });
            var result = AggregateData(values, aggregation);
            var response = JsonConvert.SerializeObject(result);
            return Ok(response);
        }
        
        [HttpGet("GetAggregatedPowerAndCostData")]
        public async Task<IActionResult> GetAggregatedPowerAndCostData(string deviceId, DateTime start, DateTime end, AggregationType aggregation, double costPerKWh)
        {
            var data = _dbHandler.GetData("DATA", deviceId, start.ToUniversalTime(), end.ToUniversalTime());
            var values = data.Select(d => new AggregatedDataModel { Timestamp = d.Timestamp, Value = d.Power });
            var energyData = AggregateData(values, aggregation);
            var costData = energyData.Select(d => new AggregatedDataModel { Timestamp = d.Timestamp, Value = d.Value * costPerKWh });
            var result = new List<IEnumerable<AggregatedDataModel>> { energyData, costData };
            var response = JsonConvert.SerializeObject(result);
            return Ok(response);
        }
        
        private IEnumerable<AggregatedDataModel> AggregateData(IEnumerable<AggregatedDataModel> data, AggregationType aggregation)
        {
            var sortedData = data.OrderBy(d => d.Timestamp).ToList();
            var aggregatedData = new List<AggregatedDataModel>();

            for (int i = 1; i < sortedData.Count; i++)
            {
                var previous = sortedData[i - 1];
                var current = sortedData[i];

                double timeDifferenceInHours = (current.Timestamp - previous.Timestamp).TotalSeconds / 3600.0; // 10 seconds in hours
                double energyUsed = (previous.Value/1000) * timeDifferenceInHours;

                aggregatedData.Add(new AggregatedDataModel
                {
                    Timestamp = current.Timestamp,
                    Value = energyUsed
                });
            }

            return aggregatedData.GroupBy(d => GetAggregationKey(d.Timestamp, aggregation))
                                 .Select(g => new AggregatedDataModel
                                 {
                                     Timestamp = GetAggregationTimestamp(g.Key, aggregation),
                                     Value = Math.Round(g.Sum(d => d.Value), 10)
                                 });
        }
        

        private object GetAggregationKey(DateTime timestamp, AggregationType aggregation)
        {
            return aggregation switch
            {
                AggregationType.Minutely => new { timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, timestamp.Minute },
                AggregationType.QuarterHourly => new { timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, Quarter = timestamp.Minute / 15 },
                AggregationType.Hourly => new { timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour },
                AggregationType.Daily => new { timestamp.Year, timestamp.Month, timestamp.Day },
                AggregationType.Weekly => new { timestamp.Year, Week = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(timestamp, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday) },
                AggregationType.Quarterly => new { timestamp.Year, Quarter = (timestamp.Month - 1) / 3 + 1 },
                AggregationType.Monthly => new { timestamp.Year, timestamp.Month },
                _ => throw new ArgumentException("Invalid aggregation type")
            };
        }

        private DateTime GetAggregationTimestamp(object key, AggregationType aggregation)
        {
            return aggregation switch
            {
                AggregationType.Minutely => new DateTime(((dynamic)key).Year, ((dynamic)key).Month, ((dynamic)key).Day, ((dynamic)key).Hour, ((dynamic)key).Minute, 0),
                AggregationType.QuarterHourly => new DateTime(((dynamic)key).Year, ((dynamic)key).Month, ((dynamic)key).Day, ((dynamic)key).Hour, ((dynamic)key).Quarter * 15, 0),
                AggregationType.Hourly => new DateTime(((dynamic)key).Year, ((dynamic)key).Month, ((dynamic)key).Day, ((dynamic)key).Hour, 0, 0),
                AggregationType.Daily => new DateTime(((dynamic)key).Year, ((dynamic)key).Month, ((dynamic)key).Day),
                AggregationType.Weekly => FirstDateOfWeek(((dynamic)key).Year, ((dynamic)key).Week),
                AggregationType.Quarterly => new DateTime(((dynamic)key).Year, (((dynamic)key).Quarter - 1) * 3 + 1, 1),
                AggregationType.Monthly => new DateTime(((dynamic)key).Year, ((dynamic)key).Month, 1),
                _ => throw new ArgumentException("Invalid aggregation type")
            };
        }

        private DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            DateTime firstMonday = jan1.AddDays(daysOffset);
            var cal = System.Globalization.CultureInfo.InvariantCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstMonday, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            return firstMonday.AddDays(weekNum * 7);
        }
    }
}