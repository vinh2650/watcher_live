
using API.Validations.Business;
using FluentValidation.Attributes;

namespace API.Models.Business
{
    /// <summary>
    /// create or update customer model
    /// </summary>
    [Validator(typeof(CustomerCreateValidator))]
    public class CustomerCreate
    {
        /// <summary>
        /// email of customer
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of customer
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of customer
        /// </summary>
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// create or update customer result
    /// </summary>
    public class CustomerCreateResult
    {
        /// <summary>
        /// id of user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// email of customer
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of customer
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of customer
        /// </summary>
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// create or update customer model
    /// </summary>
    [Validator(typeof(CustomerUpdateValidator))]
    public class CustomerUpdate
    {
        /// <summary>
        /// first name of customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of customer
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of customer
        /// </summary>
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// get customer result
    /// </summary>
    public class CustomerGetResult
    {
        /// <summary>
        /// id of user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// email of customer
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of customer
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of customer
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// organization id of customer
        /// </summary>
        public string OrganizationId { get; set; }
    }

    /// <summary>
    /// get all customer result
    /// </summary>
    public class CustomerGetAllResult
    {
        /// <summary>
        /// id of user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// email of customer
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of customer
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of customer
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// organization id of customer
        /// </summary>
        public string OrganizationId { get; set; }
    }
}