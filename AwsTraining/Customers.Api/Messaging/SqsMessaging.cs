﻿using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging
{
    public class SqsMessaging : ISqsMessaging
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly QueueSettings _queueSettings;
        private string? _queueUrl;

        public SqsMessaging(IAmazonSQS sqs, IOptions<QueueSettings> options)
        {
            _sqsClient = sqs;
            _queueSettings = options.Value;
        }

        public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
        {
            var queueUrl = await GetQueueUrlAsync();

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = JsonSerializer.Serialize(message),
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

            return await _sqsClient.SendMessageAsync(sendMessageRequest);
        }

        private async ValueTask<string> GetQueueUrlAsync()
        {
            if (_queueUrl is not null)
            {
                return _queueUrl;
            }

            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueSettings.Name);
            _queueUrl = queueUrlResponse.QueueUrl;
            return _queueUrl;
        }
    }
}
