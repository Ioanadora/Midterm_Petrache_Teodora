using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public record CreateBookingRequest(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

public record BookingCreatedResponse(Guid Id, Guid RoomId, string Status);

public static class CreateBookingEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{roomId:guid}/bookings", (Guid roomId, CreateBookingRequest request, BookingService service) =>
        {
            var booking = service.Create(
                roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees);

            return Results.Created(
                $"/api/bookings/{booking.Id}",
                new BookingCreatedResponse(booking.Id, booking.RoomId, booking.Status.ToString()));
        });
    }
}
