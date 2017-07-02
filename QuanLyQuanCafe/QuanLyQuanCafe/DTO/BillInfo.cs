using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class BillInfo
    {
        public BillInfo(int id ,int billid,int foodid,int quantity)
        {
            this.ID = id;
            this.BillID = billid;
            this.FoodID = foodid;
            this.Quantity = quantity;
        }

        public BillInfo(DataRow row)
        {
            this.ID = (int)row["id"];
            this.BillID = (int)row["idbill"];
            this.FoodID = (int)row["idfood"];
            this.Quantity = (int)row["quantity"];
        }

        private int quantity;
        private int foodID;
        private int billID;
        private int iD;

        public int BillID
        {
            get
            {
                return billID;
            }

            set
            {
                billID = value;
            }
        }

        public int ID
        {
            get
            {
                return iD;
            }

            set
            {
                iD = value;
            }
        }

        public int FoodID
        {
            get
            {
                return foodID;
            }

            set
            {
                foodID = value;
            }
        }

        public int Quantity
        {
            get
            {
                return quantity;
            }

            set
            {
                quantity = value;
            }
        }
    }
}
