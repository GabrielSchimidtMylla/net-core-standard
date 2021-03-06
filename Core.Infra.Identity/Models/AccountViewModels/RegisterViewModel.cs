﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Infra.Identity.Models.AccountViewModels
{
  public class RegisterViewModel
  {
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O nome é requerido")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O CPF é requerido")]
    [StringLength(11)]
    public string CPF { get; set; }

    [Required(ErrorMessage = "O e-mail é requerido")]
    [EmailAddress(ErrorMessage = "E-mail em formato inválido")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmação senha")]
    [Compare("Password", ErrorMessage = "A confirmação de senha e senha não são iguais.")]
    public string ConfirmPassword { get; set; }
  }
}
