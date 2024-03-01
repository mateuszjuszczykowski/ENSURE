using DATA_processor;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mqtt_client;

public class DbHandler
{
    public MongoClient DbClient { get; set; }
    public IMongoDatabase Database { get; set; }
    public ILogger log { get; set; }
    
    public readonly string connectionString = "mongodb://localhost:27017";
    public readonly string databaseName = "ENSURE";
    public readonly string sourceCollectionName = "RAW";
    public readonly string destinationCollectionName = "DATA";

    public DbHandler()
    {
        log = LoggerConfig.GetLoggerConfiguration().CreateLogger();
        DbClient = new MongoClient(connectionString);
        Database = DbClient.GetDatabase(databaseName);
    }
    
    
    public List<BsonDocument> GetMessages()
    {
        var sourceCollection = Database.GetCollection<BsonDocument>(sourceCollectionName);
        var filter = Builders<BsonDocument>.Filter.Empty;
        var documents = sourceCollection.Find(filter).ToList();
        return documents;

    }
    
    public void InsertData(DataDTO data)
    {
        var destinationCollection = Database.GetCollection<DataDTO>(destinationCollectionName);
        destinationCollection.InsertOne(data);
    }
    
    public void RemoveMessage(BsonDocument message)
    {
        var sourceCollection = Database.GetCollection<BsonDocument>(sourceCollectionName);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", message["_id"]);
        sourceCollection.DeleteOne(filter);
    }
}