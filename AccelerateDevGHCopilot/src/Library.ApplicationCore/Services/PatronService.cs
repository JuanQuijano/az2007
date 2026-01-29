using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;

namespace Library.ApplicationCore.Services;

public class PatronService : IPatronService
{
    private readonly IPatronRepository _patronRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PatronService(IPatronRepository patronRepository)
        : this(patronRepository, new SystemDateTimeProvider())
    {
    }

    public PatronService(IPatronRepository patronRepository, IDateTimeProvider dateTimeProvider)
    {
        _patronRepository = patronRepository ?? throw new ArgumentNullException(nameof(patronRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<MembershipRenewalStatus> RenewMembership(int patronId)
    {
        var patron = await _patronRepository.GetPatron(patronId);
        if (patron == null)
            return MembershipRenewalStatus.PatronNotFound;

        // don't allow to renew till 1 month before expiration
        if (patron.MembershipEnd >= _dateTimeProvider.Now.AddMonths(1))
            return MembershipRenewalStatus.TooEarlyToRenew;

        // don't allow to renew if patron has overdue loans
        if (patron.Loans.Any(l => (l.ReturnDate == null) && l.DueDate < _dateTimeProvider.Now))
            return MembershipRenewalStatus.LoanNotReturned;

        patron.MembershipEnd = patron.MembershipEnd.AddYears(1);
        try
        {
            await _patronRepository.UpdatePatron(patron);
            return MembershipRenewalStatus.Success;
        }
        catch (Exception)
        {
            return MembershipRenewalStatus.Error;
        }
    }
}
