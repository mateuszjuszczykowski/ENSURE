namespace DATABASE_library.Models.Data;

public class SummaryModel
{
    public string Category { get; set; }
    public string Name { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public int NumberOfRecords { get; set; }
    public double TotalUsage { get; set; }
    public double TotalPrice { get; set; }
}