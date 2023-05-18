using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.DTO_s.RolesDTO
{
    public class RoleDTO
    {
        [Required(ErrorMessage = "bu alan zorunludur")]
        [MinLength(3,ErrorMessage ="en az 3 karakter giriniz.....")]
        public string Name { get; set; }    
    }
}
