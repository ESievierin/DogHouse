namespace DogHouseService.BLL.DTO
{
    public sealed class DogDto
    {
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public double Tail_length { get; set; }
        public double Weight { get; set; }
    }
}
