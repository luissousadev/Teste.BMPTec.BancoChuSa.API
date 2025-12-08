using BMPTec.BancoChuSa.API.DTOs.BankAccount;
using FluentValidation;

namespace BMPTec.BancoChuSa.API.Validators;

public class CreateBankAccountRequestValidator: AbstractValidator<CreateBankAccountRequest>
{
    public CreateBankAccountRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Cliente é obrigatório.");

        RuleFor(x => x.BankCode)
            .NotEmpty().WithMessage("Código do banco é obrigatório.")
            .Length(3).WithMessage("Código do banco deve ter 3 caracteres.");

        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage("Nome do banco é obrigatório.")
            .MaximumLength(100);

        RuleFor(x => x.BankBranch)
            .NotEmpty().WithMessage("Agência é obrigatória.")
            .MaximumLength(10);

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Número da conta é obrigatório.")
            .MaximumLength(20);

        RuleFor(x => x.AccountDigit)
            .NotEmpty().WithMessage("Dígito da conta é obrigatório.")
            .MaximumLength(2);
    }
}