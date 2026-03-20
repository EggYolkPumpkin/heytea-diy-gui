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
        /// 处理完的图片
        /// </summary>
        public static Bitmap ProcessedImage { get; private set; } = null;

        /// <summary>
        /// 用于展示的图片
        /// </summary>
        public static Bitmap DisplayImage { get; private set; }

        /// <summary>
        /// picture box
        /// </summary>
        public static PictureBox BoxDisplay { get; private set; }
        /// <summary>
        /// 图像模式
        /// </summary>
        public enum ImageMode
        {
            /// <summary>
            /// 黑白
            /// </summary>
            BlackAndWhite,
            /// <summary>
            /// 灰度
            /// </summary>
            Gray,
            /// <summary>
            /// 彩色
            /// </summary>
            Colorful
        }

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
                        ProcessedImage?.Dispose();
                        ProcessedImage = null;
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
        /// 处理图像
        /// </summary>
        /// <param name="mode">处理模式</param>
        /// <param name="threshold">黑白模式的阈值（仅在BlackAndWhite模式下有效）</param>
        public static void ProcessImage(ImageMode mode, int threshold = 127)
        {
            if (OriginalImage == null)
                return;

            try
            {
                int width = OriginalImage.Width;
                int height = OriginalImage.Height;

                // 释放旧的资源
                ProcessedImage?.Dispose();

                // 创建新的位图
                ProcessedImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                // 锁定位图数据
                BitmapData originalData = OriginalImage.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                BitmapData processedData = ProcessedImage.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                try
                {
                    // 获取指针和步长
                    IntPtr originalPtr = originalData.Scan0;
                    IntPtr processedPtr = processedData.Scan0;
                    int stride = originalData.Stride;
                    int bytes = Math.Abs(stride) * height;

                    // 复制数据到字节数组
                    byte[] originalBytes = new byte[bytes];
                    Marshal.Copy(originalPtr, originalBytes, 0, bytes);

                    byte[] processedBytes = new byte[bytes];

                    // 遍历每个像素，因为所有模式都需要处理透明度变白底的逻辑
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;

                            byte b = originalBytes[index];
                            byte g = originalBytes[index + 1];
                            byte r = originalBytes[index + 2];
                            byte a = originalBytes[index + 3];

                            // 1. 统一处理透明度：计算与白色背景混合后的 RGB 真实值
                            double r_blend = r, g_blend = g, b_blend = b;

                            if (a == 0) // 完全透明，直接设为纯白
                            {
                                r_blend = 255;
                                g_blend = 255;
                                b_blend = 255;
                            }
                            else if (a < 255) // 半透明，与白色混合
                            {
                                double alpha = a / 255.0;
                                r_blend = r * alpha + 255 * (1 - alpha);
                                g_blend = g * alpha + 255 * (1 - alpha);
                                b_blend = b * alpha + 255 * (1 - alpha);
                            }

                            // 2. 根据不同模式进行像素赋值
                            if (mode == ImageMode.Colorful)
                            {
                                // 彩色模式：直接使用混合白底后的 RGB 值
                                processedBytes[index] = (byte)b_blend;     // B
                                processedBytes[index + 1] = (byte)g_blend; // G
                                processedBytes[index + 2] = (byte)r_blend; // R
                                processedBytes[index + 3] = 255;           // A (Alpha通道固定设为不透明的255)
                            }
                            else
                            {
                                // 灰度和黑白模式：基于混合后的真实颜色计算灰度值
                                double gray = 0.299 * r_blend + 0.587 * g_blend + 0.114 * b_blend;

                                if (mode == ImageMode.BlackAndWhite)
                                {
                                    byte bwValue = (byte)(gray < threshold ? 0 : 255);
                                    processedBytes[index] = bwValue;
                                    processedBytes[index + 1] = bwValue;
                                    processedBytes[index + 2] = bwValue;
                                    processedBytes[index + 3] = 255;
                                }
                                else if (mode == ImageMode.Gray)
                                {
                                    byte grayValue = (byte)Math.Min(255, Math.Max(0, gray));
                                    processedBytes[index] = grayValue;
                                    processedBytes[index + 1] = grayValue;
                                    processedBytes[index + 2] = grayValue;
                                    processedBytes[index + 3] = 255;
                                }
                            }
                        }
                    }

                    // 将处理后的数据复制回位图
                    Marshal.Copy(processedBytes, 0, processedPtr, bytes);
                }
                finally
                {
                    OriginalImage.UnlockBits(originalData);
                    ProcessedImage.UnlockBits(processedData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ProcessedImage?.Dispose();
                ProcessedImage = null;
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
            if (isDragging && ProcessedImage != null)
            {
                // 计算移动距离
                int deltaX = e.X - lastMousePos.X;
                int deltaY = e.Y - lastMousePos.Y;

                // 更新偏移量
                offsetX -= deltaX / zoomScale;
                offsetY -= deltaY / zoomScale;

                /*
                // 限制偏移范围
                float maxOffsetX = Math.Max(0, ProcessedImage.Width - DisplayImage.Width / zoomScale);
                float maxOffsetY = Math.Max(0, ProcessedImage.Height - DisplayImage.Height / zoomScale);

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
            if (ProcessedImage != null)
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
                float maxOffsetX = Math.Max(0, ProcessedImage.Width - DisplayImage.Width / zoomScale);
                float maxOffsetY = Math.Max(0, ProcessedImage.Height - DisplayImage.Height / zoomScale);

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
            if (ProcessedImage == null || DisplayImage == null)
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
                g.DrawImage(ProcessedImage, destRect, srcRect, GraphicsUnit.Pixel);
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

            if (ProcessedImage != null)
            {
                // 计算适合显示的初始缩放比例
                float scaleX = (float)DisplayImage.Width / ProcessedImage.Width;
                float scaleY = (float)DisplayImage.Height / ProcessedImage.Height;
                zoomScale = Math.Min(scaleX, scaleY);

                // 居中显示
                offsetX = (ProcessedImage.Width - DisplayImage.Width / zoomScale) / 2;
                offsetY = (ProcessedImage.Height - DisplayImage.Height / zoomScale) / 2;
            }

            RefreshDisplay();
        }
    }
}