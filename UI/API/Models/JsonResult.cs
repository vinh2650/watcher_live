using System.Collections.Generic;

namespace API.Models
{
    /// <summary>
    /// Base response result
    /// </summary>
    public abstract class JsonResult
    {
        /// <summary>
        /// List error as string
        /// </summary>
        public List<string> ErrorMessages
        {
            get;
            set;
        }

        /// <summary>
        /// Base Json result
        /// </summary>
        public JsonResult()
        {
            ErrorMessages = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        public void AddError(System.Web.Http.ModelBinding.ModelStateDictionary modelState)
        {
            foreach (var state in modelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    ErrorMessages.Add(error.ErrorMessage);
                }
            }
        }
    }
}