using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FHH.Logic.Components.Networking
{
    public static class PlayerNameUtility
    {
        // mapping for common European-specific characters to ASCII-ish equivalents
        private static readonly Dictionary<char, string> _specialCharMap = new Dictionary<char, string>
        {
            { 'Æ', "Ae" }, { 'æ', "ae" },
            { 'Ø', "O" }, { 'ø', "o" },
            { 'Å', "A" }, { 'å', "a" },
            { 'Ä', "Ae" }, { 'ä', "ae" },
            { 'Ö', "Oe" }, { 'ö', "oe" },
            { 'Ü', "Ue" }, { 'ü', "ue" },
            { 'ß', "ss" },
            { 'Ł', "L" }, { 'ł', "l" },
            { 'Đ', "D" }, { 'đ', "d" },
            { 'Ñ', "N" }, { 'ñ', "n" },
            { 'Č', "C" }, { 'č', "c" },
            { 'Š', "S" }, { 'š', "s" },
            { 'Ž', "Z" }, { 'ž', "z" }
        };

        public static string ToAuthName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return "Guest" + Random.Range(1000, 9999);
            }
            string trimmed = displayName.Trim();
            string noDiacritics = RemoveDiacritics(trimmed);

            // replace whitespace runs with a single underscore
            string noWhitespace = Regex.Replace(noDiacritics, @"\s+", "_");

            // Only allow ASCII letters, digits, and underscores in the auth name
            string allowed = Regex.Replace(noWhitespace, @"[^A-Za-z0-9_]", string.Empty);

            if (string.IsNullOrWhiteSpace(allowed))
            {
                allowed = "Guest" + Random.Range(1000, 9999);
            }
            if (allowed.Length > 50)
            {
                allowed = allowed.Substring(0, 50);
            }
            return allowed;
        }

        /// <summary>
        /// Reconstruct a readable chat name from the auth-safe name.
        /// </summary>
        /// <param name="authName"></param>
        /// <returns></returns>
        public static string FromAuthNameForChat(string authName)
        {
            if (string.IsNullOrWhiteSpace(authName))
            {
                return "Guest";
            }
            string withSpaces = authName.Replace('_', ' ');
            return withSpaces;
        }

        // extended to handle European special characters via _specialCharMap
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(formD.Length);

            foreach (char c in formD)
            {
                if (_specialCharMap.TryGetValue(c, out string mapped))
                {
                    sb.Append(mapped);
                    continue;
                }
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

                // remove combining marks (accents, etc.), keep base characters
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}