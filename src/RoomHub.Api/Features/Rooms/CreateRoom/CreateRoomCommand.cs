using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public record CreateRoomRequest(string? Name, int Capacity);

public record CreateRoomResponse(Guid Id, string Name, int Capacity);

public record CreateRoomCommand(string? Name, int Capacity) : ICommand<CreateRoomResponse>;
