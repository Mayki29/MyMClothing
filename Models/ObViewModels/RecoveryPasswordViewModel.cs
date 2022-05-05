using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMClothing.Models.ObViewModels
{
    public class RecoveryPasswordViewModel
    {
        public string token { get; set; }
        [Required]
        [DisplayName("Contraseña")]
        [StringLength(20, ErrorMessage = "La contraseña debe tener al menos 6 caracteres y maximo 20", MinimumLength = 6)]
        public string Password { get; set; }
        [Compare("Password")]
        [DisplayName("Confirmar contraseña")]
        [Required]
        public string ConfirmPassword { get; set; }
    }
}