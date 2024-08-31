using Nike.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Net;
using System.Data.Entity;
using Nike.DesignPattern;
using Nike.DesignPattern.ProductModel;
using System.Threading.Tasks;

namespace Nike.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        // GET: Admin/Product
        private readonly QuanLySanPhamEntities _db = DbContextSingleton.GetInstance();
        // GET: Product


        public ActionResult Index(string sort, int? page, string searchString)
        {
            const int pageSize = 10;
            int pageNumber = page ?? 1;

            var products = _db.Products.ToList();

            // tìm kiếm sản phẩm 
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                ViewBag.searchStr = searchString;
                products = products.Where(p => p.ProductName.ToLower().Contains(searchString) ||
                                                p.Catalog.CatalogName.ToLower().Contains(searchString))
                                                .ToList();
            }
            // sắp xếp sản phẩm
            else
            {
                ViewBag.Sort = sort;
                switch (sort)
                {
                    case "pre-sold":
                        products = products.Where(p => p.SoLuong < 50 && p.SoLuong > 0).ToList();
                        break;
                    case "sold":
                        products = products.Where(p => p.SoLuong == 0).ToList();
                        break;
                    case "now":
                        products = products.OrderByDescending(p => p.NgayNhapHang).ToList();
                        break;
                    default:
                        products = products.ToList();
                        break;
                }
            }


            ViewBag.totalPage = Math.Ceiling((double)products.Count() / pageSize);
            ViewBag.products = products.ToPagedList(pageNumber, pageSize);

            return View(ViewBag.products);
        }

        //Proxy
        public interface IProductProxy
        {
            int AddProduct(Product model);
        }
        public class ProductRepositoryProxy : IProductProxy
        {
            private readonly QuanLySanPhamEntities _db = DbContextSingleton.GetInstance();
            public int AddProduct(Product model)
            {
                if (model.PriceOld < 0 || model.SoLuong > 100)
                {
                    return -1;
                }
                var productId = _db.Products.Add(model);
                return productId.Id;
            }
        }
        //--------------------------------------------------------------------------------\\

        // Tạo sản phẩm mới 
        public ActionResult Create()
        {
            ViewBag.CatalogId = new SelectList(_db.Catalogs, "ID", "CatalogName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, [Bind(Include = "CatalogId,ProductName,ProductCode,UnitPrice,SoLuong,ProductSold,ProductSale,PriceOld")] Product model)
        {

            if (ModelState.IsValid)
            {
                String anh = "/Hinh/Product/hinhphone.jpg";
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    String path = System.IO.Path.Combine(
                                           Server.MapPath("~/Hinh"), pic);
                    file.SaveAs(path);
                    anh = pic;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                //Proxy 
                ProductRepositoryProxy productRepositoryProxy = new ProductRepositoryProxy();
                productRepositoryProxy.AddProduct(model);
                model.NgayNhapHang = DateTime.Now;
                model.Picture = anh;
                Random prCode = new Random();
                model.ProductCode = String.Concat("PR", prCode.Next(5000, 7000).ToString());
                model.ProductSold = 0;
                model.UnitPrice = model.ProductSale != null
                    ? (model.UnitPrice = model.PriceOld - (model.PriceOld * int.Parse(model.ProductSale)) / 100)
                    : model.UnitPrice = model.PriceOld;
                _db.Products.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CatalogId = new SelectList(_db.Catalogs, "ID", "CatalogName", model.CatalogId);
            return View(model);
        }
        //---------------------------------------------------------------------//

        ///Mẫu Builder
        public class ProductBuilder
        {
            private Product _product;

            public ProductBuilder()
            {
                _product = new Product();
            }

            public ProductBuilder SetCatalogId(int? catalogId)
            {
                _product.CatalogId = catalogId;
                return this;
            }

            public ProductBuilder SetProductName(string productName)
            {
                _product.ProductName = productName;
                return this;
            }

            // Thêm các phương thức khác cho các thuộc tính khác

            public Product Build()
            {
                return _product;
            }
        }
        // Chức năng sửa thông tin sản phẩm 
        public ActionResult Edit(int Id)
        {


            if (Id.ToString() == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _db.Products.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CatalogId = new SelectList(_db.Catalogs, "ID", "CatalogName", product.CatalogId);
            return View(product);
        }

        //Mẫu Builder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CatalogId,Picture,ProductName,ProductCode,PriceOld,UnitPrice,ProductSold,ProductSale,SoLuong")] Product product, HttpPostedFileBase file)
        {
            Product pr = _db.Products.Find(product.Id);
            if (ModelState.IsValid)
            {
                String anh = pr.Picture;
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    String path = System.IO.Path.Combine(
                                           Server.MapPath("~/Hinh"), pic);
                    file.SaveAs(path);
                    anh = pic;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                  pr = new ProductBuilder()
                    .SetCatalogId(product.CatalogId)
                    .SetProductName(product.ProductName)
                    .Build();

                pr.Picture = anh;
                pr.CatalogId = product.CatalogId;
                pr.ProductName = product.ProductName;
                pr.PriceOld = product.PriceOld;
                pr.UnitPrice = (pr.ProductSale != null) ? (pr.UnitPrice = (pr.PriceOld - (pr.PriceOld * int.Parse(pr.ProductSale)) / 100)) : (pr.UnitPrice = pr.PriceOld);
                pr.ProductCode = product.ProductCode;
                pr.ProductSold = product.ProductSold;
                pr.ProductSale = product.ProductSale;
                pr.SoLuong = 500 - pr.ProductSold;
                pr.NgayNhapHang = DateTime.Now;

                //_db.Entry(pr).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CatalogId = new SelectList(_db.Catalogs, "ID", "CatalogName", pr.CatalogId);
            return View(product);
        }

        //Design Pattern Command
        public interface ICommand
        {
            void Execute();
        }

        public class DeleteProductCommand : ICommand
        {
            private readonly QuanLySanPhamEntities _db;
            private readonly int _productId;

            public DeleteProductCommand(int productId)
            {
                _db = DbContextSingleton.GetInstance();
                _productId = productId;
            }

            public void Execute()
            {
                Product product = _db.Products.Find(_productId);
                if (product != null)
                {
                    _db.Products.Remove(product);
                    _db.SaveChanges();
                }
            }
        }












        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            try
            {
                Product product = _db.Products.Find(Id);
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

    }

}

        // Hàm xóa sản phẩm 
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Product product = _db.Products.Find(id);
//            if (product == null)
//            {
//                return HttpNotFound();
//            }
//            return View(product);
//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int Id)
//        {
//            try
//            {
//                Product product = _db.Products.Find(Id);
//                _db.Products.Remove(product);
//                _db.SaveChanges();
//            }
//            catch
//            {

//            }
//            return RedirectToAction("Index");
//        }

//    }
//}