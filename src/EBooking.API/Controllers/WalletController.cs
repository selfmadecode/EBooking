using EBooking.Application.DTOs.Wallet;
using EBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WalletController : BaseController
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWallet()
    {
        var result = await _walletService.GetWalletAsync(UserId);
        return ReturnResponse(result);
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpWalletDto dto)
    {
        var result = await _walletService.TopUpWalletAsync(UserId, dto);
        return ReturnResponse(result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions()
    {
        var result = await _walletService.GetTransactionHistoryAsync(UserId);
        return ReturnResponse(result);
    }
}
