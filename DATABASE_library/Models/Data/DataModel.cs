using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace DATABASE_library.Models.Data;

public class DataModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    [JsonPropertyName("deviceID")]
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
    [ForeignKey("MeasurementModel")] 
    public string? MeasurementId { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public virtual MeasurementModel? Measurement { get; set; }
}