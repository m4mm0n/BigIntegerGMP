using static Math.Gmp.Native.gmp_lib;

namespace BigIntegerGMP
{
    public partial class BigInteger : IDisposable, ICloneable, IComparable<BigInteger>
    {
        #region Abs
        /// <summary>
        /// Returns the absolute value of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Abs(BigInteger value)
        {
            var result = new BigInteger();
            mpz_abs(result._value, value._value);
            return result;
        }
        /// <summary>
        /// Returns the absolute value of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public BigInteger Abs() => Abs(this);
        #endregion
        #region Add
        /// <summary>
        /// Returns the sum of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Add(BigInteger left, BigInteger right) => left + right;
        /// <summary>
        /// Returns the sum of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Add(BigInteger left, int right) => left + right;
        /// <summary>
        /// Adds the specified <see cref="BigInteger"/> object to the current <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public BigInteger Add(BigInteger value) => this + value;
        /// <summary>
        /// Adds the specified integer to the current <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public BigInteger Add(int value) => this + value;
        #endregion
        #region Subtract
        /// <summary>
        /// Returns the difference of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Subtract(BigInteger left, BigInteger right) => left - right;
        /// <summary>
        /// Returns the difference of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Subtract(BigInteger left, int right) => left - right;
        /// <summary>
        /// Subtracts the specified <see cref="BigInteger"/> object from the current <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public BigInteger Subtract(BigInteger value) => this - value;
        #endregion
        #region Multiply
        /// <summary>
        /// Returns the product of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Multiply(BigInteger left, BigInteger right) => left * right;
        /// <summary>
        /// Returns the product of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Multiply(BigInteger left, int right) => left * right;
        /// <summary>
        /// Returns the sum of the <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public BigInteger Multiply(int right) => this * right;
        /// <summary>
        /// Returns the sum of the <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public BigInteger Multiply(BigInteger right) => this * right;
        #endregion
        #region Divide
        /// <summary>
        /// Returns the quotient of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static BigInteger Divide(BigInteger dividend, BigInteger divisor) => dividend / divisor;
        /// <summary>
        /// Returns the quotient of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static BigInteger Divide(BigInteger dividend, int divisor) => dividend / divisor;
        /// <summary>
        /// Returns the division of the specified <see cref="BigInteger"/> object and object.
        /// </summary>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public BigInteger Divide(BigInteger divisor) => this / divisor;
        #endregion

