using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

Console.WriteLine("AWS S3 test");

//await UploadFile();
await DownloadFile();


async Task DownloadFile()
{
    using var s3Client = new AmazonS3Client();

    var request = new GetObjectRequest
    {
        BucketName = "kirill-1",
        Key = "files/movies.csv"
    };

    var response = await s3Client.GetObjectAsync(request);

    Console.WriteLine($"Downloaded file status: {response.HttpStatusCode}\n");

    using var memoryStream = new MemoryStream();
    response.ResponseStream.CopyTo(memoryStream);

    var content = Encoding.Default.GetString(memoryStream.ToArray());

    Console.WriteLine(content);
}

async Task UploadFile()
{
    using var s3Client = new AmazonS3Client();
    await using var file = new FileStream("./Files/movies.csv", FileMode.Open, FileAccess.Read);

    var request = new PutObjectRequest
    {
        BucketName = "kirill-1",
        Key = "files/movies.csv",
        ContentType = "text/csv",
        InputStream = file
    };

    var response = await s3Client.PutObjectAsync(request);

    Console.WriteLine($"Uploaded file status: {response.HttpStatusCode}");
}
