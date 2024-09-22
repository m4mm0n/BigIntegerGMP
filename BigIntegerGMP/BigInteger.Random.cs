using System.Security.Cryptography;

namespace BigIntegerGMP
{
    public partial class BigInteger : IDisposable, ICloneable, IComparable<BigInteger>
    {
        /// <summary>
        /// Returns the random <see cref="BigInteger"/> object with the specified bit length.
        /// </summary>
        /// <param name="bitLength"></param>
        /// <returns></returns>
        public static BigInteger Random(int bitLength)
        {
            if (bitLength <= 0)
                throw new ArgumentException("bitLength must be a positive integer.");

            var byteLength = (bitLength + 7) / 8; // Calculate the byte length
            var randomBytes = new byte[byteLength];
            RandomNumberGenerator.Fill(randomBytes); // Fill with cryptographically secure random bytes

            // Mask the excess bits if the bit length is not a multiple of 8
            var excessBits = (byteLength * 8) - bitLength;
            if (excessBits > 0)
            {
                var mask = (byte)(0xFF >> excessBits);
                randomBytes[0] &= mask;
            }

            return new BigInteger(randomBytes, randomBytes.Length, true, true); // Use the provided constructor correctly
        }
        /// <summary>
        /// Returns the random <see cref="BigInteger"/> object within the specified bit length range.
        /// </summary>
        /// <param name="minBitLength"></param>
        /// <param name="maxBitLength"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static BigInteger Random(int minBitLength, int maxBitLength)
        {
            if (minBitLength <= 0 || maxBitLength <= 0)
                throw new ArgumentException("bitLength must be a positive integer.");

            if (minBitLength > maxBitLength)
                throw new ArgumentException("minBitLength must be less than or equal to maxBitLength.");

            var bitLength = _intRand.Next(minBitLength, maxBitLength);
            return Random(bitLength);
        }
        /// <summary>
        /// Returns the random <see cref="BigInteger"/> object within the specified range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static BigInteger Random(BigInteger min, BigInteger max)
        {
            if (min >= max)
                throw new ArgumentException("min must be less than max.");

            var range = max - min;
            var bitLength = range.BitLength(); // No need for nullable; we expect this to work for valid BigInteger

            BigInteger randomValue;
            do
            {
                randomValue = Random(bitLength); // Generate a random value up to the bit length of the range
            } while (randomValue >= range); // Ensure the random value is within the range

            return min + randomValue;
        }
    }
}
