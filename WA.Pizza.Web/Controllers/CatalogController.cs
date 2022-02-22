using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA.Pizza.Infrastructure.Data.Services;

namespace WA.Pizza.Web.Controllers
{
    public class CatalogController : BaseController
    {
        private readonly CatalogDataService _catalogDataService;

        public CatalogController(CatalogDataService catalogDataService)
        {
            _catalogDataService = catalogDataService;
        }

        [AllowAnonymous]
        [HttpGet]
        public  Task GetCatalog()
        {
            return _catalogDataService.GetAllAsync();
        }
    }
}
