using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.RazorPages.ViewModels;
using MySql.Data.MySqlClient;

namespace WebRazorPages.Controllers
{
    [Route("[controller]/[action]")]
    public class AdminController : Controller
    {
        public IActionResult Index(string fileupload)
        {
            if (fileupload == "true")
            {
                ViewBag.showfilesection = "true";
            }
            else if(fileupload== "false")
            {
                ViewBag.showfilesection = "false";
            }
            else if (fileupload == "monitor")
            {
                ViewBag.showfilesection = "monitor";
            }
            ViewBag.buttoncompletedisable = "disabled";
            ViewBag.LoginPartialPath = "~/Pages/";
            return View("~/Pages/Admin/Index.cshtml");
        }

        //[HttpPost]
        public IActionResult submittheform(AdminModel adm, IFormFile file)
        {        

            var files = Request.Form.Files;
            ViewBag.FormId = Guid.NewGuid().ToString().Substring(1,9);
            ViewBag.LoginPartialPath = "~/Pages/";
            ViewBag.Buttondisabled = "disabled";
            ViewBag.showfilesection = "true"; ;

            CatalogItem cti = new CatalogItem()
            {
                PictureUri= "http://catalogbaseurltobereplaced/images/products/"+file.FileName,
                Name = adm.Name,
                Description = adm.Description,
                 CatalogBrandId = adm.CatalogBrand,
                 CatalogTypeId=adm.CatalogType,
                 Price = Convert.ToDecimal( adm.Price)
            };
            savetoimagefolder(file);
            inserttodb(cti);
            ViewBag.showfilesection = "false";
            adm = new AdminModel();
            return View("~/Pages/Admin/Index.cshtml",adm);
        }

        private void inserttodb(CatalogItem cti)
        {
            string connstr = getconn(); 
            string command = "SELECT max(Id) as maxid FROM catalogitem;";
            int maxid = 0;
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                string select = command;
                MySqlCommand cmd = new MySqlCommand(command, conn);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {                    
                    while (dr.Read())
                    {
                       
                       
                        if (dr.IsDBNull(0))
                        {
                            maxid = 0;
                        }
                        else
                        {
                            maxid = dr.GetInt32("maxid");
                        }
                    }
                }
            }

            maxid++;
            command = "INSERT INTO catalogitem (Id, CatalogTypeId, CatalogBrandId,Name,Description,Price,PictureUri) VALUES(" + maxid+","+cti.CatalogTypeId+","+cti.CatalogBrandId+",'"+cti.Name+"','"+cti.Description+"',"+Convert.ToInt32( cti.Price)+",'"+cti.PictureUri+"');";
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                string insert = command;
                MySqlCommand cmd1 = new MySqlCommand(insert, conn);
                cmd1.ExecuteNonQuery();
            }

        }

        [HttpPost]
        public IActionResult Postjson()
        {
            var files = Request.Form.Files;
            filesModel fmodel = new filesModel();
            fmodel.files = new List<FileUploadModel>();
            //FileUploadModel fl = new FileUploadModel();
            //fl.name = "twitter.png";
            //fl.size = 600000;
            //fl.url = "Download?fname=" + fl.name;
            //fl.thumbnail_url = "https://jquery-file-upload.appspot.com/image%2Fpng/195107973/twitter.png";

            //fl.delete_type = "DELETE";
            //fl.delete_url = "";
            //fmodel.files.Add(fl);

            foreach (var file in files)
            {
                savetodisk(file);
                fmodel.files.Add(new FileUploadModel
                {
                    name = file.FileName,
                    size = file.Length,
                    url = "Download?fname=" + file.FileName,
                    thumbnailUrl = "Download?fname=" + file.FileName,
                    deleteType = "DELETE",
                    deleteUrl = "delete?fname=" + file.FileName

                });
            }


            // string k = @"{'files': [  {'name': 'picture1.jpg',  'size': 902604, 'url': 'http:\/\/ example.org\/ files\/ picture1.jpg', 'thumbnailUrl': 'http:\/\/ example.org\/ files\/ thumbnail\/ picture1.jpg', 'deleteUrl': 'http:\/\/ example.org\/ files\/ picture1.jpg','deleteType': 'DELETE'  }]}";
            return Json(fmodel);

        }

        private void savetodisk(IFormFile file)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), file.FileName);
            using (FileStream source = System.IO.File.Open(path, FileMode.Create))
            {
                file.CopyTo(source);
            }
        }

        public FileResult Download(string fname)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), fname);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = fname;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public void delete(string fname)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), fname);
            System.IO.File.Delete(path);
        }

        private void savetoimagefolder(IFormFile file)
        {
            string path = Path.Combine("images", "products"); 
            path = Path.Combine("wwwroot", path);
            path = Path.Combine(path, file.FileName);
            using (FileStream source = System.IO.File.Open(path, FileMode.Create))
            {
                file.CopyTo(source);
            }
        }
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




    public class UploadComponent
    {
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Price { get; set; }
        public string UploadedBy { get; set; }
        public string UploadedDate { get; set; } 
    }

    public class AdminModel
    {
        
       
    //    public  IEnumerable<Color> Colors = new List<Color> {
    //new Color {
    //    ColorId = 1,
    //    Name = "Red"
    //},
    //new Color {
    //    ColorId = 2,
    //    Name = "Blue"
    //}
    // };

    //    public class Color
    //    {
    //        public int ColorId { get; set; }
    //        public string Name { get; set; }
    //    }

    public string RegNo { get; set; }
        public string SnNo { get; set; }
        public string AreaCode { get; set; }
        public string Comments { get; set; }
        public string OfferType { get; set; }
        public string OfferSubtype1 { get; set; }
        public string OfferSubtype2 { get; set; }
        public string OfferSubtype3 { get; set; }
        public string Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CatalogBrand { get; set; }
        public int CatalogType { get; set; }

     
    }
}