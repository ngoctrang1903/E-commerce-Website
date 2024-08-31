using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nike.Models;

namespace Nike.DesignPattern
{
	public class DbContextSingleton
	{
        //Phải định nghĩa ra một private contractor
        private static QuanLySanPhamEntities dbInstance;

		//Sau chúng ta khởi tạo một biến static private trả về chính cái instance của Singleton
		private static readonly object lockObject = new object();

		//Và cuối cùng là Class GetInstance
		public static QuanLySanPhamEntities GetInstance()
		{
			//Đoạn code check xem Instance đã được khởi tạo trước đó hay chưa
			if (dbInstance == null)
			{
				lock (lockObject)
				{
					if (dbInstance == null)
					{
						dbInstance = new QuanLySanPhamEntities();
					}
				}
			}
			return dbInstance;
		}
	}
}