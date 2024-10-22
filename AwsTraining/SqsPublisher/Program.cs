using System.Text.Json;
using Amazon.Runtime.Internal;
using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;

var sqsClient = new AmazonSQSClient();

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "kirill@ovk.com",
    FullName = "Kirill Ovk",
    DateOfBirth = new DateTime(1990, 1, 1),
    GitHubUsername = "Kirill-Ovc"
};

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new AutoConstructedDictionary<string, MessageAttributeValue>()
    {
        {
            "MessageType",
            new MessageAttributeValue
            {
                DataType = "String",
                StringValue = nameof(CustomerCreated)
            }
        }
    }
};

var response = await sqsClient.SendMessageAsync(sendMessageRequest);

Console.WriteLine();
