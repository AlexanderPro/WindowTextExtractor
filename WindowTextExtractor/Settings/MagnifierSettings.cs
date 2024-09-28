namespace WindowTextExtractor.Settings
{
    public class MagnifierSettings
    {
        private const bool DefaultEnabled = true;
        private const decimal DefaultFactor = 3;

        public bool Enabled { get; set; }

        public decimal Factor { get; set; }

        public MagnifierSettings()
        {
            Enabled = DefaultEnabled;
            Factor = DefaultFactor;
        }
    }
}
