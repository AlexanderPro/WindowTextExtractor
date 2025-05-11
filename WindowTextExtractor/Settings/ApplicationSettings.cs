namespace WindowTextExtractor.Settings
{
    public class ApplicationSettings
    {
        private const string DefaultVideoFileName = "Window.avi";
        private const int DefaultFps = 12;
        private const decimal DefaultScale = 1;
        private const int DefaultBorderWidth = 3;
        private const string DefaultBorderColor = "Blue";
        private const bool DefaultHighDpiSupport = true;

        public const int ImageSize = 48;
        public const int IconSize = 32;

        public string VideoFileName { get; set; }

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

        public bool HighDpiSupport { get; set; }

        public TargetIconType TargetIcon { get; set; }

        public FontSettings Font { get; set; }

        public MagnifierSettings Magnifier { get; set; }

        public static ApplicationSettings CreateDefault() => new ApplicationSettings
        {
            VideoFileName = DefaultVideoFileName,
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
            HighDpiSupport = DefaultHighDpiSupport,
            TargetIcon = TargetIconType.Default,
            Font = new FontSettings(),
            Magnifier = new MagnifierSettings()
        };
    }
}
