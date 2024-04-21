using System.Runtime.InteropServices;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DATABASE_library;

public class DbHandler
{
    public MongoClient DbClient { get; set; }
    public IMongoDatabase Database { get; set; }
    
    public readonly string connectionString = "mongodb://localhost:27017";
    public readonly string databaseName = "ENSURE";
    // public readonly string collectionName = "RAW";
    // public readonly string collectionName = "DATA";
    
    public DbHandler()
    {
        DbClient = new MongoClient(connectionString);
        Database = DbClient.GetDatabase(databaseName);
    }
    
    public void InsertMessage(string payload, string collectionName)
    {
        var collection = Database.GetCollection<BsonDocument>(collectionName);
        var document = BsonDocument.Parse(payload);
        collection.InsertOne(document);
    }

    public List<DataModel> GetAllData(string collectionName)
    {
        var collection = Database.GetCollection<DataModel>(collectionName);
        var filter = Builders<DataModel>.Filter.Empty;
        var documents = collection.Find(filter).ToList();
        return documents;
    }
    
    public List<DataModel> GetData(string collectionName, [Optional] string filter, [Optional] int limit)
    {
        var collection = Database.GetCollection<DataModel>(collectionName);
        var documents = collection.Find(filter).Limit(limit).ToList();
        return documents;
    }
    
    public List<DataModel> GetDataBetweenDates(string collectionName, DateTime startDate, DateTime endDate)
    {
        var collection = Database.GetCollection<DataModel>(collectionName);
        var filter = Builders<DataModel>.Filter.Gte("Timestamp", startDate) & Builders<DataModel>.Filter.Lte("Timestamp", endDate);
        var documents = collection.Find(filter).ToList();
        return documents;
    }
    
    // data processor methods
    
    public List<BsonDocument> GetMessages(string collectionName)
    {
        var sourceCollection = Database.GetCollection<BsonDocument>(collectionName);
        var filter = Builders<BsonDocument>.Filter.Empty;
        var documents = sourceCollection.Find(filter).ToList();
        return documents;

    }
    
    public void InsertData(DataModel data, string collectionName)
    {
        var destinationCollection = Database.GetCollection<DataModel>(collectionName);
        destinationCollection.InsertOne(data);
    }
    
    public void RemoveMessage(BsonDocument message, string collectionName)
    {
        var sourceCollection = Database.GetCollection<BsonDocument>(collectionName);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", message["_id"]);
        sourceCollection.DeleteOne(filter);
    }
}