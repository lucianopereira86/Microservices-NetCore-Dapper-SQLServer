using APITestRegister.Domain.Domain.Interfaces.Repositories;
using FluentValidation;

namespace APITestRegister.Domain.Domain.Validations.User
{
    public class UserLoginReturnValidation : AbstractValidator<Models.User>
    {
        public UserLoginReturnValidation()
        {
            RuleFor(x => x.idUser)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .WithErrorCode("101").WithMessage("User not found");
        }
    }
}
