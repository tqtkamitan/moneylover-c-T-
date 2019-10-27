﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Dapper;
using STK.Entity;
using System.Collections;

namespace STK
{
    public partial class FrmDSSTK : Form
    {
        public FrmDSSTK()
        {
            InitializeComponent();
            TheTietKiems = new List<TheTietKiem>();
        }
        public string GEmail { get; set; }
        public string Key { get; set; }
        private List<TheTietKiem> TheTietKiems { get; set; }
        public SqlConnection Kn()
        {
            return Utilities.getConnect();
        }
        public void KetNoiCSDL()
        {
            string sql = "SELECT * FROM NganHang AS A INNER JOIN TheTietKiem AS B ON A.MaNganHang = B.MaNganHang and B.TatToan = 0 and B.Email = '"+LoginInfo.UserID+"'";
            List<NganHang> list = new List<NganHang>();
            using (var connection = Kn())
            {
                var nganHangDictionary = new Dictionary<string, NganHang>();


                list = connection.Query<NganHang, TheTietKiem, NganHang>(
                    sql,
                    (nganhang, thetietkiem) =>
                    {
                        NganHang nganHangEntry;

                        if (!nganHangDictionary.TryGetValue(nganhang.MaNganHang, out nganHangEntry))
                        {
                            nganHangEntry = nganhang;
                            nganHangEntry.TheTietKiems = new List<TheTietKiem>();
                            nganHangDictionary.Add(nganHangEntry.MaNganHang, nganHangEntry);
                        }
                        nganHangEntry.TheTietKiems.Add(thetietkiem);
                        return nganHangEntry;
                    })
                .Distinct()
                .ToList();
            }
            // Dinh dang cho gridview giong voi file Yeu cau
            int soLuongSo = 0;
            // Lay index cua những row chứa tên ngân hàng để style
            List<int> indexBankNameRow = new List<int>();
            // Tên ngân hàng đầu tiên sẽ năm ở row 0 của datagridview
            indexBankNameRow.Add(0);
            foreach(NganHang banks in list)
            {
                soLuongSo += banks.TheTietKiems.Count;
                // Tong tien cua moi loai ngan hang
                double sum = 0;
                banks.TheTietKiems.ForEach(theTietKiem =>
                {
                    sum += theTietKiem.SoTienGui;
                });
                // Add ten Ngan hang cho moi nhom
                TheTietKiems.Add(new TheTietKiem
                {
                    MaSo = banks.TenNganHang,
                    SoTienGui = sum
                });
                // Add ngan hang theo group
                TheTietKiems.AddRange(banks.TheTietKiems);
                // Add index cua row ten ngan hang
                indexBankNameRow.Add(indexBankNameRow[indexBankNameRow.Count - 1] + banks.TheTietKiems.Count + 1);
            }
            // Fill data into GridView
            dataGridView1.DataSource = TheTietKiems;
            // Invisible some columns not being needed
            dataGridView1.Columns["MaNganHang"].Visible = false;
            dataGridView1.Columns["KhiDenHan"].Visible = false;
            dataGridView1.Columns["KhongKyHan"].Visible = false;
            dataGridView1.Columns["TatToan"].Visible = false;
            dataGridView1.Columns["TienLai"].Visible = false;
            dataGridView1.Columns["TraLai"].Visible = false;
            dataGridView1.Columns["Email"].Visible = false;

            // Style for BankName row 
            for (int i =0; i< indexBankNameRow.Count-1; i++)
            {
                dataGridView1.Rows[indexBankNameRow[i]].DefaultCellStyle.Font = new Font("Arial", 10);
                dataGridView1.Rows[indexBankNameRow[i]].DefaultCellStyle.ForeColor = Color.Red;
            }
            // So luong so
            lbnSo.Text = soLuongSo.ToString();
            // 
            dataGridView2.Columns["Xem"].DataPropertyName = "Xem";
        }
        void KetNoiCSDLTatToan()
        {
            List<NganHang> list = new List<NganHang>();
            SqlConnection cnn = Kn();
            string sql1 = "SELECT t.ID , t.MaSo , t.SoTienGui ,t.KyHan,t.NgayGui, t.LaiSuat, n.TenNganHang, n.MaNganHang FROM TheTietKiem t, NganHang n Where Email = '" + lbnGetEmail.Text + 
                "' and TatToan = 1 and t.MaNganHang = n.MaNganHang";
            cnn.Open();
            SqlCommand com = new SqlCommand(sql1, cnn);
            com.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            da.Fill(dt);  // đổ dữ liệu vào kho
            cnn.Close();  // đóng kết nối
            dataGridView2.DataSource = dt; 
        }
        private void FrmDSSTK_Load(object sender, EventArgs e)
        {
            //lbnGetEmail.Text = gEmail;
            lbnGetEmail.Text = LoginInfo.UserID;
            lbnKey.Text = Key;
            MaximizeBox = false;
            MinimizeBox = false;// do data vao datagitbiew ak
            KetNoiCSDL();
            KetNoiCSDLTatToan();
            TongTien();
        }

