namespace BigIntegerGMP.Utils
{
    public static class MathFunctions
    {
        /// <summary>
        /// A simple implementation of the Chinese Remainder Theorem.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static BigInteger ChineseRemainderTheorem(List<BigInteger> a, List<BigInteger> m)
        {
            if(a.Count != m.Count)
                throw new ArgumentException("The number of elements in a and m must be equal.");

            var M = m.Aggregate(BigInteger.One, (current, modulus) => current * modulus);

            var x = BigInteger.Zero;
            for (var i = 0; i < a.Count; i++)
            {
                var Mi = M / m[i];
                var yi = BigInteger.ModInverse(Mi, m[i]);
                var temp = a[i] * Mi * yi;
                x += temp;
            }

            return x % M;
        }
        /// <summary>
        /// ModInverse using Fermat's Little Theorem.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static BigInteger ModInverseFermat(BigInteger a, BigInteger m)
        {
            return a <= 0 || m <= 1
                ? throw new ArgumentException(
                    "a must be greater than 0, and m must be greater than 1 and be a prime number.")
                : BigInteger.ModPow(a, m - 2, m);
        }
        /// <summary>
        /// Probabilistic primality test using Fermat's Little Theorem.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsProbablePrimeFermat(BigInteger n, int k = 10)
        {
            if (n <= 1)
                return false;
            if (n == 2 || n == 3)
                return true;

            var random = new Random();
            for (var i = 0; i < k; i++)
            {
                var a = new BigInteger(random.Next(2, (int)(n - 2)));
                var result = BigInteger.ModPow(a, n - 1, n);
                if(result != 1)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Probabilistic primality test using Miller-Rabin's algorithm.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsProbablePrimeMillerRabin(BigInteger n, int k = 10)
        {
            if (n < 2)
                return false;
            if (n == 2 || n == 3)
                return true;
            if (n % 2 == 0)
                return false;

            var d = n - 1;
            var r = 0;
            while (d % 2 == 0)
            {
                d /= 2;
                r++;
            }

            var random = new Random();
            for (var i = 0; i < k; i++)
            {
                var a = RandomBigInteger(2, n - 2, random);
                var x = BigInteger.ModPow(a, d, n);

                if (x == 1 || x == n - 1)
                    continue;

                var continueLoop = false;
                for (var j = 0; j < r - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, n);

                    if (x == n - 1)
                    {
                        continueLoop = true;
                        break;
                    }
                }

                if (!continueLoop)
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Runs the Solovay-Strassen primality test.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="iConfidence"></param>
        /// <returns></returns>
        public static bool IsProbablePrimeSolovayStrassen(BigInteger value, int iConfidence = 10)
        {
            for (var i = 0; i < iConfidence; i++)
            {
                var a = BigInteger.Random(1, value - 1);
                if (BigInteger.GreatestCommonDivisor(a, value) > BigInteger.One)
                    return false;
                if (FindJacobiSymbol(a, value) % value != BigInteger.PowMod(a, (value - 1) / 2, value))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Generates a random BigInteger within the specified range and using the specified Random object.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="rand"></param>
        /// <returns></returns>
        public static BigInteger RandomBigInteger(BigInteger min, BigInteger max, Random rand)
        {
            var bytes = max.ToByteArray();
            var result = BigInteger.Zero;
            do
            {
                rand.NextBytes(bytes);
                bytes[^1] &= 0x7F;
                result = new BigInteger(bytes);
            }while(result < min || result > max);
            return result;
        }
        /// <summary>
        /// Finds the Jacobi symbol of a and n.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static BigInteger FindJacobiSymbol(BigInteger a, BigInteger n)
        {
            if (a == 0)
                return n == 1 ? BigInteger.One : BigInteger.Zero;
            if (a == -1)
                return n % 2 == 0 ? BigInteger.One : -BigInteger.One;
            if (a == 1)
                return BigInteger.One;
            if (a == 2)
            {
                if (n % 8 == 1 || n % 8 == 7)
                    return BigInteger.One;
                if (n % 8 == 3 || n % 8 == 5)
                    return -BigInteger.One;
            }
            else if (a >= n)
                return FindJacobiSymbol(a % n, n);
            else if (a % 2 == 0)
                return FindJacobiSymbol(2, n) * FindJacobiSymbol(a / 2, n);
            else
                return a % 4 == 3 && n % 4 == 3 ? -FindJacobiSymbol(n, a) : FindJacobiSymbol(n, a);
            return BigInteger.Zero; // Fallback
        }
    }
}
