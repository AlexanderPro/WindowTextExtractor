namespace WindowTextExtractor.Native.Enums
{
    /// <summary>
    /// DPI type
    /// </summary>
    /// <remarks>
    /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/ne-shellscalingapi-monitor_dpi_type"/>
    /// </remarks>
    public enum DpiType
    {
        /// <summary>
        /// The effective DPI. This value should be used when determining the correct scale factor for scaling UI elements.
        /// </summary>
        Effective = 0,

        /// <summary>
        /// The angular DPI. This DPI ensures rendering at a compliant angular resolution on the screen.
        /// </summary>
        Angular = 1,

        /// <summary>
        /// The raw DPI. This value is the linear DPI of the screen as measured on the screen itself. Use this value when you want to read the pixel density and not the recommended scaling setting.
        /// </summary>
        Raw = 2,
    }
}
