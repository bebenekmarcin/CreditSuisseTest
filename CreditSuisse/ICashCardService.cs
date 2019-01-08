namespace CreditSuisse
{
    public interface ICashCardService
    {
        bool WithdrawMoney(decimal amount, string pin);

        void TopUp(decimal amount);
    }
}