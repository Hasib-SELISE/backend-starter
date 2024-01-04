using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class DynamicQueryResponseModel
    {
        public string Message { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public dynamic Data { get; private set; }
        public IEnumerable<string> Errors { get; set; }

        public DynamicQueryResponseModel()
        {
            Errors = new List<string>();
        }

        public DynamicQueryResponseModel HandleQuerySuccess(dynamic data, HttpStatusCode successStatusCode)
        {
            Data = data;
            HttpStatusCode = successStatusCode;
            return this;
        }

        public DynamicQueryResponseModel HandleQueryError(HttpStatusCode successStatusCode, string errorMessage, ValidationResult validationError = null)
        {
            Errors = validationError != null ? GetErrorMessages(validationError) : new List<string>();
            Message = errorMessage;
            HttpStatusCode = successStatusCode;
            return this;
        }

        private IEnumerable<string> GetErrorMessages(ValidationResult validationError)
        {
            if (validationError != null)
            {
                foreach (ValidationFailure error in validationError.Errors)
                {
                    yield return error.ErrorMessage;
                }
            }
        }
    }
}
