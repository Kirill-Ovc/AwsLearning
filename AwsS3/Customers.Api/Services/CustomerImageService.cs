using Amazon.S3;
using Amazon.S3.Model;

namespace Customers.Api.Services
{
    public class CustomerImageService : ICustomerImageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "kirill-1";

        public CustomerImageService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<PutObjectResponse> UploadImageAsync(Guid id, IFormFile file)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"images/{id}",
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType,
                Metadata =
                {
                    ["x-amz-meta-originalname"] = file.FileName,
                    ["x-amz=meta-extension"] = Path.GetExtension(file.FileName)
                }
            };

            return await _s3Client.PutObjectAsync(putObjectRequest);
        }

        public async Task<GetObjectResponse> GetImageAsync(Guid id)
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"images/{id}"
            };

            return await _s3Client.GetObjectAsync(getObjectRequest);
        }

        public async Task<DeleteObjectResponse> DeleteImageAsync(Guid id)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = $"images/{id}"
            };

            return await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }
    }
}