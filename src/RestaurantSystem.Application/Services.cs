using System;
using System.Collections.Generic;
using RestaurantSystem.Domain;

namespace RestaurantSystem.Application
{
    public class OrderApplicationService
    {
        private readonly IOrderRepository _orderRepository;

        // Впровадження залежностей через конструктор (SOLID: DIP)
        public OrderApplicationService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        // Вертикальний зріз: Сценарій створення нового замовлення
        public Order StartNewOrder(int tableNumber)
        {
            var order = new Order(tableNumber);
            _orderRepository.Save(order);
            return order;
        }

        // Вертикальний зріз: Сценарій додавання страви/напою до замовлення
        public void AddPositionToOrder(Guid orderId, MenuComponent item, int quantity)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new KeyNotFoundException("Замовлення із вказаним ID не знайдено.");

            order.AddItem(item, quantity);
            _orderRepository.Save(order); // Оновлюємо стан у сховищі
        }

        // Вертикальний зріз: Сценарій оплати чека
        public decimal ProcessPayment(Guid orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new KeyNotFoundException("Замовлення із вказаним ID не знайдено.");

            order.CloseAndPay();
            _orderRepository.Save(order);
            return order.CalculateTotal();
        }
    }
}