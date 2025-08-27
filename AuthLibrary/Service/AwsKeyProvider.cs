using System;
using System.Security.Cryptography;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using AuthLibrary.Exceptions;
using AuthLibrary.Service;

namespace AuthLibrary.Service
{
    public class AwsKeyProvider : IKeyProvider
    {
        private readonly AmazonKeyManagementServiceClient _kmsClient;
        public AwsKeyProvider(string awsRegion)
        {
            _kmsClient = new AmazonKeyManagementServiceClient(Amazon.RegionEndpoint.GetBySystemName(awsRegion));
        }
        public RSA GetRsaKey(string keyId)
        {
            try
            {
                var response = _kmsClient.GetPublicKeyAsync(new GetPublicKeyRequest { KeyId = keyId }).Result;
                var rsa = RSA.Create();
                // Try ImportRSAPublicKey first, fallback to FromXmlString if not available
                try
                {
                    rsa.ImportRSAPublicKey(response.PublicKey.ToArray(), out _);
                }
                catch (MissingMethodException)
                {
                    // If ImportRSAPublicKey is not available, try XML (user must convert key to XML format)
                    throw new SecurityKeyException("ImportRSAPublicKey not available. Please ensure your project targets .NET Core 3.0+ or .NET 5+. For older frameworks, convert your key to XML and use LocalKeyProvider.");
                }
                return rsa;
            }
            catch (Exception ex)
            {
                throw new SecurityKeyException($"AWS KMS error: {ex.Message}", ex);
            }
        }
    }
}
