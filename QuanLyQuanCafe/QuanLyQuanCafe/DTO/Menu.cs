using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Menu
    {
        public Menu(string foodName, int quantity, float price, float totalPrice=0)
        {
            this.FoodName = foodName;
            this.Quantity = quantity;
            this.Price = price;
            this.TotalPrice = totalPrice;
        }
        public Menu(DataRow row)
        {
            this.FoodName = row["name"].ToString();
            this.Quantity = (int)row["quantity"];
            this.Price = (float)(Convert.ToDouble(row["price"].ToString()));
            this.TotalPrice = (float)(Convert.ToDouble(row["totalPrice"].ToString()));
        }

        private float totalPrice;
        private float price;
        private int quantity;
        private string foodName;

        public float Price
        {
            get
            {
                return price;
            }

            set
            {
                price = value;
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

        public string FoodName
        {
            get
            {
                return foodName;
            }

            set
            {
                foodName = value;
            }
        }

        public float TotalPrice
        {
            get
            {
                return totalPrice;
            }

            set
            {
                totalPrice = value;
            }
        }
    }
}
