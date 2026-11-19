using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBookingQuery, BookingResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetBookingQuery(id), ct);
            if (result.IsFailure)
                return result.Error.ToProblem();

            return Results.Ok(result.Value);
        });
    }
}
