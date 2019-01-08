using FluentAssertions;
using Moq;
using Xunit;

namespace CreditSuisse.Test
{
    public class CashCardServiceShould
    {
        private const string ValidPin = "1234";
        private const string InvalidPin = "0000";
        private const decimal SufficientBalance = 10000;
        private const decimal InsufficientBalance = 10;
        private const decimal WithdrawnAmount = 100;

        private readonly CashCardService _cashCardService;
        private readonly Mock<IPinService> _pinServiceMock;
        private readonly Mock<ICardBalanceService> _cardBalanceServiceMock;
        private readonly Mock<ITransactionManager> _transactionManagerMock;

        public CashCardServiceShould()
        {
            _pinServiceMock = new Mock<IPinService>();
            _cardBalanceServiceMock = new Mock<ICardBalanceService>();
            _transactionManagerMock = new Mock<ITransactionManager>();
            _cashCardService = new CashCardService(_pinServiceMock.Object, _cardBalanceServiceMock.Object, _transactionManagerMock.Object);
        }

        [Fact]
        public void WithdrawMoney_WhenPinIsValid()
        {
            _pinServiceMock.Setup(s => s.IsValid(ValidPin)).Returns(true);
            _cardBalanceServiceMock.Setup(c => c.GetBalance()).Returns(SufficientBalance);

            bool areMoneyWithdrawn = _cashCardService.WithdrawMoney(WithdrawnAmount, ValidPin);

            areMoneyWithdrawn.Should().BeTrue();
        }

        [Fact]
        public void NotWithdrawMoney_WhenPinIsInValid()
        {
            _pinServiceMock.Setup(s => s.IsValid(InvalidPin)).Returns(false);

            bool areMoneyWithdrawn = _cashCardService.WithdrawMoney(WithdrawnAmount, InvalidPin);

            areMoneyWithdrawn.Should().BeFalse();
        }

        [Fact]
        public void ReduceBalance_WhenPinIsValidAndBalanceIsSufficient()
        {
            _pinServiceMock.Setup(s => s.IsValid(ValidPin)).Returns(true);
            _cardBalanceServiceMock.Setup(c => c.GetBalance()).Returns(SufficientBalance);

            _cashCardService.WithdrawMoney(100, ValidPin);

            _cardBalanceServiceMock.Verify(s => s.ReduceBalance(It.IsAny<decimal>()), Times.Once);
        }

        [Fact]
        public void NotWithdrawMoney_WhenPinIsValidButBalanceIsInsufficient()
        {
            _pinServiceMock.Setup(s => s.IsValid(ValidPin)).Returns(true);
            _cardBalanceServiceMock.Setup(c => c.GetBalance()).Returns(InsufficientBalance);

            bool areMoneyWithdrawn = _cashCardService.WithdrawMoney(WithdrawnAmount, ValidPin);

            _cardBalanceServiceMock.Verify(s => s.ReduceBalance(WithdrawnAmount), Times.Never);
            areMoneyWithdrawn.Should().BeFalse();
        }

        [Fact]
        public void TopUpBalance_WhenTopUpCalled()
        {
            _pinServiceMock.Setup(s => s.IsValid(ValidPin)).Returns(true);

            _cashCardService.TopUp(100);

            _cardBalanceServiceMock.Verify(s => s.IncreaseBalance(It.IsAny<decimal>()), Times.Once);
        }

        [Fact]
        public void WithdrawMoneyInTransaction_Always()
        {
            _cashCardService.WithdrawMoney(WithdrawnAmount, ValidPin);

            _transactionManagerMock.Verify(t => t.CreateScope(), Times.Once);
        }

        [Fact]
        public void WithdrawMoneyInTransaction_WhenPinIsValidAndBalanceIsSufficient()
        {
            _pinServiceMock.Setup(s => s.IsValid(ValidPin)).Returns(true);
            _cardBalanceServiceMock.Setup(c => c.GetBalance()).Returns(SufficientBalance);

            _cashCardService.WithdrawMoney(WithdrawnAmount, ValidPin);

            _transactionManagerMock.Verify(t => t.Complete(), Times.Once);
        }
    }
}
