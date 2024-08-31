using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nike.DesignPattern;

namespace Nike.Models
{
    public class Giohang
    {
        QuanLySanPhamEntities _db = DbContextSingleton.GetInstance();
        public int IdProduct{set; get;}
        public string ProductName { set; get; }
        public string Picture { set; get; }
        public Double DonGia { set; get; }
        public int SoLuong { set; get; }
        public string Brand { set; get; }
        public Double Price
        {
            get { return SoLuong * DonGia; }
        }
        public Giohang(int MaSP)
        {
            IdProduct = MaSP;
            Product product = _db.Products.Single(n => n.Id == IdProduct);
            ProductName = product.ProductName;
            Picture = product.Picture;
            DonGia = double.Parse(product.UnitPrice.ToString());
            SoLuong = 1;
            Brand = product.Catalog.CatalogName;
        }
    }
}