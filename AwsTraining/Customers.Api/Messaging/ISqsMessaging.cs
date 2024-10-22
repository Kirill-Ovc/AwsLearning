using Amazon.SQS.Model;

namespace Customers.Api.Messaging
{
    public interface ISqsMessaging
    {
        Task<SendMessageResponse> SendMessageAsync<T>(T message);
    }
}
