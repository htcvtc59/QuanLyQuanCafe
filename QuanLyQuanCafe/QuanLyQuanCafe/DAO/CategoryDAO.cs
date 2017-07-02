using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
   public class CategoryDAO
    {
        private static CategoryDAO instance;

        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null) { instance = new CategoryDAO(); }

                return instance;
            }

            private set
            {
                instance = value;
            }
        }
        private CategoryDAO() { }

        public List<Category> GetListCategory()
        {
            List<Category> listCategory = new List<Category>();
            string sql ="select * from FoodCategory";
            DataTable data = DataProvider.Instance.ExecuteQuery(sql);
            foreach (DataRow item in data.Rows)
            {
                Category c = new Category(item);
                listCategory.Add(c);
            }
            return listCategory;
        }

        public Category GetCategoryByID(int id)
        {
            Category category = null;
            string sql = "select * from FoodCategory where id=" + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(sql);
            foreach (DataRow item in data.Rows)
            {
                category = new Category(item);
                return category;
            }
            return category;
        }
    }
}
