using System.ComponentModel.DataAnnotations;

namespace APITestRegister.Presentation.WebAPI.Models.VM
{
    public class UserLoginVM
    {
        /// <summary>
        /// E-mail
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string password { get; set; }
    }
}
