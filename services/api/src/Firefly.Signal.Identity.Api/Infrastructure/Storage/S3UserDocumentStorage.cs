using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace Firefly.Signal.Identity.Infrastructure.Storage;

internal sealed class S3UserDocumentStorage(IAmazonS3 s3Client, IOptions<UserDocumentStorageOptions> options) : IUserDocumentStorage
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly UserDocumentStorageOptions _options = options.Value;

    public async Task<StoredUserDocument> UploadAsync(UserDocumentUploadRequest request, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.OriginalFileName);
        var storageKey = BuildStorageKey(request.UserAccountId, request.DocumentType, extension);

        await using var stream = new MemoryStream(request.Content, writable: false);
        var putRequest = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = storageKey,
            InputStream = stream,
            ContentType = request.ContentType,
            AutoCloseStream = false,
            AutoResetStreamPosition = false,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
        };

        putRequest.Metadata["checksum-sha256"] = request.ChecksumSha256;
        putRequest.Metadata["document-type"] = request.DocumentType;

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);
        return new StoredUserDocument(storageKey);
    }

    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        await _s3Client.DeleteObjectAsync(_options.BucketName, storageKey, cancellationToken);
    }

    private string BuildStorageKey(long userAccountId, string documentType, string extension)
    {
        var prefix = (_options.KeyPrefix ?? string.Empty).Trim().Trim('/');
        var keyRoot = string.IsNullOrWhiteSpace(prefix)
            ? $"users/{userAccountId}/{documentType}"
            : $"{prefix}/users/{userAccountId}/{documentType}";

        return $"{keyRoot}/{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
    }
}

public sealed class UserDocumentStorageOptions
{
    public const string SectionName = "UserDocumentStorage";

    public string BucketName { get; init; } = string.Empty;
    public string Region { get; init; } = "eu-west-2";
    public string? KeyPrefix { get; init; }
    public string? ServiceUrl { get; init; }
    public bool UsePathStyle { get; init; }
    public string? AccessKeyId { get; init; }
    public string? SecretAccessKey { get; init; }
    public string? SessionToken { get; init; }
    public long MaxFileSizeBytes { get; init; } = 10 * 1024 * 1024;
    public string[] AllowedContentTypes { get; init; } = [];
    public string[] AllowedFileExtensions { get; init; } = [];
}

internal static class UserDocumentStorageServiceCollectionExtensions
{
    public static IServiceCollection AddUserDocumentStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.Configure<UserDocumentStorageOptions>(configuration.GetSection(UserDocumentStorageOptions.SectionName));

        if (environment.IsEnvironment("Testing"))
        {
            services.AddSingleton<IUserDocumentStorage, InMemoryUserDocumentStorage>();
            return services;
        }

        services.AddSingleton<IAmazonS3>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<UserDocumentStorageOptions>>().Value;
            var config = new AmazonS3Config
            {
                ForcePathStyle = options.UsePathStyle
            };

            if (!string.IsNullOrWhiteSpace(options.ServiceUrl))
            {
                config.ServiceURL = options.ServiceUrl;
            }

            if (!string.IsNullOrWhiteSpace(options.Region))
            {
                config.RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region);
            }

            if (!string.IsNullOrWhiteSpace(options.AccessKeyId) && !string.IsNullOrWhiteSpace(options.SecretAccessKey))
            {
                AWSCredentials credentials = string.IsNullOrWhiteSpace(options.SessionToken)
                    ? new BasicAWSCredentials(options.AccessKeyId, options.SecretAccessKey)
                    : new SessionAWSCredentials(options.AccessKeyId, options.SecretAccessKey, options.SessionToken);

                return new AmazonS3Client(credentials, config);
            }

            return new AmazonS3Client(config);
        });

        services.AddSingleton<IUserDocumentStorage, S3UserDocumentStorage>();
        return services;
    }
}
