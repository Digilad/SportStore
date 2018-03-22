using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SportStore.Controllers;
using SportStore.Models;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SportStore.Tests
{
    public class AdminControllerTests
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
        public void Index_Contains_All_Products()
        {
            AdminController target = new AdminController(CreateData());

            Product[] result = GetViewModel<IEnumerable<Product>>(target.Index())?.ToArray();
            Assert.Equal(5, result.Length);
            Assert.Equal("P4", result[3].Name);
        }

        [Fact]
        public void Can_Edit_Product()
        {
            AdminController target = new AdminController(CreateData());

            Product p1 = GetViewModel<Product>(target.Edit(1));
            Product p0 = GetViewModel<Product>(target.Edit(0));

            Assert.Equal(1, p1.ProductId);
            Assert.Null(p0);
        }

        [Fact]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            AdminController target = new AdminController(mock.Object) {
                TempData = tempData.Object
            };
            Product product = new Product { Name = "Test" };
            IActionResult result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(product));
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            target.ModelState.AddModelError("error", "error");
            IActionResult result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_Valid_Products()
        {
            Product prod = new Product { ProductId = 2, Name = "Test" };
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductId = 1, Name = "P1"},
                prod,
                new Product { ProductId = 3, Name = "P3"}
            }.AsQueryable());
            AdminController target = new AdminController(mock.Object);

            target.Delete(prod.ProductId);

            mock.Verify(m => m.DeleteProduct(prod.ProductId));
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }
    }
}
