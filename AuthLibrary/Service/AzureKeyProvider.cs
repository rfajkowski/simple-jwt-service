using System;
using System.Security.Cryptography;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using AuthLibrary.Exceptions;
using AuthLibrary.Service;

namespace AuthLibrary.Service
{
    public class AzureKeyProvider : IKeyProvider
    {
        private readonly KeyClient _keyClient;
        public AzureKeyProvider(string keyVaultUrl)
        {
            _keyClient = new KeyClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }
        public RSA GetRsaKey(string keyId)
        {
            try
            {
                var key = _keyClient.GetKey(keyId);
                var rsa = RSA.Create();
                // Azure Key Vault returns public key only. For private key, use Managed HSM or certificates.
                // Example for public key:
                var n = key.Value.Key.N;
                var e = key.Value.Key.E;
                rsa.ImportParameters(new RSAParameters { Modulus = n, Exponent = e });
                return rsa;
            }
            catch (Exception ex)
            {
                throw new SecurityKeyException($"Azure Key Vault error: {ex.Message}", ex);
            }
        }
    }
}
