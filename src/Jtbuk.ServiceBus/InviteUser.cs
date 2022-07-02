namespace Jtbuk.ServiceBus;
public record EnableUser(string Name, List<Guid> AppUniqueIds);
public record InviteUser(string Name, List<Guid> AppUniqueIds);

