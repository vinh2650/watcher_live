using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        /// <summary>
        /// FirstName
        /// </summary>
        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// LastName
        /// </summary>
        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Confirm Password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        /// <summary>
        /// PhoneNumber
        /// </summary>
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// user info result
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// id of user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// username of user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// firstname of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// lastname of user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// phonenumber of user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Count of user relationship
        /// </summary>
        public int RelationCount { get; set; }

        /// <summary>
        /// DefaultLocation of BTS
        /// </summary>
        public LocationResponse DefaultLocation { get; set; }
    }

    /// <summary>
    /// Ondemand request status
    /// </summary>
    public class CheckingOndemandStatus
    {
        /// <summary>
        /// Message that will be display to customer
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Current status of on-demand request
        /// </summary>
        public OndemandStatusEnum Status { get; set; } 

        /// <summary>
        /// The time that on-demand will or be completed
        /// </summary>
        public DateTime? CompletingDateTimeUtc { get; set; }

        /// <summary>
        /// The last time that event come to SA when user try to upload file into our system
        /// </summary>
        public DateTime? LastEventTimeUtc { get; set; }
    }
    /// <summary>
    /// Status of ondemand checking
    /// </summary>
    public enum OndemandStatusEnum
    {
        /// <summary>
        /// Not upload before
        /// </summary>
        NotUpload = -1,
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 0,
        /// <summary>
        /// Processing
        /// </summary>
        Processing = 1,
        /// <summary>
        /// Success
        /// </summary>
        Success = 2,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 3

    }

    /// <summary>
    /// library model
    /// </summary>
    public class LibraryModel
    {
        /// <summary>
        /// constructor library model
        /// </summary>
        public LibraryModel()
        {
            Organization = new OrganizationModel();
        }

        /// <summary>
        /// id of library
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// name of library
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// band of library
        /// </summary>
        public string Band { get; set; }

        /// <summary>
        /// organization
        /// </summary>
        public OrganizationModel Organization { get; set; }
    }

    /// <summary>
    /// organization mode
    /// </summary>
    public class OrganizationModel
    {
        /// <summary>
        /// id of organization
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// domain of organization
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// customer
        /// </summary>
        public CustomerModel Customer { get; set; }
    }

    /// <summary>
    /// customer mode
    /// </summary>
    public class CustomerModel
    {
        /// <summary>
        /// id of customer
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// firstname of customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// lastname of customer
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// email of customer
        /// </summary>
        public string Email { get; set; }
    }

    public class Permission
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }

    /// <summary>
    /// ForgotPassword resquest
    /// </summary>
    public class ForgotPasswordResquest
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    /// <summary>
    /// ResetPassword request
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required]
        [Display(Name = "User Id")]
        public string UserId { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// ConfirmPassword
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [Required]
        public string Code { get; set; }
    }

    /// <summary>
    /// Location response
    /// </summary>
    public class LocationResponse
    {
        /// <summary>
        /// Lat
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// Lon
        /// </summary>
        public double Lon { get; set; }
    }
}
