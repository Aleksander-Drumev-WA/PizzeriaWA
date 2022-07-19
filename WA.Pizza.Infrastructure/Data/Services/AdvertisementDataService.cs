using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Exceptions;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.Advertisement;

namespace WA.Pizza.Infrastructure.Data.Services
{
	public class AdvertisementDataService
	{
		private readonly AppDbContext _dbContext;
		private readonly ILogger<AdvertisementDataService> _logger;

		public AdvertisementDataService(AppDbContext dbContext, ILogger<AdvertisementDataService> logger)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		public async Task<string> UrlToImageBytes(string url)
		{
			var httpClient = new HttpClient();

			var bytes = await httpClient.GetByteArrayAsync(url);

			var imageBytesAsString = Convert.ToBase64String(bytes);

			return imageBytesAsString;
		}

		public async Task<int> CreateAdvertisementAsync(CreateAdvertisementRequest adRequest, Guid apiKey)
		{
			var ad = adRequest.Adapt<Advertisement>();

			var client = await _dbContext.AdsClients.FirstAsync(i => i.ApiKey == apiKey);
			ad.AdsClientId = client.Id;

			_dbContext.Advertisements.Add(ad);
			await _dbContext.SaveChangesAsync();

			return ad.Id;
		}

		public async Task<int> UpdateAdvertisementAsync(UpdateAdvertisementRequest adRequest)
		{
			var ad = await _dbContext
				.Advertisements
				.FirstOrDefaultAsync(a => a.Id == adRequest.Id);

			if (ad == null)
			{
				string errorMessage = $"No advertisements found with ID {adRequest.Id}.";
				_logger.LogError(errorMessage);
				throw new ItemNotFoundException(nameof(Advertisement));
			}

			adRequest.Adapt(ad);

			await _dbContext.SaveChangesAsync();

			return ad.Id;
		}

		public Task<List<AdvertisementDTO>> GetAllAdvertisementsAsync(Guid apiKey)
		{
			return _dbContext.Advertisements
				.AsNoTracking()
				.Where(a => a.AdsClient!.ApiKey == apiKey)
				.ProjectToType<AdvertisementDTO>()
				.ToListAsync();
		}

		public Task<bool> ApiKeyIsValid(Guid apiKey)
		{
			return _dbContext.AdsClients.AnyAsync(ac => ac.ApiKey == apiKey);
		}
	}
}
