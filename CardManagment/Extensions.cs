using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardManagment
{
    public static class Extensions
    {
        public static string NewCardToSqlQuery(this Card cardDetails)
        {
            return $@"INSERT INTO dbo.[Cards] (OwnerId, Pin, CardSerialNumber, CardId) Values ('{cardDetails.OwnerId}', '{cardDetails.Pin}', '{cardDetails.CardSerialNumber}', '{cardDetails.CardId}');";
        }

        public static string SearchTermToQuery(this String? searchTerm)
        {
            return $@"SELECT OwnerId, Pin, CardSerialNumber, CardId FROM dbo.[Cards] WHERE OwnerId LIKE '{searchTerm}%' OR CardSerialNumber LIKE '{searchTerm}%' OR CardId LIKE '{searchTerm}%';";
        }
        public static string RemoveCardToQuery(this String? cardId)
        {
            return $@"DELETE FROM dbo.[Cards] WHERE CardSerialNumber LIKE '{cardId}';";
        }

        /// <summary>
        /// Validates whether the provided PIN is in the correct format.
        /// </summary>
        /// <param name="pin">The PIN to validate. This can be null.</param>
        /// <returns>
        /// <c>true</c> if the PIN is exactly 4 characters long and consists only of numeric digits (0-9); 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The method checks two conditions:
        /// 1. The PIN must be exactly 4 characters long.
        /// 2. Each character in the PIN must be a numeric digit (0-9).
        /// If the PIN is null, empty, or fails either of these conditions, the method returns <c>false</c>.
        /// </remarks>
        public static bool PinValidation(this string? pin)
        {
            if (pin == null || pin.Length != 4 )
                return false;

            foreach (char c in pin)
            {
                if (c < 48 || c > 57) // '0' to '9' (ASCII table)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the input string is valid based on a specific numeric pattern.
        /// </summary>
        /// <param name="value">The input string to validate. This can be null or empty.</param>
        /// <returns>
        /// <c>true</c> if the input string is not null, not empty, not whitespace, and matches the numeric pattern (contains only digits 0-9); 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The method uses a regular expression to validate that the input string contains only numeric characters (0-9).
        /// If the input string is null, empty, or consists solely of whitespace, the method will return <c>false</c>.
        /// </remarks>
        public static bool IsInputValid(this string? value)
        {
            string expression = "^[0-9]*$";
            if (String.IsNullOrEmpty(value))
                return false;
            return (!String.IsNullOrEmpty(value.Trim()) && Regex.IsMatch(value, expression));
        }

        
    }
}
