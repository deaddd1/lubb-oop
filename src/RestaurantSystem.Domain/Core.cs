using System;
using System.Collections.Generic;

namespace RestaurantSystem.Domain
{
    // Базовий абстрактний клас для всіх позицій меню (Абстракція)
    public abstract class MenuComponent
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public decimal Price { get; protected set; }

        protected MenuComponent(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва не може бути порожньою.");
            if (price <= 0)
                throw new ArgumentException("Ціна повинна бути більшою за нуль.");

            Id = Guid.NewGuid();
            Name = name;
            Price = price;
        }

        // Поліморфний метод для виведення специфіки позиції
        public abstract string GetCategoryType();
    }

    // Конкретна реалізація 1: Страва (Поліморфізм)
    public class DishItem : MenuComponent
    {
        public int CookingTimeMinutes { get; private set; }

        public DishItem(string name, decimal price, int cookingTime) : base(name, price)
        {
            if (cookingTime < 1)
                throw new ArgumentException("Час приготування страви не може бути меншим за 1 хвилину.");
            CookingTimeMinutes = cookingTime;
        }

        public override string GetCategoryType() => "Кухня (Страва)";
    }

    // Конкретна реалізація 2: Напій (Поліморфізм)
    public class DrinkItem : MenuComponent
    {
        public bool IsAlcoholic { get; private set; }

        public DrinkItem(string name, decimal price, bool isAlcoholic) : base(name, price)
        {
            IsAlcoholic = isAlcoholic;
        }

        public override string GetCategoryType() => IsAlcoholic ? "Бар (Алкоголь)" : "Бар (Безалкогольний)";
    }

    // Об'єкт-значення (Value Object) для позиції в замовленні
    public class OrderItem
    {
        public MenuComponent Item { get; private set; }
        public int Quantity { get; private set; }

        public OrderItem(MenuComponent item, int quantity)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            if (quantity <= 0)
                throw new ArgumentException("Кількість повинна бути більшою за 0.");
            
            Quantity = quantity;
        }

        public decimal CalculateTotal() => Item.Price * Quantity;
    }

    // Головна сутність (Entity) — Замовлення ресторану
    public class Order
    {
        public Guid Id { get; private set; }
        public int TableNumber { get; private set; }
        private readonly List<OrderItem> _items = new List<OrderItem>();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public string Status { get; private set; } // New, Preparing, Served, Paid

        public Order(int tableNumber)
        {
            if (tableNumber <= 0)
                throw new ArgumentException("Номер столика повинен бути більшим за нуль.");

            Id = Guid.NewGuid();
            TableNumber = tableNumber;
            Status = "New";
        }

        // Інкапсульована бізнес-логіка додавання позицій
        public void AddItem(MenuComponent item, int quantity)
        {
            if (Status == "Paid")
                throw new InvalidOperationException("Неможливо додати страви до вже оплаченого замовлення.");

            var existing = _items.Find(x => x.Item.Id == item.Id);
            if (existing != null)
            {
                // Якщо така страва вже є — створюємо оновлений OrderItem (інкапсуляція)
                _items.Remove(existing);
                _items.Add(new OrderItem(item, existing.Quantity + quantity));
            }
            else
            {
                _items.Add(new OrderItem(item, quantity));
            }
        }

        public void CloseAndPay()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Неможливо закрити порожнє замовлення.");
            Status = "Paid";
        }

        public decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var orderItem in _items)
            {
                total += orderItem.CalculateTotal();
            }
            return total;
        }
    }

    // Контракт інфраструктурного шару (Принцип інверсії залежностей - DIP)
    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(Guid id);
        IEnumerable<Order> GetAll();
    }
}