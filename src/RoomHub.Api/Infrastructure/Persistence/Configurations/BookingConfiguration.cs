using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.BookedBy).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Attendees).IsRequired();

        builder.Property(b => b.StartUtc).IsRequired().HasConversion(new UtcDateTimeConverter());
        builder.Property(b => b.EndUtc).IsRequired().HasConversion(new UtcDateTimeConverter());

        builder.Property(b => b.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.RoomId, b.Status });
        builder.HasIndex(b => new { b.BookedBy, b.StartUtc });
    }
}
