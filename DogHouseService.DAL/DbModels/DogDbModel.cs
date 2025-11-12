using System.ComponentModel.DataAnnotations.Schema;

namespace DogHouseService.DAL.DbModels
{
    public sealed class DogDbModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public double Tail_length { get; set; }
        public double Weight {  get; set; }
    }
}
