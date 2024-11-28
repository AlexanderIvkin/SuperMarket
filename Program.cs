using System;
using System.Collections.Generic;
using System.Linq;

namespace Supermarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Supermarket supermarket = new Supermarket();

            supermarket.Work();
        }
    }

    class Supermarket
    {
        private List<Product> _assortmentProducts = new List<Product>();
        private Queue<Client> _clients = new Queue<Client>();
        private CashRegister _cashRegister = new CashRegister();

        public Supermarket()
        {
            CreateAssortmentProducts();
        }

        public void Work()
        {
            do
            {
                int clientCount = 1;

                Console.Clear();
                GenerateClientsQueue();
                Console.WriteLine($"{_cashRegister.WorkerName} у нас очередь из {_clients.Count} человек, кончай перекур!\n");

                while (_clients.Count > 0)
                {
                    Console.WriteLine($"{_cashRegister.WorkerName} - Берусь за {clientCount} клиента, здравствуйте.\n");

                    _cashRegister.ServiceClient(_clients.Dequeue());

                    clientCount++;
                }
            }
            while (IsRestart());
        }

        private void CreateAssortmentProducts()
        {
            List<string> namesProducts = new List<string>{
                "Фитнесс Крекеры","Варенец Просроченный", "Тыквенное Семя Шелушенное", "Бидон Сыворотки",
                "Рыбьи Хрящики", "Сухарик", "Боярышника Плоды", "Свёрток Свинного Жира Обезжиренного",
                "Творог 2%", "Луковые Шелушки", "Собачий Ворс", "Гниль По Скидке", "Маленечко Пива"
            };

            for (int i = 0; i < namesProducts.Count; i++)
            {
                _assortmentProducts.Add(new Product(namesProducts[i], GeneratePrice()));
            }
        }

        private void GenerateClientsQueue()
        {
            int minClientsCount = 5;
            int maxClientsCount = 10;

            for (int i = 0; i < UserUtils.GenerateRandomNumber(minClientsCount, maxClientsCount); i++)
            {
                _clients.Enqueue(new Client(GetAssortmentProducts()));
            }
        }

        private int GeneratePrice()
        {
            int minPrice = 10;
            int maxPrice = 500;

            return UserUtils.GenerateRandomNumber(minPrice, maxPrice);
        }

        private bool IsRestart()
        {
            ConsoleKey exitKey = ConsoleKey.Escape;
            Console.WriteLine(exitKey + " - нажмите для выхода. Остальные клавиши для новых клиентов.");

            return Console.ReadKey(true).Key != exitKey;
        }

        private List<Product> GetAssortmentProducts()
        {
            return _assortmentProducts.ToList();
        }
    }

    class CashRegister
    {
        public CashRegister()
        {
            WorkerName = "Галя";
        }

        public string WorkerName { get; }

        public void ServiceClient(Client client)
        {
            Console.WriteLine($"{WorkerName} - Так, вот плюс ага, ещё это... С вас {GetCurrentPrice(client.ReturnProductsInCart())}");
            while (GetCurrentPrice(client.ReturnProductsInCart()) > client.Cash)
            {
                Console.WriteLine($"{WorkerName} - ОТМЕНА! Не хватает! Убери чего-нибудь.");

                if (client.HasProducts)
                {
                    client.RemoveRandomProduct();
                }
            }

            if (client.HasProducts)
            {
                Console.WriteLine($"{WorkerName} - Хватает!!! *Шёпотом* Золотце, а ты женат? *Подмигивает*\n");
                client.BuyProducts(GetCurrentPrice(client.ReturnProductsInCart()));
            }
            else
            {
                Console.WriteLine($"{WorkerName} - Ноу Мани Ноу Хани, вали отсюда, СЛЕДУЩИЙ");
            }

            Console.WriteLine("Нажмите что-нибудь, чтобы продолжить.\n");
            Console.ReadKey(true);
        }

        private int GetCurrentPrice(List<Product> clientCart)
        {
            int currentPrice = 0;

            foreach (Product product in clientCart)
            {
                currentPrice += product.Price;
            }

            return currentPrice;
        }
    }

    class Client
    {
        private List<Product> _cart = new List<Product>();
        private List<Product> _bag = new List<Product>();

        public Client(List<Product> assortmentProducts)
        {
            GenerateCash();
            SelectProducts(assortmentProducts);
        }

        public bool HasProducts => _cart.Count > 0;
        public int Cash { get; private set; }

        public void BuyProducts(int purchaseAmount)
        {
            ShowPurchasedProducts(purchaseAmount);
            PayCash(purchaseAmount);
            TransferProducts();
        }

        public void RemoveRandomProduct()
        {
            int index = UserUtils.GenerateRandomNumber(_cart.Count);

            Console.WriteLine($"Клиент - Выложу-ка я {_cart[index].Name}.\n");
            _cart.RemoveAt(index);
        }

        public List<Product> ReturnProductsInCart()
        {
            return _cart.ToList();
        }

        private void GenerateCash()
        {
            int minCashCount = 500;
            int maxCashCount = 3000;

            Cash = UserUtils.GenerateRandomNumber(minCashCount, maxCashCount);
        }

        private void SelectProducts(List<Product> products)
        {
            int minDesiredProductsCount = 3;
            int maxDesiredProductsCount = 10;
            int currentDesiredProductsCount = UserUtils.GenerateRandomNumber(minDesiredProductsCount, maxDesiredProductsCount);

            for (int i = 0; i < currentDesiredProductsCount; i++)
            {
                _cart.Add(products[UserUtils.GenerateRandomNumber(products.Count)]);
            }
        }

        private void ShowPurchasedProducts(int purchaseAmount)
        {
            Console.WriteLine($"Клиент имея {Cash} денег, купил на сумму {purchaseAmount} вот чего всякого:\n");

            foreach (Product product in _cart)
            {
                Console.WriteLine($"{product.Name} за {product.Price} денег.");
            }
        }

        private void TransferProducts()
        {
            int countProducts = _cart.Count;

            for (int i = countProducts - 1; i >= 0; i--)
            {
                _bag.Add(_cart[i]);
                _cart.RemoveAt(i);
            }
        }

        private void PayCash(int purchaseAmount)
        {
            Cash -= purchaseAmount;
        }
    }

    class Product
    {
        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public int Price { get; }
    }

    static class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            return s_random.Next(minValue, maxValue);
        }

        public static int GenerateRandomNumber(int maxValue)
        {
            return s_random.Next(maxValue);
        }
    }
}
