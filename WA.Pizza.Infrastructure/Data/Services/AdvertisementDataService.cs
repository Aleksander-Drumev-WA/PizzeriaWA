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
	public class AdvertisementDataService
	{
		private readonly AppDbContext _dbContext;

		public AdvertisementDataService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<string> UrlToImageBytes(string url)
		{
			var httpClient = new HttpClient();

			var bytes = await httpClient.GetByteArrayAsync(url);

			var imageBytesAsString = Convert.ToBase64String(bytes);

			return imageBytesAsString;
		}

		public async Task<int> CreateAdvertisementAsync(AdvertisementPostRequest adRequest)
		{
			var ad = adRequest.Adapt(new Advertisement());

			_dbContext.Advertisements.Add(ad);
			await _dbContext.SaveChangesAsync();

			return ad.Id;
		}

		public async Task<int> UpdateAdvertisementAsync(AdvertisementPutRequest adRequest)
		{
			var ad = await _dbContext
				.Advertisements
				.FirstOrDefaultAsync(a => a.Id == adRequest.Id);

			if (ad == null)
			{
				adRequest.Failed = true;
				return -1;
			}

			adRequest.Adapt(ad);

			await _dbContext.SaveChangesAsync();

			return ad.Id;
		}

		public async Task<AdvertisementDTO> GetAdvertisementByIdAsync(int adId)
		{
			var ad = await _dbContext
				.Advertisements
				.FirstOrDefaultAsync(a => a.Id == adId);

			var adDTO = new AdvertisementDTO();

			if (ad == null)
			{
				adDTO.Failed = true;
				return adDTO;
			}

			ad.Adapt(adDTO);

			return adDTO;
		}

		public Task<List<AdvertisementDTO>> GetAllAdvertisementsAsync()
		{
			IQueryable<Advertisement> ads = _dbContext.Advertisements;

			return ads.ProjectToType<AdvertisementDTO>().ToListAsync();
		}

		public async Task<int> DeleteAdvertisementByIdAsync(int adId)
		{
			var ad = await _dbContext
				.Advertisements
				.FirstOrDefaultAsync(a => a.Id == adId);

			if (ad == null)
			{
				return -1;
			}

			var removedEntityId = _dbContext.Advertisements.Remove(ad).Entity.Id;
			await _dbContext.SaveChangesAsync();

			return removedEntityId;
		}
	}
}
