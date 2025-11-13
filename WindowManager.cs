using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace heytea_diy_gui
{
    /// <summary>
    /// 管理heytea-diy.exe的窗口(ai太好用了你知道吗)
    /// </summary>
    public static class WindowManager
    {
        // Windows API 常量
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_MAXIMIZE = 3;
        private const int SW_RESTORE = 9;

        // Windows API 函数
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// 隐藏指定进程的所有窗口
        /// </summary>
        /// <param name="processName">进程名称（不包含.exe）</param>
        /// <returns>操作是否成功</returns>
        public static bool HideProcessWindows(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                bool success = false;

                if (processes.Length == 0)
                {
                    Console.WriteLine($"未找到进程: {processName}");
                    return false;
                }

                foreach (Process process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        success = ShowWindow(process.MainWindowHandle, SW_HIDE) || success;
                        Console.WriteLine($"已隐藏进程 {process.ProcessName} 的主窗口 (PID: {process.Id})");
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"隐藏进程窗口时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 最小化指定进程的所有窗口
        /// </summary>
        /// <param name="processName">进程名称（不包含.exe）</param>
        /// <returns>操作是否成功</returns>
        public static bool MinimizeProcessWindows(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                bool success = false;

                if (processes.Length == 0)
                {
                    Console.WriteLine($"未找到进程: {processName}");
                    return false;
                }

                foreach (Process process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        success = ShowWindow(process.MainWindowHandle, SW_MINIMIZE) || success;
                        Console.WriteLine($"已最小化进程 {process.ProcessName} 的主窗口 (PID: {process.Id})");
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"最小化进程窗口时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 显示指定进程的所有窗口
        /// </summary>
        /// <param name="processName">进程名称（不包含.exe）</param>
        /// <returns>操作是否成功</returns>
        public static bool ShowProcessWindows(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                bool success = false;

                if (processes.Length == 0)
                {
                    Console.WriteLine($"未找到进程: {processName}");
                    return false;
                }

                foreach (Process process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        success = ShowWindow(process.MainWindowHandle, SW_RESTORE) || success;
                        ShowWindow(process.MainWindowHandle, SW_SHOW); // 确保窗口显示
                        Console.WriteLine($"已显示进程 {process.ProcessName} 的主窗口 (PID: {process.Id})");
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"显示进程窗口时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 最大化指定进程的所有窗口
        /// </summary>
        /// <param name="processName">进程名称（不包含.exe）</param>
        /// <returns>操作是否成功</returns>
        public static bool MaximizeProcessWindows(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                bool success = false;

                if (processes.Length == 0)
                {
                    Console.WriteLine($"未找到进程: {processName}");
                    return false;
                }

                foreach (Process process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        success = ShowWindow(process.MainWindowHandle, SW_MAXIMIZE) || success;
                        Console.WriteLine($"已最大化进程 {process.ProcessName} 的主窗口 (PID: {process.Id})");
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"最大化进程窗口时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 终止指定进程（强制终止）
        /// </summary>
        /// <param name="processName">进程名称（不包含.exe）</param>
        /// <param name="timeoutMs">等待进程退出的超时时间（毫秒）</param>
        /// <returns>操作是否成功</returns>
        public static bool KillProcess(string processName, int timeoutMs = 5000)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length == 0)
                {
                    Console.WriteLine($"未找到进程: {processName}");
                    return false;
                }

                bool success = true;

                foreach (Process process in processes)
                {
                    try
                    {
                        Console.WriteLine($"正在终止进程: {process.ProcessName} (PID: {process.Id})");
                        process.Kill();

                        if (!process.WaitForExit(timeoutMs))
                        {
                            Console.WriteLine($"进程 {process.ProcessName} 终止超时");
                            success = false;
                        }
                        else
                        {
                            Console.WriteLine($"进程 {process.ProcessName} 已成功终止");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"终止进程 {process.ProcessName} 时出错: {ex.Message}");
                        success = false;
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"终止进程时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 优雅终止指定进程（先尝试关闭窗口，再强制终止）
        /// </summary>
        /// <param name="processName">进程名称（不包含.exe）</param>
        /// <param name="timeoutMs">等待进程退出的超时时间（毫秒）</param>
        /// <returns>操作是否成功</returns>
        public static bool KillProcessGracefully(string processName, int timeoutMs = 5000)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length == 0)
                {
                    Console.WriteLine($"未找到进程: {processName}");
                    return false;
                }

                bool success = true;

                foreach (Process process in processes)
                {
                    try
                    {
                        // 先尝试关闭主窗口（优雅关闭）
                        if (process.MainWindowHandle != IntPtr.Zero && !process.HasExited)
                        {
                            Console.WriteLine($"尝试优雅关闭进程: {process.ProcessName} (PID: {process.Id})");

                            // 发送关闭消息
                            if (process.CloseMainWindow())
                            {
                                // 等待进程自行退出
                                if (process.WaitForExit(timeoutMs))
                                {
                                    Console.WriteLine($"进程 {process.ProcessName} 已优雅关闭");
                                    continue; // 成功关闭，处理下一个进程
                                }
                            }

                            Console.WriteLine("优雅关闭失败，尝试强制终止...");
                        }

                        // 强制终止
                        Console.WriteLine($"强制终止进程: {process.ProcessName} (PID: {process.Id})");
                        process.Kill();

                        if (!process.WaitForExit(timeoutMs))
                        {
                            Console.WriteLine($"进程 {process.ProcessName} 终止超时");
                            success = false;
                        }
                        else
                        {
                            Console.WriteLine($"进程 {process.ProcessName} 已终止");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"终止进程 {process.ProcessName} 时出错: {ex.Message}");
                        success = false;
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"终止进程时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取指定进程的信息
        /// </summary>
        /// <param name="processName">进程名称（不包含.exe）</param>
        public static void ShowProcessInfo(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length == 0)
                {
                    Console.WriteLine($"未找到进程: {processName}");
                    return;
                }

                Console.WriteLine($"找到 {processes.Length} 个 {processName} 进程:");

                foreach (Process process in processes)
                {
                    string windowState = process.MainWindowHandle == IntPtr.Zero ?
                        "无窗口" : (IsWindowVisible(process.MainWindowHandle) ? "可见" : "隐藏");

                    Console.WriteLine($"- {process.ProcessName} (PID: {process.Id}, 内存: {process.WorkingSet64 / 1024 / 1024}MB, 窗口状态: {windowState})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取进程信息时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 设置进程窗口状态
        /// </summary>
        /// <param name="processName">进程名称</param>
        /// <param name="operation">操作类型: hide, show, minimize, maximize</param>
        /// <returns>操作是否成功</returns>
        public static bool SetWindowState(string processName, string operation)
        {
            switch (operation.ToLower())
            {
                case "hide":
                    return HideProcessWindows(processName);
                case "show":
                    return ShowProcessWindows(processName);
                case "minimize":
                    return MinimizeProcessWindows(processName);
                case "maximize":
                    return MaximizeProcessWindows(processName);
                default:
                    throw new ArgumentException($"不支持的操作: {operation}");
            }
        }
    }
}