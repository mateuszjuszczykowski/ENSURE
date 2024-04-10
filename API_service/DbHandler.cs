using System.Runtime.InteropServices;
using Microsoft.Extensions.Primitives;

namespace API_service;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
    

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
    
    public List<DataModel> GetAllData()
    {
        var collection = Database.GetCollection<DataModel>(collectionName);
        var filter = Builders<DataModel>.Filter.Empty;
        var documents = collection.Find(filter).ToList();
        return documents;
    }
    
    public List<DataModel> GetData([Optional] string filter, [Optional] int limit)
    {
        var collection = Database.GetCollection<DataModel>(collectionName);
        var documents = collection.Find(filter).Limit(limit).ToList();
        return documents;
    }
    
    public List<DataModel> GetDataBetweenDates(DateTime startDate, DateTime endDate)
    {
        var collection = Database.GetCollection<DataModel>(collectionName);
        var filter = Builders<DataModel>.Filter.Gte("Timestamp", startDate) & Builders<DataModel>.Filter.Lte("Timestamp", endDate);
        var documents = collection.Find(filter).ToList();
        return documents;
    }
}