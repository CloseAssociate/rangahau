using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

byte[] Compress(byte[] bytes)
{
    using (var memoryStream = new MemoryStream())
    {
        using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        {
            gzipStream.Write(bytes, 0, bytes.Length);
        }
        return memoryStream.ToArray();
    }
}

byte[] Decompress(byte[] bytes)
{
    using (var memoryStream = new MemoryStream(bytes))
    {

        using (var outputStream = new MemoryStream())
        {
            using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                decompressStream.CopyTo(outputStream);
            }
            return outputStream.ToArray();
        }
    }
}

var handoff = new Handoff()
{
    NHI = "AXF1234",
    SurvCode = Guid.NewGuid(),
    PrimaryName = "Gates",
    SecondaryNames = "William Jefferson",
    KnownAs = "Bill",
    EmailAddress = "bill@microsoftcom",
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

var options = new JsonSerializerOptions 
{ 
    WriteIndented = true,
    Converters =
    {
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    }
};

string jsonString = JsonSerializer.Serialize<Handoff>(handoff, options);
Console.WriteLine(jsonString);

byte[] dataToCompress = Encoding.UTF8.GetBytes(jsonString);
byte[] compressedData = Compress(dataToCompress);
string compressedString = System.Convert.ToBase64String(compressedData); 
Console.WriteLine("Length of compressed string: " + compressedString.Length);
Console.WriteLine($"https://www.rangahau.co.nz/?c={compressedString}");

byte[] decompressedData = System.Convert.FromBase64String(compressedString);
decompressedData = Decompress(decompressedData);
string deCompressedString = Encoding.UTF8.GetString(decompressedData);
Console.WriteLine("Length of decompressed string: " + deCompressedString.Length);
Console.WriteLine("Original Object: " + deCompressedString);


public class Handoff
{
    
    [Required]
    public string NHI { get; set; }
    [Required]
    public Guid SurvCode { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$")]
    public string PrimaryName { get; set; }
    [Required]
    public string SecondaryNames { get; set; }
    [Required]
    public string KnownAs { get; set; }
    [Required]
    public string EmailAddress { get; set; }
    [Required]
    public string MobileNumber { get; set; }
    [Required]
    public Sex BiologicalSex { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public List<AssignedTest> AssignedTests { get; set; }
    [Required]
    public ManifestType ManifestType { get; set; }
}

public enum Sex { Male = 0, Female = 1 }
public enum ManifestType { Production = 0, MarketTesting = 1, TestingOnly = 2 }

public class AssignedTest
{
    public AssignedTestEnum Test { get; set; }
}

public enum AssignedTestEnum { PCR = 0, SEROLOGY = 1 }


