using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.Advertisement;

namespace WA.Pizza.Infrastructure.Data.Services
{
	public class AdsClientDataService
	{
		private readonly AppDbContext _dbContext;

		public AdsClientDataService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

        public Task<List<AdsClientGridDto>> GetAllClients()
        {
            return _dbContext.AdsClients.AsNoTracking().ProjectToType<AdsClientGridDto>().ToListAsync();
        }

        public Task<AdsClientDto?> GetClient(int id)
        {
            return _dbContext.AdsClients
                .AsNoTracking()
                .Where(ac => ac.Id == id)
                .Include(ac => ac.Advertisements)
                .ProjectToType<AdsClientDto>()
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateClient(CreateAdsClientRequest createAdsClientRequest)
        {
            var adsClient = createAdsClientRequest.Adapt<AdsClient>();
            adsClient.ApiKey = Guid.NewGuid();
            _dbContext.AdsClients.Add(adsClient);
            await _dbContext.SaveChangesAsync();

            return adsClient.ApiKey;
        }

        public async Task<Guid> UpdateClient(UpdateAdsClientRequest updateAdsClientRequest)
        {
            var adsClient = await _dbContext.AdsClients.FirstAsync(client => client.Id == updateAdsClientRequest.Id);
            updateAdsClientRequest.Adapt(adsClient);
            await _dbContext.SaveChangesAsync();

            return adsClient.ApiKey;
        }

        public Task DeleteClient(int id)
        {
            _dbContext.AdsClients.Remove(new AdsClient() { Id = id });
            return _dbContext.SaveChangesAsync();
        }
    }
}
