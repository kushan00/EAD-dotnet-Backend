using backend.Models;

namespace backend.DTO;
    public class ScheduleSearchDTO
    {
    public String? StartCity { get; set; } = null!;
    public String? EndCity { get; set; } = null!;
    public DateTime? StartTime { get; set; } = null!;
    public DateTime? EndTime { get; set; } = null!;
    public String? Class { get; set; } = null!;
    public String? Type { get; set; } = null!;
}
