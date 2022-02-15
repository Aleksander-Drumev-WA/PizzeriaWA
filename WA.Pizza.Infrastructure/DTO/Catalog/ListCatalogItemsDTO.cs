﻿namespace WA.Pizza.Infrastructure.DTO.Catalog
{
    public record ListCatalogItemsDTO
    {
        public string Name { get; init; }

        public decimal Price { get; init; }

        public string PictureBytes { get; init; }

        public int StorageQuantity { get; init; }
    }
}
