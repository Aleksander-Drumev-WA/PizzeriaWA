using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Advertisement;
using WA.Pizza.Web.Controllers;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Pizzeria.Tests
{
	public class AdsClientTests
	{

		[Fact]
		public async Task Get_all_clients_successfully()
		{
			// Arrange
			using (var fixture = new DatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var adsClientDataService = new AdsClientDataService(dbContext);
				var sut = new AdsClientController(adsClientDataService);

				var newClients = new List<AdsClient>
				{
					new AdsClient
					{
						Name = "Fanta",
						Website = "https://example.com",
						ApiKey = Guid.NewGuid()
					},
					new AdsClient
					{
						Name = "Fanta2",
						Website = "https://example2.com",
						ApiKey = Guid.NewGuid()
					}
				};

				dbContext.AdsClients.AddRange(newClients);
				await dbContext.SaveChangesAsync();

				// Act
				var clients = await sut.GetAllClients();

				// Assert

				clients.Should().NotBeNull();
				clients.Should().HaveCount(2);
				clients.Should().BeEquivalentTo(newClients, options => options.ExcludingMissingMembers());
			}

		}

		[Fact]
		public async Task Get_client_with_its_advertisements()
		{
			// Arrange
			using (var fixture = new DatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var adsClientDataService = new AdsClientDataService(dbContext);
				var sut = new AdsClientController(adsClientDataService);
				var newClients = new List<AdsClient>
				{
					new AdsClient
					{
						Name = "Fanta",
						Website = "https://example.com",
						ApiKey = Guid.NewGuid()
					},
					new AdsClient
					{
						Name = "Fanta2",
						Website = "https://example2.com",
						ApiKey = Guid.NewGuid()
					}
				};

				dbContext.AdsClients.AddRange(newClients);


				var chosenClient = newClients.First();
				var loggerMock = new Mock<ILogger<AdvertisementDataService>>();
				var adDataService = new AdvertisementDataService(dbContext, loggerMock.Object);
				var postRequest = new CreateAdvertisementRequest()
				{
					Title = "Fresh Drink",
					Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book",
					PictureBytes = adDataService.UrlToImageBytes("https://www.bulmag.org/files/products-v2/2/review/44d89b29f34023e5b36029b7e6ab4273.jpg").GetAwaiter().GetResult(),
				};
				chosenClient.Advertisements.AddRange(new List<Advertisement>
				{
					postRequest.Adapt<Advertisement>(),
					new Advertisement
					{
						Title = "something",
						Description = "Lorem ipsum",
						PictureBytes = "none",
						AdsClientId = chosenClient.Id
					}
				});
				await dbContext.SaveChangesAsync();


				// Act
				var storedClient = await sut.GetClient(chosenClient.Id);

				// Assert

				storedClient.Should().NotBeNull();
				storedClient.Should().BeEquivalentTo(chosenClient, options => options.ExcludingMissingMembers());

			}
		}

		[Fact]
		public async Task Create_client_successfully()
		{
			// Arrange
			using (var fixture = new DatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var adsClientDataService = new AdsClientDataService(dbContext);
				var sut = new AdsClientController(adsClientDataService);


				var request = new CreateAdsClientRequest
				{
					Name = "some name",
					Website = "www.example.com"
				};

				// Act
				var newlyCreatedClientGuid = await sut.AddClient(request);

				// Assert
				var clientToAssert = await dbContext.AdsClients.AsNoTracking().SingleAsync(ac => ac.ApiKey == newlyCreatedClientGuid);
				clientToAssert.Should().NotBeNull();
				clientToAssert.ApiKey.Should().Be(newlyCreatedClientGuid);
				clientToAssert.Should().BeEquivalentTo(request);
			}
		}

		[Fact]
		public async Task Edit_existing_client_successfully()
		{
			// Arrange
			using (var fixture = new DatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var adsClientDataService = new AdsClientDataService(dbContext);
				var sut = new AdsClientController(adsClientDataService);
				var adsClient = new AdsClient()
				{
					Name = "Coca cola",
					Website = "https://coca-cola.com",
					ApiKey = Guid.NewGuid()
				};
				dbContext.AdsClients.Add(adsClient);
				await dbContext.SaveChangesAsync();

				var putRequest = new UpdateAdsClientRequest()
				{
					Id = adsClient.Id,
					Name = "test",
					ApiKey = Guid.NewGuid(),
					Website = "www.example.com"
				};

				// Act
				var newlyUpdatedClientGuid = await sut.UpdateClient(putRequest);

				// Assert
				var clientToAssert = await dbContext.AdsClients.AsNoTracking().SingleAsync(ac => ac.ApiKey == newlyUpdatedClientGuid);
				clientToAssert.Should().NotBeNull().And
					.BeEquivalentTo(putRequest);
			}
		}

		[Fact]
		public async Task Delete_existing_client_successfully()
		{
			// Arrange
			using (var fixture = new DatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var adsClientDataService = new AdsClientDataService(dbContext);
				var sut = new AdsClientController(adsClientDataService);
				var adsClient = new AdsClient()
				{
					Name = "Coca cola",
					Website = "https://coca-cola.com",
					ApiKey = Guid.NewGuid()
				};
				dbContext.AdsClients.Add(adsClient);
				await dbContext.SaveChangesAsync();
				dbContext.ChangeTracker.Clear();

				// Act
				await sut.DeleteClient(adsClient.Id);

				// Assert
				var clientToAssert = await dbContext.AdsClients.FirstOrDefaultAsync(ac => ac.Id == adsClient.Id);
				clientToAssert.Should().BeNull();
			}
		}
	}
}
