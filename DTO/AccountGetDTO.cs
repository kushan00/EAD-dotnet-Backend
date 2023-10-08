namespace backend.DTO;
    public class AccountGetDTO
{
    public String? Name { get; set; } = null!;
    public String? NIC { get; set; }
    public String? Address { get; set; } = null!;
    public String? Number { get; set; } = null!;
    public String? Email { get; set; } = null!;
    public DateTime? DOB { get; set; } = null!;
    public String? Gender { get; set; } = null!;
    public Boolean? IsActive { get; set; } = null!;
    public String? UserRole { get; set; } = null!;
    public DateTime? CreatedTime { get; set; } = null!;
}
