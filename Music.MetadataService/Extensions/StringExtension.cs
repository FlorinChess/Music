using System.Globalization;
using System.Text;

namespace Music.APIs.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Normalizes and removes language-specific diacritics from a string.
        /// </summary>
        /// <param name="str">The string to be normalized.</param>
        /// <returns>The normalized string.</returns>
        public static string NormalizeString(this string str)
        {
            try
            {
                string normalizedString = str.Normalize(NormalizationForm.FormD);

                StringBuilder stringBuilder = new();

                for (int i = 0; i < normalizedString.Length; i++)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]) != UnicodeCategory.NonSpacingMark)
                    {
                        stringBuilder.Append(normalizedString[i]);
                    }
                }

                return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Removes the '/', '\' and multiple space characters from a string.
        /// </summary>
        /// <param name="str">The string to be modified.</param>
        /// <returns>The modified string.</returns>
        public static string RemoveForbiddenCharacters(this string str)
        {
            return str.Replace("/", "").Replace("\\", "").Replace("  ", " ");
        }

        /// <summary>
        /// Returns the year from a date in the string format YYYY-MM-DD.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <returns>An <see cref="uint"/> representing the year component of the date string.</returns>
        public static uint GetYear(this string str)
        {
            uint year = Convert.ToUInt32(str.Split('-')[0]);

            return year;
        }
    }
}
