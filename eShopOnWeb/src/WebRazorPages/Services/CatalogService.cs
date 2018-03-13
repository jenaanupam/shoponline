using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.RazorPages.Interfaces;
using Microsoft.eShopWeb.RazorPages.ViewModels;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.RazorPages.Services
{
    /// <summary>
    /// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
    /// with UI-specific types (view models and SelectListItem types).
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ILogger<CatalogService> _logger;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IAsyncRepository<CatalogBrand> _brandRepository;
        private readonly IAsyncRepository<CatalogType> _typeRepository;
        private readonly IUriComposer _uriComposer;

        public CatalogService(
            ILoggerFactory loggerFactory,
            IRepository<CatalogItem> itemRepository,
            IAsyncRepository<CatalogBrand> brandRepository,
            IAsyncRepository<CatalogType> typeRepository,
            IUriComposer uriComposer)
        {
            _logger = loggerFactory.CreateLogger<CatalogService>();
            _itemRepository = itemRepository;
            _brandRepository = brandRepository;
            _typeRepository = typeRepository;
            _uriComposer = uriComposer;
        }

        public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            _logger.LogInformation("GetCatalogItems called.");

            var filterSpecification = new CatalogFilterSpecification(brandId, typeId);
            var root = _itemRepository.List(filterSpecification);

            var totalItems = root.Count();

            var itemsOnPage = root
                .Skip(itemsPage * pageIndex)
                .Take(itemsPage)
                .ToList();

            itemsOnPage.ForEach(x =>
            {
                x.PictureUri = _uriComposer.ComposePicUri(x.PictureUri);
            });
            //remove if image not exists

            //remove if image not exists
            var vm = new CatalogIndexViewModel()
            {
                CatalogItems = itemsOnPage.Select(i => new CatalogItemViewModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    PictureUri = i.PictureUri,
                    Price = i.Price
                }),
                Brands = await GetBrands(),
                Types = await GetTypes(),
                BrandFilterApplied = brandId ?? 0,
                TypesFilterApplied = typeId ?? 0,
                PaginationInfo = new PaginationInfoViewModel()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count,
                    TotalItems = totalItems,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / itemsPage)).ToString())
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return vm;
        }

        //private IEnumerable<CatalogItem> Getcatalogitemfrommysql()
        //{
        //    List<CatalogItem> catalogresult = new List<CatalogItem>();

        //    string connstr = "server=10.0.0.77;port=3306;database=cf_f4acc8ac_b5e6_4d77_aeb5_5487d4a592bc;user=blMhl1vXt7ULn05w;password=IaOADel876MdImsJ;";
        //    connstr = "server=localhost;port=3306;database=dotnetdemo;user=root;password=anupam;";
        //    string command = "SELECT * FROM catalogitem;";

        //    using (MySqlConnection conn = new MySqlConnection(connstr))
        //    {
        //        conn.Open();
        //        string select = command;
        //        MySqlCommand cmd = new MySqlCommand(command, conn);
        //        using (MySqlDataReader dr = cmd.ExecuteReader())
        //        {
        //            int i = 1;
        //            while (dr.Read())
        //            {
        //                catalogresult.Add(new CatalogItem
        //                {
        //                    Name = dr.GetString("Name"),
        //                    CatalogBrandId = dr.GetInt32("CatalogBrandId"),
        //                    CatalogTypeId = dr.GetInt32("CatalogTypeId"),
        //                    PictureUri = dr.GetString("PictureUri"),
        //                    Description = dr.GetString("Description"),
        //                    Price = dr.GetInt32("Price"),
        //                    Id = dr.GetInt32("UniqueId")
        //                });
        //                i++;
        //            }
        //        }

        //    }

        //    return catalogresult.AsEnumerable();

        //}

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            _logger.LogInformation("GetBrands called.");
            var brands = await _brandRepository.ListAllAsync();

            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            foreach (CatalogBrand brand in brands)
            {
                items.Add(new SelectListItem() { Value = brand.Id.ToString(), Text = brand.Brand });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            _logger.LogInformation("GetTypes called.");
            // AddTypes().Wait();
            var types = await _typeRepository.ListAllAsync();
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            foreach (CatalogType type in types)
            {
                items.Add(new SelectListItem() { Value = type.Id.ToString(), Text = type.Type });
            }
           
            return items;
        }

        public async Task<bool> AddTypes()
        {
            _logger.LogInformation("addtypes called.");
            CatalogType ctype = new CatalogType();
            ctype.Id = 123;
            ctype.Type = "anupam";
            var types = await _typeRepository.AddAsync(ctype);
           

            return true;
        }
    }
}
