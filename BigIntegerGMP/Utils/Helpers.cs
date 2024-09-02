using System.Runtime.InteropServices;

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
                BaseFormat.Base2 => -2,
                BaseFormat.Base8 => -8,
                BaseFormat.Base10 => -10,
                BaseFormat.Base16 => -16,
                _ => 0
            };
        }
        /// <summary>
        /// Converts a Base-64 encoded string directly to hexadecimal aka Base-16 
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static string FromBase64(this string base64String)
        {
            var data = Convert.FromBase64String(base64String);
            return new BigInteger(data).ToString(BaseFormat.Base16);
            //return data.Aggregate("", (current, b) => current + b.ToString("X2"));
        }
    }
}
