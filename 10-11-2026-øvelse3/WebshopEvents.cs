// ============================================================
// Webshop – klasser og testdata til event-øvelserne (uge 46)
//
// Kopiér filen ind i et Console App (.NET 6 eller nyere), og skriv
// using Webshop.Events; øverst i Program.cs.
// Produkterne er de samme som i WebshopData.cs fra uge 40 og i
// webshop-databasen fra uge 38, også StockQuantity (lagerbeholdning).
//
// ShoppingCart er publisher i Øvelse 3. De steder, hvor I selv skal
// skrive kode, er markeret med TODO.
//
// Eksempel:
//   var cart = new ShoppingCart();
//   cart.AddItem(WebshopData.Products[0], 1);   // iPhone 13
// ============================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webshop.Events
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    // Én linje i kurven: et produkt og et antal
    public class CartItem
    {
        public Product Product { get; set; } = new();
        public int Quantity { get; set; }
        public decimal LineTotal => Product.Price * Quantity;
    }

    // Data, der sendes med, når en vare lægges i kurven.
    // Brug klassen som skabelon, når I selv skal lave LowStockEventArgs i Øvelse 3.3.
    public class CartEventArgs : EventArgs
    {
        public Product Product { get; }
        public int Quantity { get; }
        public decimal CartTotal { get; }

        public CartEventArgs(Product product, int quantity, decimal cartTotal)
        {
            Product = product;
            Quantity = quantity;
            CartTotal = cartTotal;
        }
    }

    public class LowStockEventArgs : EventArgs
    {
        public Product Product { get; }

        public LowStockEventArgs(Product product)
        {
            Product = product;
        }
    }

    // Publisher: kurven fortæller andre, når der sker noget
    public class ShoppingCart
    {
        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items;
        public int ItemCount => _items.Sum(i => i.Quantity);
        public decimal Total => _items.Sum(i => i.LineTotal);

        // TODO Øvelse 3.1: Erklær eventet ItemAdded her (EventHandler<CartEventArgs>)
        public event EventHandler<CartEventArgs>? ItemAdded;

        public void AddItem(Product product, int quantity)
        {
            _items.Add(new CartItem { Product = product, Quantity = quantity });

            // TODO Øvelse 3.1: Udløs ItemAdded her med ?.Invoke(this, new CartEventArgs(...))
            ItemAdded?.Invoke(this, new CartEventArgs(product, quantity, Total));
        }
    }

    public class Kvittering
    {
        public void Subscribe(ShoppingCart cart)
        {
            cart.ItemAdded += OnItemAdded;
        }

        public void OnItemAdded(object? sender, CartEventArgs e)
        {
            Console.WriteLine($"Kvittering: {e.Quantity}x {e.Product.ProductName}" +
                $" à {e.Product.Price}\nKurv i alt: {e.CartTotal}\n");
        }
    }

    public class Lager
    {
        public event EventHandler<LowStockEventArgs>? LowStock;

        public void Subscribe(ShoppingCart cart)
        {
            cart.ItemAdded += OnItemAdded;
        }

        private void OnItemAdded(object? sender, CartEventArgs e)
        {
            e.Product.StockQuantity -= e.Quantity;
            Console.WriteLine($"Ny lagerbeholdning på {e.Product.ProductName}: " +
                $"{e.Product.StockQuantity} stk.");

            if (e.Product.StockQuantity < 5)
            {
                LowStock?.Invoke(this, new LowStockEventArgs(e.Product));
            }
        }
    }

    public class Indkoeb()
    {
        public void Subscribe(Lager lager)
        {
            lager.LowStock += OnLowStock;
        }

        private void OnLowStock(object? sender, LowStockEventArgs e)
        {
            Console.WriteLine($"Indkøb: Bestil flere {e.Product.ProductName} " +
                $"({e.Product.StockQuantity} stk. tilbage på lager)");
        }
    }

    public static class WebshopData
    {
        public static List<Product> Products { get; } = new()
        {
            new() { ProductId = 1, ProductName = "iPhone 13",          CategoryId = 4, Price = 4999.99m, StockQuantity = 3 },
            new() { ProductId = 2, ProductName = "Samsung Galaxy S21", CategoryId = 4, Price = 2699.99m, StockQuantity = 15 },
            new() { ProductId = 3, ProductName = "Nokia Telefon",      CategoryId = 4, Price = 999.99m,  StockQuantity = 2 },
            new() { ProductId = 4, ProductName = "Stationær Telefon",  CategoryId = 4, Price = 349.99m,  StockQuantity = 50 },
            new() { ProductId = 5, ProductName = "Bærbar Computer",    CategoryId = 1, Price = 2200.00m, StockQuantity = 2 },
            new() { ProductId = 6, ProductName = "Tablet",             CategoryId = 1, Price = 1300.00m, StockQuantity = 4 },
        };
    }
}
