using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nike.Models;
using System.Security.Cryptography;
using System.Text;
using PagedList;
using PagedList.Mvc;
using Nike.DesignPattern;


namespace Nike.Controllers
{
	public class HomeController : Controller
	{
		private readonly QuanLySanPhamEntities _db = DbContextSingleton.GetInstance();
		private readonly Repository<Product> productRepo = new Repository<Product>();

		public ActionResult Index(string searchStr, string sort, int pageIndex = 1)
		{
			//Catalog
			ViewBag.catalogs = (from s in _db.Catalogs select s).ToList();
			if (String.IsNullOrEmpty(searchStr))
			{
				Sort(sort, pageIndex);
			}
			// Tìm kiếm sản phẩm 
			else
			{
				searchStr.ToLower();
				var dsProduct2 = productRepo.GetAll(s => s.ProductName.ToLower().Contains(searchStr));
				var totalPage = (double)dsProduct2.Count() / (double)8;
				ViewBag.totalPage = Math.Ceiling(totalPage);
				ViewBag.searchStr = searchStr;
				ViewBag.products = dsProduct2.ToPagedList(pageIndex, 8);
				ViewBag.currentPage = pageIndex;
			}
			return View();
		}
		// Hàm sắp xếp bởi 
		public void Sort(string sort, int pageIndex)
		{
			//Product
			var dsProduct = (from s in productRepo.GetAll() select s).ToList();
			var totalPage = (double)dsProduct.Count() / (double)8;
			ViewBag.totalPage = Math.Ceiling(totalPage);
			ViewBag.currentPage = pageIndex;
			// sắp xếp theo filter
			if (String.IsNullOrEmpty(sort))
			{
				ViewBag.products = dsProduct.OrderByDescending(c => c.ProductSold).ToPagedList(pageIndex, 8);
			}
			else
			{
				sort = sort.ToLower();
				ViewBag.currentSort = sort;
				switch (sort)
				{
					case "asc":
						ViewBag.products = ProductSort.Ascending(dsProduct).ToPagedList(pageIndex, 8);
						break;
					case "desc":
						ViewBag.products = ProductSort.Descending(dsProduct).ToPagedList(pageIndex, 8);
						break;
					case "new":
						ViewBag.products = dsProduct.OrderByDescending(c => c.Id).ToPagedList(pageIndex, 8);
						break;
					case "hot":
						ViewBag.products = dsProduct.OrderByDescending(c => c.ProductSold).ToPagedList(pageIndex, 8);
						break;
					case "selling":
						ViewBag.products = dsProduct.Where(c => c.SoLuong > 0).ToPagedList(pageIndex, 8);
						break;
					default:
						var dsProduct2 = dsProduct.Where(s => s.Catalog.CatalogName.ToLower().Contains(sort));
						totalPage = (double)dsProduct2.Count() / (double)8;
						ViewBag.totalPage = Math.Ceiling(totalPage);
						ViewBag.products = dsProduct2.ToPagedList(pageIndex, 8);
						break;
				}
			}
		}

		// Hiển thị trang chi tiết sản phẩm
		public ActionResult Product(int? id)
		{
			Product product = _db.Products.Find(id);
			return View(product);
		}

	}
}