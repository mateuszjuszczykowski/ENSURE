using MongoDB.Bson;

namespace DATABASE_library;

public class DataModel
{
    public ObjectId _id { get; set; }
    public string deviceID { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime TotalStartTime { get; set; }
    public double Total { get; set; }
    public double Today { get; set; }
    public Int32 Power { get; set; }
    public Int32 ApparentPower { get; set; }
    public Int32 ReactivePower { get; set; }
    public double Factor { get; set; }
    public Int32 Voltage { get; set; }
    public double Current { get; set; }
}