using backend.Models;

namespace backend.DTO;
public class ScheduleSearchDTO
{
    public String? StartCity { get; set; } = null!;
    public String? EndCity { get; set; } = null!;
    public String? Time { get; set; } = null!;
}
