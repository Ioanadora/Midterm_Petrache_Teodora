using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public record BookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public static class GetBookingEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", (Guid id, BookingService service) =>
        {
            var b = service.Get(id);
            return Results.Ok(new BookingResponse(
                b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString()));
        });
    }
}
