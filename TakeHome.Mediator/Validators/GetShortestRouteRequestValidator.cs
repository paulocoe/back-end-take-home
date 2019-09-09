using FluentValidation;
using TakeHome.Mediator.Requests;

namespace TakeHome.Mediator.Validators
{
    public class GetShortestRouteRequestValidator : AbstractValidator<GetShortestRouteRequest>
    {
        private readonly string invalidOrigin = "Invalid Origin";
        private readonly string invalidDestination = "Invalid Destination";
        public GetShortestRouteRequestValidator()
        {            
            RuleFor(request => request.Origin)
                .NotNull().WithMessage(invalidOrigin)
                .NotEmpty().WithMessage(invalidOrigin)
                .MinimumLength(3).WithMessage(invalidOrigin)
                .MaximumLength(3).WithMessage(invalidOrigin)
                .Matches("[a-zA-Z]{3}").WithMessage(invalidOrigin);

            RuleFor(request => request.Destination)
                .NotNull().WithMessage(invalidDestination)
                .NotEmpty().WithMessage(invalidDestination)
                .NotEqual(request => request.Origin).WithMessage(invalidDestination)
                .MinimumLength(3).WithMessage(invalidDestination)
                .MaximumLength(3).WithMessage(invalidDestination)
                .Matches("[a-zA-Z]{3}").WithMessage(invalidDestination);
        }
    }
}
