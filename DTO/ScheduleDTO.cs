using backend.Models;

namespace backend.DTO;
    public class ScheduleDTO
    {
    public String? Id { get; set; }
    public String? StartCity { get; set; } = null!;
    public String[]? Cities { get; set; } = null!;
    public String? EndCity { get; set; } = null!;
    public int? Price { get; set; } = null!;
    public String? Train { get; set; } = null!;
    public DateTime? StartTime { get; set; } = null!;
    public DateTime? EndTime { get; set; } = null!;
    public String? Class { get; set; } = null!;
    public String? Type { get; set; } = null!;
    public String? RunBy { get; set; } = null!;
    public Boolean? IsActive { get; set; } = null!;
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
