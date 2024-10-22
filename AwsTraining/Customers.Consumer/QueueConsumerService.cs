using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Customers.Consumer
{
    public class QueueConsumerService : BackgroundService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly QueueSettings _queueSettings;
        private readonly IMediator _mediator;
        private readonly ILogger<QueueConsumerService> _logger;

        public QueueConsumerService(IAmazonSQS sqsClient, 
            IOptions<QueueSettings> queueSettings, 
            IMediator mediator, ILogger<QueueConsumerService> logger)
        {
            _sqsClient = sqsClient;
            _mediator = mediator;
            _logger = logger;
            _queueSettings = queueSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueSettings.Name, stoppingToken);
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MessageAttributeNames = new List<string> { "All" },
                MaxNumberOfMessages = 1
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
                foreach (var message in response.Messages)
                {
                    var messageType = message.MessageAttributes["MessageType"].StringValue;
                    var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");
                    if (type is null)
                    {
                        _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                        continue;
                    }

                    var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;

                    try
                    {
                        await _mediator.Send(typedMessage, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Message failed during processing");
                        continue;
                    }

                    await _sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
                }
                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
