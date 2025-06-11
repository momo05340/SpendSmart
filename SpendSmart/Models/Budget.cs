using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Models
{
    public class Budget
    {
         public int Id { get; set; }

        public decimal Value { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public DateOnly Datefrom { get; set; }

        [Required]
        public DateOnly Dateto { get; set; }

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public string? UserId { get; set; }
       

    }
}
