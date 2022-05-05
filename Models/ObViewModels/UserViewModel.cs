using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyMClothing.Models.ObViewModels
{
    public class UserViewModel
    {
        public int id { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "El nombre debe tener al menos 1 caracter y maximo 20", MinimumLength = 1)]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "El apellido debe tener al menos 1 caracter y maximo 20", MinimumLength = 1)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La direccion debe tener al menos 1 caracter y maximo 100", MinimumLength = 1)]
        [Display(Name = "Direccion")]
        public string Direccion { get; set; }

        [Required]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Solo se puede utilizar numeros")]
        [StringLength(8, ErrorMessage = "El DNI debe tener exactamente 8 caracteres", MinimumLength = 8)]
        [Display(Name = "DNI")]
        public string Dni { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "El correo electronico debe tener al menos 1 caracteres y maximo 20", MinimumLength = 1)]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [StringLength(20, ErrorMessage = "La contraseña debe tener al menos 6 caracteres y maximo 20", MinimumLength = 6)]
        public string Password { get; set; }
        [Display(Name = "Confirmar Contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no son iguales")]
        public string ConfirmPassword { get; set; }
        [Required]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Solo se puede utilizar numeros")]
        public string Edad { get; set; }
    }

    public class UserViewModelPer
    {
        public int id { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "El nombre debe tener al menos 1 caracter y maximo 20", MinimumLength = 1)]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "El apellido debe tener al menos 1 caracter y maximo 20", MinimumLength = 1)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La direccion debe tener al menos 1 caracter y maximo 100", MinimumLength = 1)]
        [Display(Name = "Direccion")]
        public string Direccion { get; set; }

        [Required]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Solo se puede utilizar numeros")]
        [StringLength(8, ErrorMessage = "El DNI debe tener exactamente 8 caracteres", MinimumLength = 8)]
        [Display(Name = "DNI")]
        public string Dni { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "El correo electronico debe tener al menos 1 caracteres y maximo 20", MinimumLength = 1)]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Solo se puede utilizar numeros")]
        public string Edad { get; set; }
    }
}