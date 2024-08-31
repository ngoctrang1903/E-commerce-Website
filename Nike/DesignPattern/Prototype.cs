
using Nike.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Nike.DesignPattern
{

     interface Prototype
    {
       Prototype Clone();
    }
  
    public class ProductPrototype : Prototype
    {
        public int Id { get; set; }
        public Nullable<int> CatalogId { get; set; }
        public string Picture { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<int> ProductSold { get; set; }
        public string ProductSale { get; set; }
        public Nullable<double> PriceOld { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(0, int.MaxValue, ErrorMessage = "Vui lòng nhập số lượng > 0")]
        public Nullable<int> SoLuong { get; set; }
        public Nullable<System.DateTime> NgayNhapHang { get; set; }

        Prototype Prototype.Clone()
        {
            ProductPrototype product = new ProductPrototype();    
            product.Id = Id;
            product.CatalogId = CatalogId;  
            product.Picture = Picture;
            product.ProductName = ProductName;
            product.ProductCode = ProductCode;
            product.UnitPrice = UnitPrice;
            product.ProductSold = ProductSold;
            product.ProductSale = ProductSale;
            return product;
          
        }
    }
}