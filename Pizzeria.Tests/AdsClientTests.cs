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

namespace Pizzeria.Tests
{
	[Collection("In-Memory Database Collection")]
	public class AdsClientTests
	{
		private readonly AppDbContext _dbContext;
		private readonly AdsClient _adsClient;
		private readonly CreateAdvertisementRequest _postRequest;
		private readonly AdsClientDataService _adsClientDataService;
		private readonly AdvertisementDataService _adDataService;
		private readonly AdsClientController _sut;



		public AdsClientTests(InMemoryDatabaseFixture fixture)
		{
			_dbContext = fixture.DbContext;
			_adsClient = new AdsClient()
			{
				Name = "Coca cola",
				Website = "https://coca-cola.com",
				ApiKey = Guid.NewGuid()
			};
			_adsClientDataService = new AdsClientDataService(_dbContext);
			var loggerMock = new Mock<ILogger<AdvertisementDataService>>();
			_adDataService = new AdvertisementDataService(_dbContext, loggerMock.Object);
			_sut = new AdsClientController(_adsClientDataService);
			_postRequest = new CreateAdvertisementRequest()
			{
				Title = "Fresh Drink",
				Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book",
				PictureBytes = _adDataService.UrlToImageBytes("https://www.bulmag.org/files/products-v2/2/review/44d89b29f34023e5b36029b7e6ab4273.jpg").GetAwaiter().GetResult(),
			};
		}

		[Fact]
		public async Task Get_all_clients_successfully()
		{
			// Arrange
			var newClients = new List<AdsClient>
			{
				_adsClient,
				new AdsClient
				{
					Name = "Fanta",
					Website = "https://example.com",
					ApiKey = Guid.NewGuid()
				}
			};

			_dbContext.AdsClients.AddRange(newClients);
			await _dbContext.SaveChangesAsync();

			// Act
			var clients = await _sut.GetAllClients();

			// Assert

			clients.Should().NotBeNull();
			clients.Should().HaveCount(2);
			clients.Should().BeEquivalentTo(newClients, options => options.ExcludingMissingMembers());
		}

		[Fact]
		public async Task Get_client_with_its_advertisements()
		{
			// Arrange
			var newClients = new List<AdsClient>
			{
				_adsClient,
				new AdsClient
				{
					Name = "Fanta",
					Website = "https://example.com",
					ApiKey = Guid.NewGuid()
				}
			};

			_dbContext.AdsClients.AddRange(newClients);


			var chosenClient = newClients.First();
			chosenClient.Advertisements.AddRange(new List<Advertisement>
			{
				_postRequest.Adapt<Advertisement>(),
				new Advertisement
				{
					Title = "something",
					Description = "Lorem ipsum",
					PictureBytes = "none",
					AdsClientId = chosenClient.Id
				}
			});
			await _dbContext.SaveChangesAsync();


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
			await _dbContext.SaveChangesAsync();

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
			await _dbContext.SaveChangesAsync();
			_dbContext.ChangeTracker.Clear();

			// Act
			await _sut.DeleteClient(_adsClient.Id);

			// Assert
			var clientToAssert = await _dbContext.AdsClients.FirstOrDefaultAsync(ac => ac.Id == _adsClient.Id);
			clientToAssert.Should().BeNull();
		}
	}
}
