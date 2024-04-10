using MongoDB.Bson;

namespace DATA_processor;

public class DataModel
{
    public BsonObjectId _id { get; set; }
    public Payload Payload { get; set; }
    public string deviceID { get; set; }
}

public class Payload
{
    public string Time { get; set; }
    public Energy ENERGY { get; set; }
}

public class Energy
{
    public string TotalStartTime { get; set; }
    public double Total { get; set; }
    public double Yesterday { get; set; }
    public double Today { get; set; }
    public Int32 Period { get; set; }
    public Int32 Power { get; set; }
    public Int32 ApparentPower { get; set; }
    public Int32 ReactivePower { get; set; }
    public double Factor { get; set; }
    public Int32 Voltage { get; set; }
    public double Current { get; set; }
}