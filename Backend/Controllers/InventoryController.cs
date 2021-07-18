using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Assignment.DataLayer;
using Assignment.APIComm.V1;
using Backend.Helpers.Extensions;
using Backend.Helpers.Exceptions;
using Backend.Services;

using EpcTag = Bytefeld.Epc.EpcTag;
using Utils = Assignment.Helpers.UtilitiesBackend;

namespace Backend.Controllers
{
	[Produces("application/json")]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class InventoryController : ControllerBase
	{
		private readonly ICosmosDBService _cdbService;
		private readonly ILogger<InventoryController> _logger;

		public InventoryController(
			ICosmosDBService cosmosDBService,
			ILogger<InventoryController> logger
			)
		{
			_cdbService = cosmosDBService;
			_logger = logger;
		}

		/// <summary>
		/// Creates a new product item and a new company item if it does not exists yet.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		///     POST /CreateProduct
		///     {
		///        "CompanyPrefix": 3319361,
		///        "CompanyName": "Sanford LLC",
		///        "ItemReference": 407205,
		///        "ItemName": "Beans - Kidney, Red Dry"
		///     }
		///
		/// </remarks>
		/// <returns>The result of the operation</returns>
		/// <response code="201">Descriptive result about the operation</response>
		/// <response code="400">If any unhandled exception occours</response>
		[HttpPost("createproduct")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<CreateProductResp>> CreateProduct([FromBody] CreateProductReq request)
		{
			try
			{
				if (string.IsNullOrEmpty(request.CompanyName)) return Ok(new CreateProductResp { InvalidCompanyName = true });
				if (string.IsNullOrEmpty(request.ItemName)) return Ok(new CreateProductResp { InvalidItemName = true });

				var companyDTO = new CompanyDTO
				{
					CompanyName = request.CompanyName,
					CompanyPrefix_SGTIN96 = request.CompanyPrefix
				};
				var productDTO = new ProductDTO
				{
					ProductName = request.ItemName,
					ProductReference_SGTIN96 = request.ItemReference
				};

				var dtos = _cdbService.UpsertCompanyAndProductTuple(companyDTO, productDTO);

				if (dtos.company == null || dtos.product == null) return Ok(new CreateProductResp { OtherError = true });

				var ret = new CreateProductResp { Completed = true };
				return Ok(ret);
			}
			catch (InvalidSGTIN96_CompanyProduct_CombinationException)
			{
				return Ok(new CreateProductResp { InvalidPrefixCombination = true });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToSafeString());
			}
		}

		/// <summary>
		/// Post inventory data batch identified by an event-id. The tags can contain any company – product combination which is already stored in the database.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		///     POST /CreateInventory
		///     {
		///        "InventoryEventID": abcd,
		///        "Location": "Graz, Austria",
		///        "DateOfInventory": 20210718T13:05:22Z,
		///        "ItemTags": [ "3098D0A357783C0034E9DF74", "307ABE3665404EC00F863485" ]
		///     }
		///
		/// </remarks>
		/// <returns>The result of the operation including a list of tags tags failed to insert</returns>
		/// <response code="201">Descriptive result about the operation</response>
		/// <response code="400">If any unhandled exception occours</response>
		[HttpPost("createinventory")]
		public async Task<ActionResult<CreateInventoryResp>> CreateInventory([FromBody] CreateInventoryReq request)
		{
			try
			{
				var Regex_AlphaNum = new Regex("^[a-zA-Z0-9]{1,32}$");
				if (Regex_AlphaNum.IsMatch(request.InventoryEventID) == false) return Ok(new CreateInventoryResp { InvalidInventoryEventID = true });

				if (Utils.TryConvertUTCStringToDateTime(request.DateOfInventory, out var dateOfInventory) == false) return Ok(new CreateInventoryResp { InvalidDateOfInventory = true });

				if (request.ItemTags == null || request.ItemTags.Count <= 0) return Ok(new CreateInventoryResp { EmptyTags = true });

				/* Group the tags into buckets containing the same company-product pairs
				 * I use the library Bytefeld EPC here with the hope to provide validating method.
				 * Every string should be validated and the relevant data should be extracted.
				 * At the moment of writing, the library is in beta stage and does not provide validation.
				 * In a real case, we would need a proper way to handle the data specification we are working with
				 * before publishing any service.
				 * 
				 * I implement mixed strategies here because it is not specified how this request should fail.
				 * */
				var ret = new CreateInventoryResp();
				var failedTags = new HashSet<string>();
				var buckets = new Dictionary<(ulong companyPrefix, uint productReference), List<Bytefeld.Epc.Sgtin96Tag>>();
				foreach (var stringTag in request.ItemTags)
				{
					try
					{
						var tag = EpcTag.FromBinary<Bytefeld.Epc.Sgtin96Tag>(stringTag);

						// depends on the specification whether we should continue from here
						//if (tag.Validate() == false) {ret.OneOrMoreTagsAreInvalid = true; failedTags.Add(stringTag);}

						ulong companyPrefix = ulong.Parse(tag.CompanyPrefix, System.Globalization.NumberStyles.Any);
						uint productReference = uint.Parse(tag.IndicatorAndItemReference, System.Globalization.NumberStyles.Any);

						if (buckets.ContainsKey((companyPrefix, productReference)) == false)
						{
							buckets[(companyPrefix, productReference)] = new List<Bytefeld.Epc.Sgtin96Tag>();
						}
						buckets[(companyPrefix, productReference)].Add(tag);
					}
					catch (Exception)
					{
						ret.OneOrMoreTagsAreInvalid = true;
						failedTags.Add(stringTag);
					}
				}

				foreach (var bucket in buckets)
				{
					try
					{
						_cdbService.InsertInventoryWithSGTIN96Tags(
							request.InventoryEventID,
							request.Location,
							dateOfInventory,
							bucket.Value);
						ret.SucceededTags += bucket.Value.Count;
					}
					catch (CompanyNotFoundException)
					{
						ret.OneOrMoreCompanyPrefixNotFound = true;
						foreach (var tag in bucket.Value) failedTags.Add(tag.ToBinary());
					}
					catch (ProductNotFoundException)
					{
						ret.OneOrMoreProductReferenceNotFound = true;
						foreach (var tag in bucket.Value) failedTags.Add(tag.ToBinary());
					}
					catch (InvalidSGTIN96TagException)
					{
						ret.OneOrMoreTagsAreInvalid = true;
						foreach (var tag in bucket.Value) failedTags.Add(tag.ToBinary());
					}
				}

				if (failedTags.Count() > 0)
				{
					ret.FailedTags = failedTags.ToList();
				}
				else
				{
					ret.Completed = true;
				}
				
				return Ok(ret);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToSafeString());
			}
		}

