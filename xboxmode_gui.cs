using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace EasyXboxMode
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("Fatal error on startup:\n\n{0}\n\n{1}", ex.Message, ex.StackTrace),
                    "Easy Xbox Mode",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }

    public class MainForm : Form
    {
        private Label lblXboxStatus;
        private Label lblHandheldStatus;
        private Button btnRefresh;
        private Button btnXboxEnable;
        private Button btnXboxDisable;
        private Button btnHandheldEnable;
        private Button btnHandheldDisable;
        private TextBox txtConsole;
        private string vivetoolPath;
        private bool isWorking;

        private static readonly Color ColorGreen = Color.FromArgb(0, 150, 0);
        private static readonly Color ColorRed = Color.FromArgb(200, 0, 0);

        public MainForm()
        {
            Text = "Easy Xbox Mode";
            Size = new Size(660, 600);
            MinimumSize = new Size(660, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);
            vivetoolPath = Path.Combine(Application.StartupPath, "ViVeTool.exe");

            BuildUI();

            Shown += async (s, e) => await SafeRefreshAsync();
        }

        private void BuildUI()
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 1,
                RowCount = 5
            };
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            table.Controls.Add(BuildStatusPanel(), 0, 0);
            table.Controls.Add(BuildXboxPanel(), 0, 1);
            table.Controls.Add(BuildHandheldPanel(), 0, 2);
            table.Controls.Add(new Panel(), 0, 3);
            table.Controls.Add(BuildConsolePanel(), 0, 4);

            Controls.Add(table);
        }

        private Panel BuildStatusPanel()
        {
            var p = new Panel { Dock = DockStyle.Fill };

            btnRefresh = new Button
            {
                Text = "Refresh Status",
                Location = new Point(0, 5),
                Size = new Size(120, 26),
                FlatStyle = FlatStyle.System
            };
            btnRefresh.Click += async (s, e) => await SafeRefreshAsync();

            lblXboxStatus = new Label
            {
                Text = "Xbox Mode: Checking...",
                Location = new Point(140, 8),
                Size = new Size(400, 22),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
            };

            lblHandheldStatus = new Label
            {
                Text = "Handheld Mode: Checking...",
                Location = new Point(140, 32),
                Size = new Size(400, 22),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
            };

            p.Controls.Add(btnRefresh);
            p.Controls.Add(lblXboxStatus);
            p.Controls.Add(lblHandheldStatus);

            return p;
        }

        private Panel BuildXboxPanel()
        {
            var p = new Panel { Dock = DockStyle.Fill };

            var header = new Label
            {
                Text = "Xbox Mode",
                Location = new Point(0, 0),
                Size = new Size(200, 20),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
            };

            var desc = new Label
            {
                Text = "Enables hidden Xbox features via ViVeTool. Gives access to Launch on\nStartup and changing the default home app.",
                Location = new Point(0, 22),
                Size = new Size(600, 35),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            btnXboxEnable = new Button
            {
                Text = "Enable Xbox Mode",
                Location = new Point(0, 65),
                Size = new Size(170, 30),
                FlatStyle = FlatStyle.System
            };
            btnXboxEnable.Click += async (s, e) => await SafeXboxEnableAsync();

            btnXboxDisable = new Button
            {
                Text = "Disable Xbox Mode",
                Location = new Point(180, 65),
                Size = new Size(170, 30),
                FlatStyle = FlatStyle.System
            };
            btnXboxDisable.Click += async (s, e) => await SafeXboxDisableAsync();

            p.Controls.Add(header);
            p.Controls.Add(desc);
            p.Controls.Add(btnXboxEnable);
            p.Controls.Add(btnXboxDisable);

            return p;
        }

        private Panel BuildHandheldPanel()
        {
            var p = new Panel { Dock = DockStyle.Fill };

            var header = new Label
            {
                Text = "Handheld Mode",
                Location = new Point(0, 0),
                Size = new Size(200, 20),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
            };

            var desc = new Label
            {
                Text = "Sets DeviceForm registry to make Windows treat this as a handheld\ndevice. Unlocks startup settings and app selection restricted to handhelds.",
                Location = new Point(0, 22),
                Size = new Size(600, 35),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            btnHandheldEnable = new Button
            {
                Text = "Enable Handheld Mode",
                Location = new Point(0, 65),
                Size = new Size(170, 30),
                FlatStyle = FlatStyle.System
            };
            btnHandheldEnable.Click += async (s, e) => await SafeHandheldEnableAsync();

            btnHandheldDisable = new Button
            {
                Text = "Disable Handheld Mode",
                Location = new Point(180, 65),
                Size = new Size(170, 30),
                FlatStyle = FlatStyle.System
            };
            btnHandheldDisable.Click += async (s, e) => await SafeHandheldDisableAsync();

            p.Controls.Add(header);
            p.Controls.Add(desc);
            p.Controls.Add(btnHandheldEnable);
            p.Controls.Add(btnHandheldDisable);

            return p;
        }

        private Panel BuildConsolePanel()
        {
            var p = new Panel { Dock = DockStyle.Fill };

            var topBar = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(620, 22),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var header = new Label
            {
                Text = "Console Output",
                Location = new Point(0, 2),
                Size = new Size(120, 18),
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold)
            };

            var btnClear = new Button
            {
                Text = "Clear",
                Location = new Point(520, 0),
                Size = new Size(55, 20),
                FlatStyle = FlatStyle.System,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClear.Click += (s, e) => txtConsole.Clear();

            var btnCopy = new Button
            {
                Text = "Copy",
                Location = new Point(580, 0),
                Size = new Size(55, 20),
                FlatStyle = FlatStyle.System,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(txtConsole.Text))
                    Clipboard.SetText(txtConsole.Text);
            };

            topBar.Controls.Add(header);
            topBar.Controls.Add(btnClear);
            topBar.Controls.Add(btnCopy);

            txtConsole = new TextBox
            {
                Location = new Point(0, 24),
                Size = new Size(620, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.White,
                Font = new Font("Consolas", 9F),
                WordWrap = false
            };

            p.Controls.Add(topBar);
            p.Controls.Add(txtConsole);

            return p;
        }

        // ===== Helpers =====

        private void SetButtonsEnabled(bool enabled)
        {
            btnXboxEnable.Enabled = enabled;
            btnXboxDisable.Enabled = enabled;
            btnHandheldEnable.Enabled = enabled;
            btnHandheldDisable.Enabled = enabled;
            btnRefresh.Enabled = enabled;
        }

        private void Log(string text)
        {
            if (txtConsole.InvokeRequired)
            {
                txtConsole.Invoke(new Action<string>(Log), text);
                return;
            }
            txtConsole.AppendText(text + Environment.NewLine);
            txtConsole.SelectionStart = txtConsole.Text.Length;
            txtConsole.ScrollToCaret();
        }

        private void SetStatus(Label label, bool enabled, string prefix)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() => SetStatus(label, enabled, prefix)));
                return;
            }
            label.Text = string.Format("{0}: {1}", prefix, enabled ? "Enabled" : "Disabled");
            label.ForeColor = enabled ? ColorGreen : ColorRed;
        }

        private async Task<string> RunProcessAsync(string fileName, string arguments)
        {
            Log(string.Format("> {0} {1}", fileName, arguments));
            try
            {
                var psi = new ProcessStartInfo(fileName, arguments)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Application.StartupPath
                };

                using (var proc = new Process { StartInfo = psi })
                {
                    var outputBuilder = new StringBuilder();
                    var errorBuilder = new StringBuilder();

                    proc.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                            outputBuilder.AppendLine(e.Data);
                    };
                    proc.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                            errorBuilder.AppendLine(e.Data);
                    };

                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    await Task.Run(() => proc.WaitForExit());

                    string output = outputBuilder.ToString().TrimEnd();
                    string error = errorBuilder.ToString().TrimEnd();

                    if (output.Length > 0) Log(output);
                    if (error.Length > 0) Log(error);

                    Log("");

                    if (output.Length > 0 && error.Length > 0)
                        return output + Environment.NewLine + error;
                    return output.Length > 0 ? output : error;
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("Error: {0}", ex.Message));
                Log("");
                return "";
            }
        }

        // ===== Operations =====

        private async Task SafeRefreshAsync()
        {
            try
            {
                await RefreshStatusAsync();
            }
            catch (Exception ex)
            {
                Log(string.Format("Unexpected error during refresh: {0}", ex.Message));
                Log("");
                isWorking = false;
                SetButtonsEnabled(true);
            }
        }

        private async Task RefreshStatusAsync()
        {
            if (isWorking) return;
            isWorking = true;
            SetButtonsEnabled(false);
            txtConsole.Clear();

            try
            {
                // Xbox mode check via ViVeTool
                bool xboxEnabled = false;
                if (File.Exists(vivetoolPath))
                {
                    string output1 = await RunProcessAsync(vivetoolPath, "/query /id:59765208");
                    string output2 = await RunProcessAsync(vivetoolPath, "/query /id:58989070");
                    xboxEnabled = output1.Contains("Enabled") && output2.Contains("Enabled");
                }
                else
                {
                    Log("ViVeTool.exe not found in application directory.");
                    Log("");
                }
                SetStatus(lblXboxStatus, xboxEnabled, "Xbox Mode");

                // Handheld mode check via registry
                bool handheldEnabled = false;
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM"))
                    {
                        if (key != null)
                        {
                            object val = key.GetValue("DeviceForm");
                            if (val is int)
                            {
                                handheldEnabled = ((int)val) == 0x2e;
                            }
                            Log(string.Format(
                                "> Registry: HKLM\\...\\OEM\\DeviceForm = {0}",
                                val != null ? string.Format("0x{0:X}", val) : "not found"));
                            Log("");
                        }
                        else
                        {
                            Log("> Registry key not found (Desktop Mode - DeviceForm not set)");
                            Log("");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("Registry check error: {0}", ex.Message));
                    Log("");
                }
                SetStatus(lblHandheldStatus, handheldEnabled, "Handheld Mode");
            }
            finally
            {
                isWorking = false;
                SetButtonsEnabled(true);
            }
        }

        private async Task SafeXboxEnableAsync()
        {
            try { await XboxEnableAsync(); }
            catch (Exception ex) { Log(string.Format("Error: {0}", ex.Message)); Log(""); isWorking = false; SetButtonsEnabled(true); }
        }

        private async Task SafeXboxDisableAsync()
        {
            try { await XboxDisableAsync(); }
            catch (Exception ex) { Log(string.Format("Error: {0}", ex.Message)); Log(""); isWorking = false; SetButtonsEnabled(true); }
        }

        private async Task SafeHandheldEnableAsync()
        {
            try { await HandheldEnableAsync(); }
            catch (Exception ex) { Log(string.Format("Error: {0}", ex.Message)); Log(""); isWorking = false; SetButtonsEnabled(true); }
        }

        private async Task SafeHandheldDisableAsync()
        {
            try { await HandheldDisableAsync(); }
            catch (Exception ex) { Log(string.Format("Error: {0}", ex.Message)); Log(""); isWorking = false; SetButtonsEnabled(true); }
        }

        private async Task XboxEnableAsync()
        {
            if (isWorking) return;
            isWorking = true;
            SetButtonsEnabled(false);

            try
            {
                Log("--- Enabling Xbox Mode ---");
                await RunProcessAsync(vivetoolPath, "/enable /id:59765208,58989070");
                Log("--- Done ---");
                Log("");
                await RefreshStatusAsync();
            }
            finally
            {
                isWorking = false;
                SetButtonsEnabled(true);
            }
        }

        private async Task XboxDisableAsync()
        {
            if (isWorking) return;
            isWorking = true;
            SetButtonsEnabled(false);

            try
            {
                Log("--- Disabling Xbox Mode ---");
                await RunProcessAsync(vivetoolPath, "/disable /id:59765208,58989070");
                Log("--- Done ---");
                Log("");
                await RefreshStatusAsync();
            }
            finally
            {
                isWorking = false;
                SetButtonsEnabled(true);
            }
        }

        private async Task HandheldEnableAsync()
        {
            if (isWorking) return;
            isWorking = true;
            SetButtonsEnabled(false);

            try
            {
                Log("--- Enabling Handheld Mode ---");
                Log("> reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\OEM\" /v DeviceForm /t REG_DWORD /d 0x2e /f");
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("DeviceForm", 0x2e, RegistryValueKind.DWord);
                            Log("DeviceForm set to 0x2e (Handheld Mode enabled).");
                        }
                        else
                        {
                            Log("Error: Cannot open registry key. Try running as Administrator.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("Registry error: {0}", ex.Message));
                }
                Log("");
                Log("--- Done ---");
                Log("");
                await RefreshStatusAsync();
            }
            finally
            {
                isWorking = false;
                SetButtonsEnabled(true);
            }
        }

        private async Task HandheldDisableAsync()
        {
            if (isWorking) return;
            isWorking = true;
            SetButtonsEnabled(false);

            try
            {
                Log("--- Disabling Handheld Mode ---");
                Log("> reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\OEM\" /v DeviceForm /f");
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM", true))
                    {
                        if (key != null)
                        {
                            key.DeleteValue("DeviceForm", false);
                            Log("DeviceForm registry value removed.");
                        }
                        else
                        {
                            Log("Registry key not found - DeviceForm was not set.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("Registry error: {0}", ex.Message));
                }
                Log("");
                Log("--- Done ---");
                Log("");
                await RefreshStatusAsync();
            }
            finally
            {
                isWorking = false;
                SetButtonsEnabled(true);
            }
        }
    }
}
