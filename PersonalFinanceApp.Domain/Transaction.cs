namespace PersonalFinanceApp.Domain
{
    /// <summary>
    /// Транзакция: ID, дата, сумма, тип (Income/Expense), описание, связь с кошельком
    /// </summary>
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public int WalletId { get; set; }
    }

    /// <summary>
    /// Тип транзации: Доход, Расход
    /// </summary>
    public enum TransactionType
    {
        Income = 0,
        Expense = 1
    }
}
