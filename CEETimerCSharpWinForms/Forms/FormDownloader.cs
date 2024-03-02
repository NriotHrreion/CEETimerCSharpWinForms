﻿using CEETimerCSharpWinForms.Modules;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CEETimerCSharpWinForms.Forms
{
    public partial class FormDownloader : Form
    {
        private string latestVersion;
        public string downloadUrl;
        public string downloadPath;
        private bool isCancelled = false;
        private CancellationTokenSource cancelRequest;

        public FormDownloader()
        {
            InitializeComponent();
        }

        private async void FormDownloader_Load(object sender, EventArgs e)
        {
            latestVersion = CheckForUpdate.LatestVersion;
            downloadUrl = $"https://wanghaonie.github.io/file-storages/github-repos/CEETimerCSharpWinForms/CEETimerCSharpWinForms_{latestVersion}_x64_Setup.exe";
            downloadPath = Path.Combine(Path.GetTempPath(), $"CEETimerCSharpWinForms_{latestVersion}_x64_Setup.exe");

            await DownloadFile(downloadUrl, downloadPath);
        }

        public async Task DownloadFile(string url, string filePath)
        {
            using var httpClient = new HttpClient();
            cancelRequest = new CancellationTokenSource();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(LaunchManager.RequestUa);

            try
            {
                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancelRequest.Token))
                {
                    response.EnsureSuccessStatusCode();
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    var buffer = new byte[8192];
                    var totalBytesRead = 0L;
                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var bytesRead = 0L;
                    var sw = Stopwatch.StartNew();

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, (int)bytesRead);
                        totalBytesRead += bytesRead;
                        var progressPercentage = totalBytes == -1 ? -1 : (int)(totalBytesRead * 100 / totalBytes);

                        LabelSize.Text = $"已下载/总共：{totalBytesRead / 1024} KB / {totalBytes / 1024} KB";
                        LabelSpeed.Text = $"下载速度：{totalBytesRead / sw.Elapsed.TotalSeconds / 1024:0.00} KB/s";
                        ProgressBarMain.Value = progressPercentage;

                        if (cancelRequest.Token.IsCancellationRequested)
                        {
                            isCancelled = true;
                            fileStream.Close();
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            return;
                        }
                    }
                }
                if (!isCancelled)
                {
                    ButtonCancel.Enabled = false;
                    ButtonRetry.Enabled = false;

                    ProcessStartInfo processStartInfo = new()
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c start \"\" \"{filePath}\" /S",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    await Task.Delay(1800);
                    Process.Start(processStartInfo);
                    Close();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                LabelDownloading.Text = "下载失败，你可以点击 重试 重新启动下载。";
                LabelSize.Text = "已下载/总共：N/A";
                LabelSpeed.Text = "下载速度：N/A";
                MessageBox.Show($"无法下载更新文件! \n\n错误信息: \n{ex.Message}", LaunchManager.ErrMsg, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ButtonRetry.Enabled = true;
                return;
            }
        }
        private void FrmDlBtnC_Click(object sender, EventArgs e)
        {
            if (cancelRequest != null && !cancelRequest.Token.IsCancellationRequested)
            {
                cancelRequest?.Cancel();
                LabelDownloading.Text = "用户已取消下载。";
                MessageBox.Show($"你已取消下载！\n\n你稍后可以在 关于 窗口点击版本号来再次检查更新。", LaunchManager.WarnMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            FormClosing -= FormDownloader_FormClosing;
            Close();
        }
        private async void FrmDlBtnR_Click(object sender, EventArgs e)
        {
            ButtonRetry.Enabled = false;
            LabelDownloading.Text = "正在重新下载更新文件，请稍侯...";
            LabelSize.Text = "已下载/总共：(获取中...)";
            LabelSpeed.Text = "下载速度：(获取中...)";

            await DownloadFile(downloadUrl, downloadPath);
        }
        private void FormDownloader_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}