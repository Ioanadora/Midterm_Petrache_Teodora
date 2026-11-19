namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // R7: active bookings of a room, used by the domain overlap check
    Task<List<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default);

    // R9: active bookings of a participant that start on the given UTC day, across all rooms
    Task<int> CountActiveForParticipantOnDateAsync(string bookedBy, DateTime dateUtc, CancellationToken cancellationToken = default);

    void Add(Booking booking);
}
