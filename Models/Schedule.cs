using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;
public class Schedule
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public String? Id { get; set; }
    public String? StartCity { get; set; } = null!;
    public String[]? Cities { get; set; } = null!;
    public String? EndCity { get; set; } = null!;
    public int? Price { get; set; } = null!;
    [BsonRepresentation(BsonType.ObjectId)]
    public String? Train { get; set; } = null!;
    public TimeOnly? StartTime { get; set; } = null!;
    public TimeOnly? EndTime { get; set; } = null!;
    public String? Class { get; set; } = null!;
    public String? Type { get; set; } = null!;
    public String? RunBy { get; set; } = null!;
    public Boolean? IsActive { get; set; } = null!;
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.DateTime)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
