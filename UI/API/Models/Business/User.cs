using System;
using System.Collections.Generic;
using API.Validations.Business;
using FluentValidation.Attributes;

namespace API.Models.Business
{
    /// <summary>
    /// create or update user model
    /// </summary>
    [Validator(typeof(UserCreateValidator))]
    public class UserCreate
    {
        /// <summary>
        /// email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Passwords
        /// </summary>
        public string Passwords { get; set; }

        /// <summary>
        /// Confirm password
        /// </summary>
        public string ConfirmPasswords { get; set; }
    }

    /// <summary>
    /// create or update user result
    /// </summary>
    public class UserCreateResult
    {
        /// <summary>
        /// id of user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// organization destination id of user
        /// </summary>
        public string OrganizationDestinationId { get; set; }

        /// <summary>
        /// role of user
        /// </summary>
        public string Role { get; set; }
    }

    /// <summary>
    /// create or update user model
    /// </summary>
    [Validator(typeof(UserUpdateValidator))]
    public class UserUpdate
    {
        /// <summary>
        /// first name of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of user
        /// </summary>
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// get user result
    /// </summary>
    public class UserGetResult
    {
        /// <summary>
        /// id of user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of user
        /// </summary>
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// get all user result
    /// </summary>
    public class UserGetAllResult
    {
        /// <summary>
        /// id of user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// phone number of user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// role of user
        /// </summary>
        public string Role { get; set; }
    }

    /// <summary>
    /// Get list user by ids model
    /// </summary>
    public class GetListUserModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public GetListUserModel()
        {
            Ids = new List<string>();
        }

        /// <summary>
        /// List of user id
        /// </summary>
        public List<String> Ids { get; set; }

    }
}