using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace heytea_diy_gui
{
    /// <summary>
    /// 还是ai
    /// </summary>
    internal static class ImageManager
    {
        static ImageManager()
        {
        }
        /// <summary>
        /// 原图
        /// </summary>
        public static Bitmap OriginalImage { get; private set; } = null;

        /// <summary>
        /// 黑白图
        /// </summary>
        public static Bitmap ColorlessImage { get; private set; } = null;

        /// <summary>
        /// 用于展示的图片
        /// </summary>
        public static Bitmap DisplayImage { get; private set; }

        /// <summary>
        /// picture box
        /// </summary>
        public static PictureBox BoxDisplay { get; private set; }

        // 平移和缩放参数
        private static float offsetX = 0;
        private static float offsetY = 0;
        private static float zoomScale = 1.0f;
        private static Point lastMousePos;
        private static bool isDragging = false;

        /// <summary>
        /// 读取原图
        /// </summary>
        /// <returns></returns>
        public static bool Read()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tiff";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 释放之前的图像资源
                        OriginalImage?.Dispose();

                        // 加载新图像
                        OriginalImage = new Bitmap(openFileDialog.FileName);

                        // 重置ColorlessImage
                        ColorlessImage?.Dispose();
                        ColorlessImage = null;
                        ResetView();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return false;
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public static bool Save()
        {
            if (DisplayImage == null)
            {
                MessageBox.Show("没有可保存的图像", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                // 确保目录存在
                string directory = "./heytea-diy-windows";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filePath = Path.Combine(directory, "target.png");

                // 直接保存为PNG
                DisplayImage.Save(filePath, ImageFormat.Png);

                /*
                // 获取文件信息显示给用户
                FileInfo fileInfo = new FileInfo(filePath);
                MessageBox.Show($"图像已保存到: {filePath}\n文件大小: {fileInfo.Length / 1024}KB",
                    "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //*/

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        /// <summary>
        /// 转黑白
        /// </summary>
        /// <param name="threshold"></param>
        public static void RemoveColor(int threshold)
        {
            if (OriginalImage == null)
                return;

            try
            {
                int width = OriginalImage.Width;
                int height = OriginalImage.Height;

                // 创建新的位图
                ColorlessImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                // 锁定位图数据
                BitmapData originalData = OriginalImage.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                BitmapData colorlessData = ColorlessImage.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                try
                {
                    // 获取指针和步长
                    IntPtr originalPtr = originalData.Scan0;
                    IntPtr colorlessPtr = colorlessData.Scan0;
                    int stride = originalData.Stride;
                    int bytes = Math.Abs(stride) * height;

                    // 复制数据到字节数组
                    byte[] originalBytes = new byte[bytes];
                    Marshal.Copy(originalPtr, originalBytes, 0, bytes);

                    byte[] colorlessBytes = new byte[bytes];

                    // 处理每个像素
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;

                            byte b = originalBytes[index];
                            byte g = originalBytes[index + 1];
                            byte r = originalBytes[index + 2];
                            byte a = originalBytes[index + 3];

                            // 如果像素完全透明，设为白色
                            if (a == 0)
                            {
                                colorlessBytes[index] = 255;     // B
                                colorlessBytes[index + 1] = 255; // G
                                colorlessBytes[index + 2] = 255; // R
                                colorlessBytes[index + 3] = 255; // A
                                continue;
                            }

                            // 计算灰度值 (使用加权平均)
                            double gray = 0.299 * r + 0.587 * g + 0.114 * b;

                            // 处理半透明像素：与白色背景混合
                            if (a < 255)
                            {
                                double alpha = a / 255.0;
                                gray = gray * alpha + 255 * (1 - alpha);
                            }

                            // 根据阈值设置黑白
                            if (gray < threshold)
                            {
                                colorlessBytes[index] = 0;     // B
                                colorlessBytes[index + 1] = 0; // G
                                colorlessBytes[index + 2] = 0; // R
                                colorlessBytes[index + 3] = 255; // A
                            }
                            else
                            {
                                colorlessBytes[index] = 255;     // B
                                colorlessBytes[index + 1] = 255; // G
                                colorlessBytes[index + 2] = 255; // R
                                colorlessBytes[index + 3] = 255; // A
                            }
                        }
                    }

                    // 将处理后的数据复制回位图
                    Marshal.Copy(colorlessBytes, 0, colorlessPtr, bytes);
                }
                finally
                {
                    OriginalImage.UnlockBits(originalData);
                    ColorlessImage.UnlockBits(colorlessData);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ColorlessImage?.Dispose();
                ColorlessImage = null;
            }
        }

        // 初始化PictureBox
        public static void InitializePictureBox(PictureBox pictureBox)
        {
            BoxDisplay = pictureBox;

            // 设置PictureBox属性
            BoxDisplay.SizeMode = PictureBoxSizeMode.Zoom;
            BoxDisplay.BackColor = Color.LightGray;
            BoxDisplay.BorderStyle = BorderStyle.FixedSingle;

            // 设置鼠标光标
            BoxDisplay.Cursor = Cursors.SizeAll;

            // 绑定事件
            BoxDisplay.MouseDown += BoxDisplay_MouseDown;
            BoxDisplay.MouseMove += BoxDisplay_MouseMove;
            BoxDisplay.MouseUp += BoxDisplay_MouseUp;
            BoxDisplay.MouseWheel += BoxDisplay_MouseWheel;

            // 创建初始的DisplayImage
            DisplayImage = new Bitmap(596, 832);
            using (Graphics g = Graphics.FromImage(DisplayImage))
            {
                g.Clear(Color.White);
            }

            BoxDisplay.Image = DisplayImage;
        }

        #region BoxDisplay事件

        // 鼠标按下事件
        private static void BoxDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePos = e.Location;
            }
        }

        // 鼠标移动事件
        private static void BoxDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && ColorlessImage != null)
            {
                // 计算移动距离
                int deltaX = e.X - lastMousePos.X;
                int deltaY = e.Y - lastMousePos.Y;

                // 更新偏移量
                offsetX -= deltaX / zoomScale;
                offsetY -= deltaY / zoomScale;

                /*
                // 限制偏移范围
                float maxOffsetX = Math.Max(0, ColorlessImage.Width - DisplayImage.Width / zoomScale);
                float maxOffsetY = Math.Max(0, ColorlessImage.Height - DisplayImage.Height / zoomScale);

                offsetX = Math.Max(0, Math.Min(offsetX, maxOffsetX));
                offsetY = Math.Max(0, Math.Min(offsetY, maxOffsetY));
                //*/

                lastMousePos = e.Location;

                // 刷新显示
                RefreshDisplay();
            }
        }

        // 鼠标释放事件
        private static void BoxDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        // 鼠标滚轮事件
        private static void BoxDisplay_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ColorlessImage != null)
            {
                // 计算缩放前的鼠标位置对应的图像位置
                float imageX = offsetX + e.X / zoomScale;
                float imageY = offsetY + e.Y / zoomScale;

                // 更新缩放比例
                float oldZoom = zoomScale;
                if (e.Delta > 0)
                {
                    zoomScale *= 1.05f; // 放大
                }
                else
                {
                    zoomScale /= 1.05f; // 缩小
                }

                // 限制缩放范围
                zoomScale = Math.Max(0.1f, Math.Min(zoomScale, 10.0f));

                // 调整偏移量，使缩放中心保持在同一图像位置
                offsetX = imageX - e.X / zoomScale;
                offsetY = imageY - e.Y / zoomScale;

                // 限制偏移范围
                float maxOffsetX = Math.Max(0, ColorlessImage.Width - DisplayImage.Width / zoomScale);
                float maxOffsetY = Math.Max(0, ColorlessImage.Height - DisplayImage.Height / zoomScale);

                offsetX = Math.Max(0, Math.Min(offsetX, maxOffsetX));
                offsetY = Math.Max(0, Math.Min(offsetY, maxOffsetY));

                // 刷新显示
                RefreshDisplay();
            }
        }

        #endregion
        // 刷新显示函数
        public static void RefreshDisplay()
        {
            if (ColorlessImage == null || DisplayImage == null)
                return;

            using (Graphics g = Graphics.FromImage(DisplayImage))
            {
                g.Clear(Color.White);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                // 计算源矩形和目标矩形
                RectangleF srcRect = new RectangleF(
                    offsetX,
                    offsetY,
                    DisplayImage.Width / zoomScale,
                    DisplayImage.Height / zoomScale
                );

                Rectangle destRect = new Rectangle(0, 0, DisplayImage.Width, DisplayImage.Height);

                // 绘制图像
                g.DrawImage(ColorlessImage, destRect, srcRect, GraphicsUnit.Pixel);
            }

            // 刷新PictureBox
            BoxDisplay.Invalidate();
        }

        // 重置视图（在加载新图像时调用）
        public static void ResetView()
        {
            offsetX = 0;
            offsetY = 0;
            zoomScale = 1.0f;

            if (ColorlessImage != null)
            {
                // 计算适合显示的初始缩放比例
                float scaleX = (float)DisplayImage.Width / ColorlessImage.Width;
                float scaleY = (float)DisplayImage.Height / ColorlessImage.Height;
                zoomScale = Math.Min(scaleX, scaleY);

                // 居中显示
                offsetX = (ColorlessImage.Width - DisplayImage.Width / zoomScale) / 2;
                offsetY = (ColorlessImage.Height - DisplayImage.Height / zoomScale) / 2;
            }

            RefreshDisplay();
        }
    }
}