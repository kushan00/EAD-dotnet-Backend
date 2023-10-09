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
    public TimeOnly? StartTime { get; set; } = null!;
    public TimeOnly? EndTime { get; set; } = null!;
    public String? Class { get; set; } = null!;
    public String? Type { get; set; } = null!;
    public String? RunBy { get; set; } = null!; // weekend & weekday
    public Boolean? IsActive { get; set; } = null!;
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
