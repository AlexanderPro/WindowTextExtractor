using System.Drawing;

namespace WindowTextExtractor.Settings
{
    public class ApplicationSettings
    {
        private const string DefaultVideoFileName = "Window.avi";
        private const float DefaultFontSize = 10;
        private const FontStyle DefaultFontStyle = FontStyle.Regular;
        private const GraphicsUnit DefaultFontUnit = GraphicsUnit.Point;
        private const int DefaultFps = 12;
        private const decimal DefaultScale = 1;
        private const int DefaultBorderWidth = 5;
        private const string DefaultBorderColor = "Blue";

        public const int ImageSize = 48;
        public const int IconSize = 32;

        public const string DefaultFontName = "Courier New";

        public string VideoFileName { get; set; }

        public string FontName { get; set; }

        public float FontSize { get; set; }

        public FontStyle FontStyle { get; set; }

        public GraphicsUnit FontUnit { get; set; }

        public int FPS { get; set; }

        public decimal Scale { get; set; }

        public int BorderWidth { get; set; }

        public string BorderColor { get; set; }

        public bool ShowTextList { get; set; }

        public bool ShowEmptyItems { get; set; }

        public bool NotRepeatedNewItems { get; set; }

        public bool AlwaysOnTop { get; set; }

        public bool AlwaysRefreshTabs { get; set; }

        public bool RefreshImage { get; set; }

        public bool CaptureCursor { get; set; }

        public TargetIconType TargetIcon { get; set; }

        public MagnifierSettings Magnifier { get; set; }

        public static ApplicationSettings CreateDefault() => new ApplicationSettings
        {
            VideoFileName = DefaultVideoFileName,
            FontName = DefaultFontName,
            FontSize = DefaultFontSize,
            FontStyle = DefaultFontStyle,
            FontUnit = DefaultFontUnit,
            FPS = DefaultFps,
            Scale = DefaultScale,
            BorderWidth = DefaultBorderWidth,
            BorderColor = DefaultBorderColor,
            ShowTextList = true,
            ShowEmptyItems = false,
            NotRepeatedNewItems = true,
            AlwaysOnTop = true,
            AlwaysRefreshTabs = true,
            RefreshImage = true,
            CaptureCursor = true,
            TargetIcon = TargetIconType.Default,
            Magnifier = new MagnifierSettings()
        };
    }
}
