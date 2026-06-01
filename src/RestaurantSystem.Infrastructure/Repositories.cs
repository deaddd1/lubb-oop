using System;
using System.Collections.Generic;
using RestaurantSystem.Domain;

namespace RestaurantSystem.Infrastructure
{
    // Імплементація інтерфейсу сховища (In-Memory версія для Ітерації 1)
    public class InMemoryOrderRepository : IOrderRepository
    {
        // Використання Dictionary для забезпечення O(1) швидкості доступу
        private readonly Dictionary<Guid, Order> _storage = new Dictionary<Guid, Order>();

        public void Save(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            _storage[order.Id] = order; // Зберігаємо або оновлюємо існуюче
        }

        public Order GetById(Guid id)
        {
            _storage.TryGetValue(id, out var order);
            return order;
        }

        public IEnumerable<Order> GetAll()
        {
            return _storage.Values;
        }
    }
}