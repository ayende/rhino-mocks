namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IBankAccount
    {
        Rhino.Mocks.Expect.Action Withdraw(int p);

        Rhino.Mocks.Expect.Action Deposit(int p);
    }
}
