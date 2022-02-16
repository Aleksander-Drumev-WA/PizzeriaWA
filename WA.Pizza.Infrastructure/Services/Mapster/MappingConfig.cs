using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
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
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Price, src => src.Price.ToString("0.00"))
				.Map(dest => dest.PictureBytes, src => src.PictureBytes)
				.Map(dest => dest.StorageQuantity, src => src.StorageQuantity);

			TypeAdapterConfig<CatalogItem, CatalogItemDTO>
				.NewConfig()
				.Ignore(dest => dest.StorageQuantity)
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Price, src => src.Price.ToString("0.00"))
				.Map(dest => dest.PictureBytes, src => src.PictureBytes);


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
				.Map(dest => dest.BasketId, src => src.BasketId)
				.Map(dest => dest.CatalogItemId, src => src.CatalogItemId)
				.Map(dest => dest.Quantity, src => src.Quantity);

			TypeAdapterConfig<BasketItemDTO, BasketItem>
				.NewConfig()
				.Ignore(dest => dest.Id)
				.Ignore(dest => dest.CatalogItem.Name)
				.Ignore(dest => dest.CatalogItem.Price)
				.Ignore(dest => dest.CatalogItem.PictureBytes)
				.Map(dest => dest.CatalogItemId, src => src.CatalogItemId)
				.Map(dest => dest.BasketId, src => src.BasketId)
				.Map(dest => dest.Quantity, src => src.Quantity);


			// Orders
			TypeAdapterConfig<Order, ListOrdersDTO>
				.NewConfig()
				.Map(dest => dest.Total, src => src.Total.ToString("0.00"))
				.Map(dest => dest.Status, src => src.OrderStatus.ToString())
				.Map(dest => dest.OrderItems.Name, src => src.OrderItems.Select(oi => oi.Name))
				.Map(dest => dest.OrderItems.Price, src => src.OrderItems.Select(oi => oi.Price.ToString("0.00")))
				.Map(dest => dest.OrderItems.Quantity, src => src.OrderItems.Select(oi => oi.Quantity));

			TypeAdapterConfig<Order, OrderDTO>
				.NewConfig()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Total, src => src.Total.ToString("0.00"))
				.Map(dest => dest.Status, src => src.OrderStatus.ToString())
				.Map(dest => dest.OrderItems, src => src.OrderItems);

			TypeAdapterConfig<OrderItem, OrderItemsDTO>
				.NewConfig()
				.Map(dest => dest.OrderId, src => src.OrderId)
				.Map(dest => dest.CatalogItemId, src => src.CatalogItemId)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Price, src => src.Price.ToString("0.00"))
				.Map(dest => dest.Quantity, src => src.Quantity);
		}
	}
}
