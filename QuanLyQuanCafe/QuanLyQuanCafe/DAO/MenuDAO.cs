using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class MenuDAO
    {
        private static MenuDAO instance;

        public static MenuDAO Instance
        {
            get
            {
                if (instance == null) { instance = new MenuDAO(); }

                return instance;
            }

            private set
            {
                instance = value;
            }
        }
        private MenuDAO() { }

        public List<Menu> GetListMenuByTable(int id)
        {
            string sql = "select f.name,bi.quantity,f.price,f.price*bi.quantity as totalPrice from dbo.BillInfo as bi , dbo.Bill as b , dbo.Food as f where bi.idbill = b.id and bi.idfood = f.id and b.status=0 and b.idtable = " + id;
            List<Menu> listMenu = new List<Menu>();
            DataTable data = DataProvider.Instance.ExecuteQuery(sql);
            foreach (DataRow item in data.Rows)
            {
                Menu m = new Menu(item);
                listMenu.Add(m);
            }

            return listMenu;
        }
    }
}
