using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = Core.Entities.Product;

namespace Infrastructure.Services;

public class PaymentService(
    IConfiguration config,
    ICartService cartService,
    IUnitOfWork unit
)
    : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        var cart = await cartService.GetCartAsync(cartId);
        if (cart == null) return null;
        var shippingPrice = 0m;
        if (cart.DeliveryMethodId.HasValue)
        {
            var dm = await unit.Repository<DeliveryMethod>().GetByIdAsync(cart.DeliveryMethodId.Value);
            shippingPrice = dm?.Price ?? 0;
        }

        foreach (var item in cart.Items)
        {
            var product = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
            if (product == null) continue;
            if (item.Price != product.Price)
                item.Price = product.Price;
        }

        var service = new PaymentIntentService();

        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * i.Price * 100) + (long)(shippingPrice * 100),
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };
            var intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * i.Price * 100) + (long)(shippingPrice * 100)
            };

            await service.UpdateAsync(cart.PaymentIntentId, options);
        }

        await cartService.SetCartAsync(cart);
        return cart;
    }
}