using System.Text;

namespace BigIntegerGMP.Utils
{
    /// <summary>
    /// A helper class for converting numbers between different bases.
    /// </summary>
    public static class BaseConversionHelper
    {
        private const string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const string Base16Chars = "0123456789ABCDEF";
        private const string Base8Chars = "01234567";
        private const string Base2Chars = "01";

        /// <summary>
        /// Converts a BigInteger to a Base-64 encoded string.
        /// </summary>
        public static string ConvertToBase64(BigInteger bigInteger)
        {
            if (bigInteger < 0)
                throw new ArgumentException("Only non-negative numbers can be converted to Base-64.");

            if (bigInteger == 0)
                return "A===";  // Representing 0 as "A===" in Base-64 with padding

            var result = new StringBuilder();
            var base64 = new BigInteger(64);
            var zero = new BigInteger(0);

            while (bigInteger > zero)
            {
                var remainder = bigInteger % base64;
                bigInteger /= base64;

                var index = (int)remainder.ToLong();
                if (index < 0 || index >= Base64Chars.Length)
                    throw new ArgumentOutOfRangeException($"Invalid character index: {index} for Base-64.");

                result.Insert(0, Base64Chars[index]); // Convert remainder to Base-64 character
            }

            // Calculate padding required to make the output length a multiple of 4
            var paddingLength = (4 - (result.Length % 4)) % 4;
            result.Append('=', paddingLength); // Add '=' padding as needed

            return result.ToString();
        }

        /// <summary>
        /// Converts a Base-64 encoded string to a BigInteger.
        /// </summary>
        public static BigInteger ConvertFromBase64(string base64String)
        {
            // Validate the Base64 string
            if (string.IsNullOrEmpty(base64String))
                throw new ArgumentException("Input string cannot be null or empty.");

            // Remove any padding characters ('=')
            base64String = base64String.TrimEnd('=');

            var result = new BigInteger(0);
            var base64 = new BigInteger(64);

            foreach (var c in base64String)
            {
                var index = Base64Chars.IndexOf(c);
                if (index < 0)
                    throw new ArgumentException($"Invalid character '{c}' in Base-64 string.");

                result = result * base64 + new BigInteger(index);
            }

            return result;
        }

        /// <summary>
        /// Converts a BigInteger to a Base-32 encoded string.
        /// </summary>
        public static string ConvertToBase32(BigInteger number) => ConvertToBase(number, 32, Base32Chars);

        /// <summary>
        /// Converts a Base-32 encoded string to a BigInteger.
        /// </summary>
        public static BigInteger ConvertFromBase32(string base32String) => ConvertFromBase(base32String, 32, Base32Chars);

        /// <summary>
        /// Converts a BigInteger to a Base-16 (hexadecimal) string.
        /// </summary>
        public static string ConvertToBase16(BigInteger number) => ConvertToBase(number, 16, Base16Chars);

        /// <summary>
        /// Converts a Base-16 (hexadecimal) string to a BigInteger.
        /// </summary>
        public static BigInteger ConvertFromBase16(string base16String) => ConvertFromBase(base16String, 16, Base16Chars);

        /// <summary>
        /// Converts a BigInteger to a Base-8 (octal) string.
        /// </summary>
        public static string ConvertToBase8(BigInteger number) => ConvertToBase(number, 8, Base8Chars);

        /// <summary>
        /// Converts a Base-8 (octal) string to a BigInteger.
        /// </summary>
        public static BigInteger ConvertFromBase8(string base8String) => ConvertFromBase(base8String, 8, Base8Chars);

        /// <summary>
        /// Converts a BigInteger to a Base-2 (binary) string.
        /// </summary>
        public static string ConvertToBase2(BigInteger number) => ConvertToBase(number, 2, Base2Chars);

        /// <summary>
        /// Converts a Base-2 (binary) string to a BigInteger.
        /// </summary>
        public static BigInteger ConvertFromBase2(string base2String) => ConvertFromBase(base2String, 2, Base2Chars);
        
        /// <summary>
        /// Converts a number-string from one base to another.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oldBase"></param>
        /// <param name="newBase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ConvertBase(string value, int oldBase, int newBase)
        {
            var input = oldBase switch
            {
                2 => new BigInteger(value, BaseFormat.Base2),
                8 => new BigInteger(value, BaseFormat.Base8),
                10 => new BigInteger(value, BaseFormat.Base10),
                16 => new BigInteger(value, BaseFormat.Base16),
                32 => new BigInteger(value, BaseFormat.Base32),
                64 => ConvertFromBase64(value),
                _ => throw new ArgumentException(
                    "Invalid base-format! Supported base-formats are: 2, 8, 10, 16, 32, and 64.")
            };

            return newBase switch
            {
                2 => input.ToString(BaseFormat.Base2),
                8 => input.ToString(BaseFormat.Base8),
                10 => input.ToString(BaseFormat.Base10),
                16 => input.ToString(BaseFormat.Base16),
                32 => input.ToString(BaseFormat.Base32),
                64 => ConvertToBase64(input),
                _ => throw new ArgumentException(
                    "Invalid base-format! Supported base-formats are: 2, 8, 10, 16, 32, and 64.")
            };
        }

        /// <summary>
        /// Converts a BigInteger to a string representation in a given base.
        /// </summary>
        private static string ConvertToBase(BigInteger number, int baseValue, string baseChars)
        {
            if (number < 0)
                throw new ArgumentException("Only non-negative numbers can be converted.");

            if (number == 0)
                return baseChars[0].ToString();  // Representing 0 as the first character

            var result = new StringBuilder();
            var baseBigInt = new BigInteger(baseValue);
            var current = number;

            while (current > 0)
            {
                current = BigInteger.DivRem(current, baseBigInt, out var remainder);
                result.Insert(0, baseChars[(int)remainder.ToLong()]); // Convert remainder to base character
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts a string representation in a given base to a BigInteger.
        /// </summary>
        private static BigInteger ConvertFromBase(string inputString, int baseValue, string baseChars)
        {
            if (string.IsNullOrEmpty(inputString))
                throw new ArgumentException("Input string cannot be null or empty.");

            var result = new BigInteger(0);
            var baseBigInt = new BigInteger(baseValue);

            foreach (var c in inputString)
            {
                var index = baseChars.IndexOf(c);
                if (index < 0)
                    throw new ArgumentException($"Invalid character '{c}' in base-{baseValue} string.");

                result = result * baseBigInt + new BigInteger(index);
            }

            return result;
        }
    }

}
