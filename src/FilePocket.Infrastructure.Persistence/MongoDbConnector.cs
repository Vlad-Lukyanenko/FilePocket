using MongoDB.Driver;


namespace FilePocket.Infrastructure.Persistence
{
    public class MongoDbConnector
    {
        public static IMongoCollection<T> ConnectToMongoDb<T>(
            in string connectionString, 
            in string databaseName, 
            in string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            return database.GetCollection<T>(collectionName);
        }
    }
}
