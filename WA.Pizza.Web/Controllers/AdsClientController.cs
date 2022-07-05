using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Advertisement;

using static WA.Pizza.Core.ConstantValues;


namespace WA.Pizza.Web.Controllers
{
	public class AdsClientController : BaseController
	{
		private readonly AdsClientDataService _adsClientDataService;

		public AdsClientController(AdsClientDataService adsClientDataService)
		{
			_adsClientDataService = adsClientDataService;
		}

        [HttpGet]
        [Authorize(Roles = UserRoles.ADMIN_ROLE_NAME)]
        [SwaggerOperation(Summary = "Returns list of all advertisement clients")]
        public Task<List<AdsClientGridDto>> GetAllClients()
        {
            return _adsClientDataService.GetAllClients();
        }

        [HttpGet("{clientId}")]
        [Authorize(Roles = UserRoles.ADMIN_ROLE_NAME)]
        [SwaggerOperation(Summary = "Returns advertisement client by ID")]
        public Task<AdsClientDto?> GetClient(int clientId)
        {
            return _adsClientDataService.GetClient(clientId);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new advertisement client")]
        public Task<Guid> AddClient(CreateAdsClientRequest createAdsClientRequest)
        {
            return _adsClientDataService.CreateClient(createAdsClientRequest);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Updates an existing advertisement client")]
        public Task<Guid> UpdateClient(UpdateAdsClientRequest updateAdsClientRequest)
        {
            return _adsClientDataService.UpdateClient(updateAdsClientRequest);
        }

        [HttpDelete("{clientId}")]
        [SwaggerOperation(Summary = "Removes the advertisement client")]
        public Task DeleteClient(int clientId)
        {
            return _adsClientDataService.DeleteClient(clientId);
        }
    }
}
