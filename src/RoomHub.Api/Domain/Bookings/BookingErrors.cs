using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class BookingErrors
{
    public static readonly Error NotFound = Error.NotFound("BookingNotFound", "The booking does not exist.");
    public static readonly Error InvalidBookedBy = Error.Validation("InvalidBookedBy", "BookedBy is required.");
    public static readonly Error InvalidRange = Error.Validation("InvalidRange", "The end must be after the start.");
    public static readonly Error StartInPast = Error.Validation("StartInPast", "Booking cannot start in the past.");
    public static readonly Error DurationExceeded = Error.Validation("DurationExceeded", "Booking duration exceeds the maximum allowed.");
    public static readonly Error InvalidAttendees = Error.Validation("InvalidAttendees", "Attendees must be positive.");
    public static readonly Error AttendeesExceedCapacity = Error.Validation("AttendeesExceedCapacity", "The room does not have enough seats.");
    public static readonly Error RoomOccupied = Error.Conflict("RoomOccupied", "The room is occupied in this interval.");
    public static readonly Error AlreadyCancelled = Error.Conflict("BookingAlreadyCancelled", "The booking is already cancelled.");
    public static readonly Error DailyLimitReached = Error.Conflict("DailyLimitReached", "Participant cannot have more than 2 active bookings on the same UTC day.");
    public static readonly Error CancellationTooLate = Error.Conflict("CancellationTooLate", "A booking can only be cancelled at least 60 minutes before its start.");
}
