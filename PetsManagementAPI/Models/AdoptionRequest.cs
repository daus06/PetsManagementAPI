using System;
using System.ComponentModel.DataAnnotations;

namespace PetsManagementAPI.Models
{
    public class AdoptionRequest
    {
        [Key]
        public int AdoptID { get; set; }

        public int UserID { get; set; }

        public int PetID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } // "Pending", "Approved", "Rejected"

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public User User { get; set; } // Navigation property

        public Pet Pet { get; set; } // Navigation property
    }
}