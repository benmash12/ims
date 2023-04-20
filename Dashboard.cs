using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ims
{
    public partial class Dashboard : Form
    {
        private int childFormNumber = 0;
        private Products p;
        private Expenses ex;
        private string curr = "p";
        public Dashboard()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }



        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            p = new Products();
            p.FormBorderStyle = FormBorderStyle.None;
            p.MdiParent = this;
            p.Dock = DockStyle.Fill;
            p.Tag = "";
            ex = new Expenses();
            ex.FormBorderStyle = FormBorderStyle.None;
            ex.MdiParent = this;
            ex.Dock = DockStyle.Fill;
            p.Show();
            Loader.LoadCat(p);
            Control l = ex.Controls.Find("dataGridView1", true)[0];
            DataGridView v = (DataGridView)l;
            Loader.LoadExp(v,ex);
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!curr.Equals("p"))
            {
                p.Show();
                ex.Hide();
                curr = "p";
            }
        }

        private void incomeAndExpensesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!curr.Equals("ex"))
            {
                ex.Show();
                p.Hide();
                curr = "ex";
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string abt = "Inventory Management System v 1.0.0 \n Developed for DNT 6th Sem Project 2023.";
            MessageBox.Show(abt, "ABOUT", MessageBoxButtons.OK);
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cr = MessageBox.Show("Ar you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo);
            if (cr == DialogResult.Yes)
            {
                var f = new LoginForm();
                f.Show();
                this.Hide();
            }
        }

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pv = Prompt.ShowDialog("Please type in new category", "New Category");
            string exp = @"[^a-zA-Z0-9\s]";
            Match m = Regex.Match(pv, exp, RegexOptions.IgnoreCase);
            if (pv.Equals(string.Empty))
            {
                Prompt.Err("Category is required!");
            }
            else if (m.Success)
            {
                Prompt.Err("Category cannot contain anything other than letters, numbers and spaces!");
            }
            else
            {
                DB d = new DB();
                int r = d.addCat(pv);
                if( r == 1)
                {
                    Prompt.Suc("Category Added!");
                    Loader.LoadCat(p);
                    
                }
                else
                {
                    Prompt.Err("Operation Failed! Category could not be added!");
                }
            }
        }

        private void productToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control l = p.Controls.Find("comboBox1", true)[0];
            ComboBox v = (ComboBox)l;
            if (v.SelectedIndex > -1)
            {
                string cat = v.Text;
                string pv = Prompt.ShowDialog("Please enter name of product", "New Product");
                string exp = @"[^a-zA-Z0-9\s\-_]";
                Match m = Regex.Match(pv, exp, RegexOptions.IgnoreCase);
                if (pv.Equals(string.Empty))
                {
                    Prompt.Err("name is required!");
                }
                else if (m.Success)
                {
                    Prompt.Err("name cannot contain anything other than letters, numbers and spaces!");
                }
                else
                {
                    string nam = pv + "";
                    int qua = 0;
                    string pvx = Prompt.ShowDialog("Quantity", "New Product");
                    exp = @"[^\d]";
                    Match x = Regex.Match(pvx, exp, RegexOptions.IgnoreCase);
                    if (pvx.Equals(string.Empty))
                    {
                        //default qua
                    }
                    else if (x.Success)
                    {
                        //default qua
                    }
                    else
                    {
                        qua = Convert.ToInt32(pvx);
                        DB d = new DB();
                        if (d.addProduct(cat, nam, qua.ToString()))
                        {
                            Loader.LoadProd(p);
                        }
                        else
                        {
                            Prompt.Err("Product could not be added");
                        }
                    }
                }
            }
            else
            {
                Prompt.Err("Please select a category first.");
            }
        }

        private void expensesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pv = Prompt.ShowDialog("Title", "Add Expenses");
            string exp = @"[^a-zA-Z0-9\s\-_]";
            Match m = Regex.Match(pv, exp, RegexOptions.IgnoreCase);
            if (pv.Equals(string.Empty))
            {
                Prompt.Err("Title is required!");
            }
            else if (m.Success)
            {
                Prompt.Err("Title cannot contain anything other than letters, numbers and spaces!");
            }
            else
            {
                string nam = pv + "";
                string pvx = Prompt.ShowDialog("Amount", "Add Expenses");
                exp = @"^[\d]+(\.[\d]{1,2})?$";
                Match x = Regex.Match(pvx, exp, RegexOptions.IgnoreCase);
                if (pvx.Equals(string.Empty))
                {
                    Prompt.Err("Amount is required!");
                }
                else if (!x.Success)
                {
                    Prompt.Err("Invalid amount entered!");
                }
                else
                {
                    float amt = float.Parse(pvx);
                    DB d = new DB();
                    if (d.addExpense("Expenses", nam, amt.ToString("F2")))
                    {
                        Prompt.Suc("Expenses added successfully!");
                        Control l = ex.Controls.Find("dataGridView1", true)[0];
                        DataGridView v = (DataGridView)l;
                        Loader.LoadExp(v,ex);
                    }
                    else
                    {
                        Prompt.Err("Expenses could not be added");
                    }
                }
            }
        }

        private void incomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pv = Prompt.ShowDialog("Title", "Add Income");
            string exp = @"[^a-zA-Z0-9\s\-_]";
            Match m = Regex.Match(pv, exp, RegexOptions.IgnoreCase);
            if (pv.Equals(string.Empty))
            {
                Prompt.Err("Title is required!");
            }
            else if (m.Success)
            {
                Prompt.Err("Title cannot contain anything other than letters, numbers and spaces!");
            }
            else
            {
                string nam = pv + "";
                string pvx = Prompt.ShowDialog("Amount", "Add Income");
                exp = @"^[\d]+(\.[\d]{1,2})?$";
                Match x = Regex.Match(pvx, exp, RegexOptions.IgnoreCase);
                if (pvx.Equals(string.Empty))
                {
                    Prompt.Err("Amount is required!");
                }
                else if (!x.Success)
                {
                    Prompt.Err("Invalid amount entered!");
                }
                else
                {
                    float amt = float.Parse(pvx);
                    DB d = new DB();
                    if (d.addExpense("Income", nam, amt.ToString("F2")))
                    {
                        Prompt.Suc("Income added successfully!");
                        Control l = ex.Controls.Find("dataGridView1", true)[0];
                        DataGridView v = (DataGridView)l;
                        Loader.LoadExp(v, ex);
                    }
                    else
                    {
                        Prompt.Err("Income could not be added");
                    }
                }
            }
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pv = Prompt.ShowDialog("New Password", "Change Password",true);
            string exp = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
            Match m = Regex.Match(pv, exp, RegexOptions.IgnoreCase);
            if (pv.Equals(string.Empty))
            {
                Prompt.Err("New Password is required!");
            }
            else if (!m.Success)
            {
                Prompt.Err("New Password is too weak. Must contain at least one lowercase, uppercase, digit, symbol and must be at least 8 chars long.");
            }
            else
            {
                string pvx = Prompt.ShowDialog("Confirm New Password", "Change Password",true);
                if(pvx != pv)
                {
                    Prompt.Err("Passwords do not match!");
                }
                else
                {
                    string pvxx = Prompt.ShowDialog("Current Password", "Change Password",true);
                    pvxx = pvxx.Replace("'", "");
                    pvxx = pvxx.Replace("\"", "");
                    DB d = new DB();
                    int xx = d.changePass(pv, pvxx);
                    if (xx == 1)
                    {
                        Prompt.Suc("Password changed successfully!");
                    }
                    else if(xx == 420)
                    {
                        Prompt.Err("Database error encountered.");
                    }
                    else
                    {
                        Prompt.Err("Operation not succesful. Affected Rows = "+xx.ToString());
                    }
                }
            }
        }
    }
    public static class Loader
    {
        public static void LoadCat(Form p)
        {
            Control l = p.Controls.Find("comboBox1", true)[0];
            ComboBox v = (ComboBox)l;
            DB d = new DB();
            d.LoadCat(v);
        }
        public static void LoadExp(DataGridView dgv,Form ex) {
            DB d = new DB();
            Control l1 = ex.Controls.Find("label1", true)[0];
            Label t1 = (Label)l1;
            Control l2 = ex.Controls.Find("label3", true)[0];
            Label t2 = (Label)l2;
            Control l3 = ex.Controls.Find("label4", true)[0];
            Label t3 = (Label)l3;
            if (!d.loadExp(dgv,t1,t2,t3))
            {
                Prompt.Err("Data table could not be refreshed.");
            }
        }
        public static void LoadProd(Form p)
        {
            Control l = p.Controls.Find("comboBox1", true)[0];
            ComboBox v = (ComboBox)l;
            if (v.SelectedIndex > -1)
            {
                string sea = (string)p.Tag;
                if (sea.Equals(string.Empty))
                {
                    sea = "";
                }
                string cat = v.Text;
                l = p.Controls.Find("listBox1", true)[0];
                ListBox vx = (ListBox)l;
                Control lx = p.Controls.Find("label3", true)[0];
                Label vc = (Label) lx;
                DB d = new DB();
                int xx = d.LoadProd(cat, vx, vc);
                if (xx == 0)
                {
                    Prompt.Err("Error loading products");
                }
                else if(xx == 2)
                {
                    Prompt.Inf("No products added yet in this category.");
                }
            }
            else
            {
                Prompt.Err("Category not selected. Could not load products");
            }
        }
    }
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, bool pax = false)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 20, Top = 15, Text = text };
            TextBox textBox = new TextBox() { Left = 20, Top = 35, Width = 240 };
            Button confirmation = new Button() { Text = "Ok", Left = 150, Width = 110, Top = 70, DialogResult = DialogResult.OK };
            if (pax)
            {
                textBox.PasswordChar = '*';
            }
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            
        }
        public static void Err(string text)
        {
            MessageBox.Show(text, "ERROR", MessageBoxButtons.OK);
        }
        public static void Inf(string text)
        {
            MessageBox.Show(text, "INFO", MessageBoxButtons.OK);
        }
        public static void Suc(string text)
        {
            MessageBox.Show(text, "SUCCESS", MessageBoxButtons.OK);
        }
    }

}
