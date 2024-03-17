namespace PLUG_emulator;

public class DataGenerator
{
    public DataModel Data { get; set; }
    
    
    public DataGenerator()
    {
        var data = new DataModel
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
        };
        Data = data;
    }
    
    public DataModel GenerateData()
    {
        var random = new Random();
        Data.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Generate a percentage change between -10% and +10%
        double percentageChange = (random.NextDouble() * 0.2) - 0.1; // Random double between -0.1 and 0.1

        // Apply the percentage change to each value
        Data.ENERGY.Total = Math.Round(Data.ENERGY.Total + Data.ENERGY.Total * Math.Abs(percentageChange), 2);
        Data.ENERGY.Today = Math.Round(Data.ENERGY.Today + Data.ENERGY.Today * Math.Abs(percentageChange), 2);
        Data.ENERGY.Period += Data.ENERGY.TotalStartTime == DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") ? 1 : 0;
        Data.ENERGY.Power = (Int32)Math.Round(Data.ENERGY.Power + (Data.ENERGY.Power * percentageChange), 2);
        Data.ENERGY.ApparentPower = (Int32)Math.Round(Data.ENERGY.ApparentPower + (Data.ENERGY.ApparentPower * percentageChange), 2);
        Data.ENERGY.ReactivePower = (Int32)Math.Round(Data.ENERGY.ReactivePower + (Data.ENERGY.ReactivePower * percentageChange), 2);
        Data.ENERGY.Factor = Math.Round(Data.ENERGY.Factor + Data.ENERGY.Factor * percentageChange, 2);
        Data.ENERGY.Voltage = (Int32)Math.Round(Data.ENERGY.Voltage + (Data.ENERGY.Voltage * percentageChange), 2);
        Data.ENERGY.Current = Math.Round(Data.ENERGY.Current + Data.ENERGY.Current * percentageChange, 2);
        return Data;
    }
}