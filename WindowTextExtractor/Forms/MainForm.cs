using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Linq;
using WindowTextExtractor.Extensions;
using WindowTextExtractor.Utils;
using WindowTextExtractor.Native;

namespace WindowTextExtractor.Forms
{
    public partial class MainForm : Form, IMessageFilter
    {
        private const string DEFAULT_FONT_NAME = "Courier New";
        private const int DEFAULT_FONT_SIZE = 10;

        private readonly int _processId;
        private readonly int _messageId;
        private bool _isButtonTargetMouseDown;
        private string _64BitFilePath;
        private string _informationFileName;
        private string _textFileName;
        private string _imageFileName;

        public MainForm()
        {
            InitializeComponent();
            _isButtonTargetMouseDown = false;
            _processId = Process.GetCurrentProcess().Id;
            _messageId = NativeMethods.RegisterWindowMessage("WINDOW_TEXT_EXTRACTOR_HOOK");
            _64BitFilePath = string.Empty;
            _informationFileName = string.Empty;
            _textFileName = string.Empty;
            _imageFileName = string.Empty;
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
                var resourceName = "WindowTextExtractor.WindowTextExtractor64.exe";
                var fileName = "WindowTextExtractor64.exe";
                var directoryName = Path.GetDirectoryName(AssemblyUtils.AssemblyLocation);
                _64BitFilePath = Path.Combine(directoryName, fileName);
                try
                {
                    if (!File.Exists(_64BitFilePath))
                    {
                        AssemblyUtils.ExtractFileFromAssembly(resourceName, _64BitFilePath);
                    }
                }
                catch
                {
                    var message = string.Format("Failed to load {0} process!", fileName);
                    MessageBox.Show(message, AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
#endif
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.RemoveMessageFilter(this);

#if WIN32
            if (Environment.Is64BitOperatingSystem && File.Exists(_64BitFilePath))
            {
                try
                {
                    File.Delete(_64BitFilePath);
                }
                catch
                {
                }
            }
#endif
        }

        private void btnTarget_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_isButtonTargetMouseDown)
            {
                _isButtonTargetMouseDown = true;
                gvInformation.Rows.Clear();
                gvInformation.Tag = null;
                txtContent.Text = string.Empty;
                if (pbContent.Image != null)
                {
                    pbContent.Image.Dispose();
                    pbContent.Image = null;
                }
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

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeConstants.WM_COPYDATA:
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
                case NativeConstants.WM_LBUTTONUP:
                    {
                        if (_isButtonTargetMouseDown)
                        {
                            _isButtonTargetMouseDown = false;
                            NativeMethods.SetCursor(Cursors.Default.Handle);
                            if (!TopMost)
                            {
                                BringToFront();
                            }
                        }
                    }
                    break;

                case NativeConstants.WM_MOUSEMOVE:
                    {
                        try
                        {
                            if (_isButtonTargetMouseDown)
                            {
                                NativeMethods.SetCursor(Properties.Resources.Target32.Handle);
                                var cursorPosition = System.Windows.Forms.Cursor.Position;
                                var element = AutomationElement.FromPoint(new System.Windows.Point(cursorPosition.X, cursorPosition.Y));
                                if (element != null && element.Current.ProcessId != _processId)
                                {
                                    var windowHandle = new IntPtr(element.Current.NativeWindowHandle);
                                    windowHandle = windowHandle == IntPtr.Zero ? NativeMethods.WindowFromPoint(new Point(cursorPosition.X, cursorPosition.Y)) : windowHandle;
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
                                            NativeMethods.SetHook(Handle, windowHandle, _messageId);
                                            NativeMethods.QueryPasswordEdit();
                                            NativeMethods.UnsetHook(Handle, windowHandle);
                                        }
                                    }
                                    else
                                    {
                                        var text = element.GetTextFromConsole() ?? element.GetTextFromWindow();
                                        txtContent.Text = text == null ? "" : text.TrimEnd().TrimEnd(Environment.NewLine);
                                        txtContent.ScrollTextToEnd();
                                        if (pbContent.Image != null)
                                        {
                                            pbContent.Image.Dispose();
                                            pbContent.Image = null;
                                        }
                                        pbContent.Image = WindowUtils.PrintWindow(windowHandle);
                                        var windowInformation = WindowUtils.GetWindowInformation(windowHandle);
                                        FillInformation(windowInformation);
                                        OnContentChanged();
                                    }
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
    }
}