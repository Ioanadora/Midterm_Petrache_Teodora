using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public record BookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public record GetBookingQuery(Guid Id) : IQuery<BookingResponse>;
