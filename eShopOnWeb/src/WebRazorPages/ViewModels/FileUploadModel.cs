using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.RazorPages.ViewModels
{
    public class FileUploadModel
    {
        internal string deleteType;

        public string name { get; set; }
        public long size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }

    }

    public class filesModel
    {

        public List<FileUploadModel> files { get; set; }
    }
}
