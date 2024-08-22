using System.Security.Cryptography;

namespace BigIntegerGMP.Utils
{
    /// <summary>
    /// Miller-Rabin primality test functions and utilities.
    /// </summary>
    public static class MillerRabin
    {
        /// <summary>
        /// Check if a prime number passes the Miller-Rabin test.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool MillerRabinPass(BigInteger a, BigInteger s, BigInteger d, BigInteger n)
        {
            var aPow = BigInteger.ModPow(a, d, n);
            if (aPow == 1)
                return true;
            for (var i = BigInteger.Zero; i < s - 1; i++)
            {
                if (aPow == n - 1)
                    return true;
                aPow = BigInteger.ModPow(aPow, 2, n);
            }

            return aPow == n - 1;
        }
        /// <summary>
        /// Generate a secure random BigInteger.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static BigInteger GenerateSecureRandomBigInteger(BigInteger maxValue)
        {
            var bytes = maxValue.ToByteArray();
            BigInteger result;

            using var rng = RandomNumberGenerator.Create();

            do
            {
                rng.GetBytes(bytes);

                // Ensure the generated number is positive
                bytes[^1] &= 0x7F;

                result = new BigInteger(bytes);

                // Reduce the result if it’s larger than maxValue
                if (result >= maxValue)
                {
                    result %= maxValue;
                }

            } while (result == 0); // Ensure result is non-zero

            return result;
        }
        /// <summary>
        /// Check if a number is prime using the Miller-Rabin primality test.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool MillerRabinTest(BigInteger n, int k = 20)
        {
            if (n <= 1 || n == 4) return false;
            if (n <= 3) return true;

            var d = n - 1;
            BigInteger s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            for (var i = 0; i < k; i++)
            {
                var a = GenerateSecureRandomBigInteger(n - 2) + 2;
                if (!MillerRabinPass(a, s, d, n))
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Generate a prime number of a given bit length.
        /// </summary>
        /// <param name="bitLength"></param>
        /// <returns></returns>
        public static BigInteger GeneratePrime(int bitLength)
        {
            BigInteger prime;
            do
            {
                prime = GenerateSecureRandomBigInteger(BigInteger.One << bitLength);
            } while (!MillerRabinTest(prime));

            return prime;
        }
        /// <summary>
        /// Generate a prime number within a given range.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public static BigInteger GeneratePrimeInRange(BigInteger start, BigInteger stop)
        {
            BigInteger prime;
            do
            {
                prime = GenerateSecureRandomBigInteger(stop - start) + start;
                prime |= 1; // Ensure it's odd
            } while (!MillerRabinTest(prime));

            return prime;
        }
    }
}
