using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.PipeLineBehavior
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {

                string result = string.Join(",", failures.Select(c => c.ErrorMessage));
                var data = (TResponse)Activator.CreateInstance(typeof(TResponse));
                Type examType = typeof(TResponse);
                PropertyInfo code = examType.GetProperty("StatusCode");
                code.SetValue(data, 422);
                PropertyInfo message = examType.GetProperty("Messages");
                message.SetValue(data, failures.Select(c => c.ErrorMessage).ToList());
                return await Task.FromResult(data);
            }
            return await next();
        }
    }
}
