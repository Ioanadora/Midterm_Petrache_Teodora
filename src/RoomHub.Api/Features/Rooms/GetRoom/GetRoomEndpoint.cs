using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRoomQuery, GetRoomResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetRoomQuery(id), ct);
            if (result.IsFailure)
                return result.Error.ToProblem();

            return Results.Ok(result.Value);
        });
    }
}
