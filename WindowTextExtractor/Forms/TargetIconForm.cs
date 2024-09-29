using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using WindowTextExtractor.Utils;
using WindowTextExtractor.Settings;

namespace WindowTextExtractor.Forms
{
    partial class TargetIconForm : Form
    {
        private readonly ApplicationSettings _settings;

        public string FileName => txtFileName.Text;

        public TargetIconForm(ApplicationSettings settings)
        {
            InitializeComponent();
            _settings = settings;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadTargetIcon();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Cursor = new Cursor(Cursors.Default.Handle);
            base.OnFormClosed(e);
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            Cursor = new Cursor(Cursors.Default.Handle);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            Cursor = new Cursor(Cursors.Default.Handle);
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonBrowseClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = "png",
                FileName = "*.png",
                Filter = "Icon Image (*.ico)|*.ico|Bitmap Image (*.bmp)|*.bmp|Gif Image (*.gif)|*.gif|JPEG Image (*.jpeg)|*.jpeg|Png Image (*.png)|*.png|Tiff Image (*.tiff)|*.tiff|Wmf Image (*.wmf)|*.wmf|All Files (*.*)|*.*"
            };

            if (File.Exists(FileName))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(FileName);
            }

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                txtFileName.Text = dialog.FileName;
                SetImageSafely(dialog.FileName);
            }
        }

        private void TextBoxFileNameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if (File.Exists(FileName))
                {
                    SetImageSafely(FileName);
                }
                else
                {
                    MessageBox.Show($"File {FileName} does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                ButtonOkClick(sender, e);
            }

            if (e.KeyValue == 27)
            {
                ButtonCancelClick(sender, e);
            }
        }

        private void LoadTargetIcon()
        {
            if (_settings.TargetIcon == TargetIconType.Default)
            {
                pbImage.Image?.Dispose();
                pbImage.Image = Properties.Resources.TargetButton;
                txtFileName.Cursor = new Cursor(Properties.Resources.Target32.Handle);
                Cursor = new Cursor(Properties.Resources.Target32.Handle);
            }
            else if (_settings.TargetIcon == TargetIconType.System)
            {
                pbImage.Image?.Dispose();
                pbImage.Image = ImageUtils.CursorToImage(Cursors.Default);
                txtFileName.Cursor = new Cursor(Cursors.Default.Handle);
                Cursor = new Cursor(Cursors.Default.Handle);
            }
            else
            {
                var imageFileInfo = ApplicationSettingsFile.GetImageFileName();
                SetImageSafely(imageFileInfo.FullName);
            }
        }

        private void SetImage(string fileName)
        {
            using var image = new Bitmap(fileName);
            var scaledImage = new Bitmap((image.Width > ApplicationSettings.ImageSize || image.Height > ApplicationSettings.ImageSize) ? ImageUtils.ResizeImage(image, ApplicationSettings.ImageSize, ApplicationSettings.ImageSize) : image);
            var scaledIcon = new Bitmap((image.Width > ApplicationSettings.IconSize || image.Height > ApplicationSettings.IconSize) ? ImageUtils.ResizeImage(image, ApplicationSettings.IconSize, ApplicationSettings.IconSize) : image);
            pbImage.Image?.Dispose();
            pbImage.Image = scaledImage;
            txtFileName.Cursor = new Cursor(scaledIcon.GetHicon());
            Cursor = new Cursor(scaledIcon.GetHicon());
        }

        private void SetImageSafely(string fileName)
        {
            try
            {
                SetImage(fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to load the file {fileName}.{Environment.NewLine}{e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
