using backend.Models;
using backend.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Services;

public class TrainService
{
    private readonly IMongoCollection<Train> _trainCollection;
    public TrainService(
        IOptions<DatabaseSetting> databaseSetting)
    {
        var mongoClient = new MongoClient(
            databaseSetting.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSetting.Value.DatabaseName);

        _trainCollection = mongoDatabase.GetCollection<Train>(databaseSetting.Value.TrainCollectionName);
    }

    public async Task<List<Train>> GetTrainAsync() => await _trainCollection.Find(_ => true).ToListAsync();
    public async Task<Train?> GetTrainAsync(string id) => await _trainCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task CreateTrainAsync(Train train) => await _trainCollection.InsertOneAsync(train);
    public async Task UpdateTrainAsync(string id, Train train) => await _trainCollection.ReplaceOneAsync(x => x.Id == id, train);
    public async Task RemoveTrainAsync(string id) => await _trainCollection.DeleteOneAsync(x => x.Id == id);
}
