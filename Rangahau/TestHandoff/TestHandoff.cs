using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Cryptography;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace TestHandoff
{
    [TestClass]
    public class TestHandoff
    {
        public Handoff GetHandoff()
        {
            return new Handoff()
            {
                NHI = "EPT6335",
                SurvCode = Guid.NewGuid(),
                PrimaryName = "Gates",
                SecondaryNames = "William Jefferson",
                KnownAs = "Bill",
                EmailAddress = "bill.gates@microsoft.com",
                BiologicalSex = Sex.Male,
                MobileNumber = "+64212223333",
                DateOfBirth = new DateTime(1996, 3, 1),
                AssignedTests = new List<AssignedTest>()
                {
                    new AssignedTest(){ Test = AssignedTestEnum.PCR },
                    new AssignedTest(){ Test = AssignedTestEnum.SEROLOGY },
                },
                ReportToGPs = new List<ReportGP>()
                {
                    new ReportGP(){  CPNNumber = "1234", FacilityCode = "5678"}
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
            handoff2.SecondaryNames.Should().Be(handoff.SecondaryNames);
            encryptedHandoffBase64.Length.Should().BeLessThan(2048);
        }
        
        [TestMethod]
        public void Handoff_nhi_should_not_validate()
        {
            var handoff = GetHandoff();
            handoff.NHI = "AAA1234";

            var context = new ValidationContext(handoff, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(handoff, context, results, true);

            results.Should().HaveCount(1);
        }
        [TestMethod]
        public void Handoff_nhi_should_validate()
        {
            var handoff = GetHandoff();

            var context = new ValidationContext(handoff, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(handoff, context, results, true);

            results.Should().HaveCount(0);
        }
        
        [TestMethod]
        public void Handoff_survcode_should_not_validate()
        {
            var handoff = GetHandoff();
            handoff.SurvCode = Guid.Empty;

            var context = new ValidationContext(handoff, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(handoff, context, results, true);

            results.Should().HaveCount(1);
        }
        [TestMethod]
        public void Handoff_survcode_should_validate()
        {
            var handoff = GetHandoff();
            handoff.SurvCode = Guid.NewGuid();

            var context = new ValidationContext(handoff, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(handoff, context, results, true);

            results.Should().HaveCount(0);
        }
    }
}