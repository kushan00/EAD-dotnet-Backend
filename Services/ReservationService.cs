using backend.Models;
using backend.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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

    public async Task<Reservation?> GetReservationLogin(string BookingId) => await _reservationCollection.Find(x => x.BookingId == BookingId).FirstOrDefaultAsync();
    public async Task<List<Reservation>> GetReservationAsync() => await _reservationCollection.Find(_ => true).ToListAsync();
    public async Task<Reservation?> GetReservationAsync(string id) => await _reservationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task CreateReservationAsync(Reservation reservation) => await _reservationCollection.InsertOneAsync(reservation);
    public async Task UpdateReservationAsync(string id, Reservation reservation) => await _reservationCollection.ReplaceOneAsync(x => x.Id == id, reservation);
    public async Task RemoveReservationAsync(string id) => await _reservationCollection.DeleteOneAsync(x => x.Id == id);
}
