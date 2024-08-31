using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nike.Models;

namespace Nike.DesignPattern
{
	public class TemplateSort
	{
		private static AbstractSort TempSort;
		public static void Ascending(IEnumerable<Product> products)
		{
			TempSort = new AscendingSort();
			TempSort.Sort(products);
		}

        //TempSort field Là một trường để lưu trữ đối tượng của lớp kế thừa từ AbstractSort
        public static void Descending(IEnumerable<Product> products)
		{
			TempSort = new DescendingSort();
			TempSort.Sort(products);
		}

        //AbstractSort Là lớp trừu tượng chứa phương thức Sort
        private abstract class AbstractSort
		{
			public void Sort(IEnumerable<Product> products)
			{
				DoSort(products);
			}

			protected abstract void DoSort(IEnumerable<Product> products);
		}

		private class AscendingSort : AbstractSort
		{
			protected override void DoSort(IEnumerable<Product> products)
			{
				products.OrderBy(p => p.UnitPrice);
			}
		}

		private class DescendingSort : AbstractSort
		{
			protected override void DoSort(IEnumerable<Product> products)
			{
				products.OrderByDescending(p => p.UnitPrice);
			}
		}
	}
}