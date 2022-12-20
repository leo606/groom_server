using Grpc.Core;
using gRoom.gRPC.Messages;
using Google.Protobuf.WellKnownTypes;
using gRoom.gRPC.Utils;

namespace gRoom.gRPC.Services;

public class GroomService : Groom.GroomBase
{
  private readonly ILogger<GroomService> _logger;
  public GroomService(ILogger<GroomService> logger)
  {
    _logger = logger;
  }

  public override async Task<RoomRegistrationResponse> RoomRegistration(RoomRegistrationRequest request, ServerCallContext context)
  {
    UsersQueues.CreateUserQueue(request.RoomName, request.UserName);
    var resp = new RoomRegistrationResponse(){Joined=true};
    return await Task.FromResult(resp);
  }

  public override async Task<NewsStreamStatus> SendNewsFlash(IAsyncStreamReader<NewsFlash> newsStream, ServerCallContext context)
  {
    while (await newsStream.MoveNext())
    {
      var news = newsStream.Current;
      MessagesQueue.AddNewsToQueue(news);
      Console.WriteLine($"news flash: {news.NewsItem}");
    }

    return new NewsStreamStatus { Success = true };
  }

  public override async Task StartMonitoring(Empty request, IServerStreamWriter<ReceivedMessage> streamWriter, ServerCallContext context)
  {
    while (true)
    {
      // await streamWriter.WriteAsync(new ReceivedMessage { MsgTime = Timestamp.FromDateTime(DateTime.UtcNow), User = "1", Contents = "Test msg" });
      if (MessagesQueue.GetNextMessagesCount() > 0)
      {
        await streamWriter.WriteAsync(MessagesQueue.GetNextMessage());
      }
      await Task.Delay(500);
    }
  }
}