using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public class BookingRepository(RoomHubDbContext db) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return db.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public Task<List<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return db.Bookings
            .AsNoTracking()
            .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountActiveForParticipantOnDateAsync(string bookedBy, DateTime dateUtc, CancellationToken cancellationToken = default)
    {
        var dayStart = DateTime.SpecifyKind(dateUtc.Date, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        return db.Bookings
            .AsNoTracking()
            .CountAsync(b =>
                b.BookedBy == bookedBy &&
                b.Status == BookingStatus.Active &&
                b.StartUtc >= dayStart &&
                b.StartUtc < dayEnd,
                cancellationToken);
    }

    public void Add(Booking booking)
    {
        db.Bookings.Add(booking);
    }
}
