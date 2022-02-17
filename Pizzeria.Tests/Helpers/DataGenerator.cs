using Faker;
using System.Collections.Generic;
using WA.Pizza.Core.Models;

namespace Pizzeria.Tests.Helpers
{
    public static class DataGenerator
    {
        public static List<CatalogItem> GenerateCatalogItems(int itemsAmount, int storageQuantity)
        {
            var resultList = new List<CatalogItem>();

            for (int i = 0; i < itemsAmount; i++)
            {
                resultList.Add(new CatalogItem
                {
                    Name = Lorem.Sentence(5),
                    Price = RandomNumber.Next(1, int.MaxValue),
                    PictureBytes = Lorem.Sentence(5),
                    StorageQuantity = storageQuantity
                });
            }

            return resultList;
        }

    }
}
