using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudiosBot.Infra.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Replace any diacritic char by it non-diacritic form.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceDiacritics(this string input)
        {
            string inputInFormD = input.Normalize(NormalizationForm.FormD);

            var output = new StringBuilder();
            foreach (char c in inputInFormD)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    output.Append(c);

            return output.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Replace spaces by plus signals to be placed at query statement.
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public static string AdjustSeach(this string term) => term.Replace(" ", "+");

        /// <summary>
        /// This method remove invalid chars, capitalize each word and remove multiple spaces in a given input string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string WellFormat(this string input) => input.RemoveInvalidChars().CapitalizeEachWord().RemoveMultipleSpaces().ReplaceDiacritics();

        /// <summary>
        /// Remove multiples spaces that occours on a given string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveMultipleSpaces(this string input)
        {
            var regex = new Regex("[ ]{2,}", RegexOptions.None);
            input = regex.Replace(input, " ").Replace("\n", " ");
            input = input.TrimEnd().TrimStart().Trim();

            return input;
        }

        /// <summary>
        /// Remove invalid chars from a given string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveInvalidChars(this string input)
        {
            var invalidChars = new string[] { ".", "*", "-", "," };
            string returnString;

            returnString = input.Contains(":") ? input.Split(':').Last() : input;

            foreach (var item in invalidChars)
                returnString = returnString.Replace(item, string.Empty);

            return new string(returnString.Where(w => !char.IsDigit(w)).ToArray());
        }

        /// <summary>
        /// Put the first letter of a word in UpperCase.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string Capitalize(this string word)
        {
            return (new CultureInfo("pt-br")).TextInfo.ToTitleCase(word);
        }

        /// <summary>
        /// Capitalizes every word that contains in a given sentence.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static string CapitalizeEachWord(this string sentence)
        {
            var result = string.Empty;

            sentence.ToLowerInvariant().Split(' ').ToList().ForEach(aWord =>
            {
                result += string.IsNullOrEmpty(result) ? aWord.Capitalize() : string.Concat(" ", aWord.Capitalize());
            });


            return result;
        }
    }
}
