﻿namespace WA.Pizza.Infrastructure.DTO.Catalog
{
    public record ListCatalogItemsDTO
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public string PictureBytes { get; init; }

        public int StorageQuantity { get; init; }
    }
}
