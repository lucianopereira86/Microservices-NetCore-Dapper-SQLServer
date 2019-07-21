using System.ComponentModel.DataAnnotations;

namespace APITestRegister.Presentation.WebAPI.Models.VM
{
    public class UserRegisterVM: UserLoginVM
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string name { get; set; }
    }
}
