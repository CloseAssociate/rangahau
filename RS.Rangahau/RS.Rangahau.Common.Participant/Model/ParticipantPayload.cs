using RS.Rangahau.Common.Participant;
using System.ComponentModel.DataAnnotations;

public class ParticipantPayload
{
    [Required]
    [RegularExpression(@"^[A-HJ-NP-Z]{3}[0-9]{4}$", ErrorMessage = "NZ NHIs are AAAnnnn format")]
    [StringLength(7, ErrorMessage = "NZ NHIs have 7 characters")]
    [CustomValidation(typeof(NHIValidator), "ValidateNHI")]
    public string NHI { get; set; }
    public string SurvCode { get; set; }
    [Required]
    [StringLength(60, ErrorMessage = "Primary name too long")]
    public string PrimaryName { get; set; }
    [Required]
    [StringLength(60, ErrorMessage = "Secondary names too long")]
    public string SecondaryNames { get; set; }
    [Required]
    [StringLength(60, ErrorMessage = "Known as name too long")]
    public string KnownAs { get; set; }
    [Required]
    [DataType(DataType.EmailAddress, ErrorMessage = "Invalid E-mail")]
    [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage ="Invalid e-mail (regex)")]
    public string EmailAddress { get; set; }
    [Required]
    [StringLength(60, ErrorMessage = "Mobile number too long")]
    public string MobileNumber { get; set; }
    [Required]
    public BiologicalSexType BiologicalSex { get; set; }
    public List<string> Ethnicity { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public List<AssignedTestType> AssignedTests { get; set; }
    [Required]
    public List<ReportGP> ReportToGPs { get; set; }
    [Required]
    public ManifestType ManifestType { get; set; }

}


