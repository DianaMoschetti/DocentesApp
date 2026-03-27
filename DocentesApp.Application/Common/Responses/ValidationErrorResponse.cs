using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.Common.Responses
{
    public class ValidationErrorResponse : ApiErrorResponse
    {
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }
}
