using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Dorisoy.Pan.API.Helpers
{
    public class UnprocessableEntityObjectResult : ObjectResult
    {
        public UnprocessableEntityObjectResult(ModelStateDictionary modelState)
            : base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            StatusCode = 422;
        }
    }
    public class UnAuthorizedEntityObjectResult : ObjectResult
    {
        public UnAuthorizedEntityObjectResult(ModelStateDictionary modelState)
            : base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            StatusCode = 403;
        }
    }
    public class UnAuthenticationEntityObjectResult : ObjectResult
    {
        public UnAuthenticationEntityObjectResult(ModelStateDictionary modelState)
            : base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            StatusCode = 401;
        }
    }
    

    public class BadRequestEntityObjectResult: ObjectResult
    {
        public BadRequestEntityObjectResult(ModelStateDictionary modelState)
        : base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            StatusCode = 500;
        }
    }

    public class AlreadyExistEntityObjectResult : ObjectResult
    {
        public AlreadyExistEntityObjectResult(String message)
            : base(message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            StatusCode = 409;
        }
    }

   
}
