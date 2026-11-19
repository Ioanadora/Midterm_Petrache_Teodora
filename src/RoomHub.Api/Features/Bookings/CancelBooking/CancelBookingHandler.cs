using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public class CancelBookingHandler(BookingRepository bookings) : ICommandHandler<CancelBookingCommand, Unit>
{
    public Task<Result<Unit>> Handle(CancelBookingCommand command, CancellationToken cancellationToken = default)
    {
        var booking = bookings.Get(command.Id);
        if (booking is null)
            return Task.FromResult<Result<Unit>>(BookingErrors.NotFound);

        var cancelResult = booking.Cancel();
        if (cancelResult.IsFailure)
            return Task.FromResult<Result<Unit>>(cancelResult.Error);

        return Task.FromResult<Result<Unit>>(Unit.Value);
    }
}
