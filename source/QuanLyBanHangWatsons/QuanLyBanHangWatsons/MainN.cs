using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{
    public partial class MainN : Form
    {
        public MainN()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        bool menuBHExpand = false;

        private void menuTransition_Tick(object sender, EventArgs e)
        {
            if (menuBHExpand == false)
            {
                pnlMenuBH.Height += 10;
                if (pnlMenuBH.Height >= 506)
                {
                    menuTransition.Stop();
                    menuBHExpand = true;
                }
            }else
            {
                pnlMenuBH.Height -= 10;
                if (pnlMenuBH.Height <= 86)
                {
                    menuTransition.Stop();
                    menuBHExpand = false;
                }
            }
        }

        private void btnBH_Click(object sender, EventArgs e)
        {
            menuTransition.Start();
        }


        bool sidebarExpand = true;

        private void sidebarTransition_Tick(object sender, EventArgs e)
        {
            if (sidebarExpand == true)
            {
                pnlLeftMain.Width -= 5;
                if (pnlLeftMain.Width <= 98)
                {
                    sidebarExpand = false;
                    sidebarTransition.Stop();

                    pnlTongQuan.Width = pnlLeftMain.Width;
                    pnlTinNhan.Width = pnlLeftMain.Width;
                    pnlNhanVien.Width = pnlLeftMain.Width;
                    pnlCaiDat.Width = pnlLeftMain.Width;
                }
            }
            else
            {
                pnlLeftMain.Width += 5;
                if(pnlLeftMain.Width >= 344)
                {
                    sidebarExpand = true;
                    sidebarTransition.Stop();

                    pnlTongQuan.Width = pnlLeftMain.Width;
                    pnlTinNhan.Width = pnlLeftMain.Width;
                    pnlNhanVien.Width = pnlLeftMain.Width;
                    pnlCaiDat.Width = pnlLeftMain.Width;
                }
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            sidebarTransition.Start();
        }
      

        bool menuMHExpand = false;

        private void menuMHTrasition_Tick(object sender, EventArgs e)
        {
            if (menuMHExpand == false)
            {
                pnlMenuMH.Height += 10;
                if (pnlMenuMH.Height >= 430)
                {
                    menuMHTrasition.Stop();
                    menuMHExpand = true;
                }
            }
            else
            {
                pnlMenuMH.Height -= 10;
                if (pnlMenuMH.Height <= 86)
                {
                    menuMHTrasition.Stop();
                    menuMHExpand = false;
                }
            }
        }

        private void btnMH_Click(object sender, EventArgs e)
        {
            menuMHTrasition.Start();
        }
    }
}
