using backend.Models;
using backend.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace backend.Services;

public class ReservationService
{
    private readonly IMongoCollection<Reservation> _reservationCollection;
    public ReservationService(
        IOptions<DatabaseSetting> databaseSetting)
    {
        var mongoClient = new MongoClient(
            databaseSetting.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSetting.Value.DatabaseName);

        _reservationCollection = mongoDatabase.GetCollection<Reservation>(databaseSetting.Value.ReservationCollectionName);
    }

    public async Task<List<Reservation>> GetReservationAsync() => await _reservationCollection.Find(_ => true).ToListAsync();
    public async Task<Reservation?> GetReservationAsync(string id) => await _reservationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task<List<Reservation>> GetByUserReservationAsync(string id) => await _reservationCollection.Find(x => x.User == id).ToListAsync();
    public async Task CreateReservationAsync(Reservation reservation) => await _reservationCollection.InsertOneAsync(reservation);
    public async Task UpdateReservationAsync(string id, Reservation reservation) => await _reservationCollection.ReplaceOneAsync(x => x.Id == id, reservation);
    public async Task RemoveReservationAsync(string id) => await _reservationCollection.DeleteOneAsync(x => x.Id == id);
    public async Task<long> GetCountReservationAsync() => await _reservationCollection.EstimatedDocumentCountAsync();
    public async Task<long> GetCountUserReservationAsync(string UserId) => await _reservationCollection.CountDocumentsAsync(x => x.User == UserId && x.IsAgentBooked == true && ((TimeSpan)(x.ReserveTime - DateTime.UtcNow)).TotalDays > 0);
    public async Task<long> GetCountScheduleReservationAsync(string ScheduleId) => await _reservationCollection.CountDocumentsAsync(x => x.Schedule == ScheduleId && ((TimeSpan)(x.ReserveTime - DateTime.UtcNow)).TotalDays > 0);
}