        /// <summary>
        /// Returns the remainder of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static BigInteger Remainder(BigInteger dividend, BigInteger divisor) => dividend % divisor;
        /// <summary>
        /// Returns the remainder of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static BigInteger Remainder(BigInteger dividend, int divisor) => dividend % divisor;
        /// <summary>
        /// Returns the quotient of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <param name="remainder"></param>
        /// <returns></returns>
        public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder)
        {
            var result = new BigInteger();
            mpz_fdiv_qr(result._value, result._value, dividend._value, divisor._value);
            remainder = result;
            return result;
        }
        /// <summary>
        /// Returns the quotient of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <param name="remainder"></param>
        /// <returns></returns>
        public static BigInteger DivRem(BigInteger dividend, int divisor, out BigInteger remainder)
        {
            var result = new BigInteger();
            mpz_fdiv_qr_ui(result._value, result._value, dividend._value, (uint)divisor);
            remainder = result;
            return result;
        }
        /// <summary>
        /// Returns the negated value of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Negate(BigInteger value) => -value;
        /// <summary>
        /// Returns the bitwise complement of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Not(BigInteger value) => ~value;
        /// <summary>
        /// Returns the and of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger And(BigInteger left, BigInteger right) => left & right;
        /// <summary>
        /// Returns the and of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger And(BigInteger left, int right) => left & right;
        /// <summary>
        /// Returns the or of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Or(BigInteger left, BigInteger right) => left | right;
        /// <summary>
        /// Returns the or of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Or(BigInteger left, int right) => left | right;
        /// <summary>
        /// Returns the xor of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Xor(BigInteger left, BigInteger right) => left ^ right;
        /// <summary>
        /// Returns the xor of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Xor(BigInteger left, int right) => left ^ right;
        /// <summary>
        /// Returns the left-shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static BigInteger LeftShift(BigInteger value, int shift) => value << shift;
        /// <summary>
        /// Returns the right-shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static BigInteger RightShift(BigInteger value, int shift) => value >> shift;
        /// <summary>
        /// Returns the modulus of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger Mod(BigInteger value, BigInteger modulus) => value % modulus;
        /// <summary>
        /// Returns the modulus of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger Mod(BigInteger value, int modulus) => value % modulus;
        /// <summary>
        /// Returns the absolute value of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Log(BigInteger value) => Log(value, System.Math.E);
        /// <summary>
        /// Returns the logarithm of the specified <see cref="BigInteger"/> object with the specified base.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static double Log(BigInteger value, double baseValue)
        {
            // Ensure the value is positive as logarithm is undefined for non-positive numbers
            if (mpz_cmp_ui(value._value, 0) <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Logarithm is undefined for non-positive values.");

            // Convert the BigInteger (mpz_t) to a double
            var doubleValue = mpz_get_d(value._value);

            // Calculate the logarithm using .NET's Math.Log function
            var logResult = System.Math.Log(doubleValue);

            // Adjust for the base
            if (baseValue != System.Math.E)
            {
                logResult /= System.Math.Log(baseValue);
            }

            return logResult;
        }
        /// <summary>
        /// Returns the natural logarithm of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Log10(BigInteger value) => Log(value, 10);
        /// <summary>
        /// Returns the greatest common divisor of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger GreatestCommonDivisor(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_gcd(result._value, left._value, right._value);
            return result;
        }
        /// <summary>
        /// Returns the greatest common divisor of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger GreatestCommonDivisor(BigInteger left, int right)
        {
            var result = new BigInteger();
            mpz_gcd_ui(result._value, left._value, (uint)right);
            return result;
        }
        /// <summary>
        /// Returns the maximum of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Max(BigInteger left, BigInteger right) => left >= right ? left : right;
        /// <summary>
        /// Returns the minimum of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Min(BigInteger left, BigInteger right) => left <= right ? left : right;
        /// <summary>
        /// Returns the modular exponentiation of the specified <see cref="BigInteger"/> object with the specified exponent and modulus.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exponent"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            var result = new BigInteger();
            mpz_powm(result._value, value._value, exponent._value, modulus._value);
            return result;
        }
        /// <summary>
        /// Returns the modular exponentiation of the specified <see cref="BigInteger"/> object with the specified exponent and modulus.
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="exponent"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger PowMod(BigInteger baseValue, BigInteger exponent, BigInteger modulus) => ModPow(baseValue, exponent, modulus);
        /// <summary>
        /// Returns the least common multiple of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
        {
            var gcd = GreatestCommonDivisor(a, b);
            var absProduct = Abs(a * b);
            return absProduct / gcd;
        }
        /// <summary>
        /// Returns the modular inverse of the specified <see cref="BigInteger"/> object with the specified modulus.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger ModInverse(BigInteger value, BigInteger modulus)
        {
            var result = new BigInteger();
            mpz_invert(result._value, value._value, modulus._value);
            return result;
        }
        /// <summary>
        /// Returns the power of the specified <see cref="BigInteger"/> object with the specified exponent.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        public static BigInteger Pow(BigInteger value, int exponent)
        {
            var result = new BigInteger();
            mpz_pow_ui(result._value, value._value, (uint)exponent);
            return result;
        }
        /// <summary>
        /// Returns the power of the specified <see cref="BigInteger"/> object with the specified exponent.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        public static BigInteger Pow(BigInteger value, BigInteger exponent)
        {
            var result = new BigInteger();
            mpz_pow_ui(result._value, value._value, mpz_get_ui(exponent._value));
            return result;
        }
        /// <summary>
        /// Returns the power of the specified <see cref="BigInteger"/> object with the specified exponent and modulus.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exponent"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger Pow(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            var result = new BigInteger();
            mpz_powm(result._value, value._value, exponent._value, modulus._value);
            return result;
        }
        /// <summary>
        /// Returns the square root of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Sqrt(BigInteger value)
        {
            var result = new BigInteger();
            mpz_sqrt(result._value, value._value);
            return result;
        }
        /// <summary>
        /// Returns the square root of the specified <see cref="BigInteger"/> object and the remainder.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="remainder"></param>
        /// <returns></returns>
        public static BigInteger SqrtRem(BigInteger value, out BigInteger remainder)
        {
            var result = new BigInteger();
            mpz_sqrtrem(result._value, result._value, value._value);
            remainder = result;
            return result;
        }
        /// <summary>
        /// Returns the left-shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static BigInteger ShiftLeft(BigInteger value, int shift) => value << shift;
        /// <summary>
        /// Returns the right-shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static BigInteger ShiftRight(BigInteger value, int shift) => value >> shift;
        /// <summary>
        /// Returns the square of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Square(BigInteger value) => value * value;
        /// <summary>
        /// Returns the rounded value of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Round(BigInteger value)
        {
            // Assume we are rounding to the nearest integer
            var roundingBase = One;

            // Calculate the remainder when dividing by the rounding base
            var remainder = value % roundingBase;

            // Determine if the remainder is less than half of the rounding base
            var halfBase = roundingBase / Two;

            // If the remainder is less than halfBase, round down, otherwise round up
            return remainder < halfBase
                ?
                // Round down by subtracting the remainder
                value - remainder
                :
                // Round up by adding the difference to the next multiple of roundingBase
                value + (roundingBase - remainder);
        }
        /// <summary>
        /// Returns the logarithm of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public double Log() => Log(this);
        /// <summary>
        /// Returns the negated value of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public BigInteger Negate() => Negate(this);
        /// <summary>
        /// Returns the division of the specified <see cref="BigInteger"/> object and object.
        /// </summary>
        /// <param name="divisor"></param>
        /// <param name="remainder"></param>
        /// <returns></returns>
        public BigInteger DivRem(BigInteger divisor, out BigInteger remainder) => DivRem(this, divisor, out remainder);
        /// <summary>
        /// Returns the greatest common divisor of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public BigInteger GreatestCommonDivisor(BigInteger value) => GreatestCommonDivisor(this, value);


        /// <summary>
        /// Returns the left shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public BigInteger ShiftLeft(int shift) => this << shift;

        /// <summary>
        /// Returns the right shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public BigInteger ShiftRight(int shift) => this >> shift;
        /// <summary>
        /// Returns the modulus inverse of the <see cref="BigInteger"/> object with the specified modulus.
        /// </summary>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public BigInteger ModInverse(BigInteger modulus) => ModInverse(this, modulus);
        /// <summary>
        /// Returns the power of the specified <see cref="BigInteger"/> object with the specified exponent.
        /// </summary>
        /// <param name="exponent"></param>
        /// <returns></returns>
        public BigInteger Pow(BigInteger exponent)
        {
            BigInteger ReduceResult(BigInteger value)
            {
                // Example reduction logic: divide by a large constant to manage size
                var largeConstant = new BigInteger("100000000000000000000000000000000000", BaseFormat.Base16);
                return value / largeConstant;
            }
            BigInteger ReduceBaseValue(BigInteger value)
            {
                // Example reduction logic: divide by a large constant to manage size
                var largeConstant = new BigInteger("100000000000000000000000000000000000", BaseFormat.Base16);
                return value / largeConstant;
            }

            // Handle edge cases
            if (exponent == Zero)
                return One; // Any number to the power of 0 is 1
            if (exponent == One)
                return this; // Any number to the power of 1 is itself

            // Check for negative exponent
            if (mpz_cmp_si(exponent._value, 0) < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exponent), "Exponent must be non-negative.");
            }

            var result = One;
            var baseValue = this;

            while (exponent > Zero)
            {
                if ((exponent % Two) == One)
                {
                    result *= baseValue;
                    // Check if result is getting too large and apply any kind of reduction if possible
                    if (result.BitLength() > 1024)
                    {
                        // Apply some reduction logic (e.g., reduce the result by dividing it by a large constant)
                        result = ReduceResult(result);
                    }
                }
                baseValue *= baseValue;
                // Similarly check baseValue if it exceeds reasonable size
                if (baseValue.BitLength() > 1024)
                {
                    // Apply some reduction logic (e.g., reduce the base value by dividing it by a large constant)
                    baseValue = ReduceBaseValue(baseValue);
                }
                exponent /= Two;
            }

            return result;
        }
        /// <summary>
        /// Returns the modulus of the <see cref="BigInteger"/> object with the specified modulus.
        /// </summary>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public BigInteger Mod(BigInteger modulus) => this % modulus;
        /// <summary>
        /// Returns the modulus of the <see cref="BigInteger"/> object with the specified modulus.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger Modulus(BigInteger value, BigInteger modulus) => (value % modulus + modulus) % modulus;
        /// <summary>
        /// Returns the modulus of the <see cref="BigInteger"/> object with the specified modulus.
        /// </summary>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public BigInteger Modulus(BigInteger modulus) => Modulus(this, modulus);
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
        /// Returns the prime factors of the specified <see cref="BigInteger"/> object using Pollard's Rho algorithm and a specified seed.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static BigInteger PollardsRho(BigInteger n, BigInteger seed)
        {
            if (n.IsEven())
                return 2;

            var x = seed;
            var y = seed;
            BigInteger d = 1;
            var one = One;

            Func<BigInteger, BigInteger> f = (z) => (z * z + one) % n;

            while (d == 1)
            {
                x = f(x); // f(x) = (x^2 + 1) % n
                y = f(f(y)); // f(f(y)) = ((y^2 + 1)^2 + 1) % n
                d = GreatestCommonDivisor(Abs(x - y), n);
            }

            return d == n ? 0 : // Failure to find a factor
                d;
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
