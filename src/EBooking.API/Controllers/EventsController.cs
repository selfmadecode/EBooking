using EBooking.Application.Common;
using EBooking.Application.DTOs.Event;
using EBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EventsController : BaseController
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost]
    [Authorize(Roles = RoleHelper.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        var result = await _eventService.CreateEventAsync(dto, UserId);
        return ReturnResponse(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _eventService.GetAllEventsAsync();
        return ReturnResponse(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _eventService.GetActiveEventsAsync();
        return ReturnResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _eventService.GetEventByIdAsync(id);
        return ReturnResponse(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleHelper.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventDto dto)
    {
        var result = await _eventService.UpdateEventAsync(id, dto, UserId);
        return ReturnResponse(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleHelper.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _eventService.DeleteEventAsync(id);
        return ReturnResponse(result);
    }
}
