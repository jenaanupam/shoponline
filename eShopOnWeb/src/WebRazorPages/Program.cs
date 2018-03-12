using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
 

namespace Microsoft.eShopWeb.RazorPages
{
    public class Program
    {
        public static void Main(string[] args)
        {
           //  Executedbscripts();
            //method to create tables
         //   entitycreationcumseeding();
            Console.WriteLine("mysqlentity sleeping for 5 sec");
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("mysqlentity wokeup for 5 sec");
            //Executedbscripts();
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {

                    //        var catalogContext = services.GetRequiredService<CatalogContext>();
                    //        CatalogContextSeed.SeedAsync(catalogContext, loggerFactory)
                    //.Wait();

                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    AppIdentityDbContextSeed.SeedAsync(userManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            host.Run();
        }

        private static void entitycreationcumseeding()
        {
            Console.WriteLine("mysqlentity started");
            using (var context = new CatalogContextInitialSetup())
            {
                try
                { 
                 bool iscreated =  context.Database.EnsureCreated();
                    Console.WriteLine("mysqlentity database created"+iscreated.ToString());
                    CatalogContextSeed.SeedAsync(context).GetAwaiter();
                Console.WriteLine("mysqlentity seeded");
                // Saves changes
                context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("mysqlentity error "+ex.InnerException);
                }
            }

            Console.WriteLine("mysqlentity ended");

        }

      

        private static string[] GetServerUrls(string[] args)
        {
            List<string> urls = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if ("--server.urls".Equals(args[i], StringComparison.OrdinalIgnoreCase))
                {
                    urls.Add(args[i + 1]);
                }
            }
            return urls.ToArray();
        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel()
                 .UseUrls(GetServerUrls(args))
                .UseStartup<Startup>()
                .Build();





        private static void Executedbscripts()
        {
            Console.WriteLine("Executedbscripts start ");
            try
            {
                string[] filedata = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "DBSCRIPTS.txt"));

                foreach (string query in filedata)
                {
                    if (query.Length > 3)
                    {
                        string connstr = "server=10.0.0.77;port=3306;database=cf_d66b9f8e_07fa_45bc_9eac_dc6f4124756c;user=BojS2T10mqqXrQIg;password=hIxKCVhllAHjaf3z;";
                        //connstr = "server=localhost;port=3306;database=testcaseissue;user=root;password=anupam;";
                        string command = query;
                        Console.WriteLine("Executedbscripts executing ");
                        using (MySqlConnection conn = new MySqlConnection(connstr))
                        {
                            conn.Open();
                            MySqlCommand cmd = new MySqlCommand(command, conn);
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Executedbscripts "+ query);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Executedbscripts error " + e.InnerException);
            }
            Console.WriteLine("Executedbscripts END ");
        }
        //public class LibraryContext : DbContext
        //{
        //    public DbSet<Book> Book { get; set; }

        //    public DbSet<Publisher> Publisher { get; set; }

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        optionsBuilder.UseMySQL("server=localhost;port=3306;database=testentityfw1;user=root;password=;");
        //    }

        //    protected override void OnModelCreating(ModelBuilder modelBuilder)
        //    {
        //        base.OnModelCreating(modelBuilder);

        //        modelBuilder.Entity<Publisher>(entity =>
        //        {
        //            entity.HasKey(e => e.ID);
        //            entity.Property(e => e.Name).IsRequired();
        //        });

        //        modelBuilder.Entity<Book>(entity =>
        //        {
        //            entity.HasKey(e => e.ISBN);
        //            entity.Property(e => e.Title).IsRequired();
        //            entity.HasOne(d => d.Publisher)
        //              .WithMany(p => p.Books);
        //        });
        //    }
        //}
        public class Book
        {
            public string ISBN { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string Language { get; set; }
            public int Pages { get; set; }
            public virtual Publisher Publisher { get; set; }
        }

        public class Publisher
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public virtual ICollection<Book> Books { get; set; }
        }
    }
}
