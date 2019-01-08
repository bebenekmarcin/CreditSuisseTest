using System.Transactions;

namespace CreditSuisse
{
    public class TransactionManager : ITransactionManager
    {
        private TransactionScope _transactionScope;

        public TransactionScope CreateScope()
        {
            _transactionScope = new TransactionScope();
            return _transactionScope;
        }

        public void Complete()
        {
            _transactionScope.Complete();
        }

        public void Dispose()
        {
            this._transactionScope.Dispose();
        }
    }
}
