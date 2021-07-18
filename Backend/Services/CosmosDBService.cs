using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

using Assignment.DataLayer;
using Backend.Helpers.Exceptions;

using Utils = Assignment.Helpers.UtilitiesBackend;

namespace Backend.Services
{
	public interface ICosmosDBService
	{
		// excample container getter in case of using Cosmos DB
		//public Container CompaniesContainer { get; }

		public (CompanyDTO company, ProductDTO product) UpsertCompanyAndProductTuple(CompanyDTO companyDTO, ProductDTO productDTO);

		public (InventoryDTO inventoryDTO, Tags_SGTIN96DTO tagsDTO) InsertInventoryWithSGTIN96Tags(
			string inventoryEventID,
			string inventoryLocation,
			DateTimeOffset dateOfInventory,
			List<Bytefeld.Epc.Sgtin96Tag> tags); // this makes sure that only valid tags can be inserted

		List<Assignment.DataLayer.CompanyDTO> Companies { get; }
		List<Assignment.DataLayer.ProductDTO> Products { get; }
		List<Assignment.DataLayer.InventoryDTO> Inventories { get; }
		List<Assignment.DataLayer.Tags_SGTIN96DTO> Tags_SGTIN96 { get; }
	}

	public class CosmosDBService : ICosmosDBService
	{
		// excample container getter in case of using Cosmos DB
		//public Container CompaniesContainer { get; private set; }

		private CosmosClient DBClient;
		private string DatabaseName;

		private List<Assignment.DataLayer.CompanyDTO> _Companies = new List<Assignment.DataLayer.CompanyDTO>();
		public List<Assignment.DataLayer.CompanyDTO> Companies => _Companies;

		private List<Assignment.DataLayer.ProductDTO> _Products = new List<Assignment.DataLayer.ProductDTO>();
		public List<Assignment.DataLayer.ProductDTO> Products => _Products;

		private List<Assignment.DataLayer.InventoryDTO> _Inventories = new List<Assignment.DataLayer.InventoryDTO>();
		public List<Assignment.DataLayer.InventoryDTO> Inventories => _Inventories;

		private List<Assignment.DataLayer.Tags_SGTIN96DTO> _Tags_SGTIN96 = new List<Assignment.DataLayer.Tags_SGTIN96DTO>();
		public List<Assignment.DataLayer.Tags_SGTIN96DTO> Tags_SGTIN96 => _Tags_SGTIN96;

		public CosmosDBService(CosmosClient dbClient, string databaseName, IConfigurationSection configSection)
		{
			DBClient = dbClient;
			DatabaseName = databaseName;

			string CompaniesContainerID = configSection.GetSection("ContainerName_Companies").Value;

			//CompaniesContainer = DBClient.GetContainer(DatabaseName, CompaniesContainerID);
		}

		public (CompanyDTO company, ProductDTO product) UpsertCompanyAndProductTuple(CompanyDTO companyDTO, ProductDTO productDTO)
		{
			// check if company-prefix and item-reference can form a valid SGTIN-96 tag
			if (Utils.CanRepresentValidSGTIN96Tag(companyDTO.CompanyPrefix_SGTIN96, productDTO.ProductReference_SGTIN96) == false) throw new InvalidSGTIN96_CompanyProduct_CombinationException();

			var companies = from c in _Companies
							where c.CompanyName.Equals(companyDTO.CompanyName, StringComparison.InvariantCultureIgnoreCase)
							select c;

			var company = companies.FirstOrDefault();
			if (company != null)
			{
				if (company.CompanyPrefix_SGTIN96 != companyDTO.CompanyPrefix_SGTIN96) throw new BackendException("Value mismatch" + nameof(company.CompanyPrefix_SGTIN96));
			}

			var products = from p in _Products
						   where p.ProductName.Equals(productDTO.ProductName, StringComparison.InvariantCultureIgnoreCase)
						   select p;

			var product = products.FirstOrDefault();
			if (product != null)
			{
				if (product.ProductReference_SGTIN96 != product.ProductReference_SGTIN96) throw new BackendException("Value mismatch" + nameof(product.ProductReference_SGTIN96));
			}

			if (company == null)
			{
				company = companyDTO.DeepCopy();
				company.CompanyID = Utils.GetGUID();
				company.CompanyID2 = company.CompanyID;
				_Companies.Add(company);
			}

			if (product == null)
			{
				product = productDTO.DeepCopy();
				product.ProductID = Utils.GetGUID();
				product.CompanyID = company.CompanyID;
				_Products.Add(product);
			}

			return (company.DeepCopy(), product.DeepCopy());
		}

		public (InventoryDTO inventoryDTO, Tags_SGTIN96DTO tagsDTO) InsertInventoryWithSGTIN96Tags(
			string inventoryEventID,
			string inventoryLocation,
			DateTimeOffset dateOfInventory,
			List<Bytefeld.Epc.Sgtin96Tag> tags) // this makes sure that only valid tags can be inserted
		{
			var Regex_AlphaNum = new Regex("^[a-zA-Z0-9]{1,32}$");
			if (Regex_AlphaNum.IsMatch(inventoryEventID) == false) throw new InvalidIDException();

			/* If it is correct to store the company-prefix and the product-reference as the input format of the method
			 * then the used SGTIN96 library would store it using the same types.
			 * Therefore the following validation process would not require to convert numbers here...
			 * Validation happens somewhat repeatedly because this method is public.
			 * */
			ulong companyPrefix = ulong.Parse(tags[0].CompanyPrefix, System.Globalization.NumberStyles.Any);
			uint productReference = uint.Parse(tags[0].IndicatorAndItemReference, System.Globalization.NumberStyles.Any);

			var companies = from c in _Companies
							where c.CompanyPrefix_SGTIN96 == companyPrefix
							select c;

			if (companies.Count() <= 0) throw new CompanyNotFoundException();
			var company = companies.FirstOrDefault();

			var products = from p in _Products
						   where p.ProductReference_SGTIN96 == productReference
						   select p;

			if (products.Count() <= 0) throw new ProductNotFoundException();
			var product = products.FirstOrDefault();

			string inventoryID = Utils.GetGUID();
			string tagsID = Utils.GetGUID();

			var tagsDTO = new Tags_SGTIN96DTO
			{
				TagsID = Utils.GetGUID(),
				CompanyID = company.CompanyID,
				ProductID = product.ProductID,
				InventoryID = inventoryID
			};

			foreach (var tag in tags)
			{
				// enforce tags representing the same product for the same company
				if (tag.CompanyPrefix != tags[0].CompanyPrefix || tag.IndicatorAndItemReference != tags[0].IndicatorAndItemReference)
				{
					throw new InvalidSGTIN96TagException();
				}
				tagsDTO.Tags.Add(tag.ToBinary());
			}

			var inventoryDTO = new InventoryDTO
			{
				InventoryID = Utils.GetGUID(),
				CompanyID = company.CompanyID,
				ProductID = product.ProductID,
				ProductCount = (ulong)tags.Count,
				InventoryEventID = inventoryEventID,
				TagsID = tagsID,
				InventoryLocation = inventoryLocation,
				DateOfInventory = Utils.ConvertDateTimeToUTCString(dateOfInventory)
			};

			_Tags_SGTIN96.Add(tagsDTO);

			_Inventories.Add(inventoryDTO);

			return (inventoryDTO, tagsDTO);
		}
	}
}
