using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        //Holds all validators for the current request 
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        //“Give me all rules that apply to this request”
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var failures = _validators.Select(x => x.Validate(context))
                    .SelectMany(x => x.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Any())
                    throw new ValidationException(failures);

                
            }

            return await next();
        }
    }
}
