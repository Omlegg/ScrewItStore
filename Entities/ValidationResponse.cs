using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrewItBackEnd.Entities
{
    public class ValidationResponse
    {
        public string Property { get; set; }
        public string Message { get; set; }

        public ValidationResponse(string property, string message)
        {
            this.Property = property;
            this.Message = message;
        }
    }
}