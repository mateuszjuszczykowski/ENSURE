using DATABASE_library.Models.Data;

namespace PLUG_emulator;
using DATABASE_library;
public class DataGenerator
{
    public RawDataModel Data { get; set; }
    
    public DataGenerator()
    {
        var data = new RawDataModel()
        {
            DeviceID = "21370911420",
            Payload = new Payload()
            {
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ENERGY = new Energy
                {
                    TotalStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Total = 245.0,
                    Yesterday = 245.0,
                    Today = 0.0,
                    Period = 0,
                    Power = 60,
                    ApparentPower = 60,
                    ReactivePower = 0,
                    Factor = 1.0,
                    Voltage = 230,
                    Current = 0.26
                }
            }
        };
        Data = data;
    }
    //{"Time":"2024-07-27T12:18:06","ENERGY":{"TotalStartTime":"2024-02-06T19:47:36","Total":11.487,"Yesterday":0.046,"Today":0.001,"Period":0,"Power":0,"ApparentPower":0,"ReactivePower":0,"Factor":0.00,"Voltage":281,"Current":0.000}}
    public RawDataModel GenerateData()
    {
        var random = new Random();
        Data.Payload.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Generate a percentage change between -10% and +10%
        double percentageChange = (random.NextDouble() * 0.2) - 0.1; // Random double between -0.1 and 0.1

        // Apply the percentage change to each value
        Data.Payload.ENERGY.Total = Math.Round(Data.Payload.ENERGY.Total + Data.Payload.ENERGY.Total * Math.Abs(percentageChange), 2);
        Data.Payload.ENERGY.Today = Math.Round(Data.Payload.ENERGY.Today + Data.Payload.ENERGY.Today * Math.Abs(percentageChange), 2);
        Data.Payload.ENERGY.Period += Data.Payload.ENERGY.TotalStartTime == DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") ? 1 : 0;
        Data.Payload.ENERGY.Power = (Int32)Math.Round(Data.Payload.ENERGY.Power + (Data.Payload.ENERGY.Power * percentageChange), 2);
        Data.Payload.ENERGY.ApparentPower = (Int32)Math.Round(Data.Payload.ENERGY.ApparentPower + (Data.Payload.ENERGY.ApparentPower * percentageChange), 2);
        Data.Payload.ENERGY.ReactivePower = (Int32)Math.Round(Data.Payload.ENERGY.ReactivePower + (Data.Payload.ENERGY.ReactivePower * percentageChange), 2);
        Data.Payload.ENERGY.Factor = Math.Round(Data.Payload.ENERGY.Factor + Data.Payload.ENERGY.Factor * percentageChange, 2);
        Data.Payload.ENERGY.Voltage = (Int32)Math.Round(Data.Payload.ENERGY.Voltage + (Data.Payload.ENERGY.Voltage * percentageChange), 2);
        Data.Payload.ENERGY.Current = Math.Round(Data.Payload.ENERGY.Current + Data.Payload.ENERGY.Current * percentageChange, 2);
        return Data;
    }
}