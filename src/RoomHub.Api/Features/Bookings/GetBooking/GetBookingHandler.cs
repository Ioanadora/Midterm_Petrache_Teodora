using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public class GetBookingHandler(BookingRepository bookings) : IQueryHandler<GetBookingQuery, BookingResponse>
{
    public Task<Result<BookingResponse>> Handle(GetBookingQuery query, CancellationToken cancellationToken = default)
    {
        var booking = bookings.Get(query.Id);
        if (booking is null)
            return Task.FromResult<Result<BookingResponse>>(BookingErrors.NotFound);

        return Task.FromResult<Result<BookingResponse>>(
            new BookingResponse(
                booking.Id,
                booking.RoomId,
                booking.BookedBy,
                booking.StartUtc,
                booking.EndUtc,
                booking.Attendees,
                booking.Status.ToString()));
    }
}
