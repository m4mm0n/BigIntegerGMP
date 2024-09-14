using System.ComponentModel;

namespace BigIntegerGMP
{
    /// <summary>
    /// Base format
    /// </summary>
    public enum BaseFormat
    {
        [Description("Base2")]
        Base2,
        [Description("Base8")]
        Base8,
        [Description("Base10")]
        Base10,
        [Description("Base16")]
        Base16,
        [Description("Base32")]
        Base32,
        [Description("Base64")]
        Base64
    }
}
