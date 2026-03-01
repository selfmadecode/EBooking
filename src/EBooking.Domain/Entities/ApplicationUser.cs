using Microsoft.AspNetCore.Identity;

namespace EBooking.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FullName { get; set; }
    public DateTime CreationTime { get; set; }
    public Wallet? Wallet { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
        Id = Guid.NewGuid();
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }
}
