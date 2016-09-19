using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.Owin.Security.DataProtection;

namespace API.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class MachineKeyProtectionProvider : IDataProtectionProvider
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="purposes"></param>
        /// <returns></returns>
        public IDataProtector Create(params string[] purposes)
        {
            return new MachineKeyDataProtector(purposes);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class MachineKeyDataProtector : IDataProtector
    {
        private readonly string[] _purposes;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="purposes"></param>
        public MachineKeyDataProtector(string[] purposes)
        {
            _purposes = purposes;
        }
        /// <summary>
        /// Protect
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public byte[] Protect(byte[] userData)
        {
            return MachineKey.Protect(userData, _purposes);
        }
        /// <summary>
        /// Unprotect
        /// </summary>
        /// <param name="protectedData"></param>
        /// <returns></returns>
        public byte[] Unprotect(byte[] protectedData)
        {
            return MachineKey.Unprotect(protectedData, _purposes);
        }
    }
}