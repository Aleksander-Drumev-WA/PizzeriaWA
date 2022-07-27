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
	[Collection("Database collection")]
	public class AdsClientTests
	{
		private readonly DatabaseFixture _fixture;
		private readonly AppDbContext _dbContext;
		private readonly AdsClientDataService _adsClientDataService;
		private readonly AdsClientController _sut;
		private readonly AdsClient _adsClient;

		public AdsClientTests(DatabaseFixture fixture)
		{
			_fixture = fixture;
			_dbContext = fixture.DbContext;
			_adsClientDataService = new AdsClientDataService(_dbContext);
			_sut = new AdsClientController(_adsClientDataService);
			_adsClient = new AdsClient()
			{
				Name = "Coca cola",
				Website = "https://coca-cola.com",
				ApiKey = Guid.NewGuid()
			};
		}


		[Fact]
		public async Task Get_all_clients_successfully()
		{
			// Arrange
			var dbClientRows = _dbContext.AdsClients.Count();

			var newClients = new List<AdsClient>
			{
				new AdsClient
				{
					Name = "Fanta",
					Website = "https://example.com",
					ApiKey = Guid.NewGuid()
				},
				_adsClient
			};

			_dbContext.AdsClients.AddRange(newClients);
			_dbContext.SaveChanges();
			dbClientRows += newClients.Count;

			// Act
			var clients = await _sut.GetAllClients();

			// Assert
			clients.Should().NotBeNull();
			clients.Should().HaveCount(dbClientRows);
			for (int i = clients.Count - newClients.Count; i < clients.Count - 1; i++)
			{
				clients[i].Id.Should().Be(newClients[i - newClients.Count].Id);
				clients[i].Name.Should().Be(newClients[i - newClients.Count].Name);
				clients[i].Website.Should().Be(newClients[i - newClients.Count].Website);
				clients[i].ApiKey.Should().Be(newClients[i - newClients.Count].ApiKey);
			}

		}

		[Fact]
		public async Task Get_client_with_its_advertisements()
		{
			// Arrange
			var newClients = new List<AdsClient>
				{
					new AdsClient
					{
						Name = "Fanta",
						Website = "https://example.com",
						ApiKey = Guid.NewGuid()
					},
					_adsClient
				};

			_dbContext.AdsClients.AddRange(newClients);


			var chosenClient = newClients.First();
			var loggerMock = new Mock<ILogger<AdvertisementDataService>>();
			var adDataService = new AdvertisementDataService(_dbContext, loggerMock.Object);
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
			_dbContext.SaveChanges();


			// Act
			var storedClient = await _sut.GetClient(chosenClient.Id);

			// Assert

			storedClient.Should().NotBeNull();
			storedClient.Should().BeEquivalentTo(chosenClient, options => options.ExcludingMissingMembers());


		}

		[Fact]
		public async Task Create_client_successfully()
		{
			// Arrange
			var request = new CreateAdsClientRequest
			{
				Name = "some name",
				Website = "www.example.com"
			};

			// Act
			var newlyCreatedClientGuid = await _sut.AddClient(request);

			// Assert
			var clientToAssert = await _dbContext.AdsClients.AsNoTracking().SingleAsync(ac => ac.ApiKey == newlyCreatedClientGuid);
			clientToAssert.Should().NotBeNull();
			clientToAssert.ApiKey.Should().Be(newlyCreatedClientGuid);
			clientToAssert.Should().BeEquivalentTo(request);


		}

		[Fact]
		public async Task Edit_existing_client_successfully()
		{
			// Arrange
			_dbContext.AdsClients.Add(_adsClient);
			_dbContext.SaveChanges();

			var putRequest = new UpdateAdsClientRequest()
			{
				Id = _adsClient.Id,
				Name = "test",
				ApiKey = Guid.NewGuid(),
				Website = "www.example.com"
			};

			// Act
			var newlyUpdatedClientGuid = await _sut.UpdateClient(putRequest);

			// Assert
			var clientToAssert = await _dbContext.AdsClients.AsNoTracking().SingleAsync(ac => ac.ApiKey == newlyUpdatedClientGuid);
			clientToAssert.Should().NotBeNull().And
				.BeEquivalentTo(putRequest);


		}

		[Fact]
		public async Task Delete_existing_client_successfully()
		{
			// Arrange
			_dbContext.AdsClients.Add(_adsClient);
			_dbContext.SaveChanges();
			_dbContext.ChangeTracker.Clear();

			// Act
			await _sut.DeleteClient(_adsClient.Id);

			// Assert
			var clientToAssert = await _dbContext.AdsClients.FirstOrDefaultAsync(ac => ac.Id == _adsClient.Id);
			clientToAssert.Should().BeNull();


		}
	}
}
