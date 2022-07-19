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
	public class AdvertisementManipulationTests
	{

		[Fact]
		public async Task Create_advertisement_successfully()
		{
			// Arrange
			using (var fixture = new InMemoryDatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var loggerMock = new Mock<ILogger<AdvertisementDataService>>();
				var adDataService = new AdvertisementDataService(dbContext, loggerMock.Object);
				var sut = new AdsController(adDataService);
				var postRequest = new CreateAdvertisementRequest()
				{
					Title = "Fresh Drink",
					Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book",
					PictureBytes = adDataService.UrlToImageBytes("https://www.bulmag.org/files/products-v2/2/review/44d89b29f34023e5b36029b7e6ab4273.jpg").GetAwaiter().GetResult(),
				};
				var adsClient = new AdsClient()
				{
					Name = "Coca cola",
					Website = "https://coca-cola.com",
					ApiKey = Guid.NewGuid()
				};
				dbContext.AdsClients.Add(adsClient);
				await dbContext.SaveChangesAsync();

				// Act
				var result = await sut.Create(postRequest, adsClient.ApiKey) as CreatedResult;


				// Assert
				var adToAssert = await dbContext.Advertisements.AsNoTracking().SingleAsync(a => a.Id == (int)result!.Value!);
				adToAssert.Should().NotBeNull();
				adToAssert.Description.Should().Be(postRequest.Description);
				adToAssert.Title.Should().Be(postRequest.Title);
				adToAssert.AdsClientId.Should().Be(adsClient.Id);
			}
		}

		[Fact]
		public async Task Editing_existing_ad_successfully()
		{
			// Arrange
			using (var fixture = new InMemoryDatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var loggerMock = new Mock<ILogger<AdvertisementDataService>>();
				var adDataService = new AdvertisementDataService(dbContext, loggerMock.Object);
				var sut = new AdsController(adDataService);
				var postRequest = new CreateAdvertisementRequest()
				{
					Title = "Fresh Drink",
					Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book",
					PictureBytes = adDataService.UrlToImageBytes("https://www.bulmag.org/files/products-v2/2/review/44d89b29f34023e5b36029b7e6ab4273.jpg").GetAwaiter().GetResult(),
				};
				var adsClient = new AdsClient()
				{
					Name = "Coca cola",
					Website = "https://coca-cola.com",
					ApiKey = Guid.NewGuid()
				};
				dbContext.AdsClients.Add(adsClient);
				await dbContext.SaveChangesAsync();

				var adId = await adDataService.CreateAdvertisementAsync(postRequest, adsClient.ApiKey);

				var putRequest = new UpdateAdvertisementRequest()
				{
					Id = adId,
					Title = "Coca colaaaaa",
					Description = "fresh cola"
				};

				// Act
				var result = await sut.Update(putRequest, adsClient.ApiKey) as OkObjectResult;

				// Assert
				var adToAssert = await dbContext.Advertisements.AsNoTracking().SingleAsync(a => a.Id == (int)result!.Value!);
				adToAssert.Should().NotBeNull().And
					.NotBeEquivalentTo(postRequest);
				adToAssert.Title.Should().Be(putRequest.Title);
				adToAssert.Description.Should().Be(putRequest.Description);
				adToAssert.PictureBytes.Should().Be(postRequest.PictureBytes);

			}
		}

		[Fact]
		public async Task Get_all_advertisements_successfully()
		{
			// Arrange
			using (var fixture = new InMemoryDatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var loggerMock = new Mock<ILogger<AdvertisementDataService>>();
				var adDataService = new AdvertisementDataService(dbContext, loggerMock.Object);
				var sut = new AdsController(adDataService);
				await dbContext.SaveChangesAsync();
				var postRequest = new CreateAdvertisementRequest()
				{
					Title = "Fresh Drink",
					Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book",
					PictureBytes = adDataService.UrlToImageBytes("https://www.bulmag.org/files/products-v2/2/review/44d89b29f34023e5b36029b7e6ab4273.jpg").GetAwaiter().GetResult(),
				};
				var adsClient = new AdsClient()
				{
					Name = "Coca cola",
					Website = "https://coca-cola.com",
					ApiKey = Guid.NewGuid()
				};
				adsClient.Advertisements = new List<Advertisement>();
				var newlyCreatedAd = postRequest.Adapt<Advertisement>();
				adsClient.Advertisements.Add(newlyCreatedAd);
				adsClient.Advertisements.Add(new Advertisement()
				{
					Title = "test",
					Description = "test",
					PictureBytes = "none",
					AdsClientId = adsClient.Id,
				});


				dbContext.AdsClients.Add(adsClient);
				await dbContext.SaveChangesAsync();

				// Act
				var ads = await sut.GetAll(adsClient.ApiKey) as OkObjectResult;

				// Assert
				ads!.Value.Should().NotBeNull();
				ads!.Value.Should().BeEquivalentTo(adsClient.Advertisements.Adapt<List<AdvertisementDTO>>());
			}
		}
	}
}
