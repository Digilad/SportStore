using SportStore.Controllers;
using SportStore.Models;
using SportStore.Models.ViewModels;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System;
using Xunit;

namespace SportStore.Tests
{
    public class ProductControllerTests
    {
        private static IProductRepository CreateData()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductId = 1, Name = "P1"},
                new Product{ProductId = 2, Name = "P2"},
                new Product{ProductId = 3, Name = "P3"},
                new Product{ProductId = 4, Name = "P4"},
                new Product{ProductId = 5, Name = "P5"},
            }.AsQueryable());
            return mock.Object;
        }

        [Fact]
        public void CanPaginate()
        {
            ProductController controller = new ProductController(CreateData()) { PageSize = 3 };
            ProductsListViewModel result = controller.List(2).ViewData.Model as ProductsListViewModel;

            Product[] prodArray = result.Products.ToArray();

            Assert.True(prodArray.Length == 2);
            Assert.Equal(4, prodArray[0].ProductId);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            ProductController controller = new ProductController(CreateData()) { PageSize = 3 };
            ProductsListViewModel result = controller.List(2).ViewData.Model as ProductsListViewModel;

            PagingInfo pagingInfo = result.PagingInfo;

            Assert.Equal(2, pagingInfo.CurrentPage);
            Assert.Equal(3, pagingInfo.ItemsPerPage);
            Assert.Equal(5, pagingInfo.TotalItems);
            Assert.Equal(2, pagingInfo.TotalPages);
        }
    }
}