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
    public partial class FrmMainBH : Form
    {
        
        public FrmMainBH()
        {
            InitializeComponent();
        }

        bool menuBHExpand = false;

        private void menuTransition_Tick(object sender, EventArgs e)
        {
            if (menuBHExpand == false)
            {
                pnlMenuBH.Height += 10;
                if (pnlMenuBH.Height >= 666)
                {
                    menuTransition.Stop();
                    menuBHExpand = true;
                }
            }
            else
            {
                pnlMenuBH.Height -= 10;
                if (pnlMenuBH.Height <= 171)
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
                if (pnlLeftMain.Width <= 100)
                {
                    sidebarExpand = false;
                    sidebarTransition.Stop();

                    pnlTQ.Width = pnlLeftMain.Width;
                    pnlTN.Width = pnlLeftMain.Width;
                    pnlNV.Width = pnlLeftMain.Width;
                    pnlSetting.Width = pnlLeftMain.Width;


                }
            }
            else
            {
                pnlLeftMain.Width += 5;
                if (pnlLeftMain.Width >= 346)
                {
                    sidebarExpand = true;
                    sidebarTransition.Stop();

                    pnlTQ.Width = pnlLeftMain.Width;
                    pnlTN.Width = pnlLeftMain.Width;
                    pnlNV.Width = pnlLeftMain.Width;
                    pnlSetting.Width = pnlLeftMain.Width;
                }
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            sidebarTransition.Start();
        }

        private void btnDH_Click(object sender, EventArgs e)
        {
            //if (pos == null)
            //{
            //    pos = new FrmPOS();
            //    pos.FormClosed += POS_FormClosed;
            //    pos.MdiParent = this;
            //    pos.Dock = DockStyle.Fill;
            //    pos.Show();
            //}
            //else
            //{
            //    pos.Activate();
            //}
        }

        private void POS_FormClosed (object sender, EventArgs e)
        {
            ////throw new NotImplementedException();
            //pos = null;
        }
    }
}
