using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Assignment.Helpers
{
	public class UtilitiesCommon
	{
		public static string GetGUID(string suffix = "")
		{
			return Guid.NewGuid().ToString("N", System.Globalization.CultureInfo.InvariantCulture) + suffix;
		}

		public static string GUIDToString(Guid guid)
		{
			return guid.ToString("N", System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ConvertDateTimeToUTCString(DateTimeOffset dateTime)
		{
			return dateTime.UtcDateTime.ToString("yyyyMMddTHH:mm:ssZ", DateTimeFormatInfo.InvariantInfo);
		}

		public static bool TryConvertUTCStringToDateTime(string dateTimeIn, out DateTimeOffset dateTimeOut)
		{
			return DateTimeOffset.TryParseExact(dateTimeIn, "yyyyMMddTHH:mm:ssZ", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dateTimeOut);
		}
	}
}