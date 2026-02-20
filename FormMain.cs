using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace heytea_diy_gui
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            MessageBox.Show("本软件为开源项目,作者拿浮木保证没有病毒,若杀毒软件误报,请点击信任/允许");
            MessageBox.Show("现在只支持霸王茶姬,喜茶不可用orz");
            MessageBox.Show("接下来你会看到2-3个窗口,请把那个有一堆英文的最小化,不要叉掉qwq");
            while (true)
            {
                bool success = Core.LaunchService();
                //success = false;
                if (success)
                    break;
                var flag = MessageBox.Show("出了一点小问题...请检查以下内容\n1.请解压后使用本程序\n2.请以管理员权限运行本程序\n3.请保证文件没有缺失", "ERROR", MessageBoxButtons.RetryCancel);
                if (flag == DialogResult.Retry)
                    continue;
                Close();
                return;
            }
            ImageManager.InitializePictureBox(boxImage);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            while (true)
            {
                bool success = Core.CloseService();
                //success = false;
                if (success)
                    return;
                var flag = MessageBox.Show("出了一点小问题...请重试\n或\n手动关闭刚刚弹出来的窗口,并运行clean.ps1", "ERROR", MessageBoxButtons.RetryCancel);
                if (flag == DialogResult.Retry)
                    continue;
                return;
            }
        }

        private void BtnReadPicture_Click(object sender, EventArgs e)
        {
            btnUpload.Enabled = false;
            MessageBox.Show("1.图片标准尺寸为596x832px,不是也没关系\n2.非线稿的效果很差(ps启动!)");
            if (!ImageManager.Read())
                return;
            if (ImageManager.OriginalImage == null)
                return;
            ImageManager.RemoveColor((int)barThreshold.Value);
            ImageManager.ResetView();
            ImageManager.RefreshDisplay();
            btnUpload.Enabled = true;
        }

        private void BarThreshold_ValueChanged(object sender, EventArgs e)
        {
            ImageManager.RemoveColor((int)barThreshold.Value);
            ImageManager.RefreshDisplay();
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            while (true)
            {
                if (!Core.IsWebsiteAccessible("http://mitm.it"))
                {
                    var result = MessageBox.Show("服务启动失败,尝试重启吗?", "", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        Core.CloseService();
                        Core.LaunchService();
                        continue;
                    }

                    break;
                }
                if (!ImageManager.Save())
                    return;
                MessageBox.Show("最后一步!!!\n" +
                                "---以下操作仅需一次\n" +
                                "-1.打开手机端微信\n" +
                                "-2.打开小程序\n" +
                                "-3.点击点单,随便选一杯饮料,加入购物车,结算(别真的付钱)\n" +
                                "-4.点击定制喜帖\n" +
                                "-5.点击开始创作,再点击右上角三个点,收藏(注意不是星标)\n" +
                                "---以上操作仅需一次\n" +
                                "6.打开电脑端微信->收藏->刚刚收藏的链接->灵感创作->创作喜帖\n" +
                                "7.随便画两笔,提交");
                MessageBox.Show("结束啦");
                break;
            }
        }
    }
}
