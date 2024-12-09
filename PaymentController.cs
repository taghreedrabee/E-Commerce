using Microsoft.AspNetCore.Mvc;
using E_Commerce.Models;
using E_Commerce.DTOs;
using E_Commerce.Services;
using Stripe;
using Stripe.Checkout;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public CartServices _cartServices { get; set; }
        public E_CommerceDB _context;

        public PaymentController(CartServices cartServices, E_CommerceDB context) { 
            _cartServices = cartServices; 
            _context = context;
        }

        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession([FromQuery] int Userid)
        {
            var LinkItems = new List<SessionLineItemOptions>();
            var Items = await _cartServices.GetAllToOrder(Userid);
            var options1 = new PaymentIntentCreateOptions
            {
                Amount = (long)await _cartServices.GetPrice(Userid),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", 123.ToString()},  
                    { "CustomerEmail", "deda@mail.com" }
                }
            };
            var service1 = new PaymentIntentService();
            PaymentIntent paymentIntent = await service1.CreateAsync(options1);
            var order = new Order
            {
                Amount = await _cartServices.GetPrice(Userid),
                Date = DateTime.Now,
                Method = Models.PaymentMethod.creditCard,
                UserId = Userid,

            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            int cartid = Items.FirstOrDefault().CartId;

            foreach (var item in Items) {
                OrderDetails details = new OrderDetails
                {
                    OrderId = order.Id,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };
                await _context.OrderDetails.AddAsync(details);
                await _context.SaveChangesAsync();
            }
            var cart = _context.Carts.Where(a => a.Id == cartid).FirstOrDefault();
            _context.Carts.Remove(cart);

            var cartProducts = _context.CartProducts.Where(a  => a.CartId == cartid).ToList();
            _context.CartProducts.RemoveRange(cartProducts);
            _context.SaveChanges();

            return Ok(new { clientsecret = paymentIntent.ClientSecret });
        }

        [HttpGet("list-payments")]
        [Authorize]
        public async Task<IActionResult> ListPayments()
        {
            try
            {
                var service = new PaymentIntentService();
                var options = new PaymentIntentListOptions
                {
                    Limit = 10,  
                };

                var paymentIntents = await service.ListAsync(options);


                if (paymentIntents.Data.Count == 0)
                {
                    return Ok(new { message = "No payments found." });
                }

                return Ok(paymentIntents);
            }
            catch (StripeException e)
            {
                return StatusCode(500, new { error = e.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
