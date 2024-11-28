using System;
using System.Collections.Generic;
using System.Linq;

namespace Supermarket
{
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
        private Queue<Client> _clients = new Queue<Client>();
        private CashRegister _cashRegister = new CashRegister();
        private List<Product> _assortmentProducts = new List<Product>();

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
                Console.WriteLine($"{_cashRegister.ReturnWorkerName()} у нас очередь из {_clients.Count} человек, кончай перекур!\n");

                while (_clients.Count > 0)
                {
                    Console.WriteLine($"{_cashRegister.ReturnWorkerName()} - Берусь за {clientCount} клиента, здравствуйте.");

                    _cashRegister.ServiceClient(_clients.Dequeue());

                    clientCount++;
                }
            }
            while (IsRestart());
        }

        private bool IsRestart()
        {
            ConsoleKey exitKey = ConsoleKey.Escape;
            Console.WriteLine(exitKey + " - нажмите для выхода. Остальные клавиши для новых клиентов.");

            return Console.ReadKey(true).Key != exitKey;
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
                _clients.Enqueue(new Client(ReturnAssortmentProducts()));
            }
        }

        private List<Product> ReturnAssortmentProducts()
        {
            return _assortmentProducts.ToList();
        }

        private int GeneratePrice()
        {
            int minPrice = 10;
            int maxPrice = 500;

            return UserUtils.GenerateRandomNumber(minPrice, maxPrice);
        }
    }

    class CashRegister
    {
        private Client _client;
        private string _workerName;
        private int _fullPrice;

        public CashRegister()
        {
            _workerName = "Галя";
        }

        public void ServiceClient(Client client)
        {
            _client = client;

            CalculatePurchasePrice();

            while (TryPayPurchase() == false)
            {
                Console.WriteLine($"{_workerName} - ОТМЕНА! Этот понабрал на {_fullPrice}, имея всего-навсего , сколоко там у вас? - {_client.Cash}.");

                if (_client.HasProducts)
                {
                    _client.RemoveRandomProduct();
                    CalculatePurchasePrice();
                }
            }

            if (_client.HasProducts)
            {
                _client.ShowPurchasedProducts(_fullPrice);
                _client.BuyProducts(_fullPrice);
            }
            else
            {
                Console.WriteLine($"{_workerName} - Чё ты ваще припёрся без денег? Позырить просто? Нечего тут шляться!(Клиент покидает суровый сельский магазин)");
            }

            Console.WriteLine("Нажмите что-нибудь, чтобы продолжить.\n");
            Console.ReadKey(true);
        }

        public string ReturnWorkerName()
        {
            return _workerName;
        }

        private void CalculatePurchasePrice()
        {
            ResetFullPrice();

            foreach (Product product in _client.ReturnProductsInCart())
            {
                _fullPrice += product.Price;
            }
        }

        private void ResetFullPrice()
        {
            _fullPrice = 0;
        }

        private bool TryPayPurchase()
        {
            return _fullPrice <= _client.Cash;
        }
    }

    class Client
    {
        private List<Product> _cart = new List<Product>();
        private List<Product> _bag = new List<Product>();

        public Client(List<Product> assortmentProducts)
        {
            GenerateClientCash();
            SelectProducts(assortmentProducts);
        }

        public bool HasProducts => _cart.Count > 0;
        public int Cash { get; private set; }

        public void BuyProducts(int purchaseAmount)
        {
            PayCash(purchaseAmount);
            TransferProducts();
        }


        public void ShowPurchasedProducts(int purchaseAmount)
        {
            Console.WriteLine($"Клиент имея {Cash} денег, купил на сумму {purchaseAmount} вот чего всякого:");

            foreach (Product product in _cart)
            {
                Console.WriteLine($"{product.Name} за {product.Price} денег.");
            }
        }

        public void RemoveRandomProduct()
        {
            int index = UserUtils.GenerateRandomNumber(_cart.Count);

            _cart.RemoveAt(index);
        }

        public List<Product> ReturnProductsInCart()
        {
            return _cart.ToList();
        }

        private void GenerateClientCash()
        {
            int minCashCount = 500;
            int maxCashCount = 3000;

            Cash = UserUtils.GenerateRandomNumber(minCashCount, maxCashCount);
        }

        private void SelectProducts(List<Product> products)
        {
            int maxDesiredProductsCount = 10;

            for (int i = 0; i < UserUtils.GenerateRandomNumber(maxDesiredProductsCount); i++)
            {
                _cart.Add(products[UserUtils.GenerateRandomNumber(products.Count)]);
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
        public string Name { get; }
        public int Price { get; }

        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }
    }
}
