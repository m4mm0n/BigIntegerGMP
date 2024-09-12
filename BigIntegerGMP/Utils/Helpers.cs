using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace BigIntegerGMP.Utils
{
    public static class Helpers
    {
        /// <summary>
        /// Converts a byte array to a pointer.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IntPtr ByteArrayToPointer(this byte[] data)
        {
            // Pin the byte array to prevent it from being moved by the garbage collector
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            // Get the address of the pinned byte array
            var pointer = handle.AddrOfPinnedObject();

            // Remember to free the handle when you're done with the pointer
            handle.Free(); // Uncomment this line when you're finished

            return pointer;
        }
        /// <summary>
        /// Converts a pointer to a byte array.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] PointerToByteArray(this IntPtr pointer, int length)
        {
            // Create a new byte array to hold the data
            var data = new byte[length];

            // Copy the data from the pointer to the byte array
            Marshal.Copy(pointer, data, 0, length);

            return data;
        }
        /// <summary>
        /// Swaps the endianness of a given 32-bit unsigned integer.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static uint SwapEndian(this uint x) =>
            ((x & 0x000000ff) << 24) |
            ((x & 0x0000ff00) << 8) |
            ((x & 0x00ff0000) >> 8) |
            ((x & 0xff000000) >> 24);
        /// <summary>
        /// Swaps the endianness of a given 64-bit unsigned integer.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static ulong SwapEndian(this ulong x) =>
            ((x & 0x00000000000000ff) << 56) |
            ((x & 0x000000000000ff00) << 40) |
            ((x & 0x0000000000ff0000) << 24) |
            ((x & 0x00000000ff000000) << 8) |
            ((x & 0x000000ff00000000) >> 8) |
            ((x & 0x0000ff0000000000) >> 24) |
            ((x & 0x00ff000000000000) >> 40) |
            ((x & 0xff00000000000000) >> 56);
        /// <summary>
        /// Swaps the endianness of a given 32-bit floating-point number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float SwapEndian(this float x)
        {
            var bytes = BitConverter.GetBytes(x);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }
        /// <summary>
        /// Swaps the endianness of a given 64-bit floating-point number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double SwapEndian(this double x)
        {
            var bytes = BitConverter.GetBytes(x);
            Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
        /// <summary>
        /// Converts an array of unsigned shorts to a byte array.
        /// </summary>
        /// <param name="ushortArray"></param>
        /// <param name="useLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this ushort[] ushortArray, bool useLittleEndian = true)
        {
            var byteLength = ushortArray.Length * sizeof(ushort);
            var byteArray = new byte[byteLength];

            for (var i = 0; i < ushortArray.Length; i++)
            {
                var temp = BitConverter.GetBytes(ushortArray[i]);
                if (BitConverter.IsLittleEndian != useLittleEndian) Array.Reverse(temp);
                Buffer.BlockCopy(temp, 0, byteArray, i * sizeof(ushort), sizeof(ushort));
            }

            return byteArray;
        }
        /// <summary>
        /// Converts an array of unsigned integers to a byte array.
        /// </summary>
        /// <param name="uintArray"></param>
        /// <param name="useLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this uint[] uintArray, bool useLittleEndian = true)
        {
            var byteLength = uintArray.Length * sizeof(uint);
            var byteArray = new byte[byteLength];

            for (var i = 0; i < uintArray.Length; i++)
            {
                var temp = BitConverter.GetBytes(uintArray[i]);
                if (BitConverter.IsLittleEndian != useLittleEndian) Array.Reverse(temp);
                Buffer.BlockCopy(temp, 0, byteArray, i * sizeof(uint), sizeof(uint));
            }

            return byteArray;
        }
        /// <summary>
        /// Basically a modified version of Marshal.Copy for copying an array of unsigned integers to a pointer.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="destination"></param>
        /// <param name="length"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static unsafe void CopyEx(uint[] value, int startIndex, nint destination, int length)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (destination == 0) throw new ArgumentNullException(nameof(destination));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > value.Length) throw new ArgumentOutOfRangeException(nameof(length));

            var ptr = (uint*)destination;
            for (var i = 0; i < length; i++)
            {
                ptr[i] = value[startIndex + i];
            }
        }
        /// <summary>
        /// In some instances, the enum value does not match the value of the enum.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static int GetBaseFromBaseFormat(BaseFormat format)
        {
            return format switch
            {
                BaseFormat.Base2 => 2,
                BaseFormat.Base8 => 8,
                BaseFormat.Base10 => 10,
                BaseFormat.Base16 => -16,
                BaseFormat.Base32 => -32,
                BaseFormat.Base64 => 64,
                _ => 0
            };
        }
        /// <summary>
        /// A simple method to check if a string is a valid BigInteger.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool IsValidBigInteger(string value, int format)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            try
            {
                switch (format)
                {
                    case 2: // Base-2 (Binary)
                        if (!Regex.IsMatch(value, "^[01]+$")) return false;
                        break;
                    case 8: // Base-8 (Octal)
                        if (!Regex.IsMatch(value, "^[0-7]+$")) return false;
                        break;
                    case 10: // Base-10 (Decimal)
                        if (!Regex.IsMatch(value, "^[0-9]+$")) return false;
                        break;
                    case 16: // Base-16 (Hexadecimal)
                        if (!Regex.IsMatch(value, "^[0-9A-Fa-f]+$")) return false;
                        break;
                    case 32: // Base-32
                        if (!Regex.IsMatch(value, "^[A-V0-9]+$")) return false; // Base32 uses A-V and 0-9
                        break;
                    case 64: // Base-64
                        if (!Regex.IsMatch(value, "^[A-Za-z0-9+/]+={0,2}$")) return false; // Includes '=' padding
                        break;
                    default:
                        return false;
                }
                var bigInt = new BigInteger(value, (BaseFormat)format);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// A simple method to get the probable base format of a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns the probable base-format if a match is found, otherwise -1 if no match is found...</returns>
        public static int ProbableBaseFormat(string value)
        {
            if (Regex.IsMatch(value, "^[A-Za-z0-9+/]+={0,2}$"))
                return 64;
            if (Regex.IsMatch(value, "^[A-V0-9]+$"))
                return 32;
            if(Regex.IsMatch(value, "^[0-9A-Fa-f]+$"))
                return 16;
            if (Regex.IsMatch(value, "^[0-9]+$"))
                return 10;
            if (Regex.IsMatch(value, "^[0-7]+$"))
                return 8;
            if (Regex.IsMatch(value, "^[01]+$"))
                return 2;

            return -1;
        }

    }
}
