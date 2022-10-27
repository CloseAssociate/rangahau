using conHandoff;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Cryptography;
using FluentAssertions;

namespace TestHandoff
{
    [TestClass]
    public class TestHandoff
    {
        public Handoff GetHandoff()
        {
            return new Handoff()
            {
                NHI = "AXF1234",
                SurvCode = Guid.NewGuid(),
                PrimaryName = "Gates",
                SecondaryNames = "William Jefferson",
                KnownAs = "Bill",
                EmailAddress = "bill@microsoft.com",
                BiologicalSex = Sex.Male,
                MobileNumber = "+64212274307",
                DateOfBirth = new DateTime(1996, 3, 1),
                AssignedTests = new List<AssignedTest>()
                {
                    new AssignedTest(){ Test = AssignedTestEnum.PCR },
                    new AssignedTest(){ Test = AssignedTestEnum.SEROLOGY },
                },
                ManifestType = ManifestType.TestingOnly,
            };
        }


        [TestMethod]
        public void Handoff_works_for_roundtrip()
        {
            var handoff = GetHandoff();
            string roundTrip, encryptedHandoffBase64 = "";

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            string jsonString = JsonSerializer.Serialize<Handoff>(handoff, options);

            using (var myAes = AesCryptoServiceProvider.Create())
            {
                byte[] encryptedHandoff = AesExample.EncryptStringToBytes_Aes(jsonString, myAes.Key, myAes.IV);
                encryptedHandoffBase64 = Convert.ToBase64String(encryptedHandoff);

                byte[] encryptedHandoffReturn = System.Convert.FromBase64String(encryptedHandoffBase64);
                roundTrip = AesExample.DecryptStringFromBytes_Aes(encryptedHandoffReturn, myAes.Key, myAes.IV);

                Console.WriteLine("Original:   {0}", jsonString);
                Console.WriteLine($"encryptedHandoffBase64:   {encryptedHandoffBase64}");
                Console.WriteLine($"encryptedHandoffBase64 (length):   {encryptedHandoffBase64.Length}");
                Console.WriteLine("Round Trip: {0}", roundTrip);
            }

            var handoff2 = JsonSerializer.Deserialize<Handoff>(roundTrip, options);

            handoff2.PrimaryName.Should().Be(handoff.PrimaryName);
            encryptedHandoffBase64.Length.Should().BeLessThan(2048);
        }
    }
}