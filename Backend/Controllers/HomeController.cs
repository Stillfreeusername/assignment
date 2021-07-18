/*using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Backend.Helpers.Extensions;

namespace Backend.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		[Route("/")]
		[HttpGet]
		public ActionResult<string> Get()
		{
			try
			{
				return "ok";
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToSafeString());
			}
		}
	}
}*/
