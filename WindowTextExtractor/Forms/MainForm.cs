using System;
using System.ComponentModel;
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
using System.Xml.Linq;
using WindowTextExtractor.Extensions;
using WindowTextExtractor.Utils;
using WindowTextExtractor.Native;
using WindowTextExtractor.Native.Enums;
using WindowTextExtractor.Native.Structs;
using AForge.Video.FFMPEG;

namespace WindowTextExtractor.Forms
{
    public partial class MainForm : Form, IMessageFilter
    {
        private const string DEFAULT_VIDEO_FILE_NAME = "Window.avi";
        private const string DEFAULT_FONT_NAME = "Courier New";
        private const int DEFAULT_FONT_SIZE = 10;
        private const int DEFAULT_FPS = 12;
        private const decimal DEFAULT_SCALE = 1;

        private readonly int _processId;
        private readonly int _messageId;
        private bool _isButtonTargetMouseDown;
        private string _64BitFilePath;
        private string _informationFileName;
        private string _textFileName;
        private string _textListFileName;
        private string _imageFileName;
        private string _videoFileName;
        private IntPtr _windowHandle;
        private string _listText;
        private int _fps;
        private decimal _scale;
        private object _lockObject;
        private bool _refreshImage;
        private bool _imageTab;
        private bool _isRecording;
        private bool _captureCursor;
        private DateTime? _startRecordingTime;
        private VideoFileWriter _videoWriter;
        private Bitmap _image;

        private AccurateTimer _captureWindowTimer;
        private AccurateTimer _updatePictureBoxTimer;
        private AccurateTimer _updateButtonTimer;
        private AccurateTimer _writeVideoFrameTimer;


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
            _textListFileName = string.Empty;
            _imageFileName = string.Empty;
            _videoFileName = Path.Combine(AssemblyUtils.AssemblyDirectory, DEFAULT_VIDEO_FILE_NAME);
            _windowHandle = IntPtr.Zero;
            _listText = string.Empty;
            _refreshImage = true;
            _captureCursor = true;
            _imageTab = false;
            _isRecording = false;
            _startRecordingTime = null;
            _fps = DEFAULT_FPS;
            _scale = DEFAULT_SCALE;
            numericFps.Value = DEFAULT_FPS;
            numericScale.Value = DEFAULT_SCALE;
            cmbRefresh.SelectedIndex = 0;
            cmbCaptureCursor.SelectedIndex = 0;
            _image = null;
            _videoWriter = new VideoFileWriter();
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

            InitTimers(_fps);
            _updateButtonTimer = new AccurateTimer(UpdateButtonCallback, 500);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            var isRecording = false;
            lock (_lockObject)
            {
                isRecording = _isRecording;
            }

            if (isRecording)
            {
                MessageBox.Show("You should stop recording.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.RemoveMessageFilter(this);
            _captureWindowTimer?.Stop();
            _updatePictureBoxTimer?.Stop();
            _updateButtonTimer?.Stop();
            _writeVideoFrameTimer?.Stop();
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
                        AddTextToList(password);
                        OnContentChanged();
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            OnContentChanged();
        }

        private void txtContent_MultilineChanged(object sender, EventArgs e)
        {
            OnContentChanged();
        }

        private void gvTextList_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gvTextList.Rows)
            {
                var firstCell = row.Cells[0];
                if (firstCell.Selected)
                {
                    txtContent.Text = ((string)firstCell.Value) ?? string.Empty;
                    OnContentChanged();
                    break;
                }
            }
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

            if (!File.Exists(_informationFileName))
            {
                dialog.InitialDirectory = AssemblyUtils.AssemblyDirectory;
            }

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

