using SportStore.Controllers;
using SportStore.Models;
using SportStore.Models.ViewModels;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace SportStore.Tests
{
    public class ProductControllerTests
    {
        private static IProductRepository CreateData()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductId = 1, Name = "P1", Category = "Cat1"},
                new Product{ProductId = 2, Name = "P2", Category = "Cat2"},
                new Product{ProductId = 3, Name = "P3", Category = "Cat1"},
                new Product{ProductId = 4, Name = "P4", Category = "Cat2"},
                new Product{ProductId = 5, Name = "P5", Category = "Cat3"},
            }.AsQueryable());
            return mock.Object;
        }

        [Fact]
        public void CanPaginate()
        {
            ProductController controller = new ProductController(CreateData()) { PageSize = 3 };
            ProductsListViewModel result = controller.List(null, 2).ViewData.Model as ProductsListViewModel;

            Product[] prodArray = result.Products.ToArray();

            Assert.True(prodArray.Length == 2);
            Assert.Equal(4, prodArray[0].ProductId);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            ProductController controller = new ProductController(CreateData()) { PageSize = 3 };
            ProductsListViewModel result = controller.List(null, 2).ViewData.Model as ProductsListViewModel;

            PagingInfo pagingInfo = result.PagingInfo;

            Assert.Equal(2, pagingInfo.CurrentPage);
            Assert.Equal(3, pagingInfo.ItemsPerPage);
            Assert.Equal(5, pagingInfo.TotalItems);
            Assert.Equal(2, pagingInfo.TotalPages);
        }

        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            ProductController controller = new ProductController(CreateData()) { PageSize = 3 };
            Func<ViewResult, ProductsListViewModel> GetModel = result => result?.ViewData.Model as ProductsListViewModel;

            int? res1 = GetModel(controller.List("Cat1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel(controller.List("Cat2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel(controller.List("Cat3"))?.PagingInfo.TotalItems;
            int? res4 = GetModel(controller.List("Cat4"))?.PagingInfo.TotalItems;
            int? resAll = GetModel(controller.List(null))?.PagingInfo.TotalItems;

            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(0, res4);
            Assert.Equal(5, resAll);
        }
    }
}