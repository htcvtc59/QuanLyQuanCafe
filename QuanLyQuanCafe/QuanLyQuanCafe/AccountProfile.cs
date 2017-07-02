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
    public partial class AccountProfile : Form
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
                loginAcc = value; ChangeAccountPro(loginAcc);
            }
        }
        public AccountProfile(Account acc)
        {
            InitializeComponent();
            LoginAcc = acc;
        }

        #region Method
        void UpdateAccount()
        {
            string displayname = txtDisplayName.Text;
            string password = txtOldPass.Text;
            string newpass = txtNewPass.Text;
            string reenterpass = txtReNewPass.Text;
            string username = txtUserDis.Text;
            if (!newpass.Equals(reenterpass))
            {
                MessageBox.Show("Nhập mật khẩu mới không trùng nhau!");
            }
            else
            {
                if (AccountDAO.Instance.UpdateAccount(username, displayname, password, newpass))
                {
                    MessageBox.Show("Cập nhật thành công");
                }else
                {
                    MessageBox.Show("Vui lòng điền đúng mật khẩu");
                }
            }

        }
        void ChangeAccountPro(Account acc)
        {
            txtUserDis.Text = acc.UserName;
            txtDisplayName.Text = acc.DisplayName;
        }
        #endregion


        #region Event
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccount();
        }
        #endregion


    }
}
