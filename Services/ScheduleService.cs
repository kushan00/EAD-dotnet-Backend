using backend.Models;
using backend.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Services;

public class ScheduleService
{
    private readonly IMongoCollection<Schedule> _scheduleCollection;
    public ScheduleService(
        IOptions<DatabaseSetting> databaseSetting)
    {
        var mongoClient = new MongoClient(
            databaseSetting.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSetting.Value.DatabaseName);

        _scheduleCollection = mongoDatabase.GetCollection<Schedule>(databaseSetting.Value.ScheduleCollectionName);
    }

    public async Task<List<Schedule>> GetScheduleAsync() => await _scheduleCollection.Find(_ => true).ToListAsync();
    public async Task<Schedule?> GetScheduleAsync(string id) => await _scheduleCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task<List<Schedule>> SearchScheduleAsync(string startCity, string endCity, TimeOnly? time)
    {
        return await _scheduleCollection.Find(x => x.Cities.Contains(endCity) && x.Cities.Contains(startCity) && (Array.IndexOf(x.Cities, startCity) < Array.IndexOf(x.Cities, endCity)) &&
    x.StartTime >= time && x.EndTime <= time).ToListAsync();
    }

    public async Task CreateScheduleAsync(Schedule schedule) => await _scheduleCollection.InsertOneAsync(schedule);
    public async Task UpdateScheduleAsync(string id, Schedule schedule) => await _scheduleCollection.ReplaceOneAsync(x => x.Id == id, schedule);
    public async Task RemoveScheduleAsync(string id) => await _scheduleCollection.DeleteOneAsync(x => x.Id == id);
}
