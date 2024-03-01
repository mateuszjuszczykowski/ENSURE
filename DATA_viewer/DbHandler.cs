using DATA_viewer.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DATA_viewer;

public class DbHandler
{
    public MongoClient DbClient { get; set; }
    public IMongoDatabase Database { get; set; }

    public readonly string connectionString = "mongodb://localhost:27017";
    public readonly string databaseName = "ENSURE";
    public readonly string collectionName = "DATA";

    
    public DbHandler()
    {
        DbClient = new MongoClient(connectionString);
        Database = DbClient.GetDatabase(databaseName);
    }
    
    public List<DataDTO> GetAllData()
    {
        var collection = Database.GetCollection<DataDTO>(collectionName);
        var filter = Builders<DataDTO>.Filter.Empty;
        var documents = collection.Find(filter).ToList();
        return documents;
    }
    
    public List<DataDTO> GetDataBetweenDates(DateTime startDate, DateTime endDate)
    {
        var collection = Database.GetCollection<DataDTO>(collectionName);
        var filter = Builders<DataDTO>.Filter.Gte("Timestamp", startDate) & Builders<DataDTO>.Filter.Lte("Timestamp", endDate);
        var documents = collection.Find(filter).ToList();
        return documents;
    }
}