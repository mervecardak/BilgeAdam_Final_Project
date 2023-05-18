using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities.Concrete;

namespace ApplicationCore.Entities.DTO_s.AccountDTO
{
    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "Bu Alan Zorunludur!")]
        [DisplayName("kullanıcı adı")]
        [MinLength(3, ErrorMessage = "en az 3 karakter girilmelidir....")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bu Alan Zorunludur!")]
        [DisplayName("E-Mail")]
        [DataType(DataType.EmailAddress, ErrorMessage = "emaiil dogru format gir")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Bu Alan Zorunludur!")]
        [DisplayName("Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public UserUpdateDTO() 
        {
        
        }  
        public UserUpdateDTO(ApplicationUser user) 
        {
            UserName = user.UserName;
            Password = user.PasswordHash;
            Email = user.Email;        
        }
    }
}
