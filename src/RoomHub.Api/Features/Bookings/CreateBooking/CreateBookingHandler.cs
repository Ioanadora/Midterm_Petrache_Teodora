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

        // Validates R2, R3, R4, R5, R6, R10
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

        // R9: Maximum 2 active bookings per participant per UTC day across ALL rooms
        var activeCount = bookings.CountActiveForParticipantOnDate(booking.BookedBy, booking.StartUtc);
        var dailyLimitResult = Booking.ValidateDailyLimit(activeCount);
        if (dailyLimitResult.IsFailure)
            return Task.FromResult<Result<BookingCreatedResponse>>(dailyLimitResult.Error);

        // R7: Overlap check for the requested room
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
