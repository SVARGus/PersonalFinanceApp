using PersonalFinanceApp.Data.Interfaces;
using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Services;
using System.Globalization;

namespace PersonalFinanceApp.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Простая настройка для доллара
            var usCulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.CurrentCulture = usCulture;
            CultureInfo.CurrentUICulture = usCulture;

            // Простая настройка зависимостей без использования DI-контейнера
            IFinanceRepository repository = new InMemoryFinanceRepository();
            var financeService = new FinanceService(repository);

            Console.WriteLine("Учет личных финансов");
            Console.WriteLine("====================");

            // Получаем год и месяц от пользователя (тестовые данные генерируются за последние 60 дней)
            Console.Write("Введите год (например, 2025): ");
            int year = int.Parse(Console.ReadLine());

            Console.Write("Введите месяц (1-12): ");
            int month = int.Parse(Console.ReadLine());

            Console.WriteLine();
            Console.WriteLine($"Отчет за {month}/{year}:");
            Console.WriteLine();

            // 1. Вывод транзакций, сгруппированных по типу
            Console.WriteLine("1. ранзакции сгруппированные по типу (отсортированы по сумме по убыванию):");
            var groupedTransactions = financeService.GetTransactionsGroupedByTypeForMonth(year, month);

            foreach (var group in groupedTransactions)
            {
                Console.WriteLine($"\n--- {group.Key} --- (Всего: {group.Sum(t => t.Amount):C})");
                foreach (var transaction in group)
                {
                    Console.WriteLine($"  {transaction.Date:dd.MM.yyyy} | {transaction.Amount,10:C} | {transaction.Description}");
                }
            }

            // 2. Вывод транзакций, сгруппированных по ТОП 3 транзакций по типу Expense и кошельку
            Console.WriteLine();
            Console.WriteLine("2. Топ-3 самых больших трат для каждого кошелька:");
            var topExpenses = financeService.GetTop3ExpensesForEachWallet(year, month);

            foreach (var (walletName, expenses) in topExpenses)
            {
                Console.WriteLine($"\n--- {walletName} ---");
                if (expenses.Any())
                {
                    foreach (var expense in expenses)
                    {
                        Console.WriteLine($"  {expense.Amount,10:C} | {expense.Description}");
                    }
                }
                else
                {
                    Console.WriteLine("  Нет трат за этот период.");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}