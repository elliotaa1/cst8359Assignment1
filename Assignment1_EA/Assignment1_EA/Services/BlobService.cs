using Azure.Storage.Blobs;

namespace Assignment1_EA.Services
{
    public class BlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName;


        public BlobService(IConfiguration configuration)
        {
            _connectionString =
                configuration["AzureBlobStorage:ConnectionString"];

            _containerName =
                configuration["AzureBlobStorage:ContainerName"];
        }


        public async Task<string> UploadFile(IFormFile file)
        {
            BlobContainerClient container =
                new BlobContainerClient(
                    _connectionString,
                    _containerName);


            await container.CreateIfNotExistsAsync();


            string fileName =
                Guid.NewGuid().ToString()
                + Path.GetExtension(file.FileName);


            BlobClient blob =
                container.GetBlobClient(fileName);


            await blob.UploadAsync(
                file.OpenReadStream(),
                true);


            return blob.Uri.ToString();
        }
    }
}