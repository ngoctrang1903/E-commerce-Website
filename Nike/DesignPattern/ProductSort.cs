using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nike.Models;

namespace Nike.DesignPattern
{

    //Strategy design pattern
    public class ProductSort
	{
		private static IProductSort tempSort;

        //Đoạn code sắp xếp sp theo thứ tự tăng dần Ascending
        public static IEnumerable<Product> Ascending(IEnumerable<Product> products)
		{
			//TempSort lưu trự một đối tượng
			tempSort = new SortAscending();
			return tempSort.Sort(products);
        }

        //Đoạn code sắp xếp sp theo thứ tự giảm dần Descending
        public static IEnumerable<Product> Descending(IEnumerable<Product> products)
		{
			tempSort = new SortDescending();
			return tempSort.Sort(products);
		}

		//Sắp xếp ds sản phẩm
		public interface IProductSort
		{
			IEnumerable<Product> Sort(IEnumerable<Product> products);
		}

        //SortAscending sắp xếp danh sách sản phẩm theo giá đơn vị tăng dần.
        public class SortAscending : IProductSort
		{
			public IEnumerable<Product> Sort(IEnumerable<Product> products)
			{
				return products.OrderBy(p => p.UnitPrice);
			}
		}
        //SortDescending sắp xếp danh sách sản phẩm theo giá đơn vị giảm dần.
        public class SortDescending : IProductSort
		{
			public IEnumerable<Product> Sort(IEnumerable<Product> products)
			{
				return products.OrderByDescending(p => p.UnitPrice);
			}
		}
	}
}