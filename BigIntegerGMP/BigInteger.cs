using Math.Gmp.Native;
using System.Runtime.InteropServices;
using BigIntegerGMP.Utils;
using static Math.Gmp.Native.gmp_lib;

namespace BigIntegerGMP
{
    /// <summary>
    /// BigInteger class using the GMP library.
    /// (c)2024 by mammon // AMPED
    /// </summary>
    public partial class BigInteger : IDisposable, ICloneable, IComparable<BigInteger>
    {
        #region Private Fields

        private mpz_t _value;
        private static gmp_randstate_t _randState;
        private static Random _intRand = new();

        #endregion

        #region Public Fields

        /// <summary>
        /// BigInteger constant representing the value 0.
        /// </summary>
        public static readonly BigInteger Zero = new(0);
        /// <summary>
        /// BigInteger constant representing the value 1.
        /// </summary>
        public static readonly BigInteger One = new(1);
        /// <summary>
        /// BigInteger constant representing the value 2.
        /// </summary>
        public static readonly BigInteger Two = new(2);
        /// <summary>
        /// BigInteger constant representing the value 3.
        /// </summary>
        public static readonly BigInteger Three = new(3);
        /// <summary>
        /// BigInteger constant representing the value -1.
        /// </summary>
        public static readonly BigInteger MinusOne = new(-1);

        #endregion

        #region Constructors

        static BigInteger()
        {
            _randState = new gmp_randstate_t();
            gmp_randinit_mt(_randState);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class.
        /// </summary>
        public BigInteger()
        {
            _value = new mpz_t();
            mpz_init(_value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(int value)
        {
            try
            {
                _value = new mpz_t();
                mpz_init_set_si(_value, value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(uint value)
        {
            try
            {
                _value = new mpz_t();
                mpz_init_set_ui(_value, value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(long value) {
            try
            {
                _value = new mpz_t();
                mpz_init_set_d(_value, value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(ulong value) {
            try
            {
                _value = new mpz_t();
                mpz_init_set_d(_value, value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(double value)
        {
            try
            {
                _value = new mpz_t();
                mpz_init_set_d(_value, value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(float value) {
            try
            {
                _value = new mpz_t();
                mpz_init_set_d(_value, value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(decimal value)
        {
            try
            {
                // Handle zero case
                if (value == 0m)
                {
                    _value = new mpz_t();
                    mpz_init_set_ui(_value, 0);
                    return;
                }

                // Handle sign
                var isNegative = value < 0;
                value = System.Math.Abs(value);

                // Extract integer part and fractional part
                var bits = decimal.GetBits(value);
                var low = bits[0] & 0xFFFFFFFFL;
                var mid = bits[1] & 0xFFFFFFFFL;
                var high = bits[2] & 0xFFFFFFFFL;
                var scale = (bits[3] >> 16) & 0x7F;

                // Create a BigInteger from the integer part
                var integerPart = new BigInteger((ulong)(high << 32 | mid << 16 | low));

                // Handle scaling (fractional part)
                if (scale > 0)
                {
                    // Convert the fractional part to an integer
                    var fractionalPart = integerPart;
                    var scaleMultiplier = Pow(10, scale);
                    integerPart /= scaleMultiplier;
                }

                // Set the final value
                _value = integerPart._value;

                if (isNegative) mpz_neg(_value, _value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value from base 16.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(string value) : this(value, BaseFormat.Base16)
        { }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value and base format.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public BigInteger(string value, BaseFormat format)
        {
            var form = 0;
            try
            {
                _value = new mpz_t();
                if (format == BaseFormat.Base64)
                {
                    var btmp = ConvertFromBase64(value);
                    if(btmp != null)
                        mpz_set(_value, btmp._value);
                    else
                        throw new FormatException("The value is not in a valid format.");
                }

                switch (format)
                {
                    case BaseFormat.Base2:
                        form = 2;
                        break;
                    case BaseFormat.Base8:
                        form = 8;
                        break;
                    case BaseFormat.Base10:
                        form = 10;
                        break;
                    case BaseFormat.Base16:
                        form = 16;
                        break;
                    case BaseFormat.Base32:
                        form = 32;
                        break;
                }
                if(mpz_init_set_str(_value, new char_ptr(value), form) != 0)
                    throw new FormatException("The value is not in a valid format.");
            }
            catch (Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value and base format.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <exception cref="FormatException"></exception>
        public BigInteger(string value, int format)
        {
            try
            {
                var form = 0;
                _value = new mpz_t();

                switch (format)
                {
                    case 2:
                        form = 2;
                        break;
                    case 8:
                        form = 8;
                        break;
                    case 10:
                        form = 10;
                        break;
                    case 16:
                        form = 16;
                        break;
                    case 32:
                        form = 32;
                        break;
                    case 64:
                        form = 64;
                        break;
                    default:
                        throw new FormatException("The value is not in a valid format.");
                }

                if (form == 64)
                {
                    var btmp = ConvertFromBase64(value);
                    if(btmp != null)
                        mpz_set(_value, btmp._value);
                    else
                        throw new FormatException("[64] The value is not in a valid format.");
                }
                else
                {
                    if (mpz_init_set_str(_value, new char_ptr(value), form) != 0)
                        throw new FormatException("The value is not in a valid format.");
                }
            }
            catch (Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(byte[] value) : this(value, value.Length, false, false)
        { }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value, length, and endianness.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueLength"></param>
        /// <param name="isNegative"></param>
        /// <param name="isLittleEndian"></param>
        public BigInteger(byte[] value, int valueLength, bool isNegative, bool isLittleEndian) {
            try
            {
                _value = new mpz_t();
                mpz_init(_value);
                var ptr = Marshal.AllocHGlobal(valueLength);
                Marshal.Copy(value, 0, ptr, valueLength);
                mpz_import(_value, (size_t)valueLength, isLittleEndian ? 1 : 0, 1, 0, 0, new void_ptr(ptr));
                Marshal.FreeHGlobal(ptr);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value, length, and endianness.
        /// </summary>
        /// <param name="isNegative"></param>
        /// <param name="value"></param>
        /// <param name="valueLength"></param>
        /// <param name="isLittleEndian"></param>
        public BigInteger(bool isNegative, byte[] value, int valueLength, bool isLittleEndian) : this(value, valueLength, isNegative, isLittleEndian)
        { }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value - copy constructor.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(BigInteger value) {
            try
            {
                _value = new mpz_t();
                mpz_init_set(_value, value._value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(uint[] value) {
            try
            {
                _value = new mpz_t();
                mpz_init(_value);
                var ptr = Marshal.AllocHGlobal(value.Length * sizeof(uint));
                Helpers.CopyEx(value, 0, ptr, value.Length);
                mpz_import(_value, (size_t)(value.Length * sizeof(uint)), 1, 4, 0, 0, new void_ptr(ptr));
                Marshal.FreeHGlobal(ptr);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(int[] value) {
            try
            {
                _value = new mpz_t();
                mpz_init(_value);
                var ptr = Marshal.AllocHGlobal(value.Length * sizeof(int));
                Marshal.Copy(value, 0, ptr, value.Length);
                mpz_import(_value, (size_t)(value.Length * sizeof(int)), 1, 4, 0, 0, new void_ptr(ptr));
                Marshal.FreeHGlobal(ptr);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }

#endregion

        #region Public Static Methods
        
        /// <summary>
        /// Checks if the specified <see cref="BigInteger"/> object is a probable prime number.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="certainty"></param>
        /// <returns></returns>
        public static bool IsProbablePrime(BigInteger value, int certainty = 10) =>
            mpz_probab_prime_p(value._value, certainty) != 0;
        /// <summary>
        /// Checks if the specified string value is a valid <see cref="BigInteger"/> object with the specified base format.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool IsValid(string value, BaseFormat format) =>
            mpz_set_str(new mpz_t(), new char_ptr(value), (int)format) == 0;
        /// <summary>
        /// Checks if the specified <see cref="BigInteger"/> object is an even number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(BigInteger value) => mpz_tstbit(value._value, 0) == 0;
        /// <summary>
        /// Checks if the specified <see cref="BigInteger"/> object is an odd number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(BigInteger value) => !IsEven(value);
        /// <summary>
        /// Checks if the specified <see cref="BigInteger"/> object is a power of two.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(BigInteger value) => mpz_popcount(value._value) == 1;

        /// <summary>
        /// Compares two <see cref="BigInteger"/> objects and returns an integer that indicates their relationship.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int Compare(BigInteger left, BigInteger right) => left.CompareTo(right);
        /// <summary>
        /// Parses the specified string value to a <see cref="BigInteger"/> object. The string must be in base 16 format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Parse(string value) => new(value);
        /// <summary>
        /// Parses the specified string value to a <see cref="BigInteger"/> object with the specified base format.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static BigInteger Parse(string value, BaseFormat format) => new(value, format);
        /// <summary>
        /// Tries to parse the specified string value to a <see cref="BigInteger"/> object. The string must be in base 16 format.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out BigInteger result)
        {
            if(IsValid(value, BaseFormat.Base16))
            {
                result = new BigInteger(value);
                return true;
            }
            result = Zero;
            return false;
        }
        /// <summary>
        /// Returns the number of trailing zeros in the binary representation of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int TrailingZeroCount(BigInteger value) => value.TrailingZeroCount();

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Gets the trailing zero count of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public int TrailingZeroCount() => (int)mpz_scan1(_value, 0);
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object's sign is one.
        /// </summary>
        public bool IsOne => Sign() == 1;
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object's sign is zero.
        /// </summary>
        public bool IsZero => Sign() == 0;
        /// <summary>
        /// Gets the sign of the BigInteger.
        /// </summary>
        /// <returns>
        /// 1 if the BigInteger is positive, 
        /// -1 if the BigInteger is negative, 
        /// 0 if the BigInteger is zero.
        /// </returns>
        public int Sign() => mpz_sgn(_value) > 0 ? 1 : mpz_sgn(_value) < 0 ? -1 : 0;
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object is an even number.
        /// </summary>
        /// <returns></returns>
        public bool IsEven() => IsEven(this);
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object is an odd number.
        /// </summary>
        /// <returns></returns>
        public bool IsOdd() => IsOdd(this);
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object is a power of two.
        /// </summary>
        /// <returns></returns>
        public bool IsPowerOfTwo() => IsPowerOfTwo(this);
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object is a probable prime number.
        /// </summary>
        /// <param name="certainty"></param>
        /// <returns></returns>
        public bool IsProbablePrime(int certainty = 10) => IsProbablePrime(this, certainty);
        /// <summary>
        /// Gets the bit length of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public int BitLength() => (int)mpz_sizeinbase(_value, 2);
        /// <summary>
        /// Tests the bit at the specified index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool TestBit(int i) => mpz_tstbit(_value, new mp_bitcnt_t((uint)i)) != 0;
        /// <summary>
        /// Sets the bit at the specified index.
        /// </summary>
        /// <param name="i"></param>
        public void SetBit(int i) => mpz_setbit(_value, new mp_bitcnt_t((uint)i));
        /// <summary>
        /// Clears the bit at the specified index.
        /// </summary>
        /// <param name="i"></param>
        public void ClearBit(int i) => mpz_clrbit(_value, new mp_bitcnt_t((uint)i));
        /// <summary>
        /// Flips the bit at the specified index.
        /// </summary>
        /// <param name="i"></param>
        public void FlipBit(int i) => mpz_combit(_value, new mp_bitcnt_t((uint)i));
        /// <summary>
        /// Gets the lowest set bit of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public int GetLowestSetBit() => (int)mpz_scan1(_value, 0).Value;
        /// <summary>
        /// Returns the byte array representation of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public byte[] ToByteArray(bool isLittleEndian = true)
        {
            var byteLength = mpz_sizeinbase(_value, 2);
            var byteArray = new byte[byteLength];
            var refSize = new size_t(0);
            var ptr = Marshal.AllocHGlobal((int)byteLength.Value);
            mpz_export(new void_ptr(ptr), ref refSize, isLittleEndian ? 1 : 0, new size_t(1), 0, new size_t(0), _value);
            Marshal.Copy(ptr, byteArray, 0, (int)byteLength.Value);
            Marshal.FreeHGlobal(ptr);
            return byteArray;
        }
        /// <summary>
        /// Returns the string representation of the <see cref="BigInteger"/> object in the base 16 format.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => mpz_get_str(new char_ptr(nint.Zero), -16, _value).ToString();
        /// <summary>
        /// Returns the string representation of the <see cref="BigInteger"/> object in the specified base format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(BaseFormat format)
        {
            switch (format)
            {
                case BaseFormat.Base2:
                    return mpz_get_str(new char_ptr(nint.Zero), 2, _value).ToString();
                case BaseFormat.Base8:
                    return mpz_get_str(new char_ptr(nint.Zero), 8, _value).ToString();
                case BaseFormat.Base10:
                    return mpz_get_str(new char_ptr(nint.Zero), 10, _value).ToString();
                case BaseFormat.Base32:
                    return mpz_get_str(new char_ptr(nint.Zero), -32, _value).ToString();
                case BaseFormat.Base64:
                    return BaseConversionHelper.ConvertToBase64(this);
                case BaseFormat.Base16:
                default:
                    return mpz_get_str(new char_ptr(nint.Zero), -16, _value).ToString();
            }
        }
        /// <summary>
        /// Returns the string representation of the <see cref="BigInteger"/> object in the specified base format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(int format)
        {
            switch (format)
            {
                case 2:
                    return mpz_get_str(new char_ptr(nint.Zero), 2, _value).ToString();
                case 8:
                    return mpz_get_str(new char_ptr(nint.Zero), 8, _value).ToString();
                case 10:
                    return mpz_get_str(new char_ptr(nint.Zero), 10, _value).ToString();
                case 32:
                    return mpz_get_str(new char_ptr(nint.Zero), -32, _value).ToString();
                case 64:
                    return BaseConversionHelper.ConvertToBase64(this);
                case 16:
                default:
                    return mpz_get_str(new char_ptr(nint.Zero), -16, _value).ToString();
            }
        }
        /// <summary>
        /// Returns the string representation of the <see cref="BigInteger"/> object in the base 16 format.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(IFormatProvider? provider) => ToString();
        /// <summary>
        /// Returns the string representation of the <see cref="BigInteger"/> object in the specified base format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            var isHex = format.StartsWith("X") || format.StartsWith("x");
            var isDecimal = format.StartsWith("D") || format.StartsWith("d");
            var isBinary = format.StartsWith("B") || format.StartsWith("b");
            var isNumber = format.StartsWith("N") || format.StartsWith("n");

            var isUpper = format.StartsWith("X") || format.StartsWith("D") || format.StartsWith("B") || format.StartsWith("N");
            var len = format.Substring(1).Length > 0 ? int.Parse(format.Substring(1)) : 0;
            if (isBinary)
                return len switch
                {
                    2 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 2 : 2, _value).ToString(),
                    8 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 8 : 8, _value).ToString(),
                    10 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : 10, _value).ToString(),
                    16 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? -16 : 16, _value).ToString(),
                    32 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? -32 : 32, _value).ToString(),
                    64 => BaseConversionHelper.ConvertToBase64(this),
                    _ => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 2 : 2, _value).ToString().Substring(0, len)
                };
            if (isHex)
                return len > 0
                    ? mpz_get_str(new char_ptr(nint.Zero), isUpper ? -16 : 16, _value).ToString()
                        .Substring(0, len)
                    : mpz_get_str(new char_ptr(nint.Zero), isUpper ? -16 : 16, _value).ToString();
            if(isDecimal)
                return len > 0
                    ? mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : 10, _value).ToString()
                        .Substring(0, len)
                    : mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : 10, _value).ToString();
            if(isNumber)
                return len > 0
                    ? mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : 10, _value).ToString()
                        .Substring(0, len)
                    : mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : 10, _value).ToString();

            return ToString();
        }
        /// <summary>
        /// Returns the string representation of the <see cref="BigInteger"/> object in the specified base format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider? provider) => ToString(format);
        /// <summary>
        /// Gets the integer value of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public int ToInt32() => mpz_get_si(_value);
        /// <summary>
        /// Gets the long value of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OverflowException"></exception>
        public long ToLong() =>
            // Check if the value fits within a signed long range
            mpz_fits_slong_p(_value) == 0
                ? throw // Returns 0 if the value doesn't fit
                    new OverflowException("The BigInteger value is too large or too small to fit into a long.")
                : mpz_get_si(_value);
        /// <summary>
        /// Gets the decimal value of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public decimal ToDecimal()
        {
            // Check if the BigInteger is zero
            if (this == Zero)
                return 0m;

            // Define the maximum decimal value as a BigInteger
            var maxValue = new BigInteger(decimal.MaxValue);
            var minValue = new BigInteger(decimal.MinValue);

            // If the value is within the range of a decimal, directly convert it
            if (this >= minValue && this <= maxValue)
                return (decimal)(double)this; // Convert BigInteger to double first, then to decimal

            // Split the BigInteger into smaller parts
            // Use the largest power of 10 that fits within a decimal
            var tenPower28 = Pow(10, 28);
            var current = this;
            var result = 0m;
            var scale = 0;

            while (current != Zero)
            {
                // Get the last 28 digits
                current = DivRem(current, tenPower28, out var remainder);

                // Convert the remainder to decimal and adjust the scale
                result += (decimal)(double)remainder / (decimal)System.Math.Pow(10, scale);
                scale += 28;
            }

            return result;
        }
        /// <summary>
        /// Gets the double value of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public double ToDouble() => mpz_get_d(_value);
        /// <summary>
        /// Clones the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public object Clone() => new BigInteger(this);

        /// <summary>
        /// Compares the <see cref="BigInteger"/> object to the specified object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object? other) =>
            other == null
                ? 1
                : other is BigInteger integer
                    ? mpz_cmp(_value, integer._value)
                    : throw new ArgumentException("Object is not a BigInteger.");
        /// <summary>
        /// Compares the <see cref="BigInteger"/> object to the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BigInteger? other) => other is null ? 1 : mpz_cmp(_value, other._value);
        /// <summary>
        /// Equality comparison for the <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => obj is BigInteger integer && mpz_cmp(_value, integer._value) == 0;
        /// <summary>
        /// Gets the hash code for the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => mpz_get_si(_value);
        /// <summary>
        /// Clears the <see cref="BigInteger"/> object and resets it to zero.
        /// </summary>
        public void Clear()
        {
            mpz_clear(_value);
            mpz_init(_value);
        }
        /// <summary>
        /// Disposes of the <see cref="BigInteger"/> object.
        /// </summary>
        public void Dispose()
        {
            mpz_clear(_value);
            _value = null;
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Equal to disposal of the <see cref="BigInteger"/> object.
        /// </summary>
        ~BigInteger() => Dispose();

        #endregion

        #region Private Methods

        private BigInteger? ConvertFromBase64(string base64String)
        {
            var Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

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

        #endregion
    }
}
