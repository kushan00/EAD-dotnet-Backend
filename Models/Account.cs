using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;
public class Account
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public String? Id { get; set; }
    public String? Name { get; set; } = null!;
    public String? NIC { get; set; }
    public String? Address { get; set; } = null!;
    public String? Number { get; set; } = null!;
    public String? Email { get; set; } = null!;
    public byte[]? Password { get; set; } = null!;
    public DateTime? DOB { get; set; } = null!;
    public String? Gender { get; set; } = null!;
    public Boolean? IsActive { get; set; } = null!;
    public byte[]? Salt { get; set; } = null!;
    public String? UserRole { get; set; } = null!;
    public DateTime? Create_at { get; set; } = null!;
    public DateTime? Update_at { get; set; } = null!;
}