        private void FrmDSSTK_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DialogResult TL;
            //TL = MessageBox.Show("Bạn thật sự muốn thoát ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            //if (TL == DialogResult.No)
            //{
            //    e.Cancel = true;
            //}
        }

        private void picPlusSTK_Click(object sender, EventArgs e)
        {
            FRMThemSTK stk = new FRMThemSTK();
            stk.G_Email = lbnGetEmail.Text;
            stk.Owner = this;
            stk.Show();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = new DataGridViewRow();
                row = dataGridView1.Rows[e.RowIndex];
                lbnKey.Text = row.Cells[0].Value.ToString();
                lbnKyHan.Text = row.Cells[9].Value.ToString();
                if (lbnKyHan.Text == "Không kỳ hạn")
                {
                    lbnIntKyHan.Text = "0";
                }
                else if (lbnKyHan.Text == "1 tháng")
                {
                    lbnIntKyHan.Text = "1";
                }
                else if (lbnKyHan.Text == "3 tháng")
                {
                    lbnIntKyHan.Text = "3";
                }
                else if (lbnKyHan.Text == "6 tháng")
                {
                    lbnIntKyHan.Text = "6";
                }
                else if (lbnKyHan.Text == "12 tháng")
                {
                    lbnIntKyHan.Text = "12";
                }
                btnEdit.Enabled = true;
                btnGuiThem.Enabled = true;
                btnRut1Phan.Enabled = true;
                btnTatToan.Enabled = true;
            } catch (Exception)
            {
                return;
            }
            
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            var key = dataGridView1.SelectedRows[0].Cells[0].Value;
            var email = GEmail;
            FrmAddEditSTK f = new FrmAddEditSTK() { Key = key.ToString() };
            f.Owner = this;
            f.Show();
        }

        void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            FrmDSSTK frm = new FrmDSSTK();
            frm.GEmail = LoginInfo.UserID;
            frm.Show();
        }
        void TongTien()
        {

            SqlConnection cnn = Kn();
            string sql1 = "SELECT SUM(SoTienGui)  FROM TheTietKiem WHERE Email='" + lbnGetEmail.Text + "' and TatToan = 0";
            cnn.Open();
            SqlCommand com = new SqlCommand(sql1, cnn);
            SqlDataReader dr = com.ExecuteReader();
            if (dr.Read())
            {
                string f = dr[0].ToString();
                string ff = String.Format("{0:0,0}", dr[0].ToString());
                lbnTongTien.Text= ff;                
            }
            
        }
        void btnGuiThem_Click(object sender, EventArgs e)
        {
            var key = dataGridView1.SelectedRows[0].Cells[0].Value;
            FrmGuiThem f = new FrmGuiThem() { Key = key.ToString() };
            f.Owner = this;
            f.Show();
        }
        bool getMonth()
        {
            int month;
            DateTime Time1, Time2;
            TimeSpan nowtime;
            SqlConnection cnn = Kn();
            cnn.Open();
            string sql = "SELECT NgayGui FROM TheTietKiem WHERE ID = '" + lbnKey.Text + "'";
            SqlCommand cm = new SqlCommand(sql, cnn);
            SqlDataReader dr = cm.ExecuteReader(); ;

            if(dr.Read())
            {
                Time1 = Convert.ToDateTime(dr[0].ToString());
                int a = Time1.Month;
                Time2 = Convert.ToDateTime(DateTime.Now.ToString());
                nowtime = Time2.Subtract(Time1);
                month = nowtime.Days / 30;
                if(month < int.Parse(lbnIntKyHan.Text))
                {
                    return false;
                }
            }
            cnn.Close();
            return true;
        }
        //string KH;
        private void btnTatToan_Click(object sender, EventArgs e)
        {
            double soTienGui = 0;
            double laiKhongKyHan = 0;
            double kyHan = 0;
            SqlConnection cnn = Kn();
            if (getMonth() == true)
            {
                try
                {
                    string sql = "SELECT SoTienGui,LaiSuat, LaiKhongKyHan FROM TheTietKiem WHERE ID = '" + lbnKey.Text + "'";
                    cnn.Open();
                    SqlCommand cm = new SqlCommand(sql, cnn);
                    SqlDataReader dr = cm.ExecuteReader();
                    if (dr.Read())
                    {
                        soTienGui = double.Parse(dr[0].ToString());
                        var LS = double.Parse(dr[1].ToString());
                        laiKhongKyHan = double.Parse(dr[2].ToString());
                        cnn.Close();
                        kyHan = double.Parse(lbnIntKH.Text);
                        var tongLai = soTienGui * LS/100 * kyHan / 12;
                        string sql2 = "UPDATE TheTietKiem SET TatToan = 1 , TienLai ='" + tongLai + "' Where ID = '" + lbnKey.Text + "' ";
                        cnn.Open();
                        SqlCommand cmd = new SqlCommand(sql2, cnn);
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                        DialogResult T;
                        T = MessageBox.Show("Tất toán sổ thành công !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch
                {
                    MessageBox.Show("Không tất toán được !");
                }
            }
            else
            {
                try
                {
                    var tongLai = soTienGui * laiKhongKyHan * kyHan/ 12;
                    string sql2 = "UPDATE TheTietKiem SET TatToan = 1 , TienLai ='" + tongLai + "' Where ID = '" + lbnKey.Text + "' ";
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand(sql2, cnn);
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                    DialogResult T;
                    T = MessageBox.Show("Tất toán sổ thành công !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Không tất toán được !");
                }
            }
        }

        private void btnRut1Phan_Click(object sender, EventArgs e)
        {
            var k = dataGridView1.SelectedRows[0].Cells[0].Value;
            FrmRut1Phan r = new FrmRut1Phan() { GKey = k.ToString()};
            r.Owner = this;
            r.Show();
        }
        // Not displaying 0, string.Empty instead.
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].DataPropertyName == "LaiSuat")
            {
                int value = Convert.ToInt32(e.Value);
                if (value == 0)
                {
                    e.Value = string.Empty;
                    e.FormattingApplied = true;
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            // Neu contentcell la button 
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                string sql = "select t.MaSo , t.SoTienGui ,t.KyHan,t.NgayGui, t.LaiSuat FROM TheTietKiem t Where Email = '" + lbnGetEmail.Text +
                "' and TatToan = 1 and t.MaSo = '" + dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString().Trim() + "'";
                try
                {
                    using (var connection = Kn())
                    {
                        var soTietKiem = connection.Query(sql).FirstOrDefault();
                        DialogResult result = MessageBox.Show(string.Format("Thông tin chi tiết: \n\n - Số tài khoản: {0}\n - Số dư hiện tại: {1}\n - Kỳ hạn gửi: {2}\n - Lãi suất năm: {3}\n - Ngày gửi: {4}", soTietKiem.MaSo, soTietKiem.SoTienGui, soTietKiem.LaiSuat, soTietKiem.KyHan, soTietKiem.NgayGui), "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } catch(Exception)
                {
                    MessageBox.Show("Lỗi xảy ra! Vui lòng thử lại.");
                }
                
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].DataPropertyName == "Xem")
            {
                int value = Convert.ToInt32(e.Value);
                if (value == 0)
                {
                    e.Value = "Chi tiết";
                    e.FormattingApplied = true;
                }
            }
        }

        private void lbnIntKH_Click(object sender, EventArgs e)
        {

        }

        private void FrmDSSTK_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
