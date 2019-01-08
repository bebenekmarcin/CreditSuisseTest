using System;
using System.Transactions;

namespace CreditSuisse
{
    public interface ITransactionManager : IDisposable
    {
        TransactionScope CreateScope();
        void Complete();
    }
}