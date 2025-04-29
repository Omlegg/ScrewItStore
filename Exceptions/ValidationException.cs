using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Exceptions
{
    public class ValidationException : Exception
    {
        public List<ValidationResponse> validationResponseItems;

        public ValidationException()
        {
            validationResponseItems = new List<ValidationResponse>();
        }
    }
}