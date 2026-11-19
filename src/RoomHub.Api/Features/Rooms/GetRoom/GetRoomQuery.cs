using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public record GetRoomResponse(Guid Id, string Name, int Capacity);

public record GetRoomQuery(Guid Id) : IQuery<GetRoomResponse>;
