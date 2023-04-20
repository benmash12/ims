using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ims
{
    public partial class Expenses : Form
    {
        string delrow = "0";
        public Expenses()
        {
            InitializeComponent();
        }

        private void Expenses_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'imsxDataSet2.ims_inc_exp' table. You can move, or remove it, as needed.
            this.ims_inc_expTableAdapter.Fill(this.imsxDataSet2.ims_inc_exp);

        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            Loader.LoadExp(dataGridView1,this);
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DB d = new DB();
            d.delExp(delrow);
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                delrow = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            }
            catch(Exception ex)
            {
                Prompt.Err(ex.Message);
            }
        }
    }
}
