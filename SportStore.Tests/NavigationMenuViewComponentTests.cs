using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Linq;
using SportStore.Components;
using SportStore.Models;
using Xunit;
using Moq;

namespace SportStore.Tests
{
    public class NavigationMenuViewComponentTests
    {
        private static IProductRepository CreateData()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductId = 1, Name = "P1", Category = "Apples"},
                new Product{ProductId = 2, Name = "P2", Category = "Apples"},
                new Product{ProductId = 3, Name = "P3", Category = "Plums"},
                new Product{ProductId = 4, Name = "P4", Category = "Oranges"}
            }.AsQueryable());
            return mock.Object;
        }

        [Fact]
        public void Can_Select_Categories()
        {
            NavigationMenuViewComponent target = new NavigationMenuViewComponent(CreateData());

            string[] results = ((IEnumerable<string>)(target.Invoke() as ViewViewComponentResult).ViewData.Model).ToArray();

            Assert.True(Enumerable.SequenceEqual(new string[] { "Apples", "Oranges", "Plums" }, results));
        }

        [Fact]
        public void Indicates_Selected_Category()
        {
            string categoryToSelect = "Apples";
            NavigationMenuViewComponent target = new NavigationMenuViewComponent(CreateData());
            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext
                {
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                }
            };
            target.RouteData.Values["category"] = categoryToSelect;

            string result = (string)(target.Invoke() as ViewViewComponentResult).ViewData["SelectedCategory"];

            Assert.Equal(categoryToSelect, result);
        }
    }
}
