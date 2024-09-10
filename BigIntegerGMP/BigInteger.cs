using System.Diagnostics;
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
    public class BigInteger : IDisposable, ICloneable, IComparable<BigInteger>
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
                    var scaleMultiplier = BigInteger.Pow(10, scale);
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
                    value = value.FromBase64();
                    format = BaseFormat.Base16;
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
                        form = -16;
                        break;
                    case BaseFormat.Base32:
                        form = -32;
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
                        form = 16;
                        value = value.FromBase64();
                        break;
                    default:
                        throw new FormatException("The value is not in a valid format.");
                }
                if (mpz_init_set_str(_value, new char_ptr(value), form) != 0)
                    throw new FormatException("The value is not in a valid format.");
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

        #region Operators

        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BigInteger(byte value)
        {
            try
            {
                return new(value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        [CLSCompliant(false)]
        public static implicit operator BigInteger(sbyte value)
        {
            try
            {
                return new(value);
            }catch(Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BigInteger"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BigInteger(short value)
        {
            try
            {
                return new(value);
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
        [CLSCompliant(false)]
        public static implicit operator BigInteger(ushort value)
        {
            try
            {
                return new(value);
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
        public static implicit operator BigInteger(int value)
        {
            try
            {
                return new(value);
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
        [CLSCompliant(false)]
        public static implicit operator BigInteger(uint value)
        {
            try
            {
                return new(value);
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
        public static implicit operator BigInteger(long value)
        {
            try
            {
                return new(value);
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
        [CLSCompliant(false)]
        public static implicit operator BigInteger(ulong value)
        {
            try
            {
                return new(value);
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
        public static explicit operator BigInteger(float value)
        {
            try
            {
                return new(value);
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
        public static explicit operator BigInteger(decimal value)
        {
            try
            {
                return new(value);
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
        public static explicit operator BigInteger(double value)
        {
            try
            {
                return new(value);
            }
            catch (Exception ex)
            {
                throw new FormatException("The value is not in a valid format.", ex);
            }
        }

        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to an integer.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator int(BigInteger? value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            try
            {
                var longVal = mpz_get_si(value._value);
                return longVal;
            }catch(Exception ex)
            {
                throw new OverflowException("Value was either too large or too small for an Int32.", ex);
            }
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to an unsigned integer.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator uint(BigInteger? value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            try
            {
                var ulongVal = mpz_get_ui(value._value);
                return ulongVal;
            }catch(Exception ex)
            {
                throw new OverflowException("Value was either too large or too small for an UInt32.", ex);
            }
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to a long integer.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator long(BigInteger? value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            try
            {
                return mpz_get_si(value._value);
            }catch(Exception ex)
            {
                throw new OverflowException("Value was either too large or too small for an Int64.", ex);
            }
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to an unsigned long integer.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator ulong(BigInteger? value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            try
            {
                return mpz_get_ui(value._value);
            }catch(Exception ex)
            {
                throw new OverflowException("Value was either too large or too small for an UInt64.", ex);
            }
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to a float.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator float(BigInteger? value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            try
            {
                return (float)mpz_get_d(value._value);
            }catch(Exception ex)
            {
                throw new OverflowException("Value was either too large or too small for a Single.", ex);
            }
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to a double.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator double(BigInteger? value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            try
            {
                return mpz_get_d(value._value);
            }catch(Exception ex)
            {
                throw new OverflowException("Value was either too large or too small for a Double.", ex);
            }
        }
        /// <summary>
        /// Converts the specified <see cref="BigInteger"/> object to a decimal.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator decimal(BigInteger? value)
        {
            // Ensure the input is not null
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            try
            {
                return value.ToDecimal();
            }catch(Exception ex)
            {
                throw new OverflowException("Value was either too large or too small for a Decimal.", ex);
            }
        }
        /// <summary>
        /// Compares two <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BigInteger? left, BigInteger? right) =>
            left is not null && right is not null && mpz_cmp(left._value, right._value) == 0;
        /// <summary>
        /// Compares a <see cref="BigInteger"/> object with an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BigInteger? left, int right) =>
            left is not null && mpz_cmp_si(left._value, right) == 0;
        /// <summary>
        /// Compares an integer with a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(int left, BigInteger? right) =>
            right is not null && mpz_cmp_si(right._value, left) == 0;
        /// <summary>
        /// Checks if two <see cref="BigInteger"/> objects are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BigInteger? left, BigInteger? right) =>
            left is not null && right is not null && mpz_cmp(left._value, right._value) != 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is not equal to an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BigInteger? left, int right) =>
            left is not null && mpz_cmp_si(left._value, right) != 0;
        /// <summary>
        /// Checks if an integer is not equal to a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(int left, BigInteger? right) =>
            right is not null && mpz_cmp_si(right._value, left) != 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is less than another <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(BigInteger? left, BigInteger? right) =>
            left is not null && right is not null && mpz_cmp(left._value, right._value) < 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is less than an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(BigInteger? left, int right) => left is not null && mpz_cmp_si(left._value, right) < 0;
        /// <summary>
        /// Checks if an integer is less than a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(int left, BigInteger? right) =>
            right is not null && mpz_cmp_si(right._value, left) > 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is greater than another <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(BigInteger? left, BigInteger? right) =>
            left is not null && right is not null && mpz_cmp(left._value, right._value) > 0;
        /// <summary>
        /// Checks if an integer is greater than a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(int left, BigInteger? right) =>
            right is not null && mpz_cmp_si(right._value, left) < 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is greater than an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(BigInteger? left, int right) =>
            left is not null && mpz_cmp_si(left._value, right) > 0;

        public static bool operator <=(BigInteger? left, BigInteger? right) =>
            left is not null && right is not null && mpz_cmp(left._value, right._value) <= 0;

        public static bool operator <=(BigInteger? left, int right) =>
            left is not null && mpz_cmp_si(left._value, right) <= 0;

        public static bool operator <=(int left, BigInteger? right) =>
            right is not null && mpz_cmp_si(right._value, left) >= 0;

        public static bool operator >=(BigInteger? left, BigInteger? right) =>
            left is not null && right is not null && mpz_cmp(left._value, right._value) >= 0;

        public static bool operator >=(BigInteger? left, int right) =>
            left is not null && mpz_cmp_si(left._value, right) >= 0;

        public static bool operator >=(int left, BigInteger? right) =>
            right is not null && mpz_cmp_si(right._value, left) <= 0;

        public static BigInteger? operator <<(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_mul_2exp(result._value, left._value, mpz_get_ui(right._value));
            return result;
        }

        public static BigInteger? operator <<(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_mul_2exp(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger? operator >>(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_fdiv_q_2exp(result._value, left._value, mpz_get_ui(right._value));
            return result;
        }



        public static BigInteger? operator >>(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_fdiv_q_2exp(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger? operator +(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_add(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger? operator +(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_add_ui(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger? operator +(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_add(result._value, value._value, result._value);
            return result;
        }

        public static BigInteger? operator ++(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_add_ui(result._value, value._value, 1);
            return result;
        }

        public static BigInteger? operator --(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_sub_ui(result._value, value._value, 1);
            return result;
        }

        public static BigInteger? operator -(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_sub(result._value, left._value, result._value);
            return result;
        }

        public static BigInteger? operator -(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_sub_ui(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger? operator -(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_neg(result._value, value._value);
            return result;
        }

        public static BigInteger? operator *(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_mul(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger? operator *(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_mul_si(result._value, left._value, right);
            return result;
        }

        public static BigInteger? operator /(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_fdiv_q(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger? operator /(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_fdiv_q_ui(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger? operator %(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_mod(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger? operator %(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_mod_ui(result._value, left._value, (uint)right);
            return result;
        }

        public static BigInteger? operator %(BigInteger? left, uint right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_mod_ui(result._value, left._value, right);
            return result;
        }

        public static BigInteger? operator &(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_and(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger? operator &(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var _right = new BigInteger();
            var result = new BigInteger();

            mpz_set_si(_right._value, right);
            mpz_and(result._value, left._value, _right._value);
            return result;
        }

        public static BigInteger? operator ~(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_com(result._value, value._value);
            return result;
        }

        public static BigInteger? operator |(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_ior(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger? operator |(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var _right = new BigInteger();
            var result = new BigInteger();

            mpz_set_si(_right._value, right);
            mpz_ior(result._value, left._value, _right._value);
            return result;
        }

        public static BigInteger? operator ^(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_xor(result._value, left._value, right._value);
            return result;
        }

        public static BigInteger? operator ^(BigInteger? left, int right)
        {
            if (left is null)
                return null;
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
        public static BigInteger Add(BigInteger left, BigInteger right) => left + right;

        /// <summary>
        /// Returns the sum of the specified <see cref="BigInteger"/> object and integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger Add(BigInteger left, int right) => left + right;

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
            var roundingBase = BigInteger.One;

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

        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the modulus power of the <see cref="BigInteger"/> object with the specified exponent and modulus.
        /// </summary>
        /// <param name="exponent"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public BigInteger ModPow(BigInteger exponent, BigInteger modulus) => ModPow(this, exponent, modulus);
        /// <summary>
        /// Returns the absolute value of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public BigInteger Abs() => Abs(this);
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object's sign is one.
        /// </summary>
        public bool IsOne => Sign() == 1;
        /// <summary>
        /// Gets if the <see cref="BigInteger"/> object's sign is zero.
        /// </summary>
        public bool IsZero => Sign() == 0;
        /// <summary>
        /// Returns the left shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public BigInteger ShiftLeft(int shift) => this << shift;
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
        /// <summary>
        /// Returns the right shifted <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public BigInteger ShiftRight(int shift) => this >> shift;

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
                    return Convert.ToBase64String(ToByteArray());
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
                case -32:
                    return mpz_get_str(new char_ptr(nint.Zero), -32, _value).ToString();
                case 64:
                    return Convert.ToBase64String(ToByteArray());
                case -16:
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
                    2 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 2 : -2, _value).ToString(),
                    4 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 4 : -4, _value).ToString(),
                    8 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 8 : -8, _value).ToString(),
                    10 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : -10, _value).ToString(),
                    16 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 16 : -16, _value).ToString(),
                    32 => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 32 : -32, _value).ToString(),
                    64 => Convert.ToBase64String(ToByteArray()),
                    _ => mpz_get_str(new char_ptr(nint.Zero), isUpper ? 2 : -2, _value).ToString().Substring(0, len)
                };
            if (isHex)
                return len > 0
                    ? mpz_get_str(new char_ptr(nint.Zero), isUpper ? 16 : -16, _value).ToString()
                        .Substring(0, len)
                    : mpz_get_str(new char_ptr(nint.Zero), isUpper ? 16 : -16, _value).ToString();
            if(isDecimal)
                return len > 0
                    ? mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : -10, _value).ToString()
                        .Substring(0, len)
                    : mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : -10, _value).ToString();
            if(isNumber)
                return len > 0
                    ? mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : -10, _value).ToString()
                        .Substring(0, len)
                    : mpz_get_str(new char_ptr(nint.Zero), isUpper ? 10 : -10, _value).ToString();

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
        /// Gets the lowest set bit of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public int GetLowestSetBit() => (int)mpz_scan1(_value, 0).Value;

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
        public long ToLong()
        {
            // Check if the value fits within a signed long range
            if (mpz_fits_slong_p(_value) == 0) // Returns 0 if the value doesn't fit
                throw new OverflowException("The BigInteger value is too large or too small to fit into a long.");

            // Perform the conversion using GMP's mpz_get_si
            return mpz_get_si(_value);
        }

        /// <summary>
        /// Gets the decimal value of the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public decimal ToDecimal()
        {
            // Check if the BigInteger is zero
            if (this == BigInteger.Zero)
                return 0m;

            // Define the maximum decimal value as a BigInteger
            var maxValue = new BigInteger(decimal.MaxValue);
            var minValue = new BigInteger(decimal.MinValue);

            // If the value is within the range of a decimal, directly convert it
            if (this >= minValue && this <= maxValue)
                return (decimal)(double)this; // Convert BigInteger to double first, then to decimal

            // Split the BigInteger into smaller parts
            // Use the largest power of 10 that fits within a decimal
            var tenPower28 = BigInteger.Pow(10, 28);
            var current = this;
            var result = 0m;
            var scale = 0;

            while (current != BigInteger.Zero)
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
        /// Clones the <see cref="BigInteger"/> object.
        /// </summary>
        /// <returns></returns>
        public object Clone() => new BigInteger(this);

        /// <summary>
        /// Compares the <see cref="BigInteger"/> object to the specified object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object? other)
        {
            if (other == null)
                return 1;
            if (other is BigInteger integer)
                return mpz_cmp(_value, integer._value);
            throw new ArgumentException("Object is not a BigInteger.");
        }

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
    }
}
