using EBooking.Application.Interfaces;

namespace EBooking.Infrastructure.Services;

public class PaymentGatewayService : IPaymentGatewayService
{
    public async Task<bool> ProcessPaymentAsync(decimal amount, string paymentMethod, string? cardNumber)
    {
        // simulate network delay
        await Task.Delay(300);

        // simulate failure for invalid card numbers (for testing)
        if (cardNumber != null && cardNumber.StartsWith("0000"))
            return false;

        return true;
    }
}
