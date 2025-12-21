using FluentValidation;
using api_joyeria.Application.Commands.Checkout;

namespace api_joyeria.Application.Validators
{
    public class CreateGuestOrderCommandValidator : AbstractValidator<CreateGuestOrderCommand>
    {
        public CreateGuestOrderCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.ShippingAddress).NotNull();
            RuleFor(x => x.Items).NotEmpty().WithMessage("Order must include at least one item");
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId).NotEmpty();
                items.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}