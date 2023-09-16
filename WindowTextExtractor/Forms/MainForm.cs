using System;
using System.ComponentModel;
using System.Collections.Generic;
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
using Windows.Media.Ocr;
using WindowTextExtractor.Extensions;
using WindowTextExtractor.Utils;
using WindowTextExtractor.Diagnostics;
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
        private const int DEFAULT_BORDER_WIDTH = 10;
        private readonly Color DEAFULT_BORDER_COLOR = Color.Blue;

        private readonly int _processId;
        private readonly int _messageId;
        private bool _isButtonTargetMouseDown;
        private string _64BitFilePath;
        private string _informationFileName;
        private string _textFileName;
        private string _textListFileName;
        private string _imageFileName;
        private string _videoFileName;
        private string _environmentFileName;
        private IntPtr _windowHandle;
        private int _windowProcessId;
        private string _listText;
        private int _fps;
        private decimal _scale;
        private bool _refreshImage;
        private bool _imageTab;
        private bool _isRecording;
        private bool _captureCursor;
        private DateTime? _startRecordingTime;
        private Bitmap _image;
        private Graphics _graphics;
        private Pen _pen;
        private int _borderWidth;
        private Color _borderColor;
        private readonly object _lockObject;
        private readonly VideoFileWriter _videoWriter;

        private AccurateTimer _captureWindowTimer;
        private AccurateTimer _updatePictureBoxTimer;
        private AccurateTimer _updateButtonTimer;
        private AccurateTimer _writeVideoFrameTimer;


        public MainForm()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            Application.ThreadException += OnThreadException;
            using var currentProcess = Process.GetCurrentProcess();

            _lockObject = new object();
            _isButtonTargetMouseDown = false;
            _processId = currentProcess.Id;
            _messageId = User32.RegisterWindowMessage("WINDOW_TEXT_EXTRACTOR_HOOK");
            _64BitFilePath = string.Empty;
            _informationFileName = string.Empty;
            _textFileName = string.Empty;
            _textListFileName = string.Empty;
            _imageFileName = string.Empty;
            _videoFileName = Path.Combine(AssemblyUtils.AssemblyDirectory, DEFAULT_VIDEO_FILE_NAME);
            _environmentFileName = string.Empty;
            _windowHandle = IntPtr.Zero;
            _windowProcessId = 0;
            _listText = string.Empty;
            _refreshImage = true;
            _captureCursor = true;
            _imageTab = false;
            _isRecording = false;
            _startRecordingTime = null;
            _fps = DEFAULT_FPS;
            _scale = DEFAULT_SCALE;
            _borderWidth = DEFAULT_BORDER_WIDTH;
            _borderColor = DEAFULT_BORDER_COLOR;
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

            MenuItemAlwaysOnTopClick(this, EventArgs.Empty);
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

            try
            {
                BindLanguages();
            }
            catch
            {
            }

