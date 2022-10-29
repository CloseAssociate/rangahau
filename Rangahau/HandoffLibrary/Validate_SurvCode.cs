using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandoffLibrary
{
    public class Validate_Handoff
    {
        // The first 3 characters of an NHI number must be alphabetic, but not ‘I’ or ‘O’, to avoid confusion with one and zero. The 4th to 6th characters must be numeric. The 7th character is also numeric, and is a check digit based on modulus 11. 
        private static string NHI_Pattern = @"^[A-HJ-NP-Z]{3}[0-9]{4}$";
        // Each alphabet character is assigned a number based on the following table, plus 1
        private static readonly char[] Letter_to_number_map = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public static ValidationResult ValidateGuid(Guid guid, ValidationContext vc)
        {
            if (guid != Guid.Empty)
                return ValidationResult.Success;
            else
                return new ValidationResult("Error, could not parse SurvCode"); 
        }
        
        public static ValidationResult ValidateNHI(string nhi, ValidationContext vc)
        {
            if (Is_NZ_NHI(nhi))
                return ValidationResult.Success;
            else
                return new ValidationResult("NHI is not valid");
        }

        /// <summary>
        /// Checks to see if an input is a valid New Zealand NHI (National Health Index) number, based on the NHI validation routine
        /// </summary>
        /// <param name="NHI"></param>
        /// <returns>Returns true if a string is a valid New Zealand NHI number</returns>
        static Boolean Is_NZ_NHI(string NHI_string)
        {
            Boolean Is_NZ_NHI = false;

            Regex regex = new Regex(NHI_Pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(NHI_string))
            {
                int checkValue = 0;
                // Assign each of the first 6 characters a numeric value, then multiply each character by 7 - i, and sum the results.
                for (int i = 0; i < 6; i++)
                {
                    // Convert each letter to a numeric value using the table above
                    int charNumericValue = GetCharNumberValue(NHI_string[i]);

                    // Multiply numeric value by 7-i and add to the checkvalue
                    checkValue += charNumericValue * (7 - i);
                }

                // Apply modulus 11 to the sum of the above numbers (checkValue)
                int checkSum = checkValue % 11;

                // If checksum is ‘0’ then the NHI number is incorrect.
                if (checkSum != 0)
                {
                    // Subtract checksum from 11 to create check digit.
                    // If the check digit equals ‘10’, convert to ‘0’ (achieved using mod 10)
                    int checkDigit = (11 - checkSum) % 10;

                    if (Char.GetNumericValue(NHI_string[6]) == checkDigit)
                    {
                        // if the last digit of the NHI matches the checkdigit, NHI is valid 
                        Is_NZ_NHI = true;
                    }
                }
            }
            return Is_NZ_NHI;
        }

        private static int GetCharNumberValue(char character)
        {
            int charValue = Array.IndexOf(Letter_to_number_map, character);
            if (charValue > -1)
            {
                // Add a 1 to the table to map the letter correctly 
                charValue += 1;
            }
            if (charValue == -1)
            {
                // The letter wasn't found, so it must be a number.
                // Get the numeric value of the character
                charValue = (int)Char.GetNumericValue(character);
            }
            return charValue;
        }
    }
}
