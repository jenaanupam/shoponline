using ApplicationCore.Interfaces;
using ApplicationCore.Services;
//using Hangfire;
//using Hangfire.Dashboard;
//using Hangfire.MemoryStorage;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.RazorPages.Interfaces;
using Microsoft.eShopWeb.RazorPages.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
 
 


namespace Microsoft.eShopWeb.RazorPages
{
    public class Startup
    {
        private IServiceCollection _services;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // use in-memory database
            ConfigureTestingServices(services);

            // use real database
           //  ConfigureProductionServices(services);


        }
        public void ConfigureTestingServices(IServiceCollection services)
        {
            // use in-memory database
            services.AddDbContext<CatalogContext>(c =>
                c.UseInMemoryDatabase("Catalog"));

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            
            // use real database
            services.AddDbContext<CatalogContext>(c =>
            {
                try
                {    
                    c.UseMySQL(getconn());
                }
                catch (System.Exception ex)
                {
                    var message = ex.Message;
                }
            });
            //services.AddHangfire(c => c.UseMemoryStorage());
            // Add Identity DbContext
            //services.AddDbContext<AppIdentityDbContext>(c =>
            //     c.UseMySQL("server=localhost;port=3306;database=testentityfw;user=root;password=anupam;"));
            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.LoginPath = "/Account/Signin";
                options.LogoutPath = "/Account/Signout";
            });

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

            services.AddScoped<ICatalogService, CachedCatalogService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IBasketViewModelService, BasketViewModelService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<CatalogService>();
            services.Configure<CatalogSettings>(Configuration);
            services.AddSingleton<IUriComposer>(new UriComposer(Configuration.Get<CatalogSettings>()));

            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddTransient<IEmailSender, EmailSender>();

            // Add memory cache services
            services.AddMemoryCache();

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Order");
                    options.Conventions.AuthorizePage("/Basket/Checkout");
                });

            _services = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env)
        {
            //app.UseHangfireServer();

            //app.UseHangfireDashboard("/batchmonitor", new DashboardOptions
            //{
            //    Authorization = new[] { new MyAuthorizationFilter() }
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                ListAllRegisteredServices(app);
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Catalog/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc();
        }

        private void ListAllRegisteredServices(IApplicationBuilder app)
        {
            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }

        //public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        //{
        //    public bool Authorize(DashboardContext context)
        //    {
        //        return true;
        //    }
        //}

        public static string getconn()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "connstr.txt");
            string[] list = System.IO.File.ReadAllLines(path);
            string connstr = "";

            foreach (string c in list)
            {
                if (!c.StartsWith("//"))
                {
                    connstr = c;
                }
            }

            return connstr;
        }
    }
}
