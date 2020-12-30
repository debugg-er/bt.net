﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChuDe1_Nhom3
{
    public partial class frmTaikhoan : Form
    {
        private string action;
        private DataTable nguoiSD;

        public frmTaikhoan()
        {
            InitializeComponent();
        }

        private void frmTaikhoan_Load(object sender, EventArgs e)
        {
            dsTKGridView.AllowUserToAddRows = false;

            // set data cho combobox
            DataTable table = new DataTable();
            table.Columns.Add("QuyenSD");
            table.Rows.Add("User");
            table.Rows.Add("AdTruong");
            table.Rows.Add("AdTinh");

            quyenSDCBBox.DataSource = table;
            quyenSDCBBox.DisplayMember = "QuyenSD";
            quyenSDCBBox.ValueMember = "QuyenSD";

            quyenSDCBBox.SelectedValue = "";

            // set data cho data ma tt combobox
            if (MyPublic.connection.State == ConnectionState.Closed)
            {
                MyPublic.connection.Open();
            }

            string query = "SELECT MaTT, TenTT FROM TrungTam";
            DataSet dataSet = new DataSet();
            MyPublic.OpenData(query, dataSet, "TrungTam");

            maTTCBBox.DataSource = dataSet.Tables["TrungTam"];
            maTTCBBox.DisplayMember = "TenTT";
            maTTCBBox.ValueMember = "MaTT";

            // set data cho data grid view
            query = "SELECT TenTaiKhoan, QuyenSD, MaTT FROM NguoiSuDung";
            dataSet = new DataSet();
            MyPublic.OpenData(query, dataSet, "NguoiSuDung");
            nguoiSD = dataSet.Tables["NguoiSuDung"];

            dsTKGridView.DataSource = dataSet.Tables["NguoiSuDung"];
            dsTKGridView.Columns[0].Width = 125;
            dsTKGridView.Columns[1].Width = 125;
            dsTKGridView.Columns[2].Width = 125;

            // highlight cac button
            if (MyPublic.quyenSD == "AdTinh")
            {
                themBtn.Enabled = true;
                suaBtn.Enabled = true;
                xoaBtn.Enabled = true;
            }
            else
            {
                themBtn.Enabled = false;
                suaBtn.Enabled = false;
                xoaBtn.Enabled = false;
            }
            luuBtn.Enabled = false;
            khongluuBtn.Enabled = false;

            // select tai khoan dau tien
            displayRowAt(0);
        }

        private void dsTKGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            displayRowAt(dsTKGridView.CurrentRow.Index);
        }

        private void themBtn_Click(object sender, EventArgs e)
        {
            action = "them";
            tenTKTextBox.Text = "";
            quyenSDCBBox.SelectedValue = "";
            maTTCBBox.SelectedValue = "";

            themBtn.Enabled = false;
            suaBtn.Enabled = false;
            xoaBtn.Enabled = false;
            dsTKGridView.Enabled = false;
            luuBtn.Enabled = true;
            khongluuBtn.Enabled = true;
        }

        private void suaBtn_Click(object sender, EventArgs e)
        {
            action = "sua";

            themBtn.Enabled = false;
            suaBtn.Enabled = false;
            xoaBtn.Enabled = false;
            dsTKGridView.Enabled = false;
            luuBtn.Enabled = true;
            khongluuBtn.Enabled = true;
        }


        private void luuBtn_Click(object sender, EventArgs e)
        {
            if (tenTKTextBox.Text == "" || quyenSDCBBox.SelectedValue == "" || maTTCBBox.SelectedValue == "")
            {
                MessageBox.Show("Không được để trống thông tin!", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            if (MyPublic.connection.State == ConnectionState.Closed)
            {
                MyPublic.connection.Open();
            }

            string query;
            SqlCommand command;
            int currentRow = dsTKGridView.CurrentRow.Index;

            if (action == "them")
            {
                query = "INSERT INTO NguoiSuDung VALUES(@TAIKHOAN, @MATKHAU, @QUYENSD, @MATT)";
                command = new SqlCommand(query, MyPublic.connection);
                command.Parameters.AddWithValue("@TAIKHOAN", tenTKTextBox.Text);
                command.Parameters.AddWithValue("@QUYENSD", quyenSDCBBox.SelectedValue);
                command.Parameters.AddWithValue("@MATT", maTTCBBox.SelectedValue);
                command.Parameters.AddWithValue("@MATKHAU", showPrompt()); ;

                nguoiSD.Rows.Add(tenTKTextBox.Text, quyenSDCBBox.SelectedValue, maTTCBBox.SelectedValue);
            }
            else
            {
                query = "UPDATE NguoiSuDung SET TenTaiKhoan=@TAIKHOAN, QuyenSD=@QUYENSD, MaTT=@MATT WHERE TenTaiKhoan=@TAIKHOANGOC";
                command = new SqlCommand(query, MyPublic.connection);
                command.Parameters.AddWithValue("@TAIKHOAN", tenTKTextBox.Text);
                command.Parameters.AddWithValue("@QUYENSD", quyenSDCBBox.SelectedValue);
                command.Parameters.AddWithValue("@MATT", maTTCBBox.SelectedValue);
                command.Parameters.AddWithValue("@TAIKHOANGOC", nguoiSD.Rows[currentRow]["TenTaiKhoan"]);

                nguoiSD.Rows[currentRow]["TenTaiKhoan"] = tenTKTextBox.Text;
                nguoiSD.Rows[currentRow]["QuyenSD"] = quyenSDCBBox.SelectedValue;
                nguoiSD.Rows[currentRow]["MaTT"] = maTTCBBox.SelectedValue;
            }

            command.ExecuteNonQuery();

            dsTKGridView.Enabled = true;
            themBtn.Enabled = true;
            suaBtn.Enabled = true;
            xoaBtn.Enabled = true;
            luuBtn.Enabled = false;
            khongluuBtn.Enabled = false;
        }

        private void khongluuBtn_Click(object sender, EventArgs e)
        {
            dsTKGridView.Enabled = true;
            themBtn.Enabled = true;
            suaBtn.Enabled = true;
            xoaBtn.Enabled = true;
            luuBtn.Enabled = false;
            khongluuBtn.Enabled = false;
            displayRowAt(dsTKGridView.CurrentRow.Index);
        }

        private string showPrompt()
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Nhập mật khẩu",
                StartPosition = FormStartPosition.CenterScreen,
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = "Mật khẩu" };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 180, PasswordChar = '*' };
            Button confirmation = new Button() { Text = "Ok", Left = 160, Width = 100, Top = 78, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void displayRowAt(int index)
        {
            DataGridViewRow row = dsTKGridView.Rows[index];
            tenTKTextBox.Text = row.Cells["TenTaiKhoan"].Value.ToString();
            quyenSDCBBox.SelectedValue = row.Cells["QuyenSD"].Value.ToString();
            maTTCBBox.SelectedValue = row.Cells["MaTT"].Value.ToString();
        }

        private void dongBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void xoaBtn_Click(object sender, EventArgs e)
        {
            int currentRow = dsTKGridView.CurrentRow.Index;

            DialogResult dialog = MessageBox.Show("Bạn có thực sự muốn xóa tài khoản này!", "Thông báo", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.No) return;

            if (MyPublic.connection.State == ConnectionState.Closed)
            {
                MyPublic.connection.Open();
            }
            string query = "DELETE FROM NguoiSuDung WHERE TenTaiKhoan=@TAIKHOAN";
            SqlCommand command = new SqlCommand(query, MyPublic.connection);
            command.Parameters.AddWithValue("@TAIKHOAN", nguoiSD.Rows[currentRow][0]);

            command.ExecuteNonQuery();
            nguoiSD.Rows.RemoveAt(currentRow);
        }
    }
}
