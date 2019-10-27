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
using System.Text.RegularExpressions;

namespace STK
{
    public partial class FrmAddEditSTK : Form
    {
        public string Key { get; set; }

        public FrmAddEditSTK()
        {
            InitializeComponent();
        }

        private void FrmAddEditSTK_Load(object sender, EventArgs e)
        {
            FrmDSSTK f = new FrmDSSTK();
            KetNoiCSDL();
        }
        public SqlConnection Kn()
        {
            return Utilities.getConnect();
        }
        public void KetNoiCSDL()
        {

            SqlConnection cnn = Kn();
            string sql1 = "SELECT t.ID , t.MaSo , t.NgayGui ,t.SoTienGui, t.LaiSuat,t.KhiDenHan,t.TraLai ,t.KyHan, n.TenNganHang," +
                "t.LaiKhongKyHan,t.MaNganHang FROM TheTietKiem t, NganHang n Where ID = " + Key + " and t.MaNganHang = n.MaNganHang";
            cnn.Open();
            SqlCommand cmd = new SqlCommand(sql1, cnn);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                txtMaSo.Text = dr[1].ToString();
                dateTimePicker1.Value = DateTime.Parse(dr[2].ToString());
                txtSoTienGui.Text = dr[3].ToString();
                txtLaiSuat.Text = dr[4].ToString();
                cbbKhiDenHan.SelectedText = dr["KhiDenHan"].ToString();
                cbbTraLai.SelectedText = dr["TraLai"].ToString();
                cbbKyHan.SelectedText = dr[7].ToString();
                 txtLKKH.Text = dr[9].ToString();
                txtNganHang.Text = dr["TenNganHang"].ToString();

            }
            cnn.Close();

        }

        private void bthHoanThanh_Click(object sender, EventArgs e)
        {

            //var prm = new SqlParameter[] {
            //    new SqlParameter("@ngaynGui", dateTimePicker1.Value),
            //    new SqlParameter("@soTienGui",txtSoTienGui.Text),
            //    new SqlParameter("@laiSuat",txtLaiSuat.Text),
            //    new SqlParameter("@khiDenHan",cbbKhiDenHan.SelectedText),
            //    new SqlParameter("@traLai",cbbTraLai.SelectedText),
            //    new SqlParameter("@kyHan",cbbKyHan.SelectedText),
            //    new SqlParameter("@laiKhongKyHan",txtLKKH)
            //};
            if (string.IsNullOrEmpty(cbbKhiDenHan.Text) || string.IsNullOrEmpty(cbbKyHan.Text) || string.IsNullOrEmpty(cbbTraLai.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin !");
            }
            else
            {
                try
                {

                    SqlConnection cnn = Kn();
                    string sql2 = "UPDATE TheTietKiem SET NgayGui=@ngayGui, SoTienGui=@soTienGui, LaiSuat=@laiSuat,KhiDenHan=@khiDenHan,TraLai=@traLai,KyHan=@kyHan,LaiKhongKyHan=@laiKhongKyHan Where ID ='" + Key + "'";
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand(sql2, cnn);
                    cmd.Parameters.AddWithValue("@ngayGui", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@soTienGui", Int32.Parse(txtSoTienGui.Text));
                    cmd.Parameters.AddWithValue("@laiSuat", Int32.Parse(txtLaiSuat.Text));
                    cmd.Parameters.AddWithValue("@khiDenHan", cbbKhiDenHan.Text);
                    cmd.Parameters.AddWithValue("@traLai", cbbTraLai.Text);
                    cmd.Parameters.AddWithValue("@kyHan", cbbKyHan.Text);
                    cmd.Parameters.AddWithValue("@laiKhongKyHan", double.Parse(txtLKKH.Text));
                    //cmd.Parameters.AddWithValue("@tatToan", labTatToan.Text);
                    cmd.ExecuteNonQuery();
                    cnn.Close();

                    DialogResult T;
                    T = MessageBox.Show("Sửa thông tin sổ thành công !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (T == DialogResult.OK)
                    {
                        this.Close();
                    }
                }
                catch
                {
                    MessageBox.Show("Sửa thông tin thất bại !");
                }

            }

        }

        private void cbbKhiDenHan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbKhiDenHan.SelectedItem.ToString() == "Tái tục gốc và lãi")
            {
                labkhidenhan.Text = "Tái tục gốc và lãi";
                labTatToan.Text = "false";
            }
            else if (cbbKhiDenHan.SelectedItem.ToString() == "Tái tục gốc")
            {
                labkhidenhan.Text = "Tái tục gốc";
                labTatToan.Text = "false";
            }
            else if (cbbKhiDenHan.SelectedItem.ToString() == "Tất toán sổ")
            {
                labkhidenhan.Text = "Tất toán sổ";
                labTatToan.Text = "true";
            }
        }

        private void FrmAddEditSTK_FormClosed(object sender, FormClosedEventArgs e)
        {
            FrmDSSTK frm = new FrmDSSTK();
            frm.GEmail = LoginInfo.UserID;
            frm.Show();
        }

        private void FrmAddEditSTK_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.FormOwnerClosing)
                Owner.Hide();
        }

        string previousInput = "0";
        private void txtLaiSuat_TextChanged(object sender, EventArgs e)
        {
            Regex r = new Regex("^[0-9]+$"); // This is the main part, can be altered to match any desired form or limitations
            Match m = r.Match(txtSoTienGui.Text);
            if (m.Success)
            {
                previousInput = txtLaiSuat.Text;
            }
            else
            {
                txtLaiSuat.Text = previousInput;
            }
            double laiXuat;
            double.TryParse(txtLaiSuat.Text, out laiXuat);
            if (laiXuat > 10) txtLaiSuat.Text = "10";
        }
    }
}
