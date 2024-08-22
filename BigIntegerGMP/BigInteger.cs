using Math.Gmp.Native;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BigIntegerGMP.Utils;
using static Math.Gmp.Native.gmp_lib;

namespace BigIntegerGMP
{
    /// <summary>
    /// BigInteger class using the GMP library.
    /// (c)2024 by mammon // AMPED
    /// </summary>
    public class BigInteger : IDisposable, ICloneable
    {
        #region Private Fields

        private mpz_t _value;
        private static gmp_randstate_t _randState;

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
            _value = new mpz_t();
            mpz_init_set_si(_value, value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(uint value)
        {
            _value = new mpz_t();
            mpz_init_set_ui(_value, value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(long value) {
            _value = new mpz_t();
            mpz_init_set_d(_value, value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(ulong value) {
            _value = new mpz_t();
            mpz_init_set_d(_value, value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(double value)
        {
            _value = new mpz_t();
            mpz_init_set_d(_value, value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(float value) {
            _value = new mpz_t();
            mpz_init_set_d(_value, value);
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
        public BigInteger(string value, BaseFormat format) {
            _value = new mpz_t();
            if(format == BaseFormat.Base64)
            {
                var byteArray = Convert.FromBase64String(value);
                var ptr = Marshal.AllocHGlobal(byteArray.Length);
                Marshal.Copy(byteArray, 0, ptr, byteArray.Length);
                mpz_import(_value, (size_t)byteArray.Length, 1, 1, 0, 0, new void_ptr(ptr));
                Marshal.FreeHGlobal(ptr);
            }
            else
                mpz_init_set_str(_value, new char_ptr(value), (int)format);
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
            _value = new mpz_t();
            mpz_init(_value);
            var ptr = Marshal.AllocHGlobal(valueLength);
            Marshal.Copy(value, 0, ptr, valueLength);
            mpz_import(_value, (size_t)valueLength, isLittleEndian ? 1 : 0, 1, 0, 0, new void_ptr(ptr));
            Marshal.FreeHGlobal(ptr);
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
            _value = new mpz_t();
            mpz_init_set(_value, value._value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(uint[] value) {
            _value = new mpz_t();
            mpz_init(_value);
            var ptr = Marshal.AllocHGlobal(value.Length * sizeof(uint));
            Helpers.CopyEx(value, 0, ptr, value.Length);
            mpz_import(_value, (size_t)(value.Length * sizeof(uint)), 1, 4, 0, 0, new void_ptr(ptr));
            Marshal.FreeHGlobal(ptr);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public BigInteger(int[] value) {
            _value = new mpz_t();
            mpz_init(_value);
            var ptr = Marshal.AllocHGlobal(value.Length * sizeof(int));
            Marshal.Copy(value, 0, ptr, value.Length);
            mpz_import(_value, (size_t)(value.Length * sizeof(int)), 1, 4, 0, 0, new void_ptr(ptr));
            Marshal.FreeHGlobal(ptr);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BigInteger(byte value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        [CLSCompliant(false)]
        public static implicit operator BigInteger(sbyte value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BigInteger(short value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        [CLSCompliant(false)]
        public static implicit operator BigInteger(ushort value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BigInteger(int value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        [CLSCompliant(false)]
        public static implicit operator BigInteger(uint value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BigInteger(long value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        [CLSCompliant(false)]
        public static implicit operator BigInteger(ulong value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator BigInteger(float value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator BigInteger(double value)
        {
            return new BigInteger(value);
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to an integer.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator int(BigInteger value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var longVal = mpz_get_si(value._value);
            if (longVal > int.MaxValue || longVal < int.MinValue)
                throw new OverflowException("Value was either too large or too small for an Int32.");
            return longVal;
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to an unsigned integer.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator uint(BigInteger value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var ulongVal = mpz_get_ui(value._value);
            if (ulongVal > uint.MaxValue)
                throw new OverflowException("Value was too large for a UInt32.");
            return ulongVal;
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to a long integer.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator long(BigInteger value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return mpz_get_si(value._value);
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to an unsigned long integer.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator ulong(BigInteger value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return mpz_get_ui(value._value);
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to a float.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator float(BigInteger value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return (float)mpz_get_d(value._value);
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to a double.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator double(BigInteger value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return mpz_get_d(value._value);
        }

        public static bool operator ==(BigInteger left, BigInteger right)
        {
            if(left is null || right is null)
                return false;

            return mpz_cmp(left._value, right._value) == 0;
        }

        public static bool operator !=(BigInteger left, BigInteger right)
        {
            return mpz_cmp(left._value, right._value) != 0;
        }

        public static bool operator ==(BigInteger left, int right)
        {
            return mpz_cmp_si(left._value, right) == 0;
        }

        public static bool operator !=(BigInteger left, int right)
        {
            return mpz_cmp_si(left._value, right) != 0;
        }

        public static bool operator <(BigInteger left, BigInteger right)
        {
            return mpz_cmp(left._value, right._value) < 0;
        }

        public static bool operator >(BigInteger left, BigInteger right)
        {
            return mpz_cmp(left._value, right._value) > 0;
        }

        public static bool operator <(BigInteger left, int right)
        {
            return mpz_cmp_si(left._value, right) < 0;
        }

        public static bool operator >(BigInteger left, int right)
        {
            return mpz_cmp_si(left._value, right) > 0;
        }

        public static bool operator <=(BigInteger left, BigInteger right)
        {
            return mpz_cmp(left._value, right._value) <= 0;
        }

        public static bool operator <=(BigInteger left, int right)
        {
            return mpz_cmp_si(left._value, right) <= 0;
        }

        public static bool operator >=(BigInteger left, BigInteger right)
        {
            return mpz_cmp(left._value, right._value) >= 0;
        }

        public static bool operator >=(BigInteger left, int right)
        {
            return mpz_cmp_si(left._value, right) >= 0;
        }

        public static BigInteger operator <<(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_mul_2exp(result._value, left._value, mpz_get_ui(right._value));
            return result;
        }

        public static BigInteger operator <<(BigInteger left, int right)
        {
            var result = new BigInteger();
            mpz_mul_2exp(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger operator >>(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_fdiv_q_2exp(result._value, left._value, mpz_get_ui(right._value));
            return result;
        }

        public static BigInteger operator >>(BigInteger left, int right)
        {
            var result = new BigInteger();
            mpz_fdiv_q_2exp(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger operator +(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_add(result._value, left._value, result._value);
            return result;
        }

        public static BigInteger operator +(BigInteger left, int right)
        {
            var result = new BigInteger();
            mpz_add_ui(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger operator +(BigInteger value)
        {
            var result = new BigInteger();
            mpz_add(result._value, value._value, result._value);
            return result;
        }

        public static BigInteger operator ++(BigInteger value)
        {
            var result = new BigInteger();
            mpz_add_ui(result._value, value._value, 1);
            return result;
        }

        public static BigInteger operator --(BigInteger value)
        {
            var result = new BigInteger();
            mpz_sub_ui(result._value, value._value, 1);
            return result;
        }

        public static BigInteger operator -(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_sub(result._value, left._value, result._value);
            return result;
        }

        public static BigInteger operator -(BigInteger left, int right)
        {
            var result = new BigInteger();
            mpz_sub_ui(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger operator -(BigInteger value)
        {
            var result = new BigInteger();
            mpz_neg(result._value, value._value);
            return result;
        }

        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_mul(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger operator *(BigInteger left, int right)
        {
            var result = new BigInteger();
            mpz_mul_si(result._value, left._value, right);
            return result;
        }

        public static BigInteger operator /(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_fdiv_q(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger operator /(BigInteger left, int right)
        {
            var result = new BigInteger();
            mpz_fdiv_q_ui(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger operator %(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_mod(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger operator %(BigInteger left, int right)
        {
            var result = new BigInteger();
            mpz_mod_ui(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger operator &(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_and(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger operator &(BigInteger left, int right)
        {
            var _right = new BigInteger();
            var result = new BigInteger();

            mpz_set_si(_right._value, right);
            mpz_and(result._value, left._value, _right._value);
            return result;
        }

        public static BigInteger operator ~(BigInteger value)
        {
            var result = new BigInteger();
            mpz_com(result._value, value._value);
            return result;
        }

        public static BigInteger operator |(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_ior(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger operator |(BigInteger left, int right)
        {
            var _right = new BigInteger();
            var result = new BigInteger();

            mpz_set_si(_right._value, right);
            mpz_ior(result._value, left._value, _right._value);
            return result;
        }

        public static BigInteger operator ^(BigInteger left, BigInteger right)
        {
            var result = new BigInteger();
            mpz_xor(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger operator ^(BigInteger left, int right)
        {
            var _right = new BigInteger();
            var result = new BigInteger();

            mpz_set_si(_right._value, right);
            mpz_xor(result._value, left._value, _right._value);
            return result;
        }

        #endregion

        #region Public Static Methods
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
        /// Returns the sum of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Add(BigInteger left, BigInteger right)
        {
            return left + right;
        }
        /// <summary>
        /// Returns the sum of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Add(BigInteger left, int right)
        {
            return left + right;
        }
        /// <summary>
        /// Returns the difference of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Subtract(BigInteger left, BigInteger right)
        {
            return left - right;
        }
        /// <summary>
        /// Returns the difference of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Subtract(BigInteger left, int right)
        {
            return left - right;
        }
        /// <summary>
        /// Returns the product of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Multiply(BigInteger left, BigInteger right)
        {
            return left * right;
        }
        /// <summary>
        /// Returns the product of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Multiply(BigInteger left, int right)
        {
            return left * right;
        }
        /// <summary>
        /// Returns the quotient of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static BigInteger Divide(BigInteger dividend, BigInteger divisor)
        {
            return dividend / divisor;
        }
        /// <summary>
        /// Returns the quotient of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static BigInteger Divide(BigInteger dividend, int divisor)
        {
            return dividend / divisor;
        }
        /// <summary>
        /// Returns the remainder of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static BigInteger Remainder(BigInteger dividend, BigInteger divisor)
        {
            return dividend % divisor;
        }
        /// <summary>
        /// Returns the remainder of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static BigInteger Remainder(BigInteger dividend, int divisor)
        {
            return dividend % divisor;
        }
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
        public static BigInteger Negate(BigInteger value)
        {
            return -value;
        }
        /// <summary>
        /// Returns the bitwise complement of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Not(BigInteger value)
        {
            return ~value;
        }
        /// <summary>
        /// Returns the and of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger And(BigInteger left, BigInteger right)
        {
            return left & right;
        }
        /// <summary>
        /// Returns the and of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger And(BigInteger left, int right)
        {
            return left & right;
        }
        /// <summary>
        /// Returns the or of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Or(BigInteger left, BigInteger right)
        {
            return left | right;
        }
        /// <summary>
        /// Returns the or of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Or(BigInteger left, int right)
        {
            return left | right;
        }
        /// <summary>
        /// Returns the xor of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Xor(BigInteger left, BigInteger right)
        {
            return left ^ right;
        }
        /// <summary>
        /// Returns the xor of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Xor(BigInteger left, int right)
        {
            return left ^ right;
        }
        /// <summary>
        /// Returns the left-shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static BigInteger LeftShift(BigInteger value, int shift)
        {
            return value << shift;
        }
        /// <summary>
        /// Returns the right-shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static BigInteger RightShift(BigInteger value, int shift)
        {
            return value >> shift;
        }
        /// <summary>
        /// Returns the modulus of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger Mod(BigInteger value, BigInteger modulus)
        {
            return value % modulus;
        }
        /// <summary>
        /// Returns the modulus of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static BigInteger Mod(BigInteger value, int modulus)
        {
            return value % modulus;
        }
        /// <summary>
        /// Returns the absolute value of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Log(BigInteger value)
        {
            return Log(value, System.Math.E);
        }
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
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Logarithm is undefined for non-positive values.");
            }

            // Convert the BigInteger (mpz_t) to a double
            double doubleValue = mpz_get_d(value._value);

            // Calculate the logarithm using .NET's Math.Log function
            double logResult = System.Math.Log(doubleValue);

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
        public static double Log10(BigInteger value)
        {
            return Log(value, 10);
        }
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
        public static BigInteger Max(BigInteger left, BigInteger right)
        {
            return left >= right ? left : right;
        }
        /// <summary>
        /// Returns the minimum of the specified <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Min(BigInteger left, BigInteger right)
        {
            return left <= right ? left : right;
        }
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
        public static BigInteger PowMod(BigInteger baseValue, BigInteger exponent, BigInteger modulus)
        {
            return ModPow(baseValue, exponent, modulus);
        }
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
        /// Checks if the specified <see cref="BigInteger"/> object is a probable prime number.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="certainty"></param>
        /// <returns></returns>
        public static bool IsProbablePrime(BigInteger value, int certainty = 10)
        {
            return mpz_probab_prime_p(value._value, certainty) != 0;
        }
        /// <summary>
        /// Checks if the specified string value is a valid <see cref="BigInteger"/> object with the specified base format.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool IsValid(string value, BaseFormat format)
        {
            return mpz_set_str(new mpz_t(), new char_ptr(value), (int)format) == 0;
        }
        /// <summary>
        /// Checks if the specified <see cref="BigInteger"/> object is an even number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(BigInteger value)
        {
            return mpz_tstbit(value._value, 0) == 0;
        }
        /// <summary>
        /// Checks if the specified <see cref="BigInteger"/> object is an odd number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(BigInteger value)
        {
            return !IsEven(value);
        }
        /// <summary>
        /// Checks if the specified <see cref="BigInteger"/> object is a power of two.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(BigInteger value)
        {
            return mpz_popcount(value._value) == 1;
        }
        /// <summary>
        /// Returns the random <see cref="BigInteger"/> object with the specified bit length.
        /// </summary>
        /// <param name="bitLength"></param>
        /// <returns></returns>
        public static BigInteger Random(uint bitLength)
        {
            var result = One;
            var bitLen = (bitLength + 7) / 8;
            var bytes = new byte[bitLen];
            RandomNumberGenerator.Fill(bytes);
            var excessBits = (bitLen * 8) - bitLength;
            if(excessBits > 0)
            {
                var mask = (byte)(0xFF >> (int)excessBits);
                bytes[0] &= mask;
            }

            result = new BigInteger(bytes, bytes.Length, false, true);
            return result;
        }
        /// <summary>
        /// Returns the left-shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static BigInteger ShiftLeft(BigInteger value, int shift)
        {
            return value << shift;
        }
        /// <summary>
        /// Returns the right-shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static BigInteger ShiftRight(BigInteger value, int shift)
        {
            return value >> shift;
        }
        /// <summary>
        /// Returns the square of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Square(BigInteger value)
        {
            return value * value;
        }
        /// <summary>
        /// Returns the rounded value of the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger Round(BigInteger value)
        {
            // Assume we are rounding to the nearest integer
            BigInteger roundingBase = BigInteger.One;

            // Calculate the remainder when dividing by the rounding base
            BigInteger remainder = value % roundingBase;

            // Determine if the remainder is less than half of the rounding base
            BigInteger halfBase = roundingBase / Two;

            // If the remainder is less than halfBase, round down, otherwise round up
            if (remainder < halfBase)
            {
                // Round down by subtracting the remainder
                return value - remainder;
            }
            else
            {
                // Round up by adding the difference to the next multiple of roundingBase
                return value + (roundingBase - remainder);
            }
        }


        #endregion

        #region Public Methods
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object is an even number.
        /// </summary>
        /// <returns></returns>
        public bool IsEven()
        {
            return IsEven(this);
        }
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object is an odd number.
        /// </summary>
        /// <returns></returns>
        public bool IsOdd()
        {
            return IsOdd(this);
        }
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object is a power of two.
        /// </summary>
        /// <returns></returns>
        public bool IsPowerOfTwo()
        {
            return IsPowerOfTwo(this);
        }
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object is a probable prime number.
        /// </summary>
        /// <param name="certainty"></param>
        /// <returns></returns>
        public bool IsProbablePrime(int certainty = 10)
        {
            return IsProbablePrime(this, certainty);
        }
        /// <summary>
        /// Returns the modulus inverse of the <see cref="BigInteger"/> object with the specified modulus.
        /// </summary>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public BigInteger ModInverse(BigInteger modulus)
        {
            return ModInverse(this, modulus);
        }
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
                BigInteger largeConstant = new BigInteger("100000000000000000000000000000000000", BaseFormat.Base16);
                return value / largeConstant;
            }
            BigInteger ReduceBaseValue(BigInteger value)
            {
                // Example reduction logic: divide by a large constant to manage size
                BigInteger largeConstant = new BigInteger("100000000000000000000000000000000000", BaseFormat.Base16);
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

            BigInteger result = One;
            BigInteger baseValue = this;

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
        /// Gets the bit length of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public int BitLength()
        {
            return (int)mpz_sizeinbase(_value, 2);
        }
        /// <summary>
        /// Tests the bit at the specified index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool TestBit(int i)
        {
            return mpz_tstbit(_value, new mp_bitcnt_t((uint)i)) != 0;
        }
        /// <summary>
        /// Sets the bit at the specified index.
        /// </summary>
        /// <param name="i"></param>
        public void SetBit(int i)
        {
            mpz_setbit(_value, new mp_bitcnt_t((uint)i));
        }
        /// <summary>
        /// Clears the bit at the specified index.
        /// </summary>
        /// <param name="i"></param>
        public void ClearBit(int i)
        {
            mpz_clrbit(_value, new mp_bitcnt_t((uint)i));
        }
        /// <summary>
        /// Flips the bit at the specified index.
        /// </summary>
        /// <param name="i"></param>
        public void FlipBit(int i)
        {
            mpz_combit(_value, new mp_bitcnt_t((uint)i));
        }
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
        /// Returns the string representation of the <see cref="BigInteger"/> object in the specified base format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(BaseFormat format)
        {
            return format == BaseFormat.Base64
                ? Convert.ToBase64String(ToByteArray())
                : mpz_get_str(new char_ptr(nint.Zero), -(int)format, _value).ToString();
        }
        /// <summary>
        /// Returns the string representation of the <see cref="BigInteger"/> object in the base 10 format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mpz_get_str(new char_ptr(nint.Zero), 10, _value).ToString();
        }
        /// <summary>
        /// Clones the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new BigInteger(this);
        }
        /// <summary>
        /// Equality comparison for the <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is BigInteger integer && mpz_cmp(_value, integer._value) == 0;
        }
        /// <summary>
        /// Gets the hash code for the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return mpz_get_si(_value);
        }
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

    }
}
