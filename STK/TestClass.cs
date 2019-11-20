using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace STK
{
    [TestFixture]
    class TestClass
    {
        [Test]
        public void CompareMoney()
        {
            FrmRut1Phan frm = new FrmRut1Phan();
            Assert.AreEqual(true, frm.compareMoney(10000, "2016"));

            Assert.AreEqual(false, frm.compareMoney(35000001, "2016"));
        }

        [Test]
        public void GetDays()
        {
            FrmRut1Phan frm = new FrmRut1Phan();
            Assert.AreEqual(283, frm.getDays("10"));

            Assert.AreEqual(285, frm.getDays("10"));
        }

        [Test]
        public void getMonth()
        {
            FrmRut1Phan frm = new FrmRut1Phan();
            Assert.AreEqual(9, frm.gMonth("10"));

            Assert.AreEqual(12, frm.gMonth("10"));
        }

        [Test]
        public void kyHan()
        {
            FrmRut1Phan frm = new FrmRut1Phan();
            Assert.AreEqual("12 tháng", frm.getKyHan("10"));

            Assert.AreEqual("10 tháng", frm.getKyHan("10"));
        }
        [Test]
        public void true0KyHan()
        {
            FrmRut1Phan frm = new FrmRut1Phan();
            Assert.AreEqual("12 tháng", frm.true0KyHan("10"));

            Assert.AreEqual("Không kỳ hạn", frm.true0KyHan("10"));
        }
        [Test]
        public void kiemTraKyHan()
        {
            FrmRut1Phan frm = new FrmRut1Phan();
            Assert.AreEqual(12, frm.kiemTraKyHan("10"));

            Assert.AreEqual(3, frm.kiemTraKyHan("10"));
        }
        [Test]
        public void Rut()
        {
            FrmRut1Phan frm = new FrmRut1Phan();
            Assert.AreEqual(true, frm.Rut(0,"10"));

            Assert.AreEqual(false, frm.Rut(10000,"10"));
        }
        [Test]
        public void denHan()
        {
            FrmRut1Phan frm = new FrmRut1Phan();
            Assert.AreEqual(true, frm.denHan("10"));

            Assert.AreEqual(false, frm.denHan("10"));
        }

        [Test]
        public void isEmail()
        {
            frmLogin frm = new frmLogin();
            Assert.AreEqual(true, frm.isEmail("QuangTan@gmail.com"));

            Assert.AreEqual(false, frm.isEmail("QuangTan@gmail.com"));
        }

        [Test]
        public void ValidatePassword()
        {
            frmLogin frm = new frmLogin();
            Assert.AreEqual(true, frm.ValidatePassword("-abcd1234"));

            Assert.AreEqual(false, frm.ValidatePassword("-abcd1234"));
        }
    }
}
