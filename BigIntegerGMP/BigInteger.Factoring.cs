namespace BigIntegerGMP
{
    public partial class BigInteger : IDisposable, ICloneable, IComparable<BigInteger>
    {
        /// <summary>
        /// Factorizes the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static List<BigInteger> Factor(BigInteger n)
        {
            var factors = new List<BigInteger>();

            // Step 1: Remove small factors using trial division
            foreach (var smallPrime in EratosthenesPrimes())
            {
                while (n % smallPrime == 0)
                {
                    factors.Add(smallPrime);
                    n /= smallPrime;
                }
                if (n == 1) return factors;
            }

            // Step 2: Use Pollard's Rho for larger factors
            if (n > 1) FactorUsingPollardsRho(n, factors);

            return factors;
        }
        /// <summary>
        /// Returns the prime factors of the specified <see cref="BigInteger"/> object using Pollard's Rho algorithm, with a specified seed, and a maximum number of iterations.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="seed"></param>
        /// <param name="maxIterations"></param>
        /// <returns></returns>
        public static BigInteger PollardsRho(BigInteger n, BigInteger seed, int maxIterations = 10000)
        {
            if (n.IsEven())
                return 2;

            var x = seed;
            var y = seed;
            BigInteger d = 1;
            var one = One;

            Func<BigInteger, BigInteger> f = (z) => (z * z + one) % n;

            var iteration = 0;
            while (d == 1 && iteration < maxIterations)
            {
                x = f(x); // f(x) = (x^2 + 1) % n
                y = f(f(y)); // f(f(y)) = ((y^2 + 1)^2 + 1) % n
                d = GreatestCommonDivisor(Abs(x - y), n);
                iteration++;
            }

            // If no factor was found and we hit maxIterations, return 0
            return d == n || d == 1 ? 0 : d;
        }
        /// <summary>
        /// Factorizes the specified <see cref="BigInteger"/> object using Pollard's Rho algorithm and outputs the factors.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="factors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void FactorUsingPollardsRho(BigInteger n, List<BigInteger> factors)
        {
            if (n == 1) return;

            if (IsProbablePrime(n))
            {
                factors.Add(n);
                return;
            }

            var divisor = PollardsRho(n, 2);
            if (divisor == 0) // Pollard's Rho failed
                throw new InvalidOperationException("Failed to factor the number.");

            // Recursively factor the divisor and quotient
            FactorUsingPollardsRho(divisor, factors);
            FactorUsingPollardsRho(n / divisor, factors);
        }
        /// <summary>
        /// Generates a list of small primes using the Sieve of Eratosthenes algorithm.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static IEnumerable<BigInteger> EratosthenesPrimes(int limit = 1000)
        {
            var sieve = new bool[limit + 1];
            for (var i = 2; i <= limit; i++) sieve[i] = true;

            for (var p = 2; p * p <= limit; p++)
                if (sieve[p])
                    for (var i = p * p; i <= limit; i += p)
                        sieve[i] = false;

            var primes = new List<BigInteger>();
            for (var i = 2; i <= limit; i++)
                if (sieve[i])
                    primes.Add(new BigInteger(i));

            return primes;
        }
    }
}
