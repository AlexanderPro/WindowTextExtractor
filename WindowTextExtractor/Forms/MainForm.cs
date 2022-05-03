using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Linq;
using WindowTextExtractor.Extensions;
using WindowTextExtractor.Utils;
using WindowTextExtractor.Native;
using WindowTextExtractor.Native.Enums;
using WindowTextExtractor.Native.Structs;

namespace WindowTextExtractor.Forms
{
    public partial class MainForm : Form, IMessageFilter
    {
        private const string DEFAULT_FONT_NAME = "Courier New";
        private const int DEFAULT_FONT_SIZE = 10;
        private const int DEFAULT_FPS = 24;

        private readonly int _processId;
        private readonly int _messageId;
        private bool _isButtonTargetMouseDown;
        private string _64BitFilePath;
        private string _informationFileName;
        private string _textFileName;
        private string _imageFileName;
        private IntPtr _windowHandle;
        private int _fps;
        private object _lockObject;
        private bool _refreshImage;
        private Thread _refreshImageThread;
        private bool _imageTab;

        public MainForm()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            Application.ThreadException += OnThreadException;

            _lockObject = new object();
            _isButtonTargetMouseDown = false;
            _processId = Process.GetCurrentProcess().Id;
            _messageId = User32.RegisterWindowMessage("WINDOW_TEXT_EXTRACTOR_HOOK");
            _64BitFilePath = string.Empty;
            _informationFileName = string.Empty;
            _textFileName = string.Empty;
            _imageFileName = string.Empty;
            _windowHandle = IntPtr.Zero;
            _refreshImage = true;
            _fps = DEFAULT_FPS;
            _imageTab = false;
            numericFps.Value = DEFAULT_FPS;
            cmbRefresh.SelectedIndex = 0;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Application.AddMessageFilter(this);

            menuItemAlwaysOnTop_Click(this, EventArgs.Empty);
            OnContentChanged();

            var font = new Font(DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE, FontStyle.Regular, GraphicsUnit.Point);
            if (font.Name == DEFAULT_FONT_NAME)
            {
                txtContent.Font = font;
            }
            else
            {
                font.Dispose();
            }

#if WIN32
            if (Environment.Is64BitOperatingSystem)
            {
                var fileName = "WindowTextExtractor64.exe";
                var directoryName = Path.GetDirectoryName(AssemblyUtils.AssemblyLocation);
                _64BitFilePath = Path.Combine(directoryName, fileName);
                if (!File.Exists(_64BitFilePath))
                {
                    var message = string.Format("{0} is not found.", fileName);
                    MessageBox.Show(message, AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
            }
#endif

            _refreshImageThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                UpdateImage();
            });
            _refreshImageThread.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.RemoveMessageFilter(this);
            _refreshImageThread?.Abort();
        }

