using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SnsPublisher;

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "kirill@ovk.com",
    FullName = "Kirill Ovk",
    DateOfBirth = new DateTime(1990, 1, 1),
    GitHubUsername = "Kirill-Ovc"
};

var sqsClient = new AmazonSimpleNotificationServiceClient();

var topicArnResponse = await sqsClient.FindTopicAsync("customers");

var publishRequest = new PublishRequest
{
    TopicArn = topicArnResponse.TopicArn,
    Message = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>()
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

var response = await sqsClient.PublishAsync(publishRequest);
