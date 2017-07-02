using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
    public partial class TableManager : Form
    {
        private Account loginAcc;

        public Account LoginAcc
        {
            get
            {
                return loginAcc;
            }

            set
            {
                loginAcc = value; ChangeAccount(loginAcc.Type);
            }
        }

        public TableManager(Account acc)
        {
            InitializeComponent();

            this.LoginAcc = acc;

            LoadTable();
            LoadCategory();
            LoadComboboxTable(cbbSwitchTable);
        }

        #region Method
        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
        }


        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbbCategory.DataSource = listCategory;
            cbbCategory.DisplayMember = "Name";

        }
        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbbFoodCategory.DataSource = listFood;
            cbbFoodCategory.DisplayMember = "Name";

        }

        void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();
            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + (item.Status == "0" ? "Trống" : "Có người");
                btn.Click += Btn_Click;
                btn.Tag = item;
                switch (item.Status)
                {
                    case "0":
                        btn.BackColor = ColorTranslator.FromHtml("#48DD95");
                        break;
                    default:
                        btn.BackColor = ColorTranslator.FromHtml("#DD9F9C");
                        break;
                }
                flpTable.Controls.Add(btn);
            }


        }
        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach (DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Quantity.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                lsvBill.Items.Add(lsvItem);
                totalPrice += item.TotalPrice;
            }
            txtTotalPrice.Text = totalPrice.ToString("C", new CultureInfo("vi-VN")).Replace(",00", " ").Trim();
        }

        void LoadComboboxTable(ComboBox cbb)
        {
            cbb.DataSource = TableDAO.Instance.LoadTableList();
            cbb.DisplayMember = "Name";
        }

        #endregion



        #region Events
        private void Btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ThôngtincánhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountProfile accp = new AccountProfile(LoginAcc);
            accp.ShowDialog();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Admin ad = new Admin();
            ad.loginAccount = LoginAcc;
            ad.InsertFoodEvent += Ad_InsertFoodEvent;
            ad.UpdateFoodEvent += Ad_UpdateFoodEvent;
            ad.DeleteFoodEvent += Ad_DeleteFoodEvent;
            ad.ShowDialog();
        }

        private void Ad_DeleteFoodEvent(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        private void Ad_UpdateFoodEvent(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void Ad_InsertFoodEvent(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void cbbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            LoadFoodListByCategoryID(id);
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            try
            {
                Table table = lsvBill.Tag as Table;
                if (table == null)
                {
                    MessageBox.Show("Hãy chọn bàn");
                    return;
                }

                int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
                int idFood = (cbbFoodCategory.SelectedItem as Food).ID;
                int quantity = (int)nmFoodCount.Value;

                if (idBill == -1)
                {
                    BillDAO.Instance.InsertBill(table.ID);
                    BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), idFood, quantity);
                }
                else
                {
                    BillInfoDAO.Instance.InsertBillInfo(idBill, idFood, quantity);


                }
                ShowBill(table.ID);
                LoadTable();
            }
            catch (Exception) { }
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)nmDiscount.Value;
            float totalPrice = float.Parse(txtTotalPrice.Text.Replace("₫", " ").Trim());
            float finalTotalPrice = totalPrice - (totalPrice / 100) * discount;


            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn có chắc chắn thanh toán cho bàn {0}\n Tổng tiền - (Tổng tiền / 100) x Giảm giá = {1} - ({1}/100) x {2} = {3}", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount, finalTotalPrice);
                    ShowBill(table.ID);
                    LoadTable();
                }

            }



        }


        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).ID;
            int id2 = (cbbSwitchTable.SelectedItem as Table).ID;
            if (MessageBox.Show(string.Format("Bạn có thực sự muốn chuyển bàn {0}  qua bàn {1} không?", (lsvBill.Tag as Table).Name, (cbbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                LoadTable();
            }
        }








        #endregion


    }
}
