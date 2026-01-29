using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;

namespace Library.ApplicationCore.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoanService(ILoanRepository loanRepository)
        : this(loanRepository, new SystemDateTimeProvider())
    {
    }

    public LoanService(ILoanRepository loanRepository, IDateTimeProvider dateTimeProvider)
    {
        _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<LoanReturnStatus> ReturnLoan(int loanId)
    {
        Loan? loan = await _loanRepository.GetLoan(loanId);
        if (loan == null)
        {
            return LoanReturnStatus.LoanNotFound;
        }

        // check if already returned
        if (loan.ReturnDate != null)
        {
            return LoanReturnStatus.AlreadyReturned;
        }

        loan.ReturnDate = _dateTimeProvider.Now;
        try
        {
            await _loanRepository.UpdateLoan(loan);
            return LoanReturnStatus.Success;
        }
        catch (Exception)
        {
            return LoanReturnStatus.Error;
        }
    }

    public const int ExtendByDays = 14;

    public async Task<LoanExtensionStatus> ExtendLoan(int loanId)
    {
        var loan = await _loanRepository.GetLoan(loanId);

        if (loan == null)
            return LoanExtensionStatus.LoanNotFound;

        // Check if patron's membership is expired
        if (loan.Patron!.MembershipEnd < _dateTimeProvider.Now)
            return LoanExtensionStatus.MembershipExpired;

        if (loan.ReturnDate != null)
            return LoanExtensionStatus.LoanReturned;

        if (loan.DueDate < _dateTimeProvider.Now)
            return LoanExtensionStatus.LoanExpired;

        loan.DueDate = loan.DueDate.AddDays(ExtendByDays);
        try
        {
            await _loanRepository.UpdateLoan(loan);
            return LoanExtensionStatus.Success;
        }
        catch (Exception)
        {
            return LoanExtensionStatus.Error;
        }
    }
}