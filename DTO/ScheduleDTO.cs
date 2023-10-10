using backend.Models;

namespace backend.DTO;
    public class ScheduleDTO
    {
    public String? StartCity { get; set; } = null!;
    public List<string>? Cities { get; set; } = null!;
    public String? EndCity { get; set; } = null!;
    public int? Price { get; set; } = null!;
    public String? Train { get; set; } = null!;
    public String? StartTime { get; set; } = null!;
    public String? EndTime { get; set; } = null!;
    public String? Class { get; set; } = null!;
    public String? Type { get; set; } = null!;
    public String? RunBy { get; set; } = null!; // weekend & weekday
    public Boolean? IsActive { get; set; } = null!;
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
