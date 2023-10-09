namespace backend.DTO;
    public class TrainDTO
    {
    public String? Id { get; set; }
    public String? TrainId { get; set; } = null!;
    public String? Name { get; set; } = null!;
    public String? SeatingCapacity { get; set; } = null!;
    public String? FuelType { get; set; } = null!;
    public String? Model { get; set; } = null!;
    public Boolean? IsActive { get; set; } = null!;
}
