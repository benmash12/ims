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
            var cr = MessageBox.Show("Ar you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo);
            if (cr == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            string un = LoginUsername.Text.Replace("'", "''");
            string ps = LoginPass.Text.Replace("'", "''");
            if (string.IsNullOrEmpty(un) || string.IsNullOrEmpty(ps))
            {
                MessageBox.Show("All fields are required!", "ERROR", MessageBoxButtons.OK);
            }
            else
            {
                LoginBtn.Enabled = false;
                Loading.Visible = true;
                DB d = new DB();
                if(d.Login(un,ps) == 1){
                    LoginBtn.Enabled = true;
                    Loading.Visible = false;
                    var ds = new Dashboard();
                    ds.Show();
                    this.Hide();
                }
                else if (d.Login(un, ps) == 2) {
                    LoginBtn.Enabled = true;
                    Loading.Visible = false;
                    MessageBox.Show("Login Failed! A database error was encountered.", "ERROR", MessageBoxButtons.OK);
                }
                else
                {
                    LoginBtn.Enabled = true;
                    Loading.Visible = false;
                    MessageBox.Show("Login Failed! Incorrect credentials provided.", "ERROR", MessageBoxButtons.OK);
                }
            }
        }
    }
    class DB
    {
        SqlConnection createConn()
        {
            string constr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=imsx;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection conn = new SqlConnection(constr);
            return conn;
        }
        public int Login(string u, string p)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"select * from ims_admin where CONVERT(VARCHAR, username) ='"+u+"' and CONVERT(VARCHAR, password) ='"+p+"';";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    conn.Close();
                    return 1;
                }
                conn.Close();
                return 0;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 2;
            }
        }
        public int addCat(string c)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"INSERT INTO ims_categories (category) VALUES ('"+c+"');";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 0;
            }
        }
        public bool addProduct(string c, string t, string q)
        {
            try
            {
                SqlConnection conn = createConn();
                string dt = DateTime.Now.ToString("dd/MM/yyyy H:mm tt");
                string query = @"INSERT INTO ims_products (category,name,quantity,last_modified) VALUES ('" + c + "','" + t + "'," + q + ",'" + dt + "');";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public bool loadExp(DataGridView dgv, Label t1, Label t2, Label t3)
        {
            try
            {
                SqlConnection conn = createConn();
                conn.Open();
                string query = "SELECT * FROM ims_inc_exp ORDER BY id DESC;";
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand(query, conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                BindingSource bs = new BindingSource();
                bs.DataSource = dt;
                dgv.DataSource = bs;
                string query2 = "SELECT SUM(val) FROM ims_inc_exp WHERE type='Expenses';";
                string query3 = "SELECT SUM(val) FROM ims_inc_exp WHERE type='Income';";
                SqlConnection connx = createConn();
                SqlConnection conny = createConn();
                connx.Open();
                conny.Open();
                SqlCommand cmd2 = new SqlCommand(query2, connx);
                SqlCommand cmd3 = new SqlCommand(query3, conny);
                SqlDataReader r2 = cmd2.ExecuteReader();
                SqlDataReader r3 = cmd3.ExecuteReader();
                r2.Read();
                r3.Read();
                float inc, net, exp;
                if (!r3.HasRows || r3[0] == DBNull.Value)
                {
                    inc = 0;
                }
                else
                {
                    inc = float.Parse(r3[0].ToString());
                }
                if (!r2.HasRows || r2[0] == DBNull.Value)
                {
                    exp = 0;
                }
                else
                {
                    exp = float.Parse(r2[0].ToString());
                }
                net = inc - exp;
                t1.Text = "Total Income: INR" + inc.ToString("#,##0.00");
                t2.Text = "Total Expenses: INR" + exp.ToString("#,##0.00");
                t3.Text = "Net Income: INR" + net.ToString("#,##0.00");
                conn.Close();
                connx.Close();
                conny.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public int changePass(string np, string cp)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"UPDATE ims_admin SET password='"+np+ "' WHERE CONVERT(VARCHAR, username) ='admin' AND CONVERT(VARCHAR, password) ='" + cp+"';";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                int ii = cmd.ExecuteNonQuery();
                conn.Close();
                return ii;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 420;
            }
        }
        public bool addExpense(string ty, string na, string am)
        {
            try
            {
                SqlConnection conn = createConn();
                //string dt = DateTime.Now.ToString("dd/MM/yyyy H:mm tt");
                string query = @"INSERT INTO ims_inc_exp (type,title,val) VALUES ('" + ty + "','" + na + "'," + am + ");";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public bool updateProduct(string id, string nam, string qua, TextBox t6)
        {
            try
            {
                SqlConnection conn = createConn();
                string dt = DateTime.Now.ToString("dd/MM/yyyy H:mm tt");
                string query = @"UPDATE ims_products SET name='"+nam+"', quantity="+qua+ ", last_modified='" + dt + "' WHERE CONVERT(INT, id) = " + id + ";";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                t6.Text = dt;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public bool deleteCat(string c, TextBox t2, TextBox t3, TextBox t4, TextBox t5, TextBox t6)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"DELETE FROM ims_categories WHERE CONVERT(VARCHAR, category) = '"+c+"' ;";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                string query2 = @"DELETE FROM ims_products WHERE CONVERT(VARCHAR, category) = '" + c + "' ;";
                SqlCommand cmdx = new SqlCommand(query2, conn);
                cmdx.ExecuteNonQuery();
                conn.Close();
                t2.Text = "";
                t3.Text = "";
                t6.Text = "";
                t4.Text = "";
                t5.Text = "";
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public bool deleteProduct(string id, TextBox t2, TextBox t3, TextBox t4, TextBox t5, TextBox t6)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"DELETE FROM ims_products WHERE CONVERT(INT, id) = " + id + " ;";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                t2.Text = "";
                t3.Text = "";
                t6.Text = "";
                t4.Text = "";
                t5.Text = "";
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public bool delExp(string id)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"DELETE FROM ims_inc_exp WHERE CONVERT(INT, id) = " + id + " ;";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public int LoadCat(ComboBox v)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"select category from ims_categories ORDER BY category ASC;";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    v.Items.Clear();
                    while (reader.Read())
                    {
                        v.Items.Add(reader[0]);
                    }
                    conn.Close();
                    return 1;
                }
                else
                {
                    v.Items.Clear();
                }
                conn.Close();
                return 2;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 0;
            }
        }
        public bool LoadProduct(string id, TextBox t2, TextBox t3, TextBox t4, TextBox t5, TextBox t6)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"select * from ims_products where CONVERT(INT, id) = " + id + ";";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        t2.Text = reader[0].ToString();
                        t3.Text = reader[2].ToString();
                        t6.Text = reader[4].ToString();
                        t4.Text = reader[1].ToString();
                        t5.Text = reader[3].ToString();
                    }
                    conn.Close();
                    return true;
                }
                conn.Close();
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public int LoadProd(string cat, ListBox v, Label lv)
        {
            try
            {
                SqlConnection conn = createConn();
                string query = @"select id,name,quantity from ims_products WHERE CONVERT(VARCHAR, category) = '" + cat + "' ORDER BY name ASC;";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    v.Items.Clear();
                    int cnt = 0;
                    while (reader.Read())
                    {
                        cnt++;
                        v.Items.Add(reader[1] + " (quantity: "+reader[2]+", id: "+reader[0]+")");
                    }
                    lv.Text = "Showing "+cnt+" Products From " + cat;
                    conn.Close();
                    return 1;
                }
                else
                {
                    v.Items.Clear();
                    lv.Text = "Showing 0 Products From " + cat;
                    return 2;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 0;
            }
        }
        public int LoadProdSearch(string cat, ListBox v, Label lv, string sea)
        {
            try
            {
                if (!sea.Equals(string.Empty))
                {
                    sea = sea.Replace("'", "");
                    sea = sea.Replace("\"", "");
                    sea = sea.Replace("_", "");
                    sea = sea.Replace("-", "");
                    sea = sea.Replace("%", "");
                    sea = sea.Replace("&", "");
                    //string[] keys = sea.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    sea = " AND ( CONVERT(VARCHAR, category) LIKE '%" + sea + "%' OR CONVERT(VARCHAR, name) LIKE '%" + sea + "%' )";
                }
                SqlConnection conn = createConn();
                string query = @"select id,name,quantity from ims_products WHERE CONVERT(VARCHAR, category) = '" + cat + "' " + sea + " ORDER BY name ASC;";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    v.Items.Clear();
                    int cnt = 0;
                    while (reader.Read())
                    {
                        cnt++;
                        v.Items.Add(reader[1] + " (quantity: " + reader[2] + ", id: " + reader[0] + ")");
                    }
                    lv.Text = "Showing " + cnt + " Result(s)";
                    conn.Close();
                    return 1;
                }
                else
                {
                    v.Items.Clear();
                    lv.Text = "Showing 0 Results";
                    return 2;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 0;
            }
        }
    }
}

