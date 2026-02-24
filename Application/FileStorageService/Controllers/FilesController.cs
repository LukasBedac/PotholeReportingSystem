using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace FileStorageService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<FilesController> _logger;

        public FilesController(IMinioClient minioClient, ILogger<FilesController> logger)
        {
            _minioClient = minioClient;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var bucketName = Environment.GetEnvironmentVariable("MINIO_BUCKET") ?? "";
            
            _logger.LogInformation("Uploading file. Bucket: {BucketName}, File: {FileName}, Size: {Length}", 
                bucketName, file.FileName, file.Length);

            if (bucketName == "")
            {
                _logger.LogError("Bucket name is missing or is invalid");
                return StatusCode(404, $"Bucket name is missing");
            }

            var objectName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            try
            {
                // Ensure bucket exists
                var beArgs = new BucketExistsArgs().WithBucket(bucketName);
                bool found = await _minioClient.BucketExistsAsync(beArgs);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(mbArgs);

                    //Because i dont want to log into minio bucket to get files i need to make it public
                    var policy = $@"{{
                        ""Version"": ""2012-10-17"",
                        ""Statement"": [
                            {{
                                ""Effect"": ""Allow"",
                                ""Principal"": {{ ""AWS"": [""*""] }},
                                ""Action"": [""s3:GetObject""],
                                ""Resource"": [""arn:aws:s3:::{bucketName}/*""]
                            }}
                        ]
                    }}";
                    await _minioClient.SetPolicyAsync(new SetPolicyArgs().WithBucket(bucketName).WithPolicy(policy));
                }

                // Upload file
                using var stream = file.OpenReadStream();
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(file.ContentType);

                await _minioClient.PutObjectAsync(putObjectArgs);

                return Ok(new { 
                    BucketName = bucketName, 
                    StorageKey = objectName,
                    OriginalName = file.FileName,
                    ContentType = file.ContentType,
                    Size = file.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to MinIO");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("view/{bucketName}/{objectName}")]
        public async Task<IActionResult> ViewFile(string bucketName, string objectName)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(objectName))
                return BadRequest("Bucket and object name are required.");

            try
            {
                var statArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);
                var stat = await _minioClient.StatObjectAsync(statArgs);

                var memoryStream = new MemoryStream();
                var getArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream((stream) => stream.CopyTo(memoryStream));

                await _minioClient.GetObjectAsync(getArgs);
                memoryStream.Position = 0;

                return File(memoryStream, stat.ContentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file {ObjectName} from {BucketName}", objectName, bucketName);
                return NotFound();
            }
        }

        [HttpDelete("delete/{bucketName}/{objectName}")]
        public async Task<IActionResult> DeleteFile(string bucketName, string objectName)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(objectName))
                return BadRequest("Bucket name and object name are required.");

            try
            {
                var args = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                await _minioClient.RemoveObjectAsync(args);
                _logger.LogInformation("Deleted file {ObjectName} from bucket {BucketName}", objectName, bucketName);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {ObjectName} from bucket {BucketName}", objectName, bucketName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
