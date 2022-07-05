using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WA.Pizza.Core.Exceptions;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Advertisement;
using WA.Pizza.Web.Filters;

using static WA.Pizza.Core.ConstantValues;

namespace WA.Pizza.Web.Controllers
{
	[Authorize(Roles = UserRoles.ADMIN_ROLE_NAME)]
	public class AdsController : BaseController
	{
		private readonly AdvertisementDataService _advertisementDataService;

		public AdsController(AdvertisementDataService advertisementDataService)
		{
			_advertisementDataService = advertisementDataService;
		}

		[HttpPost]
		[SwaggerOperation(Summary = "Creates new advertisement")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Malformed Request")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "Provided API key is not a valid key")]
		[SwaggerResponse(StatusCodes.Status201Created, "New advertisement created")]
		[ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
		public async Task<IActionResult> Create(
			CreateAdvertisementRequest request,
			[Required][FromHeader] Guid apiKey)
		{

			if (!await _advertisementDataService.ApiKeyIsValid(apiKey))
			{
				return Unauthorized(new { message = "Provided API key is not a valid key." });
			}

			if (request == null)
			{
				return BadRequest(request);
			}
			var id = await _advertisementDataService.CreateAdvertisementAsync(request, apiKey);

			return Created($"~/api/Ads/{id}", id);
		}

		[HttpPut]
		[SwaggerOperation(Summary = "Updates existing advertisement")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Malformed Request")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "Provided API key is not a valid key")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Advertisement not found")]
		[SwaggerResponse(StatusCodes.Status200OK, "Advertisement updated")]
		[ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
		public async Task<IActionResult> Update(
			UpdateAdvertisementRequest request,
			[Required][FromHeader] Guid apiKey)
		{
			if (!await _advertisementDataService.ApiKeyIsValid(apiKey))
			{
				return Unauthorized(new { message = "Provided API key is not a valid key." });
			}

			if (request == null)
			{
				return BadRequest(request);
			}

			int id;
			try
			{
				id = await _advertisementDataService.UpdateAdvertisementAsync(request);

			}
			catch (ItemNotFoundException ex)
			{
				return NotFound(ex.ToString());
			}

			return Ok(id);
		}

		[HttpGet]
		[SwaggerOperation(Summary = "Returns all advertisements of client with given API key")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Malformed API key")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "Provided API key is not a valid key")]
		[SwaggerResponse(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(List<AdvertisementDTO>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll([Required][FromHeader] Guid apiKey)
		{
			if (!await _advertisementDataService.ApiKeyIsValid(apiKey))
			{
				return Unauthorized(new { message = "Provided API key is not a valid key." });
			}

			var result = await _advertisementDataService.GetAllAdvertisementsAsync(apiKey);

			return Ok(result);
		}
	}
}
