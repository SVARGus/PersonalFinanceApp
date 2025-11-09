namespace PersonalFinanceApp.Domain
{
    /// <summary>
    /// Кошелёк: ID, название, валюта, начальный баланс и список транзакций
    /// </summary>
    public class Wallet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public decimal InitialBalance { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
                
        /// <summary>
        /// Вычисляемое свойство для текущего баланса
        /// </summary>
        public decimal CurrentBalance
        {
            get
            {
                var totalIncome = Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                var totalExpense = Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                return InitialBalance + totalIncome - totalExpense;
            }
        }

        /// <summary>
        /// Метод для добавления транзакции с проверкой баланса
        /// </summary>
        /// <param name="transaction">Транзакция</param>
        /// <exception cref="InvalidOperationException">Ошибка при недостаточности средств</exception>
        public void AddTransaction(Transaction transaction)
        {
            if (transaction.Type == TransactionType.Expense && transaction.Amount > CurrentBalance)
            {
                throw new InvalidOperationException("Insufficient funds in the wallet to complete this expense.");
            }

            transaction.WalletId = this.Id;
            Transactions.Add(transaction);
        }
    }
}
