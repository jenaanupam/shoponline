using ApplicationCore.Interfaces;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using System.Linq;
using System.Collections.Generic;
using ApplicationCore.Specifications;
using Microsoft.eShopWeb.RazorPages.Interfaces;
using Microsoft.eShopWeb.RazorPages.ViewModels;
using MySql.Data.MySqlClient;

namespace Microsoft.eShopWeb.RazorPages.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IRepository<CatalogItem> _itemRepository;

        public BasketViewModelService(IAsyncRepository<Basket> basketRepository,
            IRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer)
        {
            _basketRepository = basketRepository;
            _uriComposer = uriComposer;
            _itemRepository = itemRepository;
        }

        public async Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
        {
            var basketSpec = new BasketWithItemsSpecification(userName);
            var basket = (await _basketRepository.ListAsync(basketSpec)).FirstOrDefault();

            if (basket == null)
            {
                return await CreateBasketForUser(userName);
            }
            return CreateViewModelFromBasket(basket);
        }

        private BasketViewModel CreateViewModelFromBasket(Basket basket)
        {
            var viewModel = new BasketViewModel();
            viewModel.Id = basket.Id;
            viewModel.BuyerId = basket.BuyerId;
            viewModel.Items = basket.Items.Select(i =>
            {
                var itemModel = new BasketItemViewModel()
                {
                    Id = i.Id,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    CatalogItemId = i.CatalogItemId

                };
                 var item = _itemRepository.GetById(i.CatalogItemId);
              //  var item = getbyid(i.CatalogItemId);
                itemModel.PictureUrl = _uriComposer.ComposePicUri(item.PictureUri);
                itemModel.ProductName = item.Name;
                return itemModel;
            })
                            .ToList();
            return viewModel;
        }

        //public CatalogItem getbyid(int id)
        //{
        //    List<CatalogItem> catalogresult = new List<CatalogItem>();

        //    string connstr = "server=10.0.0.77;port=3306;database=cf_f4acc8ac_b5e6_4d77_aeb5_5487d4a592bc;user=blMhl1vXt7ULn05w;password=IaOADel876MdImsJ;";
        //    connstr = "server=localhost;port=3306;database=dotnetdemo;user=root;password=anupam;";
        //    string command = "SELECT * FROM catalogitem where UniqueId="+id+";";

        //    using (MySqlConnection conn = new MySqlConnection(connstr))
        //    {
        //        conn.Open();
        //        string select = command;
        //        MySqlCommand cmd = new MySqlCommand(command, conn);
        //        using (MySqlDataReader dr = cmd.ExecuteReader())
        //        {                     
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
        //            }
        //        }

        //    }

        //    return catalogresult[0];
        //}

        private async Task<BasketViewModel> CreateBasketForUser(string userId)
        {
            var basket = new Basket() { BuyerId = userId };
            await _basketRepository.AddAsync(basket);

            return new BasketViewModel()
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
                Items = new List<BasketItemViewModel>()
            };
        }
    }
}
