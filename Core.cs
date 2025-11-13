using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace heytea_diy_gui
{
    internal static class Core
    {
        /// <summary>
        /// 启动服务(本方法由ai生成,懒得写了)
        /// </summary>
        /// <param name="scriptPath"></param>
        /// <returns></returns>
        public static bool LaunchService(string scriptPath = "heytea-diy.ps1")
        {
            // 确定工作目录和完整脚本路径
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string workingDirectory = Path.Combine(baseDirectory, "heytea-diy-windows");
            string fullScriptPath = Path.Combine(workingDirectory, Path.GetFileName(scriptPath));

            if (!File.Exists(fullScriptPath))
            {
                Console.WriteLine($"错误：找不到脚本文件: {fullScriptPath}");
                return false;
            }

            // 组合要执行的 PowerShell 命令
            // -NoProfile: 不加载配置文件
            // -ExecutionPolicy RemoteSigned: 设置执行策略
            // -Command: 组合命令 (cd 和 .\script)
            // 使用单引号包裹路径以处理空格
            string arguments = $"-NoProfile -ExecutionPolicy RemoteSigned -Command \"cd '{workingDirectory}'; & '{fullScriptPath}'\"";

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = arguments,
                    UseShellExecute = true,         // 必须为 true 才能使用 Verb
                    Verb = "runas",                 // 以管理员身份运行 (会触发 UAC 提示)
                    CreateNoWindow = false,         // 确保窗口显示
                    WindowStyle = ProcessWindowStyle.Normal // 正常窗口样式
                };

                Console.WriteLine("正在请求管理员权限并启动 PowerShell 脚本...");

                // 启动进程
                Process process = Process.Start(startInfo);

                if (process == null)
                {
                    Console.WriteLine("错误：Powershell 进程未能启动。");
                    return false;
                }

                // 等待进程执行完毕。
                // 脚本执行完成后，Powershell 窗口将自动关闭，相当于“隐藏”了。
                process.WaitForExit();
                WindowManager.HideProcessWindows("heytea-diy");
                Console.WriteLine($"Powershell 脚本执行完毕。退出代码: {process.ExitCode}");

                // 可以根据需要检查 ExitCode，通常 0 表示成功
                return process.ExitCode == 0;
            }
            catch (System.ComponentModel.Win32Exception winEx) when (winEx.NativeErrorCode == 1223)
            {
                // 错误代码 1223 (The operation was canceled by the user) 表示用户取消了 UAC 提示。
                MessageBox.Show("用户取消了用户账户控制 (UAC) 提示，操作终止。");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"运行 PowerShell 脚本时发生错误：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭服务(本方法由ai生成,懒得写了)
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        public static bool CloseService(string scriptName = "clean.ps1")
        {
            WindowManager.KillProcessGracefully("heytea-diy");
            // 确定工作目录和完整脚本路径
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string workingDirectory = Path.Combine(baseDirectory, "heytea-diy-windows");
            string fullScriptPath = Path.Combine(workingDirectory, Path.GetFileName(scriptName));

            // 检查工作目录和脚本文件是否存在
            if (!Directory.Exists(workingDirectory))
            {
                Console.WriteLine($"错误：找不到工作目录: {workingDirectory}");
                return false;
            }

            if (!File.Exists(fullScriptPath))
            {
                Console.WriteLine($"错误：找不到脚本文件: {fullScriptPath}");
                return false;
            }

            // 组合要执行的 PowerShell 命令
            // -NoProfile: 不加载配置文件
            // -ExecutionPolicy RemoteSigned: 设置执行策略
            // -WindowStyle Hidden: 隐藏窗口
            // -Command: 组合命令 (cd 和 .\script)
            string arguments = $"-NoProfile -ExecutionPolicy RemoteSigned -WindowStyle Hidden -Command \"cd '{workingDirectory}'; & '{fullScriptPath}'\"";

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = arguments,
                    UseShellExecute = false,        // 使用 false 可以更好地控制窗口显示
                    CreateNoWindow = true,          // 不创建窗口
                    WorkingDirectory = workingDirectory // 设置工作目录
                };

                Console.WriteLine($"正在执行 PowerShell 脚本: {fullScriptPath}");

                // 启动进程
                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Console.WriteLine("错误：PowerShell 进程未能启动。");
                        return false;
                    }

                    // 等待进程执行完毕
                    process.WaitForExit();

                    Console.WriteLine($"PowerShell 脚本执行完毕。退出代码: {process.ExitCode}");

                    // 可以根据需要检查 ExitCode，通常 0 表示成功
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"运行 PowerShell 脚本时发生错误：{ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// 用于检验服务是否开启成功
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsWebsiteAccessible(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 5000;
                request.Method = "HEAD";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
