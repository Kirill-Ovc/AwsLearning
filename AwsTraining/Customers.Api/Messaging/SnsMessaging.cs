using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging
{
    public class SnsMessaging : ISnsMessaging
    {
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly QueueSettings _topicSettings;
        private string? _topicArn;

        public SnsMessaging(IAmazonSimpleNotificationService sns, IOptions<QueueSettings> options)
        {
            _snsClient = sns;
            _topicSettings = options.Value;
        }

        public async Task<PublishResponse> PublishMessageAsync<T>(T message)
        {
            var topicArn = await GetTopicArnAsync();

            var sendMessageRequest = new PublishRequest
            {
                TopicArn = topicArn,
                Message = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>()
                {
                    {
                        "MessageType",
                        new MessageAttributeValue
                        {
                            DataType = "String",
                            StringValue = typeof(T).Name
                        }
                    }
                }
            };

            return await _snsClient.PublishAsync(sendMessageRequest);
        }

        private async ValueTask<string> GetTopicArnAsync()
        {
            if (_topicArn is not null)
            {
                return _topicArn;
            }

            var queueUrlResponse = await _snsClient.FindTopicAsync(_topicSettings.Name);
            _topicArn = queueUrlResponse.TopicArn;
            return _topicArn;
        }
    }
}