#if WIN32
            if (Environment.Is64BitOperatingSystem)
            {
                var fileName = "WindowTextExtractor64.exe";
                var directoryName = Path.GetDirectoryName(AssemblyUtils.AssemblyLocation);
                _64BitFilePath = Path.Combine(directoryName, fileName);
                if (!File.Exists(_64BitFilePath))
                {
                    MessageBox.Show($"{fileName} is not found.", AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void TextContentTextChanged(object sender, EventArgs e) => OnContentChanged();

        private void TextContentMultilineChanged(object sender, EventArgs e) => OnContentChanged();

        private void GridViewTextListSelectionChanged(object sender, EventArgs e)
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

        private void GridViewTextListCellClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;

            if (e.ColumnIndex == 1)
            {
                grid.Rows.RemoveAt(e.RowIndex);
                if (grid.Rows.Count > 0)
                {
                    var lastRowIndex = grid.Rows.Count - 1;
                    var lastRow = grid.Rows[lastRowIndex];
                    var firstCell = lastRow.Cells[0];
                    lastRow.Selected = true;
                    grid.FirstDisplayedScrollingRowIndex = lastRowIndex;
                    txtContent.Text = ((string)firstCell.Value) ?? string.Empty;
                }
                else
                {
                    txtContent.Text = string.Empty;
                }
                OnContentChanged();
            }
        }

        private void MenuItemSaveInformationAsClick(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                FileName = File.Exists(_informationFileName) ? Path.GetFileName(_informationFileName) : "*.txt",
                DefaultExt = "txt",
                RestoreDirectory = false,
                Filter = "Text Documents (*.txt)|*.txt|XML Documents (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            if (!File.Exists(_informationFileName))
            {
                dialog.InitialDirectory = AssemblyUtils.AssemblyDirectory;
            }

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                _informationFileName = dialog.FileName;
                var fileExtension = Path.GetExtension(dialog.FileName).ToLower();
                if (fileExtension == ".xml")
                {
                    var information = (WindowInformation)gvInformation.Tag;
                    var document = new XDocument();
                    var elements = new XElement("information",
                        new XElement("cursorInformation", information.CursorDetails.Select(x => new XElement("item", new XAttribute("name", x.Key), new XAttribute("value", x.Value)))),
                        new XElement("windowInformation", information.WindowDetails.Select(x => new XElement("item", new XAttribute("name", x.Key), new XAttribute("value", x.Value)))),
                        new XElement("processInformation", information.ProcessDetails.Select(x => new XElement("item", new XAttribute("name", x.Key), new XAttribute("value", x.Value)))));
                    document.Add(elements);
                    FileUtils.Save(dialog.FileName, document);
                }
                else
                {
                    var content = gvInformation.Tag != null ? ((WindowInformation)gvInformation.Tag).ToString() : string.Empty;
                    File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
                }
            }
        }

        private void MenuItemSaveTextAsClick(object sender, EventArgs e)
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

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                _textFileName = dialog.FileName;
                File.WriteAllText(dialog.FileName, txtContent.Text, Encoding.UTF8);
            }
        }

        private void MenuItemSaveTextListAsClick(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                FileName = File.Exists(_textListFileName) ? Path.GetFileName(_textListFileName) : "*.txt",
                DefaultExt = "txt",
                RestoreDirectory = false,
                Filter = "Text Documents (*.txt)|*.txt|XML Documents (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            if (!File.Exists(_textListFileName))
            {
                dialog.InitialDirectory = AssemblyUtils.AssemblyDirectory;
            }

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                _textListFileName = dialog.FileName;
                var fileExtension = Path.GetExtension(dialog.FileName).ToLower();
                if (fileExtension == ".xml")
                {
                    var document = new XDocument();
                    document.Add(new XElement("items", gvTextList.Rows.OfType<DataGridViewRow>().Select(x => new XElement("item", ((string)x.Cells[0].Value) ?? string.Empty))));
                    FileUtils.Save(dialog.FileName, document);
                }
                else
                {
                    var content = string.Join($"{Environment.NewLine}{new string('-', 100)}{Environment.NewLine}", gvTextList.Rows.OfType<DataGridViewRow>().Select(x => ((string)x.Cells[0].Value) ?? string.Empty));
                    File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
                }
            }
        }

        private void MenuItemSaveImageAsClick(object sender, EventArgs e)
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

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                _imageFileName = dialog.FileName;
                var fileExtension = Path.GetExtension(dialog.FileName).ToLower();
                var imageFormat = fileExtension == ".bmp" ? ImageFormat.Bmp :
                    fileExtension == ".gif" ? ImageFormat.Gif :
                    fileExtension == ".jpeg" ? ImageFormat.Jpeg :
                    fileExtension == ".png" ? ImageFormat.Png :
                    fileExtension == ".tiff" ? ImageFormat.Tiff :
                    fileExtension == ".wmf" ? ImageFormat.Wmf : ImageFormat.Bmp;
                pbContent.Image.Save(dialog.FileName, imageFormat);
            }
        }

        private void MenuItemSaveEnvironmentAsClick(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Save As",
                FileName = File.Exists(_environmentFileName) ? Path.GetFileName(_environmentFileName) : "*.txt",
                DefaultExt = "txt",
                RestoreDirectory = false,
                Filter = "Text Documents (*.txt)|*.txt|XML Documents (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            if (!File.Exists(_environmentFileName))
            {
                dialog.InitialDirectory = AssemblyUtils.AssemblyDirectory;
            }

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                _environmentFileName = dialog.FileName;
                var fileExtension = Path.GetExtension(dialog.FileName).ToLower();
                if (fileExtension == ".xml")
                {
                    if (gvEnvironment.Tag is IDictionary<string, string> variables)
                    {
                        var document = new XDocument();
                        document.Add(new XElement("items", variables.Select(x => new XElement("item", new XAttribute("name", x.Key), new XAttribute("value", x.Value)))));
                        FileUtils.Save(dialog.FileName, document);
                    }
                }
                else
                {
                    var content = string.Empty;
                    if (gvEnvironment.Tag is IDictionary<string, string> variables)
                    {
                        const int paddingSize = 25;
                        var builder = new StringBuilder(1024);
                        foreach (var variableKey in variables.Keys)
                        {
                            builder.AppendLine($"{variableKey,-paddingSize}: {variables[variableKey]}");
                        }
                        content = builder.ToString();
                    }
                    File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
                }
            }
        }

        private void MenuItemExitClick(object sender, EventArgs e) => Close();

        private void MenuItemFontClick(object sender, EventArgs e)
        {
            var dialog = new FontDialog
            {
                ShowHelp = false,
                ShowColor = false,
                Font = txtContent.Font
            };

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                txtContent.Font = dialog.Font;
            }
        }

        private void MenuItemAlwaysOnTopClick(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            menuItemAlwaysOnTop.Checked = TopMost;
        }

        private void MenuItemCheckedClick(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            menuItem.Checked = !menuItem.Checked;
        }

        private void MenuItemShowTextListClick(object sender, EventArgs e)
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

        private void MenuItemAboutClick(object sender, EventArgs e)
        {
            var dialog = new AboutForm();
            dialog.ShowDialog(this);
        }

        private void MenuItemBorderColorClick(object sender, EventArgs e)
        {
            var dialog = new ColorDialog
            {
                AllowFullOpen = true,
                AnyColor = true,
                FullOpen = true,
                Color = _borderColor
            };

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                _borderColor = dialog.Color;
            }
        }

        private void MenuItemBorderWidthClick(object sender, EventArgs e)
        {
            var borderWidthForm = new BorderWidthForm(_borderWidth);
            var result = borderWidthForm.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                _borderWidth = borderWidthForm.BorderWidth;
            }
        }

        private void ButtonTargetMouseDown(object sender, MouseEventArgs e)
        {
            if (!_isButtonTargetMouseDown)
            {
                _isButtonTargetMouseDown = true;
                gvInformation.Rows.Clear();
                gvInformation.Tag = null;
                gvEnvironment.Rows.Clear();
                gvEnvironment.Tag = null;
                gvTextList.Rows.Clear();
                txtContent.Text = string.Empty;
                if (!TopMost)
                {
                    SendToBack();
                }
            }
        }

        private void ActionButtonStripItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var windowHandle = IntPtr.Zero;
            lock (_lockObject)
            {
                windowHandle = _windowHandle;
            }

            switch (e.ClickedItem.Name)
            {
                case "miHide":
                    User32.ShowWindow(windowHandle, ShowWindowCommands.SW_HIDE);
                    break;

                case "miShow":
                    User32.ShowWindow(windowHandle, ShowWindowCommands.SW_SHOW);
                    break;

                case "miMinimize":
                    User32.PostMessage(windowHandle, Constants.WM_SYSCOMMAND, Constants.SC_MINIMIZE, 0);
                    break;

                case "miMaximize":
                    User32.PostMessage(windowHandle, Constants.WM_SYSCOMMAND, Constants.SC_MAXIMIZE, 0);
                    break;

                case "miRestore":
                    User32.PostMessage(windowHandle, Constants.WM_SYSCOMMAND, Constants.SC_RESTORE, 0);
                    break;

                case "miClose":
                    User32.PostMessage(windowHandle, Constants.WM_CLOSE, 0, 0);
                    break;
            }

            var windowInformation = WindowUtils.GetWindowInformation(windowHandle, Cursor.Position);
            FillInformation(windowInformation);
        }

        private void ButtonRecordClick(object sender, EventArgs e)
        {
            var isRecording = false;
            lock (_lockObject)
            {
                _isRecording = !_isRecording;
                _startRecordingTime = _isRecording ? DateTime.Now : (DateTime?)null;
                if (_isRecording)
                {
                    try
                    {
                        _videoWriter.Open(_videoFileName, _image.Width, _image.Height, _fps, VideoCodec.Raw);
                    }
                    catch (Exception ex)
                    {
                        _startRecordingTime = null;
                        _isRecording = false;
                        _videoWriter.Close();
                        MessageBox.Show($"Failed to start recording a video file. {ex.Message}", AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
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
            btnAction.Enabled = !isRecording;
            btnGrab.Enabled = !isRecording;
            cmbRefresh.Enabled = !isRecording;
            cmbCaptureCursor.Enabled = !isRecording;
            cmbLanguages.Enabled = !isRecording;
            btnBrowseFile.Enabled = !isRecording;
            numericFps.Enabled = !isRecording;
            numericScale.Enabled = !isRecording;
        }

        private void ButtonGrabClick(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                if (!_isRecording)
                {
                    var text = string.Empty;
                    try
                    {
                        text = ImageUtils.ExtractTextAsync((Bitmap)_image.Clone(), cmbLanguages.SelectedValue as string).GetAwaiter().GetResult();
                        var barcodes = ImageUtils.ExtractBarcodes((Bitmap)_image.Clone());
                        if (!string.IsNullOrEmpty(barcodes))
                        {
                            text += Environment.NewLine + barcodes;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to recognize a text in the image. {ex.Message}", AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(text))
                    {
                        MessageBox.Show("Text is not found in the image.", AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        txtContent.Text = text;
                        txtContent.ScrollTextToEnd();
                        AddTextToList(text);
                        tabContent.SelectedTab = tabpText;
                    }
                }
            }
        }

        private void ButtonBrowseFileClick(object sender, EventArgs e)
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

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                lock (_lockObject)
                {
                    _videoFileName = dialog.FileName;
                }
            }
        }

        private void NumericFpsValueChanged(object sender, EventArgs e)
        {
            _fps = (int)((NumericUpDown)sender).Value;
            InitTimers(_fps);
        }

        private void NumericScaleValueChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _scale = ((NumericUpDown)sender).Value;
            }
        }

        private void ComboBoxRefreshSelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _refreshImage = ((ComboBox)sender).SelectedIndex == 0;
            }
        }

        private void ComboBoxCaptureCursorSelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _captureCursor = ((ComboBox)sender).SelectedIndex == 0;
            }
        }

        private void TabContentSelectedIndexChanged(object sender, EventArgs e)
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
            Action restore = () =>
            {
                _isButtonTargetMouseDown = false;
                if (_windowHandle != IntPtr.Zero)
                {
                    WindowUtils.UpdateWindow(_windowHandle);
                    _graphics?.Dispose();
                    _pen?.Dispose();
                }
                User32.SetCursor(Cursors.Default.Handle);
                if (!TopMost)
                {
                    BringToFront();
                }
            };

            switch (m.Msg)
            {
                case Constants.WM_LBUTTONUP:
                    {
                        if (_isButtonTargetMouseDown)
                        {
                            restore.Invoke();
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

                                    var previousHandle = IntPtr.Zero;
                                    var previousProcessId = 0;
                                    lock (_lockObject)
                                    {
                                        previousHandle = _windowHandle;
                                        previousProcessId = _windowProcessId;
                                    }

                                    if (previousHandle != windowHandle)
                                    {
                                        WindowUtils.UpdateWindow(previousHandle);
                                        _graphics?.Dispose();
                                        _pen?.Dispose();
                                        _graphics = Graphics.FromHwnd(windowHandle);
                                        _pen = new Pen(_borderColor, _borderWidth);
                                        if (_borderWidth > 0)
                                        {
                                            _graphics.DrawBorder(windowHandle, _pen);
                                        }
                                    }

                                    if (!menuItemAlwaysRefreshTabs.Checked && previousHandle == windowHandle)
                                    {
                                        return false;
                                    }

                                    using var process = Process.GetProcessById(element.Current.ProcessId);
                                    if (element.Current.IsPassword)
                                    {
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
                                                Arguments = $"{Handle.ToInt32()} {element.Current.NativeWindowHandle} {_messageId}"
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
                                        text = text == null ? "" : text.TrimEnd().TrimEnd(Environment.NewLine);
                                        txtContent.Text = text;
                                        txtContent.ScrollTextToEnd();
                                        AddTextToList(text);

                                        var scale = 1m;
                                        var captureCursor = false;
                                        lock (_lockObject)
                                        {
                                            _windowHandle = windowHandle;
                                            _windowProcessId = element.Current.ProcessId;
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
                                            using var image = WindowUtils.CaptureWindow(windowHandle, captureCursor);
                                            var newImage = ImageUtils.ResizeImage(image, (int)(image.Width * scale), (int)(image.Height * scale));
                                            FillImage(newImage);
                                        }
                                        var windowInformation = WindowUtils.GetWindowInformation(windowHandle, cursorPosition);
                                        FillInformation(windowInformation);
                                        if (previousProcessId != _windowProcessId)
                                        {
                                            process.TryReadEnvironmentVariables(out var variables);
                                            FillEnvironment(variables);
                                        }
                                        OnContentChanged();
                                    }

                                    btnAction.Visible = true;
                                }
                                else
                                {
                                    WindowUtils.UpdateWindow(_windowHandle);
                                    lock (_lockObject)
                                    {
                                        _windowHandle = IntPtr.Zero;
                                        _windowProcessId = 0;
                                    }
                                    FillImage(Properties.Resources.OnePixel);
                                    FillInformation(new WindowInformation());
                                    FillEnvironment(new Dictionary<string, string>());
                                    OnContentChanged();
                                    btnAction.Visible = false;
                                }
                                EnableImageTabControls();
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            restore.Invoke();
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

            try
            {
                if (windowHandle != null && windowHandle != IntPtr.Zero && User32.IsWindowVisible(windowHandle))
                {
                    if (imageTab || isRecording)
                    {
                        if (scale == 1m)
                        {
                            newImage = WindowUtils.CaptureWindow(windowHandle, captureCursor);
                        }
                        else
                        {
                            using var sourceImage = WindowUtils.CaptureWindow(windowHandle, captureCursor);
                            newImage = ImageUtils.ResizeImage(sourceImage, (int)(sourceImage.Width * scale), (int)(sourceImage.Height * scale));
                        }
                    }
                }
                else
                {
                    newImage = Properties.Resources.OnePixel;
                }
            }
            catch (Exception e)
            {
                Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });

                throw;
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
                Invoke((MethodInvoker)delegate
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
            btnRecord.Visible = _imageTab && btnAction.Visible;
            btnGrab.Visible = _imageTab && btnAction.Visible;
            lblRefresh.Visible = _imageTab && btnAction.Visible;
            cmbRefresh.Visible = _imageTab && btnAction.Visible;
            lblLanguages.Visible = _imageTab && btnAction.Visible;
            cmbLanguages.Visible = _imageTab && btnAction.Visible;
            lblCaptureCursor.Visible = _imageTab && btnAction.Visible;
            cmbCaptureCursor.Visible = _imageTab && btnAction.Visible;
            lblFps.Visible = _imageTab && btnAction.Visible;
            numericFps.Visible = _imageTab && btnAction.Visible;
            lblScale.Visible = _imageTab && btnAction.Visible;
            numericScale.Visible = _imageTab && btnAction.Visible;
            lblRecord.Visible = _imageTab && btnAction.Visible;
            btnBrowseFile.Visible = _imageTab && btnAction.Visible;
        }

        private void OnContentChanged()
        {
            lblTotalChars.Text = "Total Chars: " + txtContent.Text.Length;
            lblTotalLines.Text = "Total Lines: " + txtContent.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
            lblImageSize.Text = "Image Size: " + (pbContent != null && pbContent.Image != null ? $"{pbContent.Image.Width}x{pbContent.Image.Height}" : string.Empty);
            menuItemSaveTextAs.Enabled = txtContent.Text.Length > 0;
            menuItemSaveTextListAs.Enabled = gvTextList.Rows.Count > 0;
            menuItemSaveImageAs.Enabled = pbContent != null && pbContent.Image != null && (pbContent.Image.Width > 1 || pbContent.Image.Height > 1);
            menuItemSaveInformationAs.Enabled = gvInformation.Tag != null && gvInformation.Tag is WindowInformation information && information.WindowDetails.Any() && information.ProcessDetails.Any();
            menuItemSaveEnvironmentAs.Enabled = gvEnvironment.Tag != null && gvEnvironment.Tag is IDictionary<string, string> variables && variables.Any();
        }

        private void FillInformation(WindowInformation windowInformation)
        {
            gvInformation.Rows.Clear();
            gvInformation.Tag = null;

            if (windowInformation.CursorDetails.Keys.Any())
            {
                var indexHeader = gvInformation.Rows.Add();
                var rowHeader = gvInformation.Rows[indexHeader];
                rowHeader.Cells[0].Value = "Cursor Information";
                rowHeader.Cells[0].Style.BackColor = Color.LightGray;
                rowHeader.Cells[1].Style.BackColor = Color.LightGray;
            }

            foreach (var cursorDetailKey in windowInformation.CursorDetails.Keys)
            {
                var index = gvInformation.Rows.Add();
                var row = gvInformation.Rows[index];
                row.Cells[0].Value = cursorDetailKey;
                row.Cells[1].Value = windowInformation.CursorDetails[cursorDetailKey];
            }

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

        private void FillEnvironment(IDictionary<string, string> variables)
        {
            gvEnvironment.Rows.Clear();
            gvEnvironment.Tag = null;

            if (variables.Keys.Any())
            {
                var indexHeader = gvEnvironment.Rows.Add();
                var rowHeader = gvEnvironment.Rows[indexHeader];
                rowHeader.Cells[0].Value = "Variable";
                rowHeader.Cells[0].Style.BackColor = Color.LightGray;
                rowHeader.Cells[1].Value = "Value";
                rowHeader.Cells[1].Style.BackColor = Color.LightGray;
            }

            foreach (var variableKey in variables.Keys)
            {
                var index = gvEnvironment.Rows.Add();
                var row = gvEnvironment.Rows[index];
                row.Cells[0].Value = variableKey;
                row.Cells[1].Value = variables[variableKey];
            }

            gvEnvironment.Tag = variables;
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

        private void BindLanguages()
        {
            cmbLanguages.DisplayMember = "Text";
            cmbLanguages.ValueMember = "Value";
            cmbLanguages.DataSource = OcrEngine.AvailableRecognizerLanguages.Select(x => new { Text = x.DisplayName, Value = x.LanguageTag }).ToList();
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("OnCurrentDomainUnhandledException");
            OnThreadException(sender, new ThreadExceptionEventArgs(ex));
        }

        private void OnThreadException(object sender, ThreadExceptionEventArgs e) =>
            MessageBox.Show(e.Exception.Message, AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}