using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Text;
using System.Linq;
using WindowTextExtractor.Extensions;
using WindowTextExtractor.Utils;

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
        private string _fileName;

        public MainForm()
        {
            InitializeComponent();
            _isButtonTargetMouseDown = false;
            _processId = Process.GetCurrentProcess().Id;
            _messageId = NativeMethods.RegisterWindowMessage("WINDOW_TEXT_EXTRACTOR_HOOK");
            _64BitFilePath = "";
            _fileName = "";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Application.AddMessageFilter(this);

            menuItemAlwaysOnTop_Click(this, EventArgs.Empty);
            OnTextContentChanged();

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
                if (!TopMost)
                {
                    SendToBack();
                }
            }
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            OnTextContentChanged();
        }

        private void txtContent_MultilineChanged(object sender, EventArgs e)
        {
            OnTextContentChanged();
        }

        private void menuItemSaveFileAs_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                FileName = File.Exists(_fileName) ? Path.GetFileName(_fileName) : "*.txt",
                DefaultExt = "txt",
                RestoreDirectory = false,
                Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                _fileName = dialog.FileName;
                File.WriteAllText(_fileName, txtContent.Text, Encoding.UTF8);
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
                        OnTextContentChanged();
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
                                NativeMethods.SetCursor(Properties.Resources.Target.Handle);
                                var cursorPosition = System.Windows.Forms.Cursor.Position;
                                var element = AutomationElement.FromPoint(new System.Windows.Point(cursorPosition.X, cursorPosition.Y));
                                if (element != null && element.Current.ProcessId != _processId)
                                {
                                    if (element.Current.IsPassword)
                                    {
                                        var elementHandle = new IntPtr(element.Current.NativeWindowHandle);
                                        var process = Process.GetProcessById(element.Current.ProcessId);
                                        if (process.ProcessName.ToLower() == "iexplore")
                                        {
                                            elementHandle = elementHandle == IntPtr.Zero ? NativeMethods.WindowFromPoint(new Point(cursorPosition.X, cursorPosition.Y)) : elementHandle;
                                            if (elementHandle != IntPtr.Zero)
                                            {
                                                var passwords = WindowUtils.GetPasswordsFromHtmlPage(elementHandle);
                                                if (passwords.Any())
                                                {
                                                    txtContent.Text = passwords.Count > 1 ? string.Join(Environment.NewLine, passwords.Select((x, i) => "Password " + (i + 1) + ": " + x)) : passwords[0];
                                                    txtContent.ScrollTextToEnd();
                                                    OnTextContentChanged();
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
                                            NativeMethods.SetHook(Handle, elementHandle, _messageId);
                                            NativeMethods.QueryPasswordEdit();
                                            NativeMethods.UnsetHook(Handle, elementHandle);
                                        }
                                    }
                                    else
                                    {
                                        var text = element.GetTextFromConsole() ?? element.GetTextFromWindow();
                                        txtContent.Text = text == null ? "" : text.TrimEnd().TrimEnd(Environment.NewLine);
                                        txtContent.ScrollTextToEnd();
                                        OnTextContentChanged();
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

        private void OnTextContentChanged()
        {
            lblTotalChars.Text = "Total Chars: " + txtContent.Text.Length;
            lblTotalLines.Text = "Total Lines: " + txtContent.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
            menuItemSaveFileAs.Enabled = txtContent.Text.Length > 0;
        }
    }
}