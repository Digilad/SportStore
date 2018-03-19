using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SportStore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SportStore
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SportStoreDB")));
            services.AddTransient<IProductRepository, EEProductRepository>();
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IOrderRepository, EFOrderRepository>();
            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "categoryPage",
                    template: "{category}/Page{page:int}",
                    defaults: new { Controller = "Product", Action = "List" });
                routes.MapRoute(
                    name: "page",
                    template: "Page{page:int}",
                    defaults: new { Controller = "Product", Action = "List", page = 1 });
                routes.MapRoute(
                    name: "category",
                    template: "{category}",
                    defaults: new { Controller = "Product", Action = "List", page = 1 });
                routes.MapRoute(
                    name: "nothing",
                    template: "",
                    defaults: new { Controller = "Product", Action = "List", page = 1 });
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });
            SeedData.EnsurePopulated(app);
        }
    }
}
