using Amazon.SimpleNotificationService.Model;

namespace Customers.Api.Messaging
{
    public interface ISnsMessaging
    {
        Task<PublishResponse> PublishMessageAsync<T>(T message);
    }
}
