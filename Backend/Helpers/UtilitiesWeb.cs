using Assignment.Helpers;

namespace Assignment.Helpers
{
	public class UtilitiesBackend : UtilitiesCommon
	{
		public static bool CanRepresentValidSGTIN96Tag(ulong companyPrefix, uint itemReference)
		{
			// I strongly think this should be implemented by the library which is used to handle SGTIN-96 tags
			if (companyPrefix < (0x1UL << 40 + 1) && itemReference <= (0x1U << 4 + 1)) return true;
			if (companyPrefix < (0x1UL << 37 + 1) && itemReference <= (0x1U << 7 + 1)) return true;
			if (companyPrefix < (0x1UL << 34 + 1) && itemReference <= (0x1U << 10 + 1)) return true;
			if (companyPrefix < (0x1UL << 30 + 1) && itemReference <= (0x1U << 14 + 1)) return true;
			if (companyPrefix < (0x1UL << 27 + 1) && itemReference <= (0x1U << 17 + 1)) return true;
			if (companyPrefix < (0x1UL << 24 + 1) && itemReference <= (0x1U << 20 + 1)) return true;
			if (companyPrefix < (0x1UL << 20 + 1) && itemReference <= (0x1U << 24 + 1)) return true;
			return false;
		}
	}
}