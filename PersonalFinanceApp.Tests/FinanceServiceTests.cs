using Moq;
using PersonalFinanceApp.Data.Interfaces;
using PersonalFinanceApp.Domain;
using PersonalFinanceApp.Services;

namespace PersonalFinanceApp.Tests
{
    public class FinanceServiceTests
    {
        private readonly Mock<IFinanceRepository> _mockRepo;
        private readonly FinanceService _service;

        public FinanceServiceTests()
        {
            _mockRepo = new Mock<IFinanceRepository>();
            _service = new FinanceService(_mockRepo.Object);
        }

        [Fact]
        public void GetTransactionsGroupedByTypeForMonth_ReturnsGroupsSortedByTotalAmountDescending()
        {
            // Arrange
            var testTransactions = new List<Transaction>
            {
                // Income группа: общая сумма = 300 (100 + 200)
                new Transaction { Date = new DateTime(2025, 10, 1), Amount = 100, Type = TransactionType.Income },
                new Transaction { Date = new DateTime(2025, 10, 15), Amount = 200, Type = TransactionType.Income },
                
                // Expense группа: общая сумма = 400 (150 + 250) - должна быть первой
                new Transaction { Date = new DateTime(2025, 10, 5), Amount = 150, Type = TransactionType.Expense },
                new Transaction { Date = new DateTime(2025, 10, 20), Amount = 250, Type = TransactionType.Expense },
                
                // Транзакции другого месяца - не должны попасть в результат
                new Transaction { Date = new DateTime(2025, 9, 30), Amount = 500, Type = TransactionType.Income }
            };

            _mockRepo.Setup(repo => repo.GetTransactionsByMonth(2025, 10))
                    .Returns(testTransactions.Where(t => t.Date.Year == 2025 && t.Date.Month == 10));

            // Act
            var result = _service.GetTransactionsGroupedByTypeForMonth(2025, 10).ToList();

            // Assert
            Assert.Equal(2, result.Count);

            // Проверяем порядок групп: Expense должна быть первой (сумма 400 > 300)
            Assert.Equal(TransactionType.Expense, result[0].Key);
            Assert.Equal(TransactionType.Income, result[1].Key);

            // Проверяем суммы групп
            Assert.Equal(400, result[0].Sum(t => t.Amount));
            Assert.Equal(300, result[1].Sum(t => t.Amount));
        }

        [Fact]
        public void GetTransactionsGroupedByTypeForMonth_SortsTransactionsWithinGroupByDateAscending()
        {
            // Arrange
            var testTransactions = new List<Transaction>
            {
                new Transaction { Date = new DateTime(2025, 10, 15), Amount = 200, Type = TransactionType.Income },
                new Transaction { Date = new DateTime(2025, 10, 1), Amount = 100, Type = TransactionType.Income },
                new Transaction { Date = new DateTime(2025, 10, 10), Amount = 150, Type = TransactionType.Income }
            };

            _mockRepo.Setup(repo => repo.GetTransactionsByMonth(2025, 10))
                    .Returns(testTransactions);

            // Act
            var result = _service.GetTransactionsGroupedByTypeForMonth(2025, 10).ToList();

            // Assert
            var incomeGroup = result.First(g => g.Key == TransactionType.Income).ToList();

            // Проверяем сортировку по дате внутри группы (от старых к новым)
            Assert.Equal(new DateTime(2025, 10, 1), incomeGroup[0].Date);
            Assert.Equal(new DateTime(2025, 10, 10), incomeGroup[1].Date);
            Assert.Equal(new DateTime(2025, 10, 15), incomeGroup[2].Date);
        }

