using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

public class PaymentsController(
    IPaymentService paymentService,
    IUnitOfWork unit,
    ILogger<PaymentsController> logger,
    IConfiguration config,
    IHubContext<NotificationHub> hubContext) : BaseApiController
{
    private readonly string? _whSecret = config["StripeSettings:WhSecret"];

    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);


        if (cart == null) return BadRequest(new ProblemDetails { Title = "Problem with your cart" });

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await unit.Repository<DeliveryMethod>().ListAllAsync());
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = ConstructStripeEvent(json);
            if (stripeEvent.Data.Object is not PaymentIntent intent) return BadRequest("Invalid event data");
            await HandlePaymentIntentSucceeded(intent);

            return Ok();
        }
        catch (StripeException e)
        {
            logger.LogError(e, "Stripe webhook error");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            var spec = new OrderSpecification(intent.Id, true);
            var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null)
            {
                logger.LogError("Order not found for payment intent: {PaymentIntentId}", intent.Id);
                throw new Exception("Order not found for payment intent");
            }

            order.Status = (long)order.GetTotal() * 100 != intent.Amount
                ? OrderStatus.PaymentMismatch
                : OrderStatus.PaymentReceived;
            unit.Repository<Order>().Update(order);

            await unit.Complete();
            logger.LogInformation("Order {OrderId} payment received", order.Id);

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

            if (!string.IsNullOrEmpty(connectionId))
                await hubContext.Clients.Client(connectionId)
                    .SendAsync("OrderCompleteNotification", order.ToDto());
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Stripe webhook error");
            throw new StripeException("Stripe webhook error", e);
        }
    }
}