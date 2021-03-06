using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMGJ.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }
    public class Perdoruesit
    {
        public string Perdoruesi { get; set; }
        public string Institucioni { get; set; }
        public string InstitucioniEN { get; set; }
        public string InstitucioniSR { get; set; }
        public string Email { get; set; }
        public string RoliKryesor { get; set; }
        public bool AktivNeInstitucion { get; set; }
        public string User { get; set; }
        public string UserID { get; set; }
        public int ID { get; set; }  
        public int InstitucioniID { get; set; }
        public string RoliKryesorID { get; set; }
        public int? FermaID { get; set; }
        public string Ferma { get; set; }


    }
    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        //[EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
       // [Required]
        //[EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Plot??soni fush??n!")]
        [Display(Name = "UserName")] 
        public string UserName { get; set; } 
        public string Password { get; set; }
         
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Plot??soni fush??n!")]
        public string Emri { get; set; }

        [Required(ErrorMessage = "Plot??soni fush??n!")]
        public string Mbiemri { get; set; }

        public DateTime? Ditelindja { get; set; }
        public string Adresa { get; set; }
        public string Telefoni { get; set; }
        //[Required(ErrorMessage = "Plot??soni fush??n!")]
        public string NumriPersonal { get; set; } 
        public bool Statusi { get; set; }
        [Required]
        public int RoleID { get; set; } 
        public int InstitucioniID { get; set; }
        public int Gjinia { get; set; }
        public int FermaID { get; set; }
    }
    public class Editimi : RegisterViewModel
    { 
        public int ID { get; set; } 
    }
    public class ResetPasswordViewModel
    {
        //[Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public string UserId { get; set; }

    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class RegisterViewModelUser
    {
        public int ID { get; set; }
        public string username { get; set; }

        [Required(ErrorMessage = "Plot??soni fush??n!")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAdresa { get; set; }

        [Required(ErrorMessage = "Plot??soni fush??n!")]
        [StringLength(100, ErrorMessage = "Fjal??kalimi duhet t?? jet?? minimum {2} karaktere.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("UserPassword", ErrorMessage = "Fjal??kalimi dhe konfirmo fjal??kalimin nuk p??rputhen.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Plot??soni fush??n!")]
        public string Emri { get; set; }

        [Required(ErrorMessage = "Plot??soni fush??n!")]
        public string Mbiemri { get; set; }

        [Required(ErrorMessage = "Plot??soni fush??n!")]
        public int Gjinia { get; set; }

        [Required(ErrorMessage = "Plot??soni fush??n!")]
        public int KomunaID { get; set; }
         
        public string NumriLeternjoftimit { get; set; }

        public USER user { get; set; }
        public FERMA FERMA { get; set; }
        public int FermaID { get; internal set; }
    }

 


}
