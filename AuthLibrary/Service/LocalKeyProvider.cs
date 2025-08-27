using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using AuthLibrary.Exceptions;
using AuthLibrary.Service;

namespace AuthLibrary.Service
{
    public class LocalKeyProvider : IKeyProvider
    {
        private readonly Dictionary<string, string> _keyPaths;
        public LocalKeyProvider(Dictionary<string, string> keyPaths)
        {
            _keyPaths = keyPaths;
        }
        public RSA GetRsaKey(string keyId)
        {
            if (keyId == null)
                throw new SecurityKeyException("Private key path is not set");
            if (string.IsNullOrWhiteSpace(keyId))
                throw new SecurityKeyException("Private key path is not set");
            if (!_keyPaths.ContainsKey(keyId))
                throw new SecurityKeyException($"Key ID '{keyId}' not found");
            var path = _keyPaths[keyId];
            if (string.IsNullOrWhiteSpace(path))
                throw new SecurityKeyException("Private key path is not set");
            if (!File.Exists(path))
                throw new SecurityKeyException("File not found");
            var pemKey = File.ReadAllText(path);
            var rsaKey = RSA.Create();
            if (pemKey.Contains("BEGIN RSA PRIVATE KEY") || pemKey.Contains("BEGIN PRIVATE KEY"))
                throw new SecurityKeyException("PEM format detected. Please use .NET 5+ or integrate BouncyCastle for PEM support.");
            try
            {
                rsaKey.FromXmlString(pemKey);
            }
            catch (System.Exception ex)
            {
                throw new SecurityKeyException("Invalid XML file", ex);
            }
            if (rsaKey.KeySize < 2048)
                throw new SecurityKeyException("RSA key size must be at least 2048 bits");
            return rsaKey;
        }
    }
}
