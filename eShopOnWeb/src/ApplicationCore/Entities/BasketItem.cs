namespace Microsoft.eShopWeb.ApplicationCore.Entities
{
    public class basketitem : BaseEntity
    {
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int CatalogItemId { get; set; }
    }
}
