using System;
using Xunit;
using RestaurantSystem.Domain;

namespace RestaurantSystem.Tests
{
    public class RestaurantDomainTests
    {
        // Тест 1: Перевірка правильного розрахунку суми однієї позиції меню
        [Fact]
        public void OrderItem_CalculateTotal_ShouldReturnCorrectValue()
        {
            // Arrange
            var dish = new DishItem("Піца Чотири Сири", 250.00m, 15);
            var orderItem = new OrderItem(dish, 3); // 3 піци

            // Act
            decimal total = orderItem.CalculateTotal();

            // Assert
            Assert.Equal(750.00m, total);
        }

        // Тест 2: Граничний кейс — Перевірка валідації від'ємної ціни в конструкторі
        [Fact]
        public void MenuComponent_Constructor_ShouldThrowException_WhenPriceIsNegative()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new DishItem("Суп", -10.00m, 10));
            Assert.Contains("Ціна повинна бути більшою за нуль", exception.Message);
        }

        // Тест 3: Перевірка інкапсуляції та накопичення кількості страв при повторному додаванні
        [Fact]
        public void Order_AddItem_ShouldAccumulateQuantity_IfItemAlreadyExists()
        {
            // Arrange
            var order = new Order(1);
            var juice = new DrinkItem("Мохіто", 90.00m, false);

            // Act
            order.AddItem(juice, 2);
            order.AddItem(juice, 3); // Додаємо той самий напій ще раз

            // Assert
            Assert.Single(order.Items); // В списку повинен залишитися 1 унікальний об'єкт
            foreach (var item in order.Items)
            {
                Assert.Equal(5, item.Quantity); // Сумарна кількість має бути 5
            }
        }

        // Тест 4: Негативний кейс — заборона додавання страв у закритий чек
        [Fact]
        public void Order_AddItem_ShouldThrowException_WhenOrderIsAlreadyPaid()
        {
            // Arrange
            var order = new Order(2);
            var dish = new DishItem("Салат Цезар", 180.00m, 12);
            order.AddItem(dish, 1);
            order.CloseAndPay();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => order.AddItem(dish, 1));
        }

        // Тест 5: Перевірка поліморфної поведінки категорій продуктів
        [Fact]
        public void MenuComponent_Polymorphism_ShouldReturnCorrectCategoryStrings()
        {
            // Arrange
            MenuComponent steak = new DishItem("Стейк", 350.00m, 20);
            MenuComponent wine = new DrinkItem("Червоне вино", 120.00m, true);

            // Act & Assert
            Assert.Equal("Кухня (Страва)", steak.GetCategoryType());
            Assert.Equal("Бар (Алкоголь)", wine.GetCategoryType());
        }
    }
}