using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using PetsManagementAPI.Models;

namespace PetsManagementAPI.Models
{
    public class Pet
    {
        [Key]
        public int PetID { get; set; }

        [Required]
        [MaxLength(50)]
        public string PetName { get; set; }

        [Required]
        [MaxLength(20)]
        public string PetType { get; set; } // "Cat" or "Dog"

        public int Age { get; set; } // Now in months

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;

        public bool Neutered { get; set; }

        public bool Vaccine { get; set; } // True = Fully Vaccinated, False = Not Vaccinated

        [MaxLength(50)]
        public string Breed { get; set; } // e.g., "Persian", "Labrador"
    }
}