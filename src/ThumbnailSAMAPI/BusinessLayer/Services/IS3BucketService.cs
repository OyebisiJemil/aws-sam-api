using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThumbnailSAMAPI.BusinessLayer.Services
{
    public interface IS3BucketService
    {
        Task<string> UploadImage(string base64Image, string bucketName, string fileName);
    }
}
