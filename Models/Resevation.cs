using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;
public class Resevation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public String? Id { get; set; }
    public String? BookingId { get; set; } = null!;
    [BsonRepresentation(BsonType.ObjectId)]
    public String? User { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public String? Schedule { get; set; } = null!;
    public DateTime? BookedTime { get; set; } = null!;
    public DateTime? ReserveTime { get; set; } = null!;
    public String? StartCity { get; set; } = null!;
    public String? EndCity { get; set; } = null!;
    public int? PaxCount { get; set; } = null!;
    public int? Status { get; set; } = null!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.DateTime)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
