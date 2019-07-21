using APITestRegister.Domain.Domain.Interfaces.Repositories;
using FluentValidation;

namespace APITestRegister.Domain.Domain.Validations.User
{
    public class UserLoginValidation : AbstractValidator<Models.User>
    {
        public UserLoginValidation(IUserRepository repo)
        {
            RuleFor(x => x.email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty();

            RuleFor(x => x.password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty();
        }
    }
}
