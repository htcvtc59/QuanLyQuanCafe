using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null) { instance = new AccountDAO(); }
                return instance;
            }

            private set
            {
                instance = value;
            }
        }
        private AccountDAO() { }

        public bool loginacc(string user, string pass)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(pass);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(temp);
            
            //var list = hash.ToString();
            //list.Reverse();


            string sql = "USP_Login @user , @pass";
            DataTable result = DataProvider.Instance.ExecuteQuery(sql, new object[] { user, pass /*list*/ });
            return result.Rows.Count > 0;
        }

        public Account GetAccByUserName(string username)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select * from Account where username='" + username + "'");
            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }
            return null;
        }
        public bool UpdateAccount(string user, string disname, string pass, string newpass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount @username , @displayname , @password , @newpassword", new object[] { user, disname, pass, newpass });
            return result > 0;
        }

        public bool InsertAccount(string name, string displayname, int type)
        {
            string sql = string.Format("insert into dbo.Account(username,displayname,type) values (N'{0}', N'{1}', {2})", name, displayname, type);
            int result = DataProvider.Instance.ExecuteNonQuery(sql);
            return result > 0;
        }
        public bool UpdateTAccount(string name, string displayname, int type,int id)
        {
            string sql = string.Format("update dbo.Account set username=N'{0}', displayname=N'{1}' ,type={2} where id={3}", name, displayname, type, id);
            int result = DataProvider.Instance.ExecuteNonQuery(sql);
            return result > 0;
        }

        public bool DeleteAccount(int idacc)
        {
            string sql = string.Format("delete dbo.Account where id={0}", idacc);
            int result = DataProvider.Instance.ExecuteNonQuery(sql);
            return result > 0;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("select id,username,displayname,type from dbo.Account");
        }

        public bool ResetPassword(int id)
        {
            string sql = string.Format("update dbo.Account set password=N'0' where id={0}", id);
            int result = DataProvider.Instance.ExecuteNonQuery(sql);
            return result > 0;
        }

    }
}
