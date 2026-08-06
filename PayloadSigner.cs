using System.Security.Cryptography;
using System.Text;

namespace WilliamBelle.Monitoring;

/// <summary>
/// HMAC-SHA256 signing for sensor payloads. Kept self-contained (duplicated
/// from WilliamBelle.Portal.Core.PayloadVerifier) so the sensor package ships with no
/// William Belle dependencies into customer applications. A round-trip test in
/// WilliamBelle.Portal.Tests pins the two implementations to each other.
/// </summary>
public static class PayloadSigner
{
    public static string Sign(string payload, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexStringLower(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
    }
}
