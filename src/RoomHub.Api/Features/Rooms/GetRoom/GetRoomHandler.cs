using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public class GetRoomHandler(RoomRepository rooms) : IQueryHandler<GetRoomQuery, GetRoomResponse>
{
    public Task<Result<GetRoomResponse>> Handle(GetRoomQuery query, CancellationToken cancellationToken = default)
    {
        var room = rooms.Get(query.Id);
        if (room is null)
            return Task.FromResult<Result<GetRoomResponse>>(RoomErrors.NotFound);

        return Task.FromResult<Result<GetRoomResponse>>(
            new GetRoomResponse(room.Id, room.Name, room.Capacity));
    }
}
