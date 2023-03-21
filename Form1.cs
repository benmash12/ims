using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ims
{
    public partial class LoginForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                cp.Style |= CS_DROPSHADOW;
                return cp;
            }
        }
        public LoginForm()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            var cr = MessageBox.Show("Ar you sure you want to exit?","Confirm Exit", MessageBoxButtons.YesNo);
            if(cr == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            string un = LoginUsername.Text;
            string ps = LoginPass.Text;
            if(string.IsNullOrEmpty(un) || string.IsNullOrEmpty(ps))
            {
                MessageBox.Show("All fields are required!", "ERROR", MessageBoxButtons.OK);
            }
            else
            {
                LoginBtn.Enabled = false;
                Loading.Visible = true;
                DB d = new DB();
                if(d.createTables() == 1){
                    LoginBtn.Enabled = false;
                    Loading.Visible = false;
                    MessageBox.Show("App Initialised!", "SUCCESS", MessageBoxButtons.OK);
                }
                else
                {
                    LoginBtn.Enabled = false;
                    Loading.Visible = false;
                    MessageBox.Show("App tables could not be created!", "ERROR", MessageBoxButtons.OK);
                }
            }
        }
    }
    class DB
    {
        SqlConnection createConn()
        {
            string constr = "Data Source=(localdb)\\ProjectModels;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection conn = new SqlConnection(constr);
            return conn;
        }
        public int createTables()
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"IF OBJECT_ID(N'ins_admin', N'U') IS NULL CREATE TABLE ims_admin (id int identity(1,1) primary key, username varchar(20) NOT NULL, password text);";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                /*cmd.CommandText = 
                cmd.ExecuteNonQuery();*/
                cmd.CommandText = @"IF OBJECT_ID(N'ins_categories', N'U') IS NULL CREATE TABLE ims_categories (id int identity(1,1) primary key, category varchar(50) NOT NULL);";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"IF OBJECT_ID(N'ins_products', N'U') IS NULL CREATE TABLE ims_products (id int identity(1,1) primary key, name varchar(100) NOT NULL, category_id int NOT NULL, quantity int DEFAULT '0', last_modified varchar(40) NOT NULL);";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"IF OBJECT_ID(N'ins_inc_exp', N'U') IS NULL CREATE TABLE ims_inc_exp (id int identity(1,1) primary key, type varchar(10) NOT NULL, title varchar(100), val float(10,2) DEFAULT '0.00', date_added DATE default CURRENT_DATE); ";
                cmd.ExecuteNonQuery();
                /*cmd.CommandText = @"IF EXISTS (SELECT * FROM ims_admin WHERE username='admin') ELSE INSERT INTO ims_admin (username,password) VALUES('admin','pass');";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"SET sql_notes = 1;";
                cmd.ExecuteNonQuery();*/
                conn.Close();
                return 1;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 0;
            }
        }
        public bool initApp()
        {
            try
            {

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}

