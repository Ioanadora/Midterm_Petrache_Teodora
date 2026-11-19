using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Repositories;

public class BookingRepository
{
    private readonly List<Booking> _bookings = new();
    private readonly object _lock = new();

    public void Add(Booking booking)
    {
        lock (_lock)
        {
            _bookings.Add(booking);
        }
    }

    public Booking? Get(Guid id)
    {
        lock (_lock)
        {
            return _bookings.FirstOrDefault(b => b.Id == id);
        }
    }

    public List<Booking> ListForRoom(Guid roomId)
    {
        lock (_lock)
        {
            return _bookings.Where(b => b.RoomId == roomId).ToList();
        }
    }

    // R9: Count active bookings for a participant on a specific UTC day across ALL rooms
    public int CountActiveForParticipantOnDate(string bookedBy, DateTime dateUtc)
    {
        lock (_lock)
        {
            var targetDate = dateUtc.Date;
            return _bookings.Count(b =>
                b.Status == BookingStatus.Active &&
                string.Equals(b.BookedBy, bookedBy, StringComparison.OrdinalIgnoreCase) &&
                b.StartUtc.Date == targetDate);
        }
    }
}
