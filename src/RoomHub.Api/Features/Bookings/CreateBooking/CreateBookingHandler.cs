using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CreateBookingCommand, BookingCreatedResponse>
{
    public async Task<Result<BookingCreatedResponse>> Handle(CreateBookingCommand command, CancellationToken cancellationToken = default)
    {
        // 404 for a missing room comes before any validation
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        // R2, R3, R4, R5, R6, R10
        var bookingResult = Booking.Create(
            command.RoomId,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            room.Capacity,
            nowUtc);

        if (bookingResult.IsFailure)
            return bookingResult.Error;

        var booking = bookingResult.Value;

        // R9: at most 2 active bookings per participant per UTC day, across all rooms
        var activeCount = await bookings.CountActiveForParticipantOnDateAsync(booking.BookedBy, booking.StartUtc, cancellationToken);
        var dailyLimitResult = Booking.ValidateDailyLimit(activeCount);
        if (dailyLimitResult.IsFailure)
            return dailyLimitResult.Error;

        // R7: no overlap with active bookings of the same room
        var activeBookings = await bookings.ListActiveForRoomAsync(command.RoomId, cancellationToken);
        if (activeBookings.Any(existing => existing.OverlapsWith(booking.StartUtc, booking.EndUtc)))
            return BookingErrors.RoomOccupied;

        bookings.Add(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new BookingCreatedResponse(booking.Id, booking.RoomId, booking.Status.ToString());
    }
}
