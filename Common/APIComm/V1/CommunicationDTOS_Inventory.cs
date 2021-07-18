using System;
using System.Collections.Generic;

// Mark the model with attributes to help drive the Swagger UI
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace Assignment.APIComm.V1
{
	/* I use string for representing types given as “Numeric” in the assignment. I do it because:
	 * 1: Everything is stored as string in CosmosDB.
	 * 2: Everything is serialized to strings during network transfer.
	 * 3: The size of the type is not defined in the specification.
	 * 4: If any numerical computation is required later and the size of the integral types would not be enough then it can be converted to BigInteger in System.Numerics.
	 * */
	public class CreateProductReq
	{
		[Required]
		public uint ItemReference { get; set; }

		[Required]
		public string ItemName { get; set; }

		[Required]
		public ulong CompanyPrefix { get; set; }

		[Required]
		public string CompanyName { get; set; }
	}

	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class CreateProductResp : CommunicationDTOBase
	{
		public bool? InvalidCompanyName { get; set; }

		public bool? InvalidPrefixCombination { get; set; }

		public bool? InvalidItemName { get; set; }
	}

	public class CreateInventoryReq
	{
		[Required]
		public string InventoryEventID { get; set; }

		[Required]
		public string Location { get; set; }

		[Required]
		public string DateOfInventory { get; set; }

		[Required]
		public List<string> ItemTags { get; set; }
	}

	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class CreateInventoryResp : CommunicationDTOBase
	{
		public bool? InvalidInventoryEventID { get; set; }

		public bool? InvalidDateOfInventory { get; set; }

		public bool? EmptyTags { get; set; }

		public bool? OneOrMoreTagsAreInvalid { get; set; }
		public List<string> FailedTags { get; set; }

		public bool? OneOrMoreCompanyPrefixNotFound { get; set; }

		public bool? OneOrMoreProductReferenceNotFound { get; set; }

		public int SucceededTags { get; set; }
	}

	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class InventoriedItemsCountGroupedByProductResp : CommunicationDTOBase
	{
		public List<(string productID, ulong inventoriedItemsCount)> Res { get; set; }
	}

	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class InventoriedItemsCountGroupedByProductPerDayResp : CommunicationDTOBase
	{
		public List<(string productID, string day, ulong inventoriedItemsCount)> Res { get; set; }
	}

	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class InventoriedItemsCountGroupedByCompaniesResp : CommunicationDTOBase
	{
		public List<(string companyID, ulong inventoriedItemsCount)> Res { get; set; }
	}
}