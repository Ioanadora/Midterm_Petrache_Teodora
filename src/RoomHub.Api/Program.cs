using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// TimeProvider registered in DI (Lecture 03 / Rule R5)
builder.Services.AddSingleton(TimeProvider.System);

// In-memory repositories
builder.Services.AddSingleton<RoomRepository>();
builder.Services.AddSingleton<BookingRepository>();

// CQRS Handlers auto-registration (Lecture 06)
builder.Services.AddHandlers();

var app = builder.Build();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRoomsEndpoints();
app.MapBookingsEndpoints();

app.Run();
