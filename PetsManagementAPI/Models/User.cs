using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using PetsManagementAPI.Models;

namespace PetsManagementAPI.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string PasswordHash { get; set; } // Hash in production

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}