            if (!File.Exists(_textFileName))
            {
                dialog.InitialDirectory = AssemblyUtils.AssemblyDirectory;
            }

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                _textFileName = dialog.FileName;
                File.WriteAllText(dialog.FileName, txtContent.Text, Encoding.UTF8);
            }
        }

        private void menuItemSaveTextListAs_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                FileName = File.Exists(_textListFileName) ? Path.GetFileName(_textListFileName) : "*.xml",
                DefaultExt = "xml",
                RestoreDirectory = false,
                Filter = "XML Documents (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            if (!File.Exists(_textListFileName))
            {
                dialog.InitialDirectory = AssemblyUtils.AssemblyDirectory;
            }

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                _textListFileName = dialog.FileName;
                var document = new XDocument();
                document.Add(new XElement("items", gvTextList.Rows.OfType<DataGridViewRow>().Select(x => new XElement("item", ((string)x.Cells[0].Value) ?? string.Empty))));
                FileUtils.Save(_textListFileName, document);
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

            if (!File.Exists(_imageFileName))
            {
                dialog.InitialDirectory = AssemblyUtils.AssemblyDirectory;
            }

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

        private void menuItemChecked_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            menuItem.Checked = !menuItem.Checked;
        }

        private void menuItemShowTextList_Click(object sender, EventArgs e)
        {
            menuItemShowTextList.Checked = !menuItemShowTextList.Checked;
            if (menuItemShowTextList.Checked)
            {
                splitTextContainer.Panel2Collapsed = false;
                splitTextContainer.Panel2.Show();
            }
            else
            {
                splitTextContainer.Panel2Collapsed = true;
                splitTextContainer.Panel2.Hide();
            }
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            var dialog = new AboutForm();
            dialog.ShowDialog(this);
        }

        private void btnTarget_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_isButtonTargetMouseDown)
            {
                _isButtonTargetMouseDown = true;
                gvInformation.Rows.Clear();
                gvInformation.Tag = null;
                gvTextList.Rows.Clear();
                txtContent.Text = string.Empty;
                if (!TopMost)
                {
                    SendToBack();
                }
            }
        }

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            var windowHandle = IntPtr.Zero;
            lock(_lockObject)
            {
                windowHandle = _windowHandle;
            }
            var button = (Button)sender;
            var cmdShow = button.Text == "Show" ? ShowWindowCommands.SW_SHOW : ShowWindowCommands.SW_HIDE;
            User32.ShowWindow(windowHandle, cmdShow);
            button.Text = User32.IsWindowVisible(windowHandle) ? "Hide" : "Show";
            var windowInformation = WindowUtils.GetWindowInformation(windowHandle);
            FillInformation(windowInformation);
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            var isRecording = false;
            lock (_lockObject)
            {
                _isRecording = !_isRecording;
                _startRecordingTime = _isRecording ? DateTime.Now : (DateTime?)null;
                if (_isRecording)
                {
                    _videoWriter.Open(_videoFileName, _image.Width, _image.Height, _fps, VideoCodec.Raw);
                }
                else
                {
                    _videoWriter.Close();
                }
                isRecording = _isRecording;
            }

            var button = (Button)sender;
            button.Text = isRecording ? "Stop" : "Record";
            btnTarget.Enabled = !isRecording;
            btnShowHide.Enabled = !isRecording;
            cmbRefresh.Enabled = !isRecording;
            cmbCaptureCursor.Enabled = !isRecording;
            btnBrowseFile.Enabled = !isRecording;
            numericFps.Enabled = !isRecording;
            numericScale.Enabled = !isRecording;
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                InitialDirectory = Path.GetDirectoryName(_videoFileName),
                FileName = string.IsNullOrEmpty(_videoFileName) ? "*.avi" : Path.GetFileName(_videoFileName),
                DefaultExt = "avi",
                RestoreDirectory = false,
                Filter = "Video Files (*.avi)|*.avi|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                lock (_lockObject)
                {
                    _videoFileName = dialog.FileName;
                }
            }
        }

        private void numericFps_ValueChanged(object sender, EventArgs e)
        {
            _fps = (int)((NumericUpDown)sender).Value;
            InitTimers(_fps);
        }

        private void numericScale_ValueChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _scale = ((NumericUpDown)sender).Value;
            }
        }

        private void cmbRefresh_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _refreshImage = ((ComboBox)sender).SelectedIndex == 0;
            }
        }

        private void cmbCaptureCursor_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _captureCursor = ((ComboBox)sender).SelectedIndex == 0;
            }
        }

        private void tabContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                var tab = (TabControl)sender;
                _imageTab = tab.SelectedTab.Text == "Image";
                EnableImageTabControls();
            }
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
                                var cursorPosition = Cursor.Position;
                                var element = AutomationElement.FromPoint(new System.Windows.Point(cursorPosition.X, cursorPosition.Y));
                                if (element != null && element.Current.ProcessId != _processId)
                                {
                                    var windowHandle = new IntPtr(element.Current.NativeWindowHandle);
                                    windowHandle = windowHandle == IntPtr.Zero ? User32.WindowFromPoint(new Point(cursorPosition.X, cursorPosition.Y)) : windowHandle;
                                    
                                    var previouseHandle = IntPtr.Zero;
                                    lock (_lockObject)
                                    {
                                        previouseHandle = _windowHandle;
                                    }

                                    if (!menuItemAlwaysRefreshTabs.Checked && previouseHandle == windowHandle)
                                    {
                                        return false;
                                    }

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
                                                    var text = passwords.Count > 1 ? string.Join(Environment.NewLine, passwords.Select((x, i) => "Password " + (i + 1) + ": " + x)) : passwords[0];
                                                    txtContent.Text = text;
                                                    txtContent.ScrollTextToEnd();
                                                    AddTextToList(text);
                                                    OnContentChanged();
                                                }
                                            }
                                        }
                                        else if (Environment.Is64BitOperatingSystem && !process.HasExited && !process.IsWow64Process())
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
                                        AddTextToList(text);

                                        var scale = 1m;
                                        var captureCursor = false;
                                        lock (_lockObject)
                                        {
                                            _windowHandle = windowHandle;
                                            scale = _scale;
                                            captureCursor = _captureCursor;
                                        }
                                        if (scale == 1m)
                                        {
                                            var newImage = WindowUtils.CaptureWindow(windowHandle, captureCursor);
                                            FillImage(newImage);
                                        }
                                        else
                                        {
                                            using (var image = WindowUtils.CaptureWindow(windowHandle, captureCursor))
                                            {
                                                var newImage = ImageUtils.ResizeImage(image, (int)(image.Width * scale), (int)(image.Height * scale));
                                                FillImage(newImage);
                                            }
                                        }
                                        var windowInformation = WindowUtils.GetWindowInformation(windowHandle);
                                        FillInformation(windowInformation);
                                        OnContentChanged();
                                    }

                                    btnShowHide.Text = User32.IsWindowVisible(windowHandle) ? "Hide" : "Show";
                                    btnShowHide.Visible = true;
                                }
                                else
                                {
                                    lock (_lockObject)
                                    {
                                        _windowHandle = IntPtr.Zero;
                                    }
                                    FillImage(Properties.Resources.OnePixel);
                                    FillInformation(new WindowInformation());
                                    OnContentChanged();
                                    btnShowHide.Visible = false;
                                }
                                EnableImageTabControls();
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }
                    break;
            }

            return false;
        }

        private void CaptureWindowCallback()
        {
            var scale = 1m;
            var windowHandle = IntPtr.Zero;
            var imageTab = false;
            var isRecording = false;
            var captureCursor = false;


            lock (_lockObject)
            {
                scale = _scale;
                windowHandle = _windowHandle;
                imageTab = _imageTab;
                isRecording = _isRecording;
                captureCursor = _captureCursor;
            }

            var newImage = (Bitmap)null;

            if (windowHandle != null && User32.IsWindow(windowHandle))
            {
                if (imageTab || isRecording)
                {
                    if (scale == 1m)
                    {
                        newImage = WindowUtils.CaptureWindow(windowHandle, captureCursor);
                    }
                    else
                    {
                        using (var sourceImage = WindowUtils.CaptureWindow(windowHandle, captureCursor))
                        {
                            newImage = ImageUtils.ResizeImage(sourceImage, (int)(sourceImage.Width * scale), (int)(sourceImage.Height * scale));
                        }
                    }
                }
            }
            else
            {
                newImage = (Bitmap)Properties.Resources.OnePixel.Clone();
            }

            var oldImage = (Bitmap)null;

            lock (_lockObject)
            {
                if (newImage != null)
                {
                    oldImage = _image;
                    _image = newImage;
                }
            }

            oldImage?.Dispose();
        }

        private void UpdatePictureBoxCallback()
        {
            var image = (Bitmap)null;

            lock (_lockObject)
            {
                if (_imageTab && _refreshImage)
                {
                    image = (Bitmap)_image.Clone();
                }
            }

            if (image != null)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    FillImage(image);
                    OnContentChanged();
                });
            }
        }

        private void UpdateButtonCallback()
        {
            var startRecordingTime = (DateTime?)null;

            lock (_lockObject)
            {
                startRecordingTime = _startRecordingTime;
            }

            if (startRecordingTime.HasValue)
            {
                var text = $"Stop{Environment.NewLine}{Environment.NewLine}{DateTime.Now.Subtract(startRecordingTime.Value):hh\\:mm\\:ss}";
                BeginInvoke((MethodInvoker)delegate
                {
                    btnRecord.Text = text;
                });
            }
        }

        private void WriteVideoFrameCallback()
        {
            try
            {
                lock (_lockObject)
                {
                    if (_isRecording)
                    {
                        _videoWriter.WriteVideoFrame(_image);
                    }
                }
            }
            catch (ArgumentException e) when (e.Message.Contains("size must be of the same as video size"))
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Don't resize the window while recording.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });

                throw;
            }
        }

        private void InitTimers(int fps)
        {
            var timerInterval = (int)Math.Round(1000m / fps, 0, MidpointRounding.AwayFromZero);
            _captureWindowTimer?.Stop();
            _updatePictureBoxTimer?.Stop();
            _writeVideoFrameTimer?.Stop();
            _captureWindowTimer = new AccurateTimer(CaptureWindowCallback, timerInterval);
            _updatePictureBoxTimer = new AccurateTimer(UpdatePictureBoxCallback, timerInterval);
            _writeVideoFrameTimer = new AccurateTimer(WriteVideoFrameCallback, timerInterval);
        }

        private void EnableImageTabControls()
        {
            btnRecord.Visible = _imageTab && btnShowHide.Visible;
            lblRefresh.Visible = _imageTab && btnShowHide.Visible;
            cmbRefresh.Visible = _imageTab && btnShowHide.Visible;
            lblCaptureCursor.Visible = _imageTab && btnShowHide.Visible;
            cmbCaptureCursor.Visible = _imageTab && btnShowHide.Visible;
            lblFps.Visible = _imageTab && btnShowHide.Visible;
            numericFps.Visible = _imageTab && btnShowHide.Visible;
            lblScale.Visible = _imageTab && btnShowHide.Visible;
            numericScale.Visible = _imageTab && btnShowHide.Visible;
            lblRecord.Visible = _imageTab && btnShowHide.Visible;
            btnBrowseFile.Visible = _imageTab && btnShowHide.Visible;
        }

        private void OnContentChanged()
        {
            lblTotalChars.Text = "Total Chars: " + txtContent.Text.Length;
            lblTotalLines.Text = "Total Lines: " + txtContent.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
            lblImageSize.Text = "Image Size: " + (pbContent != null && pbContent.Image != null ? $"{pbContent.Image.Width}x{pbContent.Image.Height}" : string.Empty);
            menuItemSaveTextAs.Enabled = txtContent.Text.Length > 0;
            menuItemSaveTextListAs.Enabled = gvTextList.Rows.Count > 0;
            menuItemSaveImageAs.Enabled = pbContent.Image != null;
            menuItemSaveInformationAs.Enabled = gvInformation.Tag != null;
        }

        private void FillInformation(WindowInformation windowInformation)
        {
            gvInformation.Rows.Clear();
            gvInformation.Tag = null;

            if (windowInformation.WindowDetails.Keys.Any())
            {
                var indexHeader = gvInformation.Rows.Add();
                var rowHeader = gvInformation.Rows[indexHeader];
                rowHeader.Cells[0].Value = "Window Information";
                rowHeader.Cells[0].Style.BackColor = Color.LightGray;
                rowHeader.Cells[1].Style.BackColor = Color.LightGray;
            }

            foreach (var windowDetailKey in windowInformation.WindowDetails.Keys)
            {
                var index = gvInformation.Rows.Add();
                var row = gvInformation.Rows[index];
                row.Cells[0].Value = windowDetailKey;
                row.Cells[1].Value = windowInformation.WindowDetails[windowDetailKey];
            }

            if (windowInformation.ProcessDetails.Keys.Any())
            {
                var indexHeader = gvInformation.Rows.Add();
                var rowHeader = gvInformation.Rows[indexHeader];
                rowHeader.Cells[0].Value = "Process Information";
                rowHeader.Cells[0].Style.BackColor = Color.LightGray;
                rowHeader.Cells[1].Style.BackColor = Color.LightGray;
            }

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

        private void AddTextToList(string text)
        {
            if (!menuItemShowEmptyItems.Checked && string.IsNullOrEmpty(text))
            {
                return;
            }

            if (menuItemNotRepeated.Checked && _listText == text)
            {
                return;
            }

            _listText = text;

            var index = gvTextList.Rows.Add();
            var row = gvTextList.Rows[index];
            row.Cells[0].Value = text;
            row.Selected = true;
            gvTextList.FirstDisplayedScrollingRowIndex = index;
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