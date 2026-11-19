using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            ICommandHandler<CancelBookingCommand, Unit> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new CancelBookingCommand(id), ct);
            if (result.IsFailure)
                return result.Error.ToProblem();

            return Results.NoContent();
        });
    }
}
