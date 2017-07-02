using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class FoodDAO
    {
        private static FoodDAO instance;

        public static FoodDAO Instance
        {
            get
            {
                if (instance == null) { instance = new FoodDAO(); }

                return instance;
            }

            private set
            {
                instance = value;
            }
        }
        private FoodDAO() { }

        public List<Food> GetFoodByCategoryID(int id)
        {
            List<Food> listFood = new List<Food>();
            string sql = "select * from Food where idcategory =" + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(sql);
            foreach (DataRow item in data.Rows)
            {
                Food f = new Food(item);
                listFood.Add(f);
            }
            return listFood;
        }

        public List<Food> GetListFood()
        {
            List<Food> listFood = new List<Food>();
            string sql = "select * from dbo.Food";
            DataTable data = DataProvider.Instance.ExecuteQuery(sql);
            foreach (DataRow item in data.Rows)
            {
                Food f = new Food(item);
                listFood.Add(f);
            }
            return listFood;
        }


        public DataTable GetListFoodUSP()
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListFood");
        }


        public bool InsertFood(string name, int id, float price)
        {
            string sql = string.Format("insert into dbo.Food(name,idcategory,price) values (N'{0}', {1}, {2})", name, id, price);
            int result = DataProvider.Instance.ExecuteNonQuery(sql);
            return result > 0;
        }
        public bool UpdateFood(string name, int idcate, float price,int id)
        {
            string sql = string.Format("update dbo.Food set name=N'{0}', idcategory={1} ,price={2} where id={3}", name, idcate, price,id);
            int result = DataProvider.Instance.ExecuteNonQuery(sql);
            return result > 0;
        }

        public bool DeleteFood(int idfood)
        {
            BillInfoDAO.Instance.DeleteBillInfoByFoodID(idfood);
            string sql = string.Format("delete dbo.Food where id={0}",idfood);
            int result = DataProvider.Instance.ExecuteNonQuery(sql);
            return result > 0;
        }


        public DataTable SearchFoodByName(string name)
        {
            string sql = string.Format("select id as [Mã],name as [Tên món ăn],idcategory as [Mã danh mục],price as [Đơn giá] from dbo.Food where  dbo.fuConvertToUnsign1(name) like N'%' + dbo.fuConvertToUnsign1(N'{0}') +'%'", name);
            return DataProvider.Instance.ExecuteQuery(sql);
        }
    }
}
