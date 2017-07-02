using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Account
    {
        public Account(int id, string displayname, string username,int type,string password=null)
        {
            this.ID = id;
            this.DisplayName = displayname;
            this.UserName = username;
            this.Password = password;
            this.Type = type;
        }
        public Account(DataRow row)
        {
            this.ID = (int)row["id"];
            this.DisplayName = row["displayname"].ToString();
            this.UserName = row["username"].ToString();
            this.Password = row["password"].ToString();
            this.Type = (int)row["type"];
        }



        private int type;
        private string password;
        private string userName;
        private string displayName;
        private int iD;

        public string DisplayName
        {
            get
            {
                return displayName;
            }

            set
            {
                displayName = value;
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

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public int Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }
    }
}
