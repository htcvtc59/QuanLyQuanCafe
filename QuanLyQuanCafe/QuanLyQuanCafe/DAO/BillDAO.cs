using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance
        {
            get
            {
                if (instance == null) { instance = new BillDAO(); }

                return instance;
            }

            private set
            {
                instance = value;
            }
        }
        private BillDAO() { }


        /// <summary>
        /// Thành công bill ID
        /// Thất bại -1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetUncheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select * from dbo.Bill where idtable=" + id + " and status=0");
            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }
            return -1;
        }
        public void CheckOut(int id,int discount,float totalprice)
        {
            string sql = "update dbo.Bill set datecheckout=GETDATE(), status = 1" + ",discount="+discount+ ", totalprice="+totalprice+ " where id="+id;
            DataProvider.Instance.ExecuteNonQuery(sql);
        }

        public void InsertBill(int id)
        {
            DataProvider.Instance.ExecuteNonQuery("exec USP_InsertBill @idTable", new object[] { id });
        }

        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("select max(id) from dbo.Bill");
            }
            catch (Exception)
            {
                return 1;
            }

        }

        public DataTable GetBillListByDate(DateTime checkIn,DateTime checkOut)
        {
          return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDate @checkIn , @checkOut",new object[] { checkIn,checkOut});
        }


        public DataTable GetBillListByDateAndPage(DateTime checkIn,DateTime checkOut,int pagenums)
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDateAndPage @checkIn , @checkOut , @page",new object[] { checkIn,checkOut,pagenums});
        }

        public int GetNumBilllListByDate(DateTime checkIn, DateTime checkOut)
        {
            return (int)DataProvider.Instance.ExecuteScalar("exec USP_GetNumBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }

    }
}
