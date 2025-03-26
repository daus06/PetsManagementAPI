using System.ComponentModel.DataAnnotations;

namespace PetsManagementAPI.Models
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Username { get; set; }
    }
}