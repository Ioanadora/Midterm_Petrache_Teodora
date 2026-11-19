using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed record DateRange
{
    public DateTime StartUtc { get; }
    public DateTime EndUtc { get; }

    private DateRange(DateTime startUtc, DateTime endUtc)
    {
        StartUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
        EndUtc = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);
    }

    public static Result<DateRange> Create(DateTime startUtc, DateTime endUtc, DateTime nowUtc)
    {
        var start = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);
        var now = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        // R3: endUtc must be strictly after startUtc
        if (end <= start)
            return BookingErrors.InvalidRange;

        // R5: booking cannot start in the past
        if (start < now)
            return BookingErrors.StartInPast;

        // R6: maximum duration of a booking is 4 hours (exactly 4 hours is allowed)
        if (end - start > TimeSpan.FromHours(4))
            return BookingErrors.DurationExceeded;

        return new DateRange(start, end);
    }

    public bool Overlaps(DateRange other)
    {
        return StartUtc < other.EndUtc && other.StartUtc < EndUtc;
    }

    public bool Overlaps(DateTime otherStartUtc, DateTime otherEndUtc)
    {
        return StartUtc < otherEndUtc && otherStartUtc < EndUtc;
    }
}