		/// <summary>
		/// Count of inventoried items grouped by a specific product for a specific inventory.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		///     GET /InventoriedItemsCountGroupedByProduct
		///
		/// </remarks>
		/// <returns>A list of tuples containing the a product-id, and the respective count of inventoried items</returns>
		/// <param name="nameof(inventoryEventID)">The Inventory Id in the assignment.</param>
		/// <response code="200">Returns the newly created list</response>
		/// <response code="400">If any unhandled exception occours</response>
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet("inventorieditemscountgroupedbyproduct")]
		public async Task<ActionResult<InventoriedItemsCountGroupedByProductResp>> InventoriedItemsCountGroupedByProduct([FromQuery] string inventoryEventID)
		{
			try
			{
				/* Do query like
				 * SELECT COUNT(1) AS groupedCount, inv.ProductID FROM Inventories inv WHERE inv.InventoryEventID = @inventoryEventID GROUP BY inv.ProductID
				 * 
				 * Use .WithParameter("@inventoryEventID", inventoryEventID);
				 * */

				var res = new Dictionary<string, ulong>();
				foreach (var inv in _cdbService.Inventories)
				{
					if (inv.InventoryEventID == inventoryEventID)
					{
						if (res.ContainsKey(inv.ProductID) == false) res.Add(inv.ProductID, 0);
						res[inv.ProductID] += inv.ProductCount;
					}
				}

				var ret = new InventoriedItemsCountGroupedByProductResp();
				ret.Res = new List<(string productID, ulong inventoriedItemsCount)>();
				foreach (var v in res)
				{
					ret.Res.Add((v.Key, v.Value));
				}

				ret.Completed = true;
				return ret;
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToSafeString());
			}
		}

		/// <summary>
		/// Count of inventoried items grouped by a specific product per day.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		///     GET /InventoriedItemsCountGroupedByProductPerDay
		///
		/// </remarks>
		/// <returns>A list of tuples containing the a product-id, the day of inventory and the respective count of inventoried items</returns>
		/// <response code="200">Returns the newly created list</response>
		/// <response code="400">If any unhandled exception occours</response>
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet("inventorieditemscountgroupedbyproductperday")]
		public async Task<ActionResult<InventoriedItemsCountGroupedByProductPerDayResp>> InventoriedItemsCountGroupedByProductPerDay()
		{
			try
			{
				/* Do query like
				 * SELECT COUNT(1) AS groupedCount, inv.ProductID, inv.DateOfInventory FROM Inventories inv GROUP BY inv.ProductID, inv.DateOfInventory
				 * 
				 * Consider creating an UDF to omit the time part of the DateOfInventory field.
				 * In that case the modified query would give back the desired result and no need for further processing.
				 * More info about how to define UDF: https://docs.microsoft.com/en-us/azure/cosmos-db/sql-query-udfs
				 * */

				var res = new Dictionary<(string productID, string day), ulong>();
				foreach (var inv in _cdbService.Inventories)
				{
					var key = (inv.ProductID, inv.DateOfInventory.Substring(0, 8));
					if (res.ContainsKey(key) == false) res.Add(key, 0);
					res[key] += inv.ProductCount;
				}

				var ret = new InventoriedItemsCountGroupedByProductPerDayResp();
				ret.Res = new List<(string productID, string day, ulong inventoriedItemsCount)>();
				foreach (var v in res)
				{
					ret.Res.Add((v.Key.productID, v.Key.day, v.Value));
				}

				ret.Completed = true;
				return ret;
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToSafeString());
			}
		}

		/// <summary>
		/// Count of inventoried items grouped by a specific company.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		///     GET /InventoriedItemsCountGroupedByCompanies
		///
		/// </remarks>
		/// <returns>A list of tuples containing the a company-id and the respective count of inventoried items</returns>
		/// <response code="200">Returns the newly created list</response>
		/// <response code="400">If any unhandled exception occours</response>
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet("inventorieditemscountgroupedbycompanies")]
		public async Task<ActionResult<InventoriedItemsCountGroupedByCompaniesResp>> InventoriedItemsCountGroupedByCompanies()
		{
			try
			{
				/* Do query like
				 * SELECT COUNT(1) AS groupedCount, inv.CompanyID FROM Inventories inv GROUP BY inv.CompanyID
				 * */

				var res = new Dictionary<string, ulong>();
				foreach (var inv in _cdbService.Inventories)
				{
					if (res.ContainsKey(inv.CompanyID) == false) res.Add(inv.CompanyID, 0);
					res[inv.CompanyID] += inv.ProductCount;
				}

				var ret = new InventoriedItemsCountGroupedByCompaniesResp();
				ret.Res = new List<(string companyID, ulong inventoriedItemsCount)>();
				foreach (var v in res)
				{
					ret.Res.Add((v.Key, v.Value));
				}

				ret.Completed = true;
				return ret;
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToSafeString());
			}
		}
	}
}
