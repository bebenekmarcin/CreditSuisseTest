namespace CreditSuisse
{
    public interface ICardBalanceService
    {
        bool ReduceBalance(decimal withdrawnAmount);
        void IncreaseBalance(decimal amount);
        decimal GetBalance();
    }
}
