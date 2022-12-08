using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Cryptography;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace RS.Rangahau.Participant.Tests
{
    [TestClass]
    public class PayloadValidationTests : PayloadTests
    {        
        [TestMethod]
        public void TestPayloadBadNHIValidation()
        {
            var handoff = this.GetTestPayload();
            handoff.NHI = "AAA1234";

            var context = new ValidationContext(handoff, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(handoff, context, results, true);

            results.Should().HaveCount(1);
        }

        [TestMethod]
        public void TestPayloadNHIValidation()
        {
            var handoff = this.GetTestPayload();

            var context = new ValidationContext(handoff, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(handoff, context, results, true);

            results.Should().HaveCount(0);
        }
    }
}