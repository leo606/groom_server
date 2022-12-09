using Grpc.Core;
using gRoom.gRPC.Messages;

namespace gRoom.gRPC.Services;

public class GroomService : Groom.GroomBase
{
  private readonly ILogger<GroomService> _logger;
  public GroomService(ILogger<GroomService> logger)
  {
    _logger = logger;
  }

  public override Task<RoomRegistrationResponse> RoomRegistration(RoomRegistrationRequest request, ServerCallContext context)
  {
    _logger.LogInformation("service called :)");

    var rnd = new Random();
    var roomNumber = rnd.Next(1, 100);
    _logger.LogInformation($"room number {roomNumber}");
    var resp = new RoomRegistrationResponse { RoomId = roomNumber };
    return Task.FromResult(resp);
  }

  public override async Task<NewsStreamStatus> SendNewsFlash(IAsyncStreamReader<NewsFlash> newsStream, ServerCallContext context)
  {
    while (await newsStream.MoveNext())
    {
      var news = newsStream.Current;
      Console.WriteLine($"news flash: {news.NewsItem}");
    }

    return new NewsStreamStatus { Success = true };
  }
}