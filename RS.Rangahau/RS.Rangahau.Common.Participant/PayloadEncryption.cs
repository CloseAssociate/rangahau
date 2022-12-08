using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
public class PayloadEncryption : IPayloadEncryption
{
    public string EncryptPayload(ParticipantPayload payload, byte[] key)
    {
        var json = JsonSerializer.Serialize(payload, this.GetJsonSerializerOptions());
        var encrypted = this.EncryptStringToBytes(json, key);
        return Convert.ToBase64String(encrypted);
    }

    public ParticipantPayload DecryptPayload(string encrypted, byte[] key)
    {
        var saltedCipher = Convert.FromBase64String(encrypted);
        var json = this.DecryptStringFromBytes(saltedCipher, key);
        return JsonSerializer.Deserialize<ParticipantPayload>(json, this.GetJsonSerializerOptions());
    }

    protected JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    public byte[] EncryptStringToBytes(string plainText, byte[] key)
    {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException(nameof(plainText));
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException(nameof(key));

        using var aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.Mode = CipherMode.CBC;
        aesAlg.Padding = PaddingMode.PKCS7;

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV); // note we're relying on Aes.Create() to set the IV

        using MemoryStream msEncrypt = new();

        msEncrypt.Write(aesAlg.IV); // prepend the encrypted payload with the IV

        using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(plainText); // append the cipher after the IV
        }

        return msEncrypt.ToArray(); // return salted cipher
    }

    public string DecryptStringFromBytes(byte[] saltedCipher, byte[] key)
    {
        if (saltedCipher == null || saltedCipher.Length <= 0)
            throw new ArgumentNullException(nameof(saltedCipher));
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException(nameof(key));

        using var aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.Mode = CipherMode.CBC;
        aesAlg.Padding = PaddingMode.PKCS7;

        // extract the IV and cipher from the start of the salted cipher
        var iv = new byte[aesAlg.IV.Length];
        var cipher = new byte[saltedCipher.Length-aesAlg.IV.Length];
        Buffer.BlockCopy(saltedCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(saltedCipher, iv.Length, cipher, 0, cipher.Length);
        aesAlg.IV = iv;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new(cipher);
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}
