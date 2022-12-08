using FluentAssertions;
using System.Security.Cryptography;

namespace RS.Rangahau.Participant.Tests
{
    [TestClass]
    public class PayloadTests
    {
        public ParticipantPayload GetTestPayload()
        {
            return new ParticipantPayload()
            {
                NHI = "EPT6335",
                SurvCode = Guid.NewGuid().ToString(),
                PrimaryName = "Gates",
                SecondaryNames = "William Jefferson",
                KnownAs = "Bill",
                EmailAddress = "bill.gates@microsoft.com",
                BiologicalSex = BiologicalSexType.Male,
                MobileNumber = "+64212223333",
                DateOfBirth = new DateTime(1996, 3, 1),
                AssignedTests = new List<AssignedTestType>()
                {
                    AssignedTestType.PCR,
                    AssignedTestType.SEROLOGY,
                },
                ReportToGPs = new List<ReportGP>()
                {
                    new ReportGP { CPNNumber = "1234", HPICode = "5678" }
                },
                ManifestType = ManifestType.TestingOnly,
            };
        }


        [TestMethod]
        public void TestPayloadEncryptionRoundtrip()
        {
            var payloadEncryption = new PayloadEncryption();

            // use Aes.Create() to give us a random key - usually this will be shared between participants (IV is generated inside the PayloadEncryption class)
            using var aesAlg = Aes.Create();
            var key = aesAlg.Key;

            // round-trip the encryption
            var payload = this.GetTestPayload();
            var encrypted = payloadEncryption.EncryptPayload(payload, key);
            var decryptedPayload = payloadEncryption.DecryptPayload(encrypted, key);

            // assert that the fields were successfully decrypted
            decryptedPayload.PrimaryName.Should().Be(payload.PrimaryName);
            decryptedPayload.SecondaryNames.Should().Be(payload.SecondaryNames);
            encrypted.Length.Should().BeLessThan(2048);
        }
    }
}