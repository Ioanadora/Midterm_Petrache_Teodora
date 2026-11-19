using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public class CreateBookingHandler(
    RoomRepository rooms,
    BookingRepository bookings,
    TimeProvider timeProvider) : ICommandHandler<CreateBookingCommand, BookingCreatedResponse>
{
    public Task<Result<BookingCreatedResponse>> Handle(CreateBookingCommand command, CancellationToken cancellationToken = default)
    {
        // Precedence: Room check FIRST! Returns 404 before validation
        var room = rooms.Get(command.RoomId);
        if (room is null)
            return Task.FromResult<Result<BookingCreatedResponse>>(RoomErrors.NotFound);

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var bookingResult = Booking.Create(
            command.RoomId,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            room.Capacity,
            nowUtc);

        if (bookingResult.IsFailure)
            return Task.FromResult<Result<BookingCreatedResponse>>(bookingResult.Error);

        var booking = bookingResult.Value;

        // R7: Overlap check
        var existingBookings = bookings.ListForRoom(command.RoomId);
        foreach (var existing in existingBookings)
        {
            if (existing.OverlapsWith(booking.StartUtc, booking.EndUtc))
                return Task.FromResult<Result<BookingCreatedResponse>>(BookingErrors.RoomOccupied);
        }

        bookings.Add(booking);

        return Task.FromResult<Result<BookingCreatedResponse>>(
            new BookingCreatedResponse(booking.Id, booking.RoomId, booking.Status.ToString()));
    }
}
