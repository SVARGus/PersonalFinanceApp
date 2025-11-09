using PersonalFinanceApp.Domain;

namespace PersonalFinanceApp.Data.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с финансовыми данными (кошельки и транзакции)
    /// </summary>
    public interface IFinanceRepository
    {
        /// <summary>
        /// Получить все кошельки с их транзакциями
        /// </summary>
        /// <returns>Коллекция кошельков</returns>
        IEnumerable<Wallet> GetAllWallets();

        /// <summary>
        /// Получить все транзакции за указанный месяц
        /// </summary>
        /// <param name="year">Год для фильтрации</param>
        /// <param name="month">Месяц для фильтрации (1-12)</param>
        /// <returns>Коллекция транзакций за указанный месяц</returns>
        IEnumerable<Transaction> GetTransactionsByMonth(int year, int month);
    }
}
