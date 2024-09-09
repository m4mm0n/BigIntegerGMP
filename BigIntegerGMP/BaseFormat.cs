using System.ComponentModel;

namespace BigIntegerGMP
{
    /// <summary>
    /// Base format
    /// </summary>
    public enum BaseFormat
    {
        [Description("Base 2")]
        Base2 = 2,
        [Description("Base 8")]
        Base8 = 8,
        [Description("Base 10")]
        Base10 = 10,
        [Description("Base 16")]
        Base16 = 16,
        [Description("Base 32")]
        Base32 = 32,
        [Description("Base 64")]
        Base64 = 64
    }
}
