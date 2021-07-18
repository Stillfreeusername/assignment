using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

using Newtonsoft.Json;

using Utils = Assignment.Helpers.UtilitiesCommon;

namespace Assignment.DataLayer
{
	public class CompanyDTO
	{
		[JsonPropertyName("id")] //For System.Text.Json.Serialization based libraries
		[JsonProperty(PropertyName = "id")] //For Newtonsoft.Json based libraries
		public string CompanyID { get; set; } // the permanent ID of the company

		public string CompanyID2 { get; set; } // just a copy here for partition key

		public string CompanyName { get; set; }

		public ulong CompanyPrefix_SGTIN96 { get; set; }

		public CompanyDTO DeepCopy()
		{
			var clone = (CompanyDTO)this.MemberwiseClone();
			return clone;
		}
	}

	public class ProductDTO
	{
		[JsonPropertyName("id")] //For System.Text.Json.Serialization based libraries
		[JsonProperty(PropertyName = "id")] //For Newtonsoft.Json based libraries
		public string ProductID { get; set; } // the permanent ID of the product

		public string CompanyID { get; set; } // partition key

		public string ProductName { get; set; }

		public uint ProductReference_SGTIN96 { get; set; } // also ItemReference in some context

		public ProductDTO DeepCopy()
		{
			var clone = (ProductDTO)this.MemberwiseClone();
			return clone;
		}
	}

	public class InventoryDTO
	{
		[JsonPropertyName("id")] //For System.Text.Json.Serialization based libraries
		[JsonProperty(PropertyName = "id")] //For Newtonsoft.Json based libraries
		public string InventoryID { get; set; } // the permanent ID of an inventory

		public string CompanyID { get; set; } // partition key

		public string ProductID {get; set;}

		public ulong ProductCount { get; set; }

		// The last four fields can be stored in separate container if it becomes necessary

		public string InventoryEventID { get; set; }

		public string TagsID { get; set; }

		public string InventoryLocation { get; set; }

		public string DateOfInventory { get; set; }

		public InventoryDTO DeepCopy()
		{
			var clone = (InventoryDTO)this.MemberwiseClone();
			return clone;
		}
	}

	public class Tags_SGTIN96DTO
	{
		[JsonPropertyName("id")] //For System.Text.Json.Serialization based libraries
		[JsonProperty(PropertyName = "id")] //For Newtonsoft.Json based libraries
		public string TagsID { get; set; } // the permanent ID of an inventory

		public string CompanyID { get; set; } // partition key

		public string ProductID { get; set; }

		public string InventoryID { get; set; }

		public List<string> Tags = new List<string>();

		public Tags_SGTIN96DTO DeepCopy()
		{
			var clone = (Tags_SGTIN96DTO)this.MemberwiseClone();
			clone.Tags = Tags == null ? new List<string>() : new List<string>(Tags);
			return clone;
		}
	}
}