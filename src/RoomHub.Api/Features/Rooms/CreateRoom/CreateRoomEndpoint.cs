using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public record CreateRoomRequest(string? Name, int Capacity);

public record CreateRoomResponse(Guid Id, string Name, int Capacity);

public static class CreateRoomEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", (CreateRoomRequest request, RoomService service) =>
        {
            var room = service.Create(request.Name, request.Capacity);
            return Results.Created(
                $"/api/rooms/{room.Id}",
                new CreateRoomResponse(room.Id, room.Name, room.Capacity));
        });
    }
}
