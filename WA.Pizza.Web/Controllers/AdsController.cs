using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Advertisement;
using WA.Pizza.Web.Filters;

using static WA.Pizza.Core.ConstantValues;

namespace WA.Pizza.Web.Controllers
{
	[ApiKeyAuth]
	[Authorize(Roles = UserRoles.ADMIN_ROLE_NAME)]
	public class AdsController : BaseController
	{
		private readonly AdvertisementDataService _advertisementDataService;

		public AdsController(AdvertisementDataService advertisementDataService)
		{
			_advertisementDataService = advertisementDataService;
		}

		[HttpPost]
		public IActionResult Create(AdvertisementPostRequest request)
		{
			if (request == null)
			{
				return BadRequest(request);
			}
			var result = _advertisementDataService.CreateAdvertisementAsync(request).Result;

			return CreatedAtAction(nameof(Create), result);
		}

		[HttpPut]
		public IActionResult Update(AdvertisementPutRequest request)
		{
			if (request == null)
			{
				return BadRequest(request);
			}
			if (request.Failed)
			{
				return NotFound(request);
			}

			var result = _advertisementDataService.UpdateAdvertisementAsync(request).Result;

			return Ok(result);
		}

		[HttpGet("{adId}")]
		public IActionResult Get(int adId)
		{
			var result = _advertisementDataService.GetAdvertisementByIdAsync(adId).Result;

			if (result.Failed)
			{
				return NotFound(adId);
			}

			return Ok(result);
		}

		[HttpGet()]
		public IActionResult GetAll()
		{
			var result = _advertisementDataService.GetAllAdvertisementsAsync().Result;

			return Ok(result);
		}

		[HttpDelete("{adId}")]
		public IActionResult Delete(int adId)
		{
			var result = _advertisementDataService.DeleteAdvertisementByIdAsync(adId).Result;

			if (result == -1)
			{
				return NotFound(adId);
			}

			return Ok(result);
		}
	}
}
