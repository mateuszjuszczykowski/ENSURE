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
    public readonly string collectionName = "RAW";

    public DbHandler()
    {
        log = LoggerConfig.GetLoggerConfiguration().CreateLogger();
        DbClient = new MongoClient(connectionString);
        Database = DbClient.GetDatabase(databaseName);
    }
    
    public void InsertMessage(string payload)
    {
        var collection = Database.GetCollection<BsonDocument>(this.collectionName);
        var document = BsonDocument.Parse(payload);
        collection.InsertOne(document);
    }
}