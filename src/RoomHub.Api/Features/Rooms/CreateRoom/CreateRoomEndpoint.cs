using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateRoomRequest request,
            ICommandHandler<CreateRoomCommand, CreateRoomResponse> handler,
            CancellationToken ct) =>
        {
            var command = new CreateRoomCommand(request.Name, request.Capacity);
            var result = await handler.Handle(command, ct);
            if (result.IsFailure)
                return result.Error.ToProblem();

            return Results.Created($"/api/rooms/{result.Value.Id}", result.Value);
        });
    }
}
