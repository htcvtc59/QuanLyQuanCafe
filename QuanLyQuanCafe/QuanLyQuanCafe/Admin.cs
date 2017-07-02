using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
    public partial class Admin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        public Account loginAccount;
        public Admin()
        {
            InitializeComponent();
            LoadAdmin();
            LoadListBillByDate(dtpFormDate.Value, dtpToDate.Value);
        }

        #region Method
        void LoadAdmin()
        {
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;

            LoadDateTimePickerBill();
            LoadListFood();
            AddAccountBinding();
            AddFoodBinding();
            LoadCategoryIntoCBB(cbbFoodCategory);
            LoadListAccount();

        }
        void AddFoodBinding()
        {
            txtFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Mã", true, DataSourceUpdateMode.Never));
            txtFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Tên món ăn", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Đơn giá", true, DataSourceUpdateMode.Never));
        }
        void LoadCategoryIntoCBB(ComboBox cbb)
        {
            cbb.DataSource = CategoryDAO.Instance.GetListCategory();
            cbb.DisplayMember = "Name";
        }
        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpFormDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpToDate.Value = dtpFormDate.Value.AddMonths(1).AddDays(-1);
        }
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFoodUSP();
        }

        DataTable SearchFoodByName(string name)
        {
            return FoodDAO.Instance.SearchFoodByName(name);
        }

        void LoadListAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }
        void AddAccountBinding()
        {
            txtAccID.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "id", true, DataSourceUpdateMode.Never));
            txtNameAcc.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "username", true, DataSourceUpdateMode.Never));
            txtDisNameAcc.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "displayname", true, DataSourceUpdateMode.Never));
            txtTypeAcc.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "type", true, DataSourceUpdateMode.Never));
        }

        void AddAccount(string username, string displayname, int type)
        {
            if (AccountDAO.Instance.InsertAccount(username, displayname, type))
            {
                MessageBox.Show("Thêm thành công");
            }
            else
            {
                MessageBox.Show("Thêm thất bại");
            }
            LoadListAccount();
        }
        void UpdateAccount(string username, string displayname, int type, int id)
        {
            if (AccountDAO.Instance.UpdateTAccount(username, displayname, type, id))
            {
                MessageBox.Show("Sửa thành công");
            }
            else
            {
                MessageBox.Show("Sửa thất bại");
            }
            LoadListAccount();

        }
        void DeleteAccount(int id)
        {
            if (loginAccount.ID.Equals(id))
            {
                MessageBox.Show("Vui lòng đừng xóa chính bạn");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(id))
            {
                MessageBox.Show("Xóa thành công");
            }
            else
            {
                MessageBox.Show("Xóa thất bại");
            }
            LoadListAccount();


        }

        void ResetPassAccount(int id)
        {
            if (AccountDAO.Instance.ResetPassword(id))
            {
                MessageBox.Show("Đổi mật khẩu thành công");
            }
            else
            {
                MessageBox.Show("Đổi mật khẩu thất bại");
            }
            LoadListAccount();

        }

        #endregion

        #region Events


        private void btnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpFormDate.Value, dtpToDate.Value);
        }
        private void btnShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void txtFoodID_TextChanged(object sender, EventArgs e)
        {
            if (dtgvFood.SelectedCells.Count > 0)
            {
                try
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["Mã danh mục"].Value;
                    Category category = CategoryDAO.Instance.GetCategoryByID(id);
                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbbFoodCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    cbbFoodCategory.SelectedIndex = index;
                }
                catch (Exception) { }
            }

        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txtFoodName.Text;
            int categoryID = (cbbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            if (FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Thêm thành công");
                LoadListFood();
                if (insertFoodEvent != null)
                    insertFoodEvent(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Thêm thất bại");
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txtFoodName.Text;
            int categoryID = (cbbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txtFoodID.Text);
            if (FoodDAO.Instance.UpdateFood(name, categoryID, price, id))
            {
                MessageBox.Show("Sửa thành công");
                LoadListFood();
                if (updateFoodEvent != null)
                    updateFoodEvent(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Sửa thất bại");
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtFoodID.Text);
            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa thành công");
                LoadListFood();
                if (deleteFoodEvent != null)
                    deleteFoodEvent(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Xóa thất bại");
            }
        }


        private event EventHandler insertFoodEvent;
        public event EventHandler InsertFoodEvent
        {
            add { insertFoodEvent += value; }
            remove { insertFoodEvent -= value; }
        }
        private event EventHandler deleteFoodEvent;
        public event EventHandler DeleteFoodEvent
        {
            add { deleteFoodEvent += value; }
            remove { deleteFoodEvent -= value; }
        }
        private event EventHandler updateFoodEvent;
        public event EventHandler UpdateFoodEvent
        {
            add { updateFoodEvent += value; }
            remove { updateFoodEvent -= value; }
        }

        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txtSearchFood.Text);
        }
        //Account
        private void btnShowAcc_Click(object sender, EventArgs e)
        {
            LoadListAccount();
        }
        private void btnAddAcc_Click(object sender, EventArgs e)
        {
            string username = txtNameAcc.Text;
            string displayname = txtDisNameAcc.Text;
            int type = (int)txtTypeAcc.Value;
            AddAccount(username, displayname, type);
        }

        private void btnDeleteAcc_Click(object sender, EventArgs e)
        {
            int id = Int32.Parse(txtAccID.Text);
            DeleteAccount(id);
        }

        private void btnEditAcc_Click(object sender, EventArgs e)
        {
            int id = Int32.Parse(txtAccID.Text);
            string username = txtNameAcc.Text;
            string displayname = txtDisNameAcc.Text;
            int type = (int)txtTypeAcc.Value;
            UpdateAccount(username, displayname, type, id);
        }

        private void btnResetPass_Click(object sender, EventArgs e)
        {
            int id = Int32.Parse(txtAccID.Text);
            ResetPassAccount(id);
        }

        #endregion

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            txtPageIndex.Text = "1";
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txtPageIndex.Text);
            if (page > 1)
                page--;
            txtPageIndex.Text = page.ToString();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txtPageIndex.Text);
            int sumrecord = BillDAO.Instance.GetNumBilllListByDate(dtpFormDate.Value, dtpToDate.Value);

            if (page < sumrecord)
                page++;
            txtPageIndex.Text = page.ToString();
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            int sumrecord = BillDAO.Instance.GetNumBilllListByDate(dtpFormDate.Value, dtpToDate.Value);
            int lastpage = sumrecord / 3;
            if (lastpage % 3!= 0)
            {
                lastpage++;
            }
            txtPageIndex.Text = lastpage.ToString();
        }

        private void txtPageIndex_TextChanged(object sender, EventArgs e)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dtpFormDate.Value, dtpToDate.Value, Convert.ToInt32(txtPageIndex.Text));
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'QuanLyQuanCafeDataSet.USP_GetTableList' table. You can move, or remove it, as needed.
            this.USP_GetTableListTableAdapter.Fill(this.QuanLyQuanCafeDataSet.USP_GetTableList);

            this.reportViewer1.RefreshReport();
            this.reportViewer1.RefreshReport();
        }
    }
}
