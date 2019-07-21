using APITestRegister.Domain.Domain.Interfaces.Repositories;
using FluentValidation;

namespace APITestRegister.Domain.Domain.Validations.User
{
    public class UserRegisterValidation : AbstractValidator<Models.User>
    {
        public UserRegisterValidation(IUserRepository repo)
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty();

            RuleFor(x => x.email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Must(m => !repo.Exists(m))
                .WithErrorCode("100").WithMessage("E-mail already registered by another user").WithName("email");

            RuleFor(x => x.password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty();
        }
    }
}
