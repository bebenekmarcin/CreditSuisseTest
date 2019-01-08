namespace CreditSuisse
{
    public class CashCardService : ICashCardService
    {
        private readonly IPinService _pinService;
        private readonly ICardBalanceService _cardBalanceService;
        private readonly ITransactionManager _transactionManager;

        public CashCardService(IPinService pinService, ICardBalanceService cardBalanceService,
            ITransactionManager transactionManager)
        {
            _pinService = pinService;
            _cardBalanceService = cardBalanceService;
            _transactionManager = transactionManager;
        }

        public bool WithdrawMoney(decimal amount, string pin)
        {
            using (_transactionManager.CreateScope())
            {
                if (_pinService.IsValid(pin))
                {
                    var balance = _cardBalanceService.GetBalance();
                    if (balance >= amount)
                    {
                        _cardBalanceService.ReduceBalance(amount);
                        _transactionManager.Complete();
                        return true;
                    }
                }
            }

            return false;
        }

        public void TopUp(decimal amount)
        {
            _cardBalanceService.IncreaseBalance(amount);
        }
    }
}