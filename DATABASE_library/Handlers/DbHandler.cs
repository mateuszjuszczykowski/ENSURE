using System.Runtime.InteropServices;
using System.Text.Json;
using DATABASE_library.Models.Data;
using DATABASE_library.Models.User;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// using MongoDB.Bson;
// using MongoDB.Driver;

namespace DATABASE_library;

public class DbHandler
{
    public AppDbContext Context {get; set;}
    // public readonly string collectionName = "RAW";
    // public readonly string collectionName = "DATA";
    
    public DbHandler(AppDbContext context)
    {
        Context = context;
    }
    
    //processor
    public void InsertRawMessage(string payloadWithId)
    {
        var model = JsonConvert.DeserializeObject<RawDataModel>(payloadWithId);
        Context.RawData.Add(model);
        Context.SaveChanges();
    }
    
    public List<RawDataModel> GetRawMessages()
    {
        var rawMessages = Context.RawData.ToList();
        return rawMessages;
    } 
    
    public void InsertData(DataModel data, string collectionName)
    {
        var collection = Context.Data;
        data.Timestamp = DateTime.SpecifyKind(data.Timestamp, DateTimeKind.Utc);
        Console.WriteLine(data.Timestamp);
        collection.Add(data);
        Context.SaveChanges();
    }
    
    public void RemoveMessage(RawDataModel message, string collectionName)
    {
        var collection = Context.RawData;
        collection.Remove(message);
        Context.SaveChanges();
    }
    
    //Data
    public List<DataModel> GetDataBetweenDates(string collectionName, DateTime startDate, DateTime endDate)
    {
        var start = startDate.ToUniversalTime();
        var end = endDate.ToUniversalTime();
        var collection = Context.Data;
        var documents = collection
            .Where(d => d.Timestamp >= start && d.Timestamp <= end)
            .OrderBy(d => d.Timestamp)
            .ToList();
        return documents;
    }
    
    public List<DataModel> GetAllData(string collectionName)
    {
        var collection = Context.Data;
        var documents = collection
            .Include(d => d.Measurement)
            .OrderBy(d => d.Timestamp)
            .ToList();
        return documents;
    }
    
    public List<DataModel> GetData(string collectionName, [Optional] string deviceId, [Optional]  DateTime startDate, [Optional] DateTime endDate)
    {
        var documents = Context.Data
            .Where(d => d.deviceID == deviceId &&
                        d.Timestamp >= startDate &&
                        d.Timestamp <= endDate)
            .ToList();
        return documents;
    }
    
    public void DeleteData(string data, string deviceId, DateTime startDate, DateTime endDate) 
    {
        var start = startDate.ToUniversalTime();
        var end = endDate.ToUniversalTime();
        var collection = Context.Data;
        var documents = collection
            .Where(d => d.deviceID == deviceId &&
                        d.Timestamp >= start &&
                        d.Timestamp <= end)
            .ToList();
        collection.RemoveRange(documents);
        Context.SaveChanges();
    }

    //Measurement - API TO BE TESTED & FIXED
    public void SetMeasurement(string deviceId, string measurementName, string measurementCategory, DateTime startTime, DateTime endTime)
    {
        var startTimeUtc = DateTime.SpecifyKind(startTime.ToUniversalTime(), DateTimeKind.Utc);
        var endTimeUtc = DateTime.SpecifyKind(endTime.ToUniversalTime(), DateTimeKind.Utc);
        var data = Context.Data.Where(d => d.deviceID == deviceId && d.Timestamp >= startTimeUtc && d.Timestamp <= endTimeUtc).ToList();
        var measurement = new MeasurementModel
        {
            DeviceId = deviceId,
            StartTime = startTimeUtc,
            EndTime = endTimeUtc,
            Name = measurementName,
            Category = measurementCategory,
            isFinished = true,
            Data = data
        };
        Context.Measurements.Add(measurement);
        Context.SaveChanges();
    }
    
    public void StartMeasurement(string deviceId, string measurementName, string measurementCategory)
    {
        //check if there is an ongoing measurement
        var measurement = Context.Measurements.FirstOrDefault(m => m.isFinished == false && m.DeviceId == deviceId);
        if (measurement == null)
        {
            var newMeasurement = new MeasurementModel
            {
                DeviceId = deviceId,
                StartTime = DateTime.UtcNow,
                Name = measurementName,
                Category = measurementCategory,
                isFinished = false
            };
            Context.Measurements.Add(newMeasurement);
            Context.SaveChanges();
            return;
        }
        measurement.StartTime = DateTime.UtcNow;
        Context.SaveChanges();
    }

    public void StopMeasurement(string deviceId)
    {
        var measurement = Context.Measurements.FirstOrDefault(m => m.DeviceId == deviceId && m.isFinished == false);
        if (measurement != null)
        {
            measurement.EndTime = DateTime.UtcNow;
            Context.SaveChanges();
        }
    }

    public void ResumeMeasurement(string deviceId)
    {
        var measurement = Context.Measurements.FirstOrDefault(m => m.DeviceId == deviceId && m.isFinished == false);
        if (measurement != null)
        {
            measurement.EndTime = null;
            Context.SaveChanges();
        }
    }

    public void EndMeasurement(string deviceId)
    {
        var measurement = Context.Measurements.FirstOrDefault(m => m.DeviceId == deviceId && m.isFinished == false);
        if (measurement != null)
        {
            measurement.EndTime = DateTime.UtcNow;
            measurement.isFinished = true;
            Context.SaveChanges();
        }
    }

    public List<MeasurementModel> GetMeasurements()
    {
        var measurements = Context.Measurements
            .Include(e => e.Data).ToList();
        return measurements;
    }

    public MeasurementModel GetMeasurement(string id)
    {
        var collection = Context.Measurements;
        var measurement = collection.Include(e => e.Data).FirstOrDefault(m => m._id == id);
        return measurement;
    }

    public object? GetMeasurementData(string id)
    {
        var collection = Context.Measurements;
        var measurement = collection.Include(measurementModel => measurementModel.Data).FirstOrDefault(m => m._id == id);
        var data = measurement?.Data;
        return data;
    }


    public MeasurementModel? GetCurrentMeasurement(string dataDtoDeviceId)
    {
        var measurement = Context.Measurements.FirstOrDefault(m => m.DeviceId == dataDtoDeviceId && m.isFinished == false);
        return measurement;
    }

    public void UpdateMeasurement(string measurementId, DataModel dataDto)
    {
        var measurement = Context.Measurements.FirstOrDefault(m => m._id == measurementId);
        if (measurement != null)
        {
            measurement.Data.Add(dataDto);
            Context.SaveChanges();
        }
    }

    public void DeleteMeasurement(string measurementId)
    {   
        var measurement = Context.Measurements.FirstOrDefault(m => m._id == measurementId);
        if (measurement == null) return;
        
        var data = Context.Data.Where(d => d.MeasurementId == measurementId).ToList();
        foreach (var record in data)
        {
            record.MeasurementId = null;
        }
        
        Context.Measurements.Remove(measurement);
        Context.SaveChanges();
    }


}