using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Advertisement;
using WA.Pizza.Web.Controllers;
using Xunit;

namespace Pizzeria.Tests
{
	[Collection("Database collection")]
	public class AdvertisementManipulationTests
	{
		private readonly AppDbContext _dbContext;
		private readonly AdvertisementDataService _adDataService;
		private readonly CreateAdvertisementRequest _postRequest;
		private readonly AdsController _sut;
		private readonly AdsClient _adsClient;


		public AdvertisementManipulationTests(DatabaseFixture databaseFixture)
		{
			_dbContext = databaseFixture.DbContext;
			var loggerMock = new Mock<ILogger<AdvertisementDataService>>();
			_adDataService = new AdvertisementDataService(_dbContext, loggerMock.Object);
			_sut = new AdsController(_adDataService);
			_adsClient = new AdsClient()
			{
				Name = "Coca cola",
				Website = "https://coca-cola.com",
				ApiKey = Guid.NewGuid()
			};
			_postRequest = new CreateAdvertisementRequest()
			{
				Title = "Fresh Drink",
				Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book",
				PictureBytes = _adDataService.UrlToImageBytes("https://www.bulmag.org/files/products-v2/2/review/44d89b29f34023e5b36029b7e6ab4273.jpg").GetAwaiter().GetResult(),
			};

		}

		[Fact]
		public async Task Create_advertisement_successfully()
		{
			// Arrange
			_dbContext.AdsClients.Add(_adsClient);
			await _dbContext.SaveChangesAsync();

			// Act
			var result = await _sut.Create(_postRequest, _adsClient.ApiKey) as CreatedResult;

			// Assert
			var adToAssert = await _dbContext.Advertisements.AsNoTracking().SingleAsync(a => a.Id == (int)result!.Value!);
			adToAssert.Should().NotBeNull();
			adToAssert.Description.Should().Be(_postRequest.Description);
			adToAssert.Title.Should().Be(_postRequest.Title);
			adToAssert.AdsClientId.Should().Be(_adsClient.Id);
		}

		[Fact]
		public async Task Editing_existing_ad_successfully()
		{
			// Arrange
			_dbContext.AdsClients.Add(_adsClient);
			await _dbContext.SaveChangesAsync();

			var adId = await _adDataService.CreateAdvertisementAsync(_postRequest, _adsClient.ApiKey);

			var putRequest = new UpdateAdvertisementRequest()
			{
				Id = adId,
				Title = "Coca colaaaaa",
				Description = "fresh cola"
			};

			// Act
			var result = await _sut.Update(putRequest, _adsClient.ApiKey) as OkObjectResult;

			// Assert
			var adToAssert = await _dbContext.Advertisements.AsNoTracking().SingleAsync(a => a.Id == (int)result!.Value!);
			adToAssert.Should().NotBeNull().And
				.NotBeEquivalentTo(_postRequest);
			adToAssert.Title.Should().Be(putRequest.Title);
			adToAssert.Description.Should().Be(putRequest.Description);
			adToAssert.PictureBytes.Should().Be(_postRequest.PictureBytes);
		}

		[Fact]
		public async Task Get_all_advertisements_successfully()
		{
			// Arrange
			_adsClient.Advertisements = new List<Advertisement>();
			var newlyCreatedAd = _postRequest.Adapt<Advertisement>();
			_adsClient.Advertisements.Add(newlyCreatedAd);
			_adsClient.Advertisements.Add(new Advertisement()
			{
				Title = "test",
				Description = "test",
				PictureBytes = "none",
				AdsClientId = _adsClient.Id,
			});


			_dbContext.AdsClients.Add(_adsClient);
			await _dbContext.SaveChangesAsync();

			// Act
			var ads = await _sut.GetAll(_adsClient.ApiKey) as OkObjectResult;

			// Assert
			ads!.Value.Should().NotBeNull();
			ads!.Value.Should().BeEquivalentTo(_adsClient.Advertisements.Adapt<List<AdvertisementDTO>>());

		}
	}
}
