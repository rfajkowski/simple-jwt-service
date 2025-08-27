using System.Security.Cryptography;
using AuthLibrary.Service;

namespace AuthLibrary.Service
{
    public interface IKeyProvider
    {
        RSA GetRsaKey(string keyId);
    }
}
