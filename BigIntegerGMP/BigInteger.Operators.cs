using static Math.Gmp.Native.gmp_lib;

namespace BigIntegerGMP
{
    public partial class BigInteger : IDisposable, ICloneable, IComparable<BigInteger>
    {
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
        public static implicit operator BigInteger(sbyte value)
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
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
        /// Checks if a long integer is less than a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(BigInteger? left, long right) =>
            left is not null && mpz_cmp_ui(left._value, (uint)right) < 0;
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
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is greater than a double.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(BigInteger? left, double right) =>
            left is not null && mpz_cmp_d(left._value, right) > 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is greater than a long integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(BigInteger? left, long right) =>
            left is not null && mpz_cmp_ui(left._value, (uint)right) > 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is less than a double.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(BigInteger? left, double right) =>
            left is not null && mpz_cmp_d(left._value, right) < 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is less than or equal to another double.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(BigInteger? left, BigInteger? right) =>
            left is not null && right is not null && mpz_cmp(left._value, right._value) <= 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is less than or equal to an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(BigInteger? left, int right) =>
            left is not null && mpz_cmp_si(left._value, right) <= 0;
        /// <summary>
        /// Checks if an integer is less than or equal to a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(int left, BigInteger? right) =>
            right is not null && mpz_cmp_si(right._value, left) >= 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is greater than or equal to another <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(BigInteger? left, BigInteger? right) =>
            left is not null && right is not null && mpz_cmp(left._value, right._value) >= 0;
        /// <summary>
        /// Checks if a <see cref="BigInteger"/> object is greater than or equal to an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(BigInteger? left, int right) =>
            left is not null && mpz_cmp_si(left._value, right) >= 0;
        /// <summary>
        /// Checks if an integer is greater than or equal to a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(int left, BigInteger? right) =>
            right is not null && mpz_cmp_si(right._value, left) <= 0;
        /// <summary>
        /// Shifts a <see cref="BigInteger"/> object to the left by the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator <<(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_mul_2exp(result._value, left._value, mpz_get_ui(right._value));
            return result;
        }
        /// <summary>
        /// Shifts a <see cref="BigInteger"/> object to the left by the specified integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator <<(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_mul_2exp(result._value, left._value, (uint)right);
            return result;
        }
        /// <summary>
        /// Shifts a <see cref="BigInteger"/> object to the right by the specified <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator >>(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_fdiv_q_2exp(result._value, left._value, mpz_get_ui(right._value));
            return result;
        }
        /// <summary>
        /// Shifts a <see cref="BigInteger"/> object to the right by the specified integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator >>(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_fdiv_q_2exp(result._value, left._value, (uint)right);
            return result;
        }
        /// <summary>
        /// Adds two <see cref="BigInteger"/> objects together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator +(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_add(result._value, left._value, right._value);
            return result;
        }
        /// <summary>
        /// Adds an integer to a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator +(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_add_ui(result._value, left._value, (uint)right);
            return result;
        }
        /// <summary>
        /// Adds a <see cref="BigInteger"/> object to a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger? operator +(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_add(result._value, value._value, result._value);
            return result;
        }
        /// <summary>
        /// Adds a <see cref="BigInteger"/> object to a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger? operator ++(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_add_ui(result._value, value._value, 1);
            return result;
        }
        /// <summary>
        /// Negates a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger? operator --(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_sub_ui(result._value, value._value, 1);
            return result;
        }
        /// <summary>
        /// Subtracts a <see cref="BigInteger"/> object from another <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator -(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_sub(result._value, left._value, result._value);
            return result;
        }
        /// <summary>
        /// Subtracts an integer from a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator -(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_sub_ui(result._value, left._value, (uint)right);
            return result;
        }
        /// <summary>
        /// Subtracts a <see cref="BigInteger"/> object from a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger? operator -(BigInteger? value)
        {
            if (value is null)
                return null;
            var result = new BigInteger();
            mpz_neg(result._value, value._value);
            return result;
        }
        /// <summary>
        /// Multiplies two <see cref="BigInteger"/> objects together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator *(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_mul(result._value, left._value, right._value);
            return result;
        }
        /// <summary>
        /// Multiplies a <see cref="BigInteger"/> object by an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator *(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_mul_si(result._value, left._value, right);
            return result;
        }
        /// <summary>
        /// Divides a <see cref="BigInteger"/> object by another <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator /(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_fdiv_q(result._value, left._value, right._value);
            return result;
        }
        /// <summary>
        /// Divides a <see cref="BigInteger"/> object by an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator /(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_fdiv_q_ui(result._value, left._value, (uint)right);
            return result;
        }
        /// <summary>
        /// Performs a modulo operation on two <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator %(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_mod(result._value, left._value, right._value);
            return result;
        }
        /// <summary>
        /// Performs a modulo operation on a <see cref="BigInteger"/> object and an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator %(BigInteger? left, int right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_mod_ui(result._value, left._value, (uint)right);
            return result;
        }
        /// <summary>
        /// Performs a modulo operation on a <see cref="BigInteger"/> object and an unsigned integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator %(BigInteger? left, uint right)
        {
            if (left is null)
                return null;
            var result = new BigInteger();
            mpz_mod_ui(result._value, left._value, right);
            return result;
        }
        /// <summary>
        /// Performs a bitwise AND operation on two <see cref="BigInteger"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger? operator &(BigInteger? left, BigInteger? right)
        {
            if (left is null || right is null)
                return null;
            var result = new BigInteger();
            mpz_and(result._value, left._value, right._value);
            return result;
        }
        /// <summary>
        /// Performs a bitwise AND operation on a <see cref="BigInteger"/> object and an integer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Performs a bitwise OR operation on a <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
    }
}
