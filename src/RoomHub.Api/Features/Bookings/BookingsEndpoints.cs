using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.GetBooking;

namespace RoomHub.Api.Features.Bookings;

public static class BookingsEndpoints
{
    public static void MapBookingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings").WithTags("Bookings");
        GetBookingEndpoint.Map(group);
        CancelBookingEndpoint.Map(group);
    }
}
