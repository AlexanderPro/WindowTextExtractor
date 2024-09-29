using System.Drawing;

namespace WindowTextExtractor.Settings
{
    public class FontSettings
    {
        private const float DefaultFontSize = 10;
        private const FontStyle DefaultFontStyle = FontStyle.Regular;
        private const GraphicsUnit DefaultFontUnit = GraphicsUnit.Point;
        private const string DefaultFontName = "Courier New";

        public string Name { get; set; }

        public float Size { get; set; }

        public FontStyle Style { get; set; }

        public GraphicsUnit Unit { get; set; }

        public FontSettings()
        {
            Name = DefaultFontName;
            Size = DefaultFontSize;
            Style = DefaultFontStyle;
            Unit = DefaultFontUnit;
        }
    }
}
