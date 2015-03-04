using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GS.Lib.Util
{
    internal static class StringExtensions
    {
        public static string RemoveEnd(this string p_String, string p_Match)
        {
            return p_String.EndsWith(p_Match) ? p_String.Substring(0, p_String.Length - p_Match.Length) : p_String;
        }

        public static bool IsEmail(this string p_String)
        {
            const string s_Pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                                    + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                                    + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(s_Pattern, RegexOptions.IgnoreCase).IsMatch(p_String);
        }

        public static bool TryGetDate(this string p_String, out DateTime p_Output)
        {
            p_Output = new DateTime(1800, 1, 1);

            const string c_Pattern = "(((?:(?:[0-2]?\\d{1})|(?:[3][01]{1})))[-:\\/.]((?:[0]?[1-9]|[1][012]))[-:\\/.]((?:(?:[1]{1}\\d{1}\\d{1}\\d{1})|(?:[2]{1}\\d{3}))))(?![\\d])"; // DDMMYYYY 1

            var s_Regex = new Regex(c_Pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var s_Match = s_Regex.Match(p_String);

            if (!s_Match.Success)
                return false;

            try
            {
                p_Output = new DateTime(Int32.Parse(s_Match.Groups[4].ToString()), Int32.Parse(s_Match.Groups[3].ToString()), Int32.Parse(s_Match.Groups[2].ToString()));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsASCII(this string p_String)
        {
            // ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
            return Encoding.UTF8.GetByteCount(p_String) == p_String.Length;
        }

        public static String SecureRandom(this String p_String, int p_Length)
        {
            if (p_Length < 0)
                return null;

            if (string.IsNullOrEmpty(p_String))
                return null;

            const int c_ByteSize = 0x100;
            var s_AllowedCharSet = new HashSet<char>(p_String).ToArray();

            if (c_ByteSize < s_AllowedCharSet.Length)
                return null;

            using (var s_Rng = new RNGCryptoServiceProvider())
            {
                var s_Result = new StringBuilder();
                var s_Buf = new byte[128];

                while (s_Result.Length < p_Length)
                {
                    s_Rng.GetBytes(s_Buf);

                    for (var i = 0; i < s_Buf.Length && s_Result.Length < p_Length; ++i)
                    {
                        var s_OutOfRangeStart = c_ByteSize - (c_ByteSize % s_AllowedCharSet.Length);

                        if (s_OutOfRangeStart <= s_Buf[i])
                            continue;

                        s_Result.Append(s_AllowedCharSet[s_Buf[i] % s_AllowedCharSet.Length]);
                    }
                }

                return s_Result.ToString();
            }
        }

        public static string ToUnderscoreCase(this string p_Str)
        {
            return string.Concat(p_Str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }

    public static class DateTimeExtensions
    {
        public static Int64 ToUnixTimestamp(this DateTime p_Time)
        {
            return (Int64)(p_Time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static Int64 ToUnixTimestampMillis(this DateTime p_Time)
        {
            return (Int64)(p_Time.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }

        public static DateTime FromUnixTimestamp(this Int64 p_Timestamp)
        {
            var s_DateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            s_DateTime = s_DateTime.AddSeconds(p_Timestamp).ToLocalTime();
            return s_DateTime;
        }
    }
}
