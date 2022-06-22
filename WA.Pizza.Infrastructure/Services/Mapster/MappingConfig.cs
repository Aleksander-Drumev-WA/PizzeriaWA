using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.Advertisement;
using WA.Pizza.Infrastructure.DTO.Basket;
using WA.Pizza.Infrastructure.DTO.Catalog;
using WA.Pizza.Infrastructure.DTO.Orders;

namespace WA.Pizza.Infrastructure.Services.Mapster
{
	public static class MappingConfig
	{
		public static void Configure()
		{
			TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
			TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;

			// Catalog Items
			TypeAdapterConfig<CatalogItemDTO, CatalogItem>
				.NewConfig()
				.Ignore(dest => dest.Id)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Price, src => src.Price.ToString("0.00"))
				.Map(dest => dest.PictureBytes, src => src.PictureBytes)
				.Map(dest => dest.StorageQuantity, src => src.StorageQuantity);

			TypeAdapterConfig<CatalogItem, ListCatalogItemsDTO>
				.NewConfig()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Price, src => src.Price.ToString("0.00"))
				.Map(dest => dest.PictureBytes, src => src.PictureBytes)
				.Map(dest => dest.StorageQuantity, src => src.StorageQuantity);

			// Basket
			TypeAdapterConfig<Basket, BasketDTO>
				.NewConfig()
				.Map(dest => dest.UserName, src => src.User.UserName)
				.Map(dest => dest.BasketItems, src => src.BasketItems);

			TypeAdapterConfig<BasketItem, BasketItemDTO>
				.NewConfig()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.BasketId, src => src.BasketId)
				.Map(dest => dest.Quantity, src => src.Quantity)
				.Map(dest => dest.Name, src => src.CatalogItem.Name)
				.Map(dest => dest.Price, src => src.CatalogItem.Price.ToString("0.00"))
				.Map(dest => dest.PictureBytes, src => src.CatalogItem.PictureBytes);

			TypeAdapterConfig<CatalogItemToBasketItemRequest, BasketItem>
				.NewConfig()
				// error wanted theses properties mapped or ignored
				.Ignore(dest => dest.Id)
				.Ignore(dest => dest.CatalogItem)
				.Ignore(dest => dest.Basket)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Price, src => src.Price)
				.Map(dest => dest.BasketId, src => src.BasketId)
				.Map(dest => dest.CatalogItemId, src => src.CatalogItemId)
				.Map(dest => dest.Quantity, src => src.Quantity)
				.Map(dest => dest.Basket.UserId, src => src.UserId);

			TypeAdapterConfig<BasketItemDTO, BasketItem>
				.NewConfig()
				.Ignore(dest => dest.CatalogItem)
				.Ignore(dest => dest.Basket)
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.CatalogItemId, src => src.CatalogItemId)
				.Map(dest => dest.BasketId, src => src.BasketId)
				.Map(dest => dest.Quantity, src => src.Quantity)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Price, src => src.Price);
			


			// Orders
			TypeAdapterConfig<Order, ListOrdersDTO>
				.NewConfig()
				.Map(dest => dest.OrderItems, src => src.OrderItems)
				.Map(dest => dest.Total, src => src.Total.ToString("0.00"))
				.Map(dest => dest.Status, src => src.OrderStatus.ToString());

			TypeAdapterConfig<Order, OrderDTO>
				.NewConfig()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Total, src => src.Total.ToString("0.00"))
				.Map(dest => dest.Status, src => src.OrderStatus.ToString())
				.Map(dest => dest.OrderItems, src => src.OrderItems);

			// Advertisements
			TypeAdapterConfig<AdvertisementPostRequest, Advertisement>
				.NewConfig()
				.Ignore(src => src.Id)
				.Map(dest => dest.Advertiser, src => src.Advertiser)
				.Map(dest => dest.AdvertiserUrl, src => src.AdvertiserUrl)
				.Map(dest => dest.PictureBytes, src => src.PictureBytes)
				.Map(dest => dest.Title, src => src.Title)
				.Map(dest => dest.Description, src => src.Description);

			TypeAdapterConfig<AdvertisementPutRequest, Advertisement>
				.NewConfig()
				.Ignore(src => src.Id)
				.IgnoreNullValues(true)
				.Map(dest => dest.Advertiser, src => src.Advertiser)
				.Map(dest => dest.AdvertiserUrl, src => src.AdvertiserUrl)
				.Map(dest => dest.PictureBytes, src => src.PictureBytes)
				.Map(dest => dest.Title, src => src.Title)
				.Map(dest => dest.Description, src => src.Description);

			TypeAdapterConfig<Advertisement, AdvertisementDTO>
				.NewConfig()
				.Ignore(dest => dest.Failed)
				.Map(dest => dest.Advertiser, src => src.Advertiser)
				.Map(dest => dest.AdvertiserUrl, src => src.AdvertiserUrl)
				.Map(dest => dest.PictureBytes, src => src.PictureBytes)
				.Map(dest => dest.Title, src => src.Title)
				.Map(dest => dest.Description, src => src.Description);
		}
	}
}
