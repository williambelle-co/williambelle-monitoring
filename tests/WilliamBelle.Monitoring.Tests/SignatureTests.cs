using WilliamBelle.Monitoring;

namespace WilliamBelle.Monitoring.Tests;

/// <summary>
/// The signature scheme is the security boundary of this package: it is what
/// lets an endpoint trust that a snapshot came from the application it claims
/// to be. These tests pin its behaviour so a refactor cannot quietly change it
/// and break every installation at once.
/// </summary>
public class SignatureTests
{
    [Fact]
    public void Signing_is_deterministic()
    {
        const string payload = """{"AppId":"abc","RuntimeVersion":".NET 10.0.4"}""";

        Assert.Equal(
            PayloadSigner.Sign(payload, "a-signing-key"),
            PayloadSigner.Sign(payload, "a-signing-key"));
    }

    [Fact]
    public void A_known_payload_and_key_produce_a_known_signature()
    {
        // HMAC-SHA256 of "hello" keyed with "key", lowercase hex. Pinned
        // against an independent implementation: if this ever changes, the
        // scheme changed, and every deployed installation stops verifying.
        Assert.Equal(
            "9307b3b915efb5171ff14d8cb55fbcc798c6c0ef1456d66ded1a6aa723a58b7b",
            PayloadSigner.Sign("hello", "key"));
    }

    [Fact]
    public void A_different_payload_produces_a_different_signature()
        => Assert.NotEqual(
            PayloadSigner.Sign("original", "k"),
            PayloadSigner.Sign("tampered", "k"));

    [Fact]
    public void A_different_key_produces_a_different_signature()
        => Assert.NotEqual(
            PayloadSigner.Sign("payload", "one-key"),
            PayloadSigner.Sign("payload", "another-key"));

    [Fact]
    public void Signatures_are_lowercase_hex_of_the_expected_length()
    {
        var signature = PayloadSigner.Sign("payload", "key");

        Assert.Equal(64, signature.Length);                 // SHA-256, hex encoded
        Assert.Equal(signature.ToLowerInvariant(), signature);
        Assert.All(signature, c => Assert.Contains(c, "0123456789abcdef"));
    }
}
