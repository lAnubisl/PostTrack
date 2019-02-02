using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Posttrack.Web.Models
{
    public class OperationResult
    {
        private readonly IEnumerable<KeyValuePair<string, string[]>> _errors;
        private readonly bool _success;

        internal OperationResult(bool success)
        {
            _success = success;
        }

        internal OperationResult(bool success, ModelStateDictionary modelState)
            : this(success)
        {
            if (!modelState.IsValid)
            {
                _errors = modelState
                    .ToDictionary(x => x.Key, y => y.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                    .Where(m => m.Value.Any())
                    .ToList();
            }
        }

        public bool Success
        {
            get { return _success; }
        }

        public IEnumerable<KeyValuePair<string, string[]>> Errors
        {
            get { return _errors; }
        }
    }
}
