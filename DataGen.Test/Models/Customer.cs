using System.ComponentModel.DataAnnotations;

namespace DataGen.Test.Models
{
    public record Customer
    {        
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(2)]
        public string State { get; set; }

        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(100)]
        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
