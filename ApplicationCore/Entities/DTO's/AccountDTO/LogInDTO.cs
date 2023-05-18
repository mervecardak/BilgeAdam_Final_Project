using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.DTO_s.AccountDTO
{
    public class LogInDTO
    {

        [Required(ErrorMessage = "Bu Alan Zorunludur!")]
        [DisplayName("kullanıcı adı")]
        public string UserName { get; set; }

       
        [Required(ErrorMessage = "Bu Alan Zorunludur!")]
        [DisplayName("Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
