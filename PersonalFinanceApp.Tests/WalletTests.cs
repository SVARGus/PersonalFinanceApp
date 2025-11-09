using PersonalFinanceApp.Domain;
using Xunit;

namespace PersonalFinanceApp.Tests
{
    public class WalletTests
    {
        [Fact]
        public void AddTransaction_ExpenseExceedingBalance_ThrowsException()
        {
            // Arrange
            var wallet = new Wallet { InitialBalance = 100 };

            var largerExeption = new Transaction
            {
                Date = DateTime.UtcNow,
                Amount = 150,
                Type = TransactionType.Expense,
                Description = "Test"
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => wallet.AddTransaction(largerExeption));
        }

        [Fact]
        public void CurrentBalance_CalculatedCorrectly()
        {
            // Arrange
            var wallet = new Wallet { InitialBalance = 100 };
            wallet.Transactions.Add(new Transaction { Amount = 50, Type = TransactionType.Income });
            wallet.Transactions.Add(new Transaction { Amount = 30, Type = TransactionType.Expense });

            // Act
            var balance = wallet.CurrentBalance;

            // Assert
            Assert.Equal(120, balance);
        }
    }
}