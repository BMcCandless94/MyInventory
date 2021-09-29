using System;
using System.ComponentModel.DataAnnotations;
using MILibrary.Constants;

namespace MyInventory.Areas.UserManagement.Models
{
    public class IndexViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display (Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Date Account Created")]
        public DateTime CreatedDTTM { get; set; }
    }

    public class SignInViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(Constants.USR_EMAIL_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(Constants.USR_PASSWORD_MAXLENGTH, MinimumLength = Constants.USR_PASSWORD_MINLENGTH, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        public string Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(Constants.USR_EMAIL_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(Constants.USR_PASSWORD_MAXLENGTH, MinimumLength = Constants.USR_PASSWORD_MINLENGTH, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The two passwords do not match.")]
        [StringLength(Constants.USR_PASSWORD_MAXLENGTH, MinimumLength = Constants.USR_PASSWORD_MINLENGTH, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(Constants.USR_FIRSTNAME_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(Constants.USR_LASTNAME_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string LastName { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(Constants.USR_EMAIL_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Email { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public int UserID { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(Constants.USR_PASSWORD_MAXLENGTH, MinimumLength = Constants.USR_PASSWORD_MINLENGTH, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(Constants.USR_PASSWORD_MAXLENGTH, MinimumLength = Constants.USR_PASSWORD_MINLENGTH, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The two passwords do not match.")]
        [StringLength(Constants.USR_PASSWORD_MAXLENGTH, MinimumLength = Constants.USR_PASSWORD_MINLENGTH, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        public string ConfirmNewPassword { get; set; }
    }

    public class UpdateUserViewModel
    {
        public int UserID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(Constants.USR_FIRSTNAME_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(Constants.USR_LASTNAME_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(Constants.USR_EMAIL_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Email { get; set; }
    }

    public class DisableUserViewModel
    {
        public int UserID { get; set; }
    }
}