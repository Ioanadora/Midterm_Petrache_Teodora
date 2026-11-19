using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public class CreateRoomHandler(RoomRepository rooms) : ICommandHandler<CreateRoomCommand, CreateRoomResponse>
{
    public Task<Result<CreateRoomResponse>> Handle(CreateRoomCommand command, CancellationToken cancellationToken = default)
    {
        var roomResult = Room.Create(command.Name, command.Capacity);
        if (roomResult.IsFailure)
            return Task.FromResult<Result<CreateRoomResponse>>(roomResult.Error);

        var room = roomResult.Value;
        rooms.Add(room);

        return Task.FromResult<Result<CreateRoomResponse>>(
            new CreateRoomResponse(room.Id, room.Name, room.Capacity));
    }
}
