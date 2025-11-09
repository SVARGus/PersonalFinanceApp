using PersonalFinanceApp.Data.Interfaces;
using PersonalFinanceApp.Domain;

namespace PersonalFinanceApp.Services
{
    public class FinanceService
    {
        private readonly IFinanceRepository _repository;

        public FinanceService(IFinanceRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Группировка транзакций по типу за указанный месяц
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <returns>Сгрупированная коллекция транзакции по "month"</returns>
        public IEnumerable<IGrouping<TransactionType, Transaction>> GetTransactionsGroupedByTypeForMonth(int year, int month)
        {
            var transactions = _repository.GetTransactionsByMonth(year, month);

            return transactions
                .GroupBy(t => t.Type)
                .OrderByDescending(g => g.Sum(t => t.Amount)) // Сортировка групп по убыванию общей суммы
                .Select(g => new
                {
                    Group = g,
                    SortedTransactions = g.OrderBy(t => t.Date) // Сортировка внутри группы по дате (старые -> новые)
                })
                .SelectMany(x => x.SortedTransactions) // "Распаковываем" отсортированные транзакции
                .GroupBy(t => t.Type); // Снова группируем, т.к. предыдущие операции "сломали" IGrouping
        }

        /// <summary>
        /// Формирование Топ-3 самых больших трат за указанный месяц для каждого кошелька
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <returns>Словарь 3 самых больших тран за "month"</returns>
        public Dictionary<string, List<Transaction>> GetTop3ExpensesForEachWallet(int year, int month)
        {
            var allWallets = _repository.GetAllWallets();
            var result = new Dictionary<string, List<Transaction>>();

            foreach (var wallet in allWallets)
            {
                var topExpenses = wallet.Transactions
                    .Where(t => t.Type == TransactionType.Expense &&
                               t.Date.Year == year &&
                               t.Date.Month == month)
                    .OrderByDescending(t => t.Amount)
                    .Take(3)
                    .ToList();

                result[wallet.Name] = topExpenses;
            }

            return result;
        }
    }
}
