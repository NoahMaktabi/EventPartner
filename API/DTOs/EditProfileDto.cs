using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class EditProfileDto
    {
        [Required(ErrorMessage = "The Display Name Can Not Be Empty")]
        public string DisplayName { get; set; }
        public string Bio { get; set; }
    }
}
