/*
 * Example handoff data structure for sending participant data 
 * between Ministry of Health Data and Digital team and Rangahau
 * Version 0.1a Stephen Grice 28 October 2022
*/

using System.ComponentModel.DataAnnotations;

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


