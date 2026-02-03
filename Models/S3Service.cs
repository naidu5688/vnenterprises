using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace vnenterprises.Models
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _config;
        private readonly string _awsAccessKey;
        private readonly string _awsSecretKey;
        private readonly string _awsBucketName;
        private readonly string _awsRegion;
        private readonly string _employeeImagePath;

        public S3Service(IConfiguration config)
        {
            _config = config;
            _awsAccessKey = _config["AWS:AccessKey"];
            _awsSecretKey = _config["AWS:SecretKey"];
            _awsBucketName = _config["AWS:BucketName"].Trim(); // no trailing slash
            _awsRegion = _config["AWS:Region"];
            _employeeImagePath = _config["AWS:EmployeeImagePath"].Trim('/'); // remove leading/trailing slash

            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_awsRegion),
                ForcePathStyle = true
            };

            _s3Client = new AmazonS3Client(_awsAccessKey, _awsSecretKey, s3Config);
        }

        // Upload IFormFile using EmployeeImagePath folder
        public async Task<FileInsertIntoS3BucketResponseModel> UploadFileToS3Bucket(IFormFile file)
        {
            var response = new FileInsertIntoS3BucketResponseModel
            {
                IsSuccess = false,
                FileCompletePath = string.Empty
            };

            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                string fileExtension = Path.GetExtension(file.FileName);
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                // Build S3 key
                string key = $"{_employeeImagePath}/{uniqueFileName}";

                using var stream = file.OpenReadStream();

                var putRequest = new PutObjectRequest
                {
                    BucketName = _awsBucketName,
                    Key = key,
                    InputStream = stream
                    // Do NOT set CannedACL since bucket disables ACLs
                };

                var putResponse = await _s3Client.PutObjectAsync(putRequest);

                if (putResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    response.IsSuccess = true;
                    response.FileCompletePath = $"https://s3.{_awsRegion}.amazonaws.com/{_awsBucketName}/{key}";
                }
                else
                {
                    response.FileCompletePath = "Upload failed";
                }
            }
            catch (AmazonS3Exception ex)
            {
                response.FileCompletePath = $"S3 Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.FileCompletePath = $"Error: {ex.Message}";
            }

            return response;
        }

        public class FileInsertIntoS3BucketResponseModel
        {
            public bool IsSuccess { get; set; }
            public string FileCompletePath { get; set; }
        }
    }
}
