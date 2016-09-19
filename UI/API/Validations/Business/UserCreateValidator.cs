using API.Models.Business;
using FluentValidation;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace API.Validations.Business
{
    /// <summary>
    /// the validator class of UserCreate
    /// </summary>
    public class UserCreateValidator : AbstractValidator<UserCreate>
    {
        /// <summary>
        /// ctor
        /// </summary>
        public UserCreateValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be valid");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
            RuleFor(x => x.PhoneNumber).Matches("^[0-9]*$").WithMessage("Phone number is number");
            //RuleFor(x => x.OrganizationDestinationId).NotEmpty().WithMessage("OrganizationDestinationId is required");
        }
    }

    /// <summary>
    /// the validator class of CustomerCreate
    /// </summary>
    public class CustomerCreateValidator : AbstractValidator<CustomerCreate>
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CustomerCreateValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be valid");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
            RuleFor(x => x.PhoneNumber).Matches("^[0-9]*$").WithMessage("Phone number is number");
        }
    }

    /// <summary>
    /// the validator class of CustomerUpdate
    /// </summary>
    public class CustomerUpdateValidator : AbstractValidator<CustomerUpdate>
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CustomerUpdateValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
            RuleFor(x => x.PhoneNumber).Matches("^[0-9]*$").WithMessage("Phone number is number");
        }
    }

    /// <summary>
    /// the validator class of UserUpdate
    /// </summary>
    public class UserUpdateValidator : AbstractValidator<UserUpdate>
    {
        /// <summary>
        /// ctor
        /// </summary>
        public UserUpdateValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
            RuleFor(x => x.PhoneNumber).Matches("^[0-9]*$").WithMessage("Phone number is number");
        }
    }
}