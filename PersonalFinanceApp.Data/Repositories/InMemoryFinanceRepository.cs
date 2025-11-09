using PersonalFinanceApp.Data.Interfaces;
using PersonalFinanceApp.Domain;

namespace PersonalFinanceApp.Data.Repositories
{
    public class InMemoryFinanceRepository : IFinanceRepository
    {
        private readonly List<Wallet> _wallets;

        public InMemoryFinanceRepository()
        {
            //тестовые данные
            _wallets = new List<Wallet>
            {
                new Wallet { Id = 1, Name = "Main Card", Currency = "USD", InitialBalance = 1000 },
                new Wallet { Id = 2, Name = "Cash", Currency = "USD", InitialBalance = 200 }
            };

            var random = new Random();
            var transactions = new List<Transaction>();

            for (var i = 1; i <= 50; i++)
            {
                var walletId = random.Next(1, 3); // Случайно выбираем кошелек
                var type = random.Next(0, 2) == 0 ? TransactionType.Income : TransactionType.Expense;
                var amount = type == TransactionType.Income ? random.Next(50, 500) : random.Next(1, 150);
                var date = DateTime.Now.AddDays(-random.Next(1, 60)); // Транзакции за последние ~2 месяца, для тестового вывода

                transactions.Add(new Transaction
                {
                    Id = i,
                    Date = date,
                    Amount = amount,
                    Type = type,
                    Description = $"{type} Transaction #{i}",
                    WalletId = walletId
                });
            }

            // Распределяем транзакции по кошелькам
            foreach (var transaction in transactions)
            {
                var wallet = _wallets.First(w => w.Id == transaction.WalletId);
                // Обходим проверку баланса для инициализации, иначе данные могут быть некорректны
                wallet.Transactions.Add(transaction);
            }
        }
        public IEnumerable<Wallet> GetAllWallets()
        {
            return _wallets;
        }

        public IEnumerable<Transaction> GetTransactionsByMonth(int year, int month)
        {
            return _wallets
                .SelectMany(w => w.Transactions)
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .ToList();
        }
    }
}
