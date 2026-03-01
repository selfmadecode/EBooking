namespace EBooking.Application.Interfaces
{
    public interface IPaymentGatewayService
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string paymentMethod, string? cardNumber);
    }
}
