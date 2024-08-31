using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Nike.Models;

namespace Nike.DesignPattern
{
    //Repository<T> class Là một lớp generic, nơi T là kiểu của đối tượng cần thao tác trong cơ sở dữ liệu. 
    public class Repository<T> where T : class
	{
		private readonly QuanLySanPhamEntities db = DbContextSingleton.GetInstance();

	
		public T Find(int id)
		{
			return db.Set<T>().Find(id);
		}

        //GetAll method Trả về tất cả các đối tượng của kiểu T từ cơ sở dữ liệu.
        public IEnumerable<T> GetAll(Func<T, bool> filter = null)
		{
			var query = db.Set<T>();

			if (filter != null)
			{
				query.Where(filter);
			}

			return query;
		}

		//Thêm 1 đối tượng
		public void Add(T entity)
		{
			db.Set<T>().Add(entity);
		}

		//Đánh dấu 1 đối tượng đã thay đổi
		public void Update(T entity)
		{
			db.Entry(entity).State = EntityState.Modified;
		}

		//Xóa một đối tượng
		public void Delete(T entity)
		{
			db.Set<T>().Remove(entity);
		}

		//Lưu các thay đổi
		public void Save()
		{
			db.SaveChanges();
		}
	}
}