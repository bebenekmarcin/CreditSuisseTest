namespace CreditSuisse
{
    /// <summary>
    /// This is just simple implementation which meets requirements. If this would be part of production system then
    /// eg. some logging should be added. I decided to return false if operation fail instead of exception notification
    /// because I was not sure if class should inform why it fail or not. In production system probably exception
    /// notification would work better and higher layer will take care about deciding what information will share
    /// externally. In production system also we need to ensure about integration test if all components works together. 
    /// </summary>
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