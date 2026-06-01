using System;
using RestaurantSystem.Domain;
using RestaurantSystem.Application;
using RestaurantSystem.Infrastructure;

namespace RestaurantSystem.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            // Ініціалізація інфраструктури та сервісів (Складання додатку)
            IOrderRepository repository = new InMemoryOrderRepository();
            OrderApplicationService service = new OrderApplicationService(repository);

            // Створення тестового каталогу продуктів
            MenuComponent borsch = new DishItem("Український Борщ", 145.50m, 25);
            MenuComponent steak = new DishItem("Стейк Рібай", 420.00m, 20);
            MenuComponent juice = new DrinkItem("Апельсиновий Фреш", 65.00m, false);

            Console.WriteLine("=== СИСТЕМА УПРАВЛІННЯ РЕСТОРАНОМ | ІТЕРАЦІЯ 1 ===");
            Console.WriteLine($"Студент: Крупка Іван | Варіант №6\n");

            // Запуск Наскрізного Вертикального Зрізу (Vertical Slice Use Case)
            try
            {
                Console.WriteLine("[UI] Крок 1. Офіціант відкриває замовлення для Столика №5...");
                Order currentOrder = service.StartNewOrder(5);
                Console.WriteLine($"[UI] Замовлення успішно створено. ID: {currentOrder.Id}. Статус: {currentOrder.Status}\n");

                Console.WriteLine("[UI] Крок 2. Додаємо страви до замовлення...");
                service.AddPositionToOrder(currentOrder.Id, borsch, 2); // 2 борщі
                Console.WriteLine($"[UI] Додано: {borsch.Name} ({borsch.GetCategoryType()}) x2");
                
                service.AddPositionToOrder(currentOrder.Id, juice, 3);  // 3 фреші
                Console.WriteLine($"[UI] Додано: {juice.Name} ({juice.GetCategoryType()}) x3\n");

                Console.WriteLine("[UI] Крок 3. Поточний попередній рахунок столу:");
                foreach (var orderItem in currentOrder.Items)
                {
                    Console.WriteLine($" - {orderItem.Item.Name} | {orderItem.Quantity} шт. x {orderItem.Item.Price} грн = {orderItem.CalculateTotal()} грн.");
                }
                Console.WriteLine($"Загальна сума до сплати: {currentOrder.CalculateTotal()} грн.\n");

                Console.WriteLine("[UI] Крок 4. Клієнт просить порахувати. Проводимо оплату чека...");
                decimal finalPaidSum = service.ProcessPayment(currentOrder.Id);
                Console.WriteLine($"[UI] УСПІХ! Чек на суму {finalPaidSum} грн оплачено. Фінальний статус замовлення: {currentOrder.Status}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI ПОМИЛКА] Відбувся збій під час виконання сценарію: {ex.Message}");
            }

            Console.WriteLine("Натисніть будь-яку клавішу для виходу з програми...");
            Console.ReadKey();
        }
    }
}