namespace Rihal_Cinema.Helpers
{
    using Minio;

    public class PrivateMinioClient
    {
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _secretKey;

        public PrivateMinioClient()
        {
            _endpoint = "localhost:9000";
            _accessKey = "ROOTUSER";
            _secretKey = "CHANGEME123";
        }

        public MinioClient CreateMinioClient()
        {
            var minioClient = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(_accessKey, _secretKey)
                .Build();
            minioClient.SetAppInfo("test", "1.2.2");

            return (MinioClient)minioClient;
        }
    }

}
