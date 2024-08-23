using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DATABASE_library.Models.Data;

public class MeasurementModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string _id { get; set; }
    public string? Category { get; set; } = "default";
    public string? Name { get; set; } = $"ms_{DateTime.Now:MM/dd_HH:mm:ss}";
    public DateTime? StartTime { get; set; } = null;
    public DateTime? EndTime { get; set; } = null;
    public string DeviceId { get; set; }
    public bool isFinished { get; set; } = false;
    public virtual List<DataModel> Data { get; set; } = new List<DataModel>();
}