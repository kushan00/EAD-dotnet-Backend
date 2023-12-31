using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;
public class Train
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public String? Id { get; set; }
    public String? TrainId { get; set; } = null!;
    public String? Name { get; set; } = null!;
    public String? SeatingCapacity { get; set; } = null!;
    public String? FuelType { get; set; } = null!;
    public String? Model { get; set; } = null!;
    public Boolean? IsActive { get; set; } = null!;
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.DateTime)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
