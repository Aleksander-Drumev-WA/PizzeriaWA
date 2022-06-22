using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.Advertisement;
using WA.Pizza.Infrastructure.Data;
using Pizzeria.Tests.Fixtures;
using FluentAssertions;

namespace Pizzeria.Tests
{
	[Collection("Database collection")]
	public class AdvertisementDataServiceTests
	{
		private readonly AppDbContext _dbContext;
		private readonly AdvertisementDataService _sut;
		private readonly AdvertisementPostRequest _postRequest;

		public AdvertisementDataServiceTests(DatabaseFixture databaseFixture)
		{
			_dbContext = databaseFixture.DbContext;
			_sut = new AdvertisementDataService(_dbContext);
			_postRequest = new AdvertisementPostRequest()
			{
				Advertiser = "Coca cola",
				AdvertiserUrl = "example1.com",
				Title = "Fresh Drink",
				Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book",
				PictureBytes = _sut.UrlToImageBytes("https://www.bulmag.org/files/products-v2/2/review/44d89b29f34023e5b36029b7e6ab4273.jpg").GetAwaiter().GetResult(),
			};

		}

		[Fact]
		public async Task Create_advertisement_successfully()
		{
			// Arrange

			// Act
			var adId = await _sut.CreateAdvertisementAsync(_postRequest);

			// Assert
			var adToAssert = await _dbContext.Advertisements.FindAsync(adId);
			adToAssert.Should().NotBeNull().And
				.BeEquivalentTo(_postRequest);
		}

		[Fact]
		public async Task Editing_existing_ad_successfully()
		{
			// Arrange
			var adId = await _sut.CreateAdvertisementAsync(_postRequest);

			var putRequest = new AdvertisementPutRequest()
			{
				Id = adId,
				Advertiser = "Coca colaaaaa",
				AdvertiserUrl = "example1.commmm"
			};

			// Act
			await _sut.UpdateAdvertisementAsync(putRequest);

			// Assert
			var adToAssert = await _dbContext.Advertisements.FindAsync(adId);
			adToAssert.Should().NotBeNull().And
				.NotBeEquivalentTo(_postRequest);
			adToAssert!.Advertiser.Should().Be(putRequest.Advertiser);
			adToAssert!.AdvertiserUrl.Should().Be(putRequest.AdvertiserUrl);
		}

		[Fact]
		public async Task Editing_unexisting_ad_fails()
		{
			// Arrange
			var adId = 1;

			var putRequest = new AdvertisementPutRequest()
			{
				Id = adId,
				Advertiser = "Coca colaaaaa",
				AdvertiserUrl = "example1.commmm"
			};

			// Act
			var result = await _sut.UpdateAdvertisementAsync(putRequest);

			// Assert
			result.Should().Be(-1);
			putRequest.Failed.Should().BeTrue();
		}

		[Fact]
		public async Task Get_valid_advertisement_successfully()
		{
			// Arrange
			var adId = await _sut.CreateAdvertisementAsync(_postRequest);

			// Act
			var ad = await _sut.GetAdvertisementByIdAsync(adId);

			// Assert
			ad.Should().NotBeNull().And.BeEquivalentTo(_postRequest);
		}

		[Fact]
		public async Task Get_unexisting_advertisement_fails()
		{
			// Arrange
			var adId = 1;

			// Act
			var ad = await _sut.GetAdvertisementByIdAsync(adId);

			// Assert
			ad.Failed.Should().BeTrue();
			ad.Advertiser.Should().BeNull();
			ad.AdvertiserUrl.Should().BeNull();
			ad.PictureBytes.Should().BeNull();
			ad.Title.Should().BeNull();
			ad.Description.Should().BeNull();
		}

		[Fact]
		public async Task Get_all_advertisements_successfully()
		{
			// Arrange
			await _sut.CreateAdvertisementAsync(_postRequest);
			await _sut.CreateAdvertisementAsync(_postRequest);
			await _sut.CreateAdvertisementAsync(_postRequest);

			// Act
			var ads = await _sut.GetAllAdvertisementsAsync();

			// Assert
			ads.Should().NotBeNull().And
				.NotBeEmpty().And
				.HaveCount(3);
		}

		[Fact]
		public async Task Delete_advertisement_successfully()
		{
			// Arrange
			var adId = await _sut.CreateAdvertisementAsync(_postRequest);

			// Act
			var removedAdId = await _sut.DeleteAdvertisementByIdAsync(adId);

			// Assert
			var adToAssert = await _dbContext.Advertisements.FindAsync(removedAdId);
			adToAssert.Should().BeNull();
		}

		[Fact]
		public async Task Delete_unexisting_advertisement_fails()
		{
			// Arrange
			var adId = 1;

			// Act
			var deletedEntityId = await _sut.DeleteAdvertisementByIdAsync(adId);

			// Assert
			var deletedEntity = await _dbContext.Advertisements.FindAsync(deletedEntityId);
			deletedEntity.Should().BeNull();
			deletedEntityId.Should().Be(-1);
		}
	}
}
