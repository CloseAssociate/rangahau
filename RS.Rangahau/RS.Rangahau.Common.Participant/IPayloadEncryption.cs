using System;
using System.Linq;
public interface IPayloadEncryption
{
    /// <summary>
    /// Decrypts a ParticipantPayload.  Assumes the IV is at the beginning of the byte array, i.e. a salted cipher.
    /// </summary>
    /// <param name="encrypted"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    ParticipantPayload DecryptPayload(string encrypted, byte[] key);
    /// <summary>
    /// Encrypts the specified ParticipantPayload.  Prepends the encrypted output with the IV to generated salted cipher.
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    string EncryptPayload(ParticipantPayload payload, byte[] key);
}