        private void btnTarget_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_isButtonTargetMouseDown)
            {
                _isButtonTargetMouseDown = true;
                gvInformation.Rows.Clear();
                gvInformation.Tag = null;
                txtContent.Text = string.Empty;
                pbContent.Image?.Dispose();
                if (!TopMost)
                {
                    SendToBack();
                }
            }
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            OnContentChanged();
        }

        private void txtContent_MultilineChanged(object sender, EventArgs e)
        {
            OnContentChanged();
        }

        private void menuItemSaveInformationAs_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                FileName = File.Exists(_informationFileName) ? Path.GetFileName(_informationFileName) : "*.txt",
                DefaultExt = "txt",
                RestoreDirectory = false,
                Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                _informationFileName = dialog.FileName;
                var content = gvInformation.Tag != null ? ((WindowInformation)gvInformation.Tag).ToString() : string.Empty;
                File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
            }
        }

        private void menuItemSaveTextAs_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                FileName = File.Exists(_textFileName) ? Path.GetFileName(_textFileName) : "*.txt",
                DefaultExt = "txt",
                RestoreDirectory = false,
                Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                _textFileName = dialog.FileName;
                File.WriteAllText(dialog.FileName, txtContent.Text, Encoding.UTF8);
            }
        }

        private void menuItemSaveImageAs_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                FileName = File.Exists(_imageFileName) ? Path.GetFileName(_imageFileName) : "*.bmp",
                DefaultExt = "bmp",
                RestoreDirectory = false,
                Filter = "Bitmap Image (*.bmp)|*.bmp|Gif Image (*.gif)|*.gif|JPEG Image (*.jpeg)|*.jpeg|Png Image (*.png)|*.png|Tiff Image (*.tiff)|*.tiff|Wmf Image (*.wmf)|*.wmf"
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                _imageFileName = dialog.FileName;
                var fileExtension = Path.GetExtension(dialog.FileName).ToLower();
                var imageFormat = fileExtension == ".bmp" ? ImageFormat.Bmp :
                    fileExtension == ".gif" ? ImageFormat.Gif :
                    fileExtension == ".jpeg" ? ImageFormat.Jpeg :
                    fileExtension == ".png" ? ImageFormat.Png :
                    fileExtension == ".tiff" ? ImageFormat.Tiff : ImageFormat.Wmf;
                pbContent.Image.Save(dialog.FileName, imageFormat);
            }
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuItemFont_Click(object sender, EventArgs e)
        {
            var dialog = new FontDialog();
            dialog.ShowHelp = false;
            dialog.ShowColor = false;
            dialog.Font = txtContent.Font;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                txtContent.Font = dialog.Font;
            }
        }

        private void menuItemAlwaysOnTop_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            menuItemAlwaysOnTop.Checked = TopMost;
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            var dialog = new AboutForm();
            dialog.ShowDialog(this);
        }

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            lock(_lockObject)
            {
                var button = (Button)sender;
                var cmdShow = button.Text == "Show" ? ShowWindowCommands.SW_SHOW : ShowWindowCommands.SW_HIDE;
                User32.ShowWindow(_windowHandle, cmdShow);
                button.Text = User32.IsWindowVisible(_windowHandle) ? "Hide" : "Show";
                var windowInformation = WindowUtils.GetWindowInformation(_windowHandle);
                FillInformation(windowInformation);
            }
        }       

        private void numericFps_ValueChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _fps = (int)numericFps.Value;
            }
        }

        private void cmbRefresh_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _refreshImage = ((ComboBox)sender).SelectedIndex == 0;
            }
        }

        private void tabContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                var tab = (TabControl)sender;
                _imageTab = tab.SelectedTab.Text == "Image";
                lblRefresh.Visible = _imageTab;
                cmbRefresh.Visible = _imageTab;
                lblFps.Visible = _imageTab;
                numericFps.Visible = _imageTab;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Constants.WM_COPYDATA:
                    {
                        var cds = (CopyDataStruct)Marshal.PtrToStructure(m.LParam, typeof(CopyDataStruct));
                        var password = Marshal.PtrToStringAuto(cds.lpData);
                        txtContent.Text = password;
                        txtContent.ScrollTextToEnd();
                        OnContentChanged();
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case Constants.WM_LBUTTONUP:
                    {
                        if (_isButtonTargetMouseDown)
                        {
                            _isButtonTargetMouseDown = false;
                            User32.SetCursor(Cursors.Default.Handle);
                            if (!TopMost)
                            {
                                BringToFront();
                            }
                        }
                    }
                    break;

                case Constants.WM_MOUSEMOVE:
                    {
                        try
                        {
                            if (_isButtonTargetMouseDown)
                            {
                                User32.SetCursor(Properties.Resources.Target32.Handle);
                                var cursorPosition = System.Windows.Forms.Cursor.Position;
                                var element = AutomationElement.FromPoint(new System.Windows.Point(cursorPosition.X, cursorPosition.Y));
                                if (element != null && element.Current.ProcessId != _processId)
                                {
                                    var windowHandle = new IntPtr(element.Current.NativeWindowHandle);
                                    windowHandle = windowHandle == IntPtr.Zero ? User32.WindowFromPoint(new Point(cursorPosition.X, cursorPosition.Y)) : windowHandle;
                                    if (element.Current.IsPassword)
                                    {
                                        var process = Process.GetProcessById(element.Current.ProcessId);
                                        if (process.ProcessName.ToLower() == "iexplore")
                                        {
                                            if (windowHandle != IntPtr.Zero)
                                            {
                                                var passwords = WindowUtils.GetPasswordsFromHtmlPage(windowHandle);
                                                if (passwords.Any())
                                                {
                                                    txtContent.Text = passwords.Count > 1 ? string.Join(Environment.NewLine, passwords.Select((x, i) => "Password " + (i + 1) + ": " + x)) : passwords[0];
                                                    txtContent.ScrollTextToEnd();
                                                    OnContentChanged();
                                                }
                                            }
                                        } else if (Environment.Is64BitOperatingSystem && !process.HasExited && !process.IsWow64Process())
                                        {
                                            Process.Start(new ProcessStartInfo
                                            {
                                                FileName = _64BitFilePath,
                                                Arguments = string.Format("{0} {1} {2}", Handle.ToInt32(), element.Current.NativeWindowHandle, _messageId)
                                            });
                                        }
                                        else
                                        {
                                            Hook.SetHook(Handle, windowHandle, _messageId);
                                            Hook.QueryPasswordEdit();
                                            Hook.UnsetHook(Handle, windowHandle);
                                        }
                                    }
                                    else
                                    {
                                        var text = element.GetTextFromConsole() ?? element.GetTextFromWindow();
                                        txtContent.Text = text == null ? "" : text.TrimEnd().TrimEnd(Environment.NewLine);
                                        txtContent.ScrollTextToEnd();
                                        lock (_lockObject)
                                        {
                                            _windowHandle = windowHandle;
                                            var image = WindowUtils.CaptureWindow(windowHandle);
                                            FillImage(image);
                                            var windowInformation = WindowUtils.GetWindowInformation(windowHandle);
                                            FillInformation(windowInformation);
                                        }
                                        OnContentChanged();
                                    }

                                    btnShowHide.Text = User32.IsWindowVisible(windowHandle) ? "Hide" : "Show";
                                    btnShowHide.Visible = true;
                                }
                                else
                                {
                                    btnShowHide.Visible = false;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    break;
            }

            return false;
        }

        private void UpdateImage()
        {
            try
            {                
                while(true)
                {
                    var fps = 0;
                    var windowHandle = IntPtr.Zero;
                    var refreshImage = true;
                    var imageTab = true;

                    lock(_lockObject)
                    {
                        fps = _fps;
                        windowHandle = _windowHandle;
                        refreshImage = _refreshImage;
                        imageTab = _imageTab;
                    }

                    if (imageTab && refreshImage)
                    {
                        if (windowHandle != null && windowHandle != IntPtr.Zero && User32.IsWindow(windowHandle))
                        {
                            var image = WindowUtils.CaptureWindow(windowHandle);
                            BeginInvoke((MethodInvoker)delegate
                            {
                                FillImage(image);
                            });
                        }
                        else
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                FillImage(Properties.Resources.OnePixel);
                            });
                        }
                    }

                    var timeout = 1000 / fps;
                    Thread.Sleep(timeout);
                }
            }
            catch
            {
            }
        }

        private void OnContentChanged()
        {
            lblTotalChars.Text = "Total Chars: " + txtContent.Text.Length;
            lblTotalLines.Text = "Total Lines: " + txtContent.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
            lblImageSize.Text = "Image Size: " + (pbContent != null && pbContent.Image != null ? $"{pbContent.Image.Width}x{pbContent.Image.Height}" : string.Empty);
            menuItemSaveTextAs.Enabled = txtContent.Text.Length > 0;
            menuItemSaveImageAs.Enabled = pbContent.Image != null;
            menuItemSaveInformationAs.Enabled = gvInformation.Tag != null;
        }

        private void FillInformation(WindowInformation windowInformation)
        {
            gvInformation.Rows.Clear();
            gvInformation.Tag = null;

            var indexHeader = gvInformation.Rows.Add();
            var rowHeader = gvInformation.Rows[indexHeader];
            rowHeader.Cells[0].Value = "Window Information";
            rowHeader.Cells[0].Style.BackColor = Color.LightGray;
            rowHeader.Cells[1].Style.BackColor = Color.LightGray;

            foreach (var windowDetailKey in windowInformation.WindowDetails.Keys)
            {
                var index = gvInformation.Rows.Add();
                var row = gvInformation.Rows[index];
                row.Cells[0].Value = windowDetailKey;
                row.Cells[1].Value = windowInformation.WindowDetails[windowDetailKey];
            }

            indexHeader = gvInformation.Rows.Add();
            rowHeader = gvInformation.Rows[indexHeader];
            rowHeader.Cells[0].Value = "Process Information";
            rowHeader.Cells[0].Style.BackColor = Color.LightGray;
            rowHeader.Cells[1].Style.BackColor = Color.LightGray;

            foreach (var processDetailKey in windowInformation.ProcessDetails.Keys)
            {
                var index = gvInformation.Rows.Add();
                var row = gvInformation.Rows[index];
                row.Cells[0].Value = processDetailKey;
                row.Cells[1].Value = windowInformation.ProcessDetails[processDetailKey];
            }
            gvInformation.Tag = windowInformation;
        }

        private void FillImage(Image image)
        {
            pbContent.Image?.Dispose();
            pbContent.Image = image;
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            ex = ex ?? new Exception("OnCurrentDomainUnhandledException");
            OnThreadException(sender, new ThreadExceptionEventArgs(ex));
        }

        private void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}