using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThumbnailSAMAPI.BusinessLayer.Services;

namespace ThumbnailSAMAPI.BusinessLayer.Implementations
{
    public class S3BucketService:IS3BucketService
    {
        private IAmazonS3 s3Client;
        ILogger Logger { get; set; }

        public S3BucketService(ILogger<S3BucketService> logger, IAmazonS3 s3Client)
        {
            this.s3Client = s3Client;
            this.Logger = logger;
        }

        public async Task<string> UploadImage(string base64Image, string bucketName, string fileName)
        {
            this.Logger.LogInformation("Begin uploading...");
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                string name = string.Empty;
                using (var newMemoryStream = new MemoryStream())
                {
                    this.Logger.LogInformation("I got here how now 2 to upload image");

                    var imageParts = base64Image.Split(',').ToList();
                    var buffer = Convert.FromBase64String(imageParts[1]);
                    await newMemoryStream.WriteAsync(buffer, 0, buffer.Length);

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    string fileExtension = GetImageExtension(imageParts[0]);
                    this.Logger.LogInformation($"File extension {fileExtension}");
                    fileNameWithoutExtension = fileNameWithoutExtension + Guid.NewGuid();

                    name = fileNameWithoutExtension + fileExtension;
                    this.Logger.LogInformation($"File extension {name}");
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = name,
                        BucketName = bucketName,
                        StorageClass = S3StorageClass.Standard,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }
                return "success";
            }
            catch (AmazonS3Exception e)
            {
                this.Logger.LogInformation($"Exception 1{e}");
                return e.ToString();
            }
            catch (Exception e)
            {
                this.Logger.LogInformation($"Exception 2 {e}");
                return e.ToString();
            }
        }

        string GetImageExtension(string image)
        {
            return "."+ image.Split('/').ToList()[1].Split(';').ToList()[0];
        }
    }
}
