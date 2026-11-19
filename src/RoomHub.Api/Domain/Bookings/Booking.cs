using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    private Booking() { }

    private Booking(
        Guid id,
        Guid roomId,
        string bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        BookingStatus status)
    {
        Id = id;
        RoomId = roomId;
        BookedBy = bookedBy;
        StartUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
        EndUtc = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);
        Attendees = attendees;
        Status = status;
    }

    public static Result<Booking> Create(
        Guid roomId,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        int roomCapacity,
        DateTime nowUtc)
    {
        // R2: bookedBy is required
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.InvalidBookedBy;

        // R3, R5, R6 validated via DateRange
        var rangeResult = DateRange.Create(startUtc, endUtc, nowUtc);
        if (rangeResult.IsFailure)
            return rangeResult.Error;

        // R4: attendees strictly positive and at most room capacity
        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees > roomCapacity)
            return BookingErrors.AttendeesExceedCapacity;

        return new Booking(
            Guid.NewGuid(),
            roomId,
            bookedBy.Trim(),
            rangeResult.Value.StartUtc,
            rangeResult.Value.EndUtc,
            attendees,
            BookingStatus.Active);
    }

    public Result<Unit> Cancel()
    {
        // R8: cancelling an already cancelled booking gives 409
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }

    public bool OverlapsWith(DateTime startUtc, DateTime endUtc)
    {
        // R7: only active bookings block
        if (Status != BookingStatus.Active)
            return false;

        return StartUtc < endUtc && startUtc < EndUtc;
    }
}
