using Webshop.Events;

namespace _10_11_2026_øvelse3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cart = new ShoppingCart();
            var lager = new Lager();
            var kvittering = new Kvittering();

            lager.Subscribe(cart);
            kvittering.Subscribe(cart);

            Product iphone = WebshopData.Products[0]; // iPhone 13, 3 på lager
            Product nokia = WebshopData.Products[2]; // Nokia telefon, 2 på lager
            Product statTlf = WebshopData.Products[3]; // Stationær telefon, 50 på lager

            
            cart.AddItem(iphone, 1);
            cart.AddItem(nokia, 1);
            cart.AddItem(statTlf, 2);
        }
    }
}
