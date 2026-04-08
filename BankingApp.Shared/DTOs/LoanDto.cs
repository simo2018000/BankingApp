using System;

namespace BankingApp.Shared // Or BankingApp.API.Dtos
{
    // INPUT: User only sends what is needed to apply
    public record LoanApplicationDto(
        Guid AccountId,
        decimal Amount,
        int TermInMonths
    );

    // OUTPUT: User sees the loan details, but not internal secrets
    public record LoanResponseDto(
        Guid LoanId,
        decimal Amount,
        double InterestRate,
        string Status,
        DateTime PaymentDueDate
    );
}