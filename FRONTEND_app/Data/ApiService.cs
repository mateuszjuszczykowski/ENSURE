using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorBootstrap;
using DATABASE_library;
using DATABASE_library.Models.Data;
using Newtonsoft.Json;
using FilterItem = BlazorBootstrap.FilterItem;
using SortDirection = BlazorBootstrap.SortDirection;

namespace FRONTEND_app.Data;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5025";
    private readonly string _dataEndpoint = "/Data";
    private readonly string _aggregationEndpoint = "/Aggregation";
    private readonly string _measurementEndpoint = "/Measurement";
    private readonly string _summaryEndpoint = "/Summary";
    
    public ApiService(HttpClient httpClient)
    {
        if(Environment.GetEnvironmentVariable("API_URL") != null)
        {
            _baseUrl = Environment.GetEnvironmentVariable("API_URL");
        }
        _httpClient = httpClient;
    }
    
    public async Task<List<DataModel>> getAllDataAsync()
    {
        var uri = $"{_baseUrl}{_dataEndpoint}/all";
        return await _httpClient.GetFromJsonAsync<List<DataModel>>(uri);
    }
    
    //todo: could pick deviceid from list of devices
    public async Task<List<DataModel>> getDataAsync(string deviceID, DateTime start, DateTime end)
    {
        var startDate = start.ToUniversalTime().ToString("s");
        var endDate = end.ToUniversalTime().ToString("s");
        var query = $"deviceId={deviceID}&start={startDate}&end={endDate}";
        var uri = $"{_baseUrl}{_dataEndpoint}?{query}";
        var response = await _httpClient.GetAsync(uri);
        var test = await response.Content.ReadAsStringAsync();
        var data = await response.Content.ReadFromJsonAsync<List<DataModel>>();
        return data;
    }

    public async Task<List<AggregatedDataModel>> GetAggregatedPowerData(string deviceid, DateTime start, DateTime end, AggregationType selectedView)
    {
       var startDate = start.ToUniversalTime();
       var endDate = end.ToUniversalTime();
       var query = $"deviceId={deviceid}&start={startDate}&end={endDate}&aggregation={selectedView}";
       var uri = $"{_baseUrl}{_aggregationEndpoint}/GetAggregatedPowerData?{query}";
       return await _httpClient.GetFromJsonAsync<List<AggregatedDataModel>>(uri);
    }
    
    public async Task<List<AggregatedDataModel>> GetAggregatedCostData(string deviceid, DateTime start, DateTime end, AggregationType selectedView, double costPerKWh)
    {
        var startDate = start.ToUniversalTime();
        var endDate = end.ToUniversalTime();
        var query = $"deviceId={deviceid}&start={startDate}&end={endDate}&aggregation={selectedView}&costPerKWh={costPerKWh}";
        var uri = $"{_baseUrl}{_aggregationEndpoint}/GetAggregatedCostData?{query}";
        return await _httpClient.GetFromJsonAsync<List<AggregatedDataModel>>(uri);
    }

    public async Task<List<List<AggregatedDataModel>>>  GetAggregatedPowerAndCostData(string deviceid,
        DateTime start, DateTime end, AggregationType selectedView, double costPerKWh)
    {
        var startDate = start.ToUniversalTime();
        var endDate = end.ToUniversalTime();
        var query = $"deviceId={deviceid}&start={startDate}&end={endDate}&aggregation={selectedView}&costPerKWh={costPerKWh}";
        var uri = $"{_baseUrl}{_aggregationEndpoint}/GetAggregatedPowerAndCostData?{query}";
        var data = await _httpClient.GetFromJsonAsync<List<List<AggregatedDataModel>>>(uri);
        return data;
    }
    
    public async Task<List<MeasurementModel>> GetMeasurements()
    {
        var uri = $"{_baseUrl}{_measurementEndpoint}/all";
        var data = await _httpClient.GetFromJsonAsync<List<MeasurementModel>>(uri);
        return data;
    }
    
    public async Task<MeasurementModel> GetMeasurement(string measurementId)
    {
        var query = $"measurementId={measurementId}";
        var uri = $"{_baseUrl}{_measurementEndpoint}?{query}";
        var data = await _httpClient.GetFromJsonAsync<MeasurementModel>(uri);
        return data;
    }

    public async Task SetMeasurement(string deviceId, string name, string category, DateTime start, DateTime end)
    {
        var query = $"deviceId={deviceId}&name={name}&category={category}&start={start}&end={end}";
        var uri = $"{_baseUrl}{_measurementEndpoint}/set?{query}";
        await _httpClient.PostAsync(uri, null);
    }
    
    public async Task StartMeasurement(string deviceId, string name, string category)
    {
        var query = $"deviceId={deviceId}&name={name}&category={category}";
        var uri = $"{_baseUrl}{_measurementEndpoint}/start?{query}";
        await _httpClient.PostAsync(uri, null);
    }

    public async Task StopMeasurement(string deviceId)
    {
        var query = $"deviceId={deviceId}";
        var uri = $"{_baseUrl}{_measurementEndpoint}/stop?{query}";
        await _httpClient.PostAsync(uri, null);
    }

    public async Task ResumeMeasurement(string deviceId)
    {
        var query = $"deviceId={deviceId}";
        var uri = $"{_baseUrl}{_measurementEndpoint}/resume?{query}";
        await _httpClient.PostAsync(uri, null);
    }

    public async Task EndMeasurement(string deviceId)
    {
        var query = $"deviceId={deviceId}";
        var uri = $"{_baseUrl}{_measurementEndpoint}/end?{query}";
        await _httpClient.PostAsync(uri, null);
    }

    public async Task SetMeasurement(string deviceId, DateTime startDate, DateTime endDate)
    {
        var query = $"deviceId={deviceId}&start={startDate}&end={endDate}";
        var uri = $"{_baseUrl}{_measurementEndpoint}/Set?{query}";
        await _httpClient.PostAsync(uri, null);
    }
    
    public async Task RemoveMeasurement(string measurementId)
    {
        var query = $"measurementId={measurementId}";
        var uri = $"{_baseUrl}{_measurementEndpoint}?{query}";
        await _httpClient.DeleteAsync(uri);
    }

    //Summary
    
    public async Task<List<SummaryModel>> GetSummaryData(string deviceId)
    {
        var query = $"deviceId={deviceId}";
        var uri = $"{_baseUrl}{_summaryEndpoint}?{query}";
        var data = await _httpClient.GetFromJsonAsync<List<SummaryModel>>(uri);
        return data;
    }

    public class SummaryResponse
    {
        public List<SummaryModel> Data { get; set; }
        public int TotalCount { get; set; }
    }
}