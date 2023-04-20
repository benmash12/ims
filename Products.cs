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
    public partial class Products : Form
    {
        public Products()
        {
            InitializeComponent();
        }

        private void Products_Load(object sender, EventArgs e)
        {
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex > -1)
            {
                string cat = comboBox1.Text;
                DB d = new DB();
                if (d.deleteCat(cat, textBox2, textBox3, textBox4, textBox5, textBox6))
                {
                    Loader.LoadProd(this);
                    comboBox1.Text = "";
                    Prompt.Suc("category is deleted.");
                    d = new DB();
                    Control l = this.Controls.Find("comboBox1", true)[0];
                    ComboBox v = (ComboBox)l;
                    d.LoadCat(v);
                    Control lx = this.Controls.Find("listBox1", true)[0];
                    ListBox vx = (ListBox)lx;
                    vx.Items.Clear();
                }
                else
                {
                    Prompt.Err("category could not be deleted.");
                }
            }
            else
            {
                Prompt.Err("No category is selected.");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Loader.LoadProd(this);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string txt = listBox1.Text;
            int i = txt.LastIndexOf(": ") + 1;
            int j = txt.LastIndexOf(")") - i;
            string id = txt.Substring(i, j);
            DB d = new DB();
            if (d.LoadProduct(id, textBox2, textBox3, textBox4, textBox5, textBox6))
            {

            }
            else
            {
                Prompt.Err("Product could not be loaded!");
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try { 
                int curr = Convert.ToInt32(textBox5.Text);
                textBox5.Text = (++curr).ToString();
            }
            catch(Exception ex)
            {
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try { 
                int curr = Convert.ToInt32(textBox5.Text);
                if(curr > 0)
                {
                    textBox5.Text = (--curr).ToString();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(textBox2.Text);
                string name = textBox4.Text;
                int quan = int.Parse(textBox5.Text);
                string err = "\n";
                string exp = @"[^a-zA-Z0-9\s\-_]";
                Match m = Regex.Match(name, exp, RegexOptions.IgnoreCase);
                if (name.Equals(string.Empty))
                {
                    err += "name is required!\n";
                }
                else if (m.Success)
                {
                    err += "name cannot contain anything other than letters, numbers and spaces!\n";
                }
                exp = @"[^\d]";
                Match x = Regex.Match(quan.ToString(), exp, RegexOptions.IgnoreCase);
                if (x.Success)
                {
                    err += "illegal quantity!\n";
                }
                if (!err.Equals("\n"))
                {
                    Prompt.Err(err);
                }
                else
                {
                    DB d = new DB();
                    if (d.updateProduct(id.ToString(), name, quan.ToString(), textBox6))
                    {
                        Prompt.Suc("Product updated!");
                        Loader.LoadProd(this);
                    }
                    else
                    {
                        Prompt.Err("Product not updated. An error was encountered.");
                    }
                }
            }
            catch(Exception ex)
            {
                Prompt.Err(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(textBox2.Text);
                var cr = MessageBox.Show("Ar you sure you want to delete this product?", "Confirm Deletion", MessageBoxButtons.YesNo);
                if (cr == DialogResult.Yes)
                {
                    DB d = new DB();
                    if (d.deleteProduct(id.ToString(), textBox2, textBox3, textBox4, textBox5, textBox6))
                    {
                        Prompt.Suc("Product deleted!");
                        Loader.LoadProd(this);
                    }
                    else
                    {
                        Prompt.Err("Product not updated. An error was encountered.");
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.Err(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string txt = textBox1.Text;
            if (!txt.Equals(string.Empty))
            {
                Control l = this.Controls.Find("comboBox1", true)[0];
                ComboBox v = (ComboBox)l;
                if (v.SelectedIndex > -1)
                {
                    string cat = v.Text;
                    l = this.Controls.Find("listBox1", true)[0];
                    ListBox vx = (ListBox)l;
                    Control lx = this.Controls.Find("label3", true)[0];
                    Label vc = (Label)lx;
                    DB d = new DB();
                    int xx = d.LoadProdSearch(cat, vx, vc, txt);
                    if (xx == 0)
                    {
                        Prompt.Err("Error loading products");
                    }
                    else if (xx == 2)
                    {
                        Prompt.Inf("No search results found.");
                    }
                }
                else
                {
                    Prompt.Err("Category not selected. Could not load products");
                }
            }
            else {
                Prompt.Err("Please type something in the search box.");
            }
        }

    }
}