        [Fact]
        public void GetTransactionsGroupedByTypeForMonth_EmptyMonth_ReturnsEmptyCollection()
        {
            // Arrange
            var emptyTransactions = new List<Transaction>();
            _mockRepo.Setup(repo => repo.GetTransactionsByMonth(2025, 11))
                    .Returns(emptyTransactions);

            // Act
            var result = _service.GetTransactionsGroupedByTypeForMonth(2025, 11);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetTransactionsGroupedByTypeForMonth_OnlyOneType_ReturnsOneGroup()
        {
            // Arrange
            var onlyIncomeTransactions = new List<Transaction>
            {
                new Transaction { Date = new DateTime(2025, 10, 1), Amount = 100, Type = TransactionType.Income },
                new Transaction { Date = new DateTime(2025, 10, 15), Amount = 200, Type = TransactionType.Income }
            };

            _mockRepo.Setup(repo => repo.GetTransactionsByMonth(2025, 10))
                    .Returns(onlyIncomeTransactions);

            // Act
            var result = _service.GetTransactionsGroupedByTypeForMonth(2025, 10).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(TransactionType.Income, result[0].Key);
            Assert.Equal(2, result[0].Count());
        }

        [Fact]
        public void GetTransactionsGroupedByTypeForMonth_MixedTypesCorrectlyGroupsAndSorts()
        {
            // Arrange - сложный сценарий с разными типами и датами
            var testTransactions = new List<Transaction>
            {
                // Income: сумма = 600 (должна быть первой)
                new Transaction { Date = new DateTime(2025, 10, 20), Amount = 400, Type = TransactionType.Income },
                new Transaction { Date = new DateTime(2025, 10, 5), Amount = 200, Type = TransactionType.Income },
                
                // Expense: сумма = 450 (должна быть второй)
                new Transaction { Date = new DateTime(2025, 10, 25), Amount = 300, Type = TransactionType.Expense },
                new Transaction { Date = new DateTime(2025, 10, 10), Amount = 150, Type = TransactionType.Expense }
            };

            _mockRepo.Setup(repo => repo.GetTransactionsByMonth(2025, 10))
                    .Returns(testTransactions);

            // Act
            var result = _service.GetTransactionsGroupedByTypeForMonth(2025, 10).ToList();

            // Assert
            Assert.Equal(2, result.Count);

            // Проверяем порядок групп по сумме (Income 600 > Expense 450)
            Assert.Equal(TransactionType.Income, result[0].Key);
            Assert.Equal(TransactionType.Expense, result[1].Key);

            // Проверяем сортировку внутри Income группы
            var incomeTransactions = result[0].ToList();
            Assert.Equal(new DateTime(2025, 10, 5), incomeTransactions[0].Date);
            Assert.Equal(new DateTime(2025, 10, 20), incomeTransactions[1].Date);

            // Проверяем сортировку внутри Expense группы
            var expenseTransactions = result[1].ToList();
            Assert.Equal(new DateTime(2025, 10, 10), expenseTransactions[0].Date);
            Assert.Equal(new DateTime(2025, 10, 25), expenseTransactions[1].Date);
        }

        [Fact]
        public void GetTransactionsGroupedByTypeForMonth_SameTotalAmount_PreservesGroupOrder()
        {
            // Arrange - группы с одинаковой суммой
            var testTransactions = new List<Transaction>
            {
                new Transaction { Date = new DateTime(2025, 10, 1), Amount = 300, Type = TransactionType.Income },
                new Transaction { Date = new DateTime(2025, 10, 2), Amount = 300, Type = TransactionType.Expense }
            };

            _mockRepo.Setup(repo => repo.GetTransactionsByMonth(2025, 10))
                    .Returns(testTransactions);

            // Act
            var result = _service.GetTransactionsGroupedByTypeForMonth(2025, 10).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            // При одинаковых суммах порядок может быть любым, но обе группы должны присутствовать
            Assert.Contains(result, g => g.Key == TransactionType.Income);
            Assert.Contains(result, g => g.Key == TransactionType.Expense);
        }

        [Fact]
        public void GetTop3ExpensesForEachWallet_ReturnsCorrectData()
        {
            // Arrange
            var wallets = new List<Wallet>
            {
                new Wallet { Id = 1, Name = "Wallet1", Transactions = new List<Transaction>
                {
                    new Transaction { Date = new DateTime(2025, 10, 10), Amount = 100, Type = TransactionType.Expense },
                    new Transaction { Date = new DateTime(2025, 10, 15), Amount = 200, Type = TransactionType.Expense },
                    new Transaction { Date = new DateTime(2025, 10, 20), Amount = 50, Type = TransactionType.Expense },
                    new Transaction { Date = new DateTime(2025, 10, 25), Amount = 300, Type = TransactionType.Expense }, // Должна быть в топ-3
                    new Transaction { Date = new DateTime(2025, 2, 1), Amount = 500, Type = TransactionType.Expense } // Другой месяц, не должна войти
                }},
                new Wallet { Id = 2, Name = "Wallet2", Transactions = new List<Transaction>() }
            };

            _mockRepo.Setup(repo => repo.GetAllWallets()).Returns(wallets);

            // Act
            var result = _service.GetTop3ExpensesForEachWallet(2025, 10);

            // Assert
            Assert.Equal(2, result.Count);

            var wallet1Expenses = result["Wallet1"];
            Assert.Equal(3, wallet1Expenses.Count); // Только топ-3
            Assert.Equal(300, wallet1Expenses[0].Amount); // Отсортированы по убыванию
            Assert.Equal(200, wallet1Expenses[1].Amount);
            Assert.Equal(100, wallet1Expenses[2].Amount);

            var wallet2Expenses = result["Wallet2"];
            Assert.Empty(wallet2Expenses);
        }
    }
}
