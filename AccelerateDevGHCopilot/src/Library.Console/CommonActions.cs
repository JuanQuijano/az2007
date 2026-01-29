namespace Library.Console;

[Flags]
public enum CommonActions
{
    Repeat = 0,
    Select = 1,
    Quit = 2,
    SearchPatrons = 4,
    SearchBooks = 8,
    RenewPatronMembership = 16,
    ReturnLoanedBook = 32,
    ExtendLoanedBook = 64
}
