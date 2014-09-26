using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Posttrack.Web.Models
{
    public class OperationResult
    {
        private readonly bool success;
        private readonly IEnumerable<KeyValuePair<string, string[]>> errors;

        internal OperationResult(bool success)
        {
            this.success = success;
        }

        internal OperationResult(bool success, ModelStateDictionary modelState) : this(success)
        {
            if (!modelState.IsValid){
                this.errors = modelState
                    .ToDictionary(x => x.Key, y => y.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                    .Where(m => m.Value.Any())
                    .ToList();
            }
        }

        public bool Success
        {
            get
            {
                return this.success;
            }
        }

        public IEnumerable<KeyValuePair<string, string[]>> Errors
        {
            get
            {
                return this.errors;
            }
        }
    }
}