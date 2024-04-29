using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using System.Windows.Forms;
using mshtml;
using WindowTextExtractor.Extensions;
using WindowTextExtractor.Native;
using WindowTextExtractor.Native.Enums;
using WindowTextExtractor.Native.Structs;

namespace WindowTextExtractor.Utils
{
    static class WindowUtils
    {
        public static void UpdateWindow(IntPtr handle) =>
            User32.RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Frame | RedrawWindowFlags.AllChildren | RedrawWindowFlags.UpdateNow | RedrawWindowFlags.Erase | RedrawWindowFlags.Invalidate);

        public static IList<string> GetPasswordsFromHtmlPage(IntPtr handle)
        {
            var result = new List<string>();
            var proc = new User32.EnumProc(EnumWindows);
            User32.EnumChildWindows(handle, proc, ref handle);
            if (handle != IntPtr.Zero)
            {
                var message = User32.RegisterWindowMessage("WM_HTML_GETOBJECT");
                if (message != 0)
                {
                    User32.SendMessageTimeout(handle, message, 0, 0, Constants.SMTO_ABORTIFHUNG, 1000, out var messageResult);
                    if (messageResult != 0)
                    {
                        IHTMLDocument2 document = null;
                        var iidIHtmlDocument = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");
                        Oleacc.ObjectFromLresult(messageResult, ref iidIHtmlDocument, 0, ref document);
                        if (document != null)
                        {
                            foreach (var element in document.all)
                            {
                                if (element is IHTMLInputElement inputElement && inputElement.type != null && inputElement.type.ToLower() == "password" && !string.IsNullOrEmpty(inputElement.value))
                                {
                                    result.Add(inputElement.value);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static WindowInformation GetWindowInformation(IntPtr handle, Point cursorPosition)
        {
            var text = GetWindowText(handle);
            var wmText = GetWmGettext(handle);
            var className = GetClassName(handle);
            var realWindowClass = RealGetWindowClass(handle);
            var handleParent = User32.GetParent(handle);
            var size = GetWindowSize(handle);
            var clientSize = GetWindowClientSize(handle);
            var isVisible = User32.IsWindowVisible(handle);
            var placement = GetWindowPlacement(handle);
            var threadId = User32.GetWindowThreadProcessId(handle, out var processId);
            using var process = GetProcessByIdSafely(processId);

            var gwlStyle = User32.GetWindowLong(handle, Constants.GWL_STYLE);
            var gwlExstyle = User32.GetWindowLong(handle, Constants.GWL_EXSTYLE);
            var gwlUserData = User32.GetWindowLong(handle, Constants.GWL_USERDATA);
            var gclStyle = User32.GetClassLong(handle, Constants.GCL_STYLE);
            var gclWndproc = User32.GetClassLong(handle, Constants.GCL_WNDPROC);
            var dwlDlgproc = User32.GetClassLong(handle, Constants.DWL_DLGPROC);
            var dwlUser = User32.GetClassLong(handle, Constants.DWL_USER);

            var cursorDetailes = new Dictionary<string, string>();
            cursorDetailes.Add("Position", $"X = {cursorPosition.X}, Y = {cursorPosition.Y}");

            var screenFromPoint = Screen.FromPoint(cursorPosition);
            var screens = Screen.AllScreens.Select((x, i) => new { Index = i + 1, Item = x }).ToList();
            var screen = screens.FirstOrDefault(x => x.Item.Equals(screenFromPoint));
            if (screen != null)
            {
                cursorDetailes.Add("Monitor Position", $"X = {cursorPosition.X - screen.Item.Bounds.Left}, Y = {cursorPosition.Y - screen.Item.Bounds.Top}");
                cursorDetailes.Add("Monitor", screen.Index.ToString());
            }

            var color = GetColorUnderCursor(cursorPosition);
            cursorDetailes.Add("Color Picker", ColorTranslator.ToHtml(color));

            var windowDetailes = new Dictionary<string, string>();
            windowDetailes.Add("GetWindowText", text);
            windowDetailes.Add("WM_GETTEXT", wmText);
            windowDetailes.Add("GetClassName", className);
            windowDetailes.Add("RealGetClassName", realWindowClass);
            try
            {
                windowDetailes.Add("Font Name", GetFontName(handle));
            }
            catch
            {
            }

            windowDetailes.Add("Window Handle", $"0x{handle.ToInt64():X}");
            windowDetailes.Add("Parent Window Handle", handleParent == IntPtr.Zero ? "-" : $"0x{handleParent.ToInt64():X}");
            windowDetailes.Add("Is Window Visible", isVisible.ToString());
            windowDetailes.Add("Window Placement (showCmd)", placement.showCmd.ToString());
            windowDetailes.Add("Window Size", $"{size.Width}x{size.Height}");
            windowDetailes.Add("Window Client Size", $"{clientSize.Width}x{clientSize.Height}");

            try
            {
                var bounds = GetFrameBounds(handle);
                windowDetailes.Add("Window Extended Frame Bounds", $"{bounds.Top} {bounds.Right} {bounds.Bottom} {bounds.Left}");
            }
            catch
            {
            }

            try
            {
                windowDetailes.Add("Instance", $"0x{process.Modules[0].BaseAddress.ToInt64():X}");
            }
            catch
            {
            }
            
            windowDetailes.Add("GCL_WNDPROC", $"0x{gclWndproc:X}");
            windowDetailes.Add("DWL_DLGPROC", $"0x{dwlDlgproc:X}");
            windowDetailes.Add("GWL_STYLE", $"0x{gwlStyle:X}");
            windowDetailes.Add("GCL_STYLE", $"0x{gclStyle:X}");
            windowDetailes.Add("GWL_EXSTYLE", $"0x{gwlExstyle:X}");
            
            try
            {
                var windowInfo = new WINDOW_INFO();
                windowInfo.cbSize = Marshal.SizeOf(windowInfo);
                if (User32.GetWindowInfo(handle, ref windowInfo))
                {
                    windowDetailes.Add("WindowInfo.ExStyle", $"0x{windowInfo.dwExStyle:X}");
                }
            }
            catch
            {
            }
            
            try
            {
                var result = User32.GetLayeredWindowAttributes(handle, out var key, out var alpha, out var flags);
                var layeredWindow = (LayeredWindow)flags;
                windowDetailes.Add("LWA_ALPHA", layeredWindow.HasFlag(LayeredWindow.LWA_ALPHA) ? "+" : "-");
                windowDetailes.Add("LWA_COLORKEY", layeredWindow.HasFlag(LayeredWindow.LWA_COLORKEY) ? "+" : "-");
            }
            catch
            {
            }
            
            windowDetailes.Add("GWL_USERDATA", $"0x{gwlUserData:X}");
            windowDetailes.Add("DWL_USER", $"0x{dwlUser:X}");

            var processDetailes = new Dictionary<string, string>();
            try
            {
                try
                {
                    processDetailes.Add("Full Path", process.MainModule.FileName);
                }
                catch
                {
                    var fileNameBuilder = new StringBuilder(1024);
                    var bufferLength = (uint)fileNameBuilder.Capacity + 1;
                    var fullPath = Kernel32.QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ? fileNameBuilder.ToString() : "";
                    processDetailes.Add("Full Path", fullPath);
                }
            }
            catch
            {
            }

            var processInfo = (WmiProcessInfo)null;
            try
            {
                processInfo = GetWmiProcessInfo(processId);
                processDetailes.Add("Command Line", processInfo.CommandLine);
            }
            catch
            {
            }

            try
            {
                processDetailes.Add("Started at", $"{process.StartTime:dd.MM.yyyy HH:mm:ss}");
            }
            catch
            {
            }

            try
            {
                processDetailes.Add("Owner", processInfo.Owner);
            }
            catch
            {
            }

            processDetailes.Add("Process Id", processId.ToString());
            try
            {
                using var parentProcess = process.GetParentProcess();
                using var mainModule = parentProcess.MainModule;
                processDetailes.Add("Parent Process Id", parentProcess.Id.ToString());
                processDetailes.Add("Parent", Path.GetFileName(mainModule.FileName));
            }
            catch
            {
            }
            
            processDetailes.Add("Thread Id", threadId.ToString());

            try
            {
                processDetailes.Add("Priority", process.GetProcessPriority().ToString());
            }
            catch
            {
            }

            
            try
            {
                processDetailes.Add("Threads", processInfo.ThreadCount.ToString());
                processDetailes.Add("Handles", processInfo.HandleCount.ToString());
                processDetailes.Add("Working Set Size", processInfo.WorkingSetSize.ToString());
                processDetailes.Add("Virtual Size", processInfo.VirtualSize.ToString());
            }
            catch
            {
            }
            
            try
            {
                using var mainModule = process.MainModule;
                var fileVersionInfo = mainModule.FileVersionInfo;
                processDetailes.Add("Product Name", fileVersionInfo.ProductName);
                processDetailes.Add("Copyright", fileVersionInfo.LegalCopyright);
                processDetailes.Add("File Version", fileVersionInfo.FileVersion);
                processDetailes.Add("Product Version", fileVersionInfo.ProductVersion);
            }
            catch
            {
            }
            
            return new WindowInformation(cursorDetailes, windowDetailes, processDetailes);
        }

        public static Bitmap CaptureWindow(IntPtr handle, bool captureCursor = false)
        {
            User32.GetWindowRect(handle, out var rectangle);
            var posX = rectangle.Left;
            var posY = rectangle.Top;
            var width = rectangle.Width;
            var height = rectangle.Height;

            var hDesk = User32.GetDesktopWindow();
            var hSrce = User32.GetWindowDC(hDesk);
            var hDest = Gdi32.CreateCompatibleDC(hSrce);
            var hBmp = Gdi32.CreateCompatibleBitmap(hSrce, width, height);
            var hOldBmp = Gdi32.SelectObject(hDest, hBmp);

            var b = Gdi32.BitBlt(hDest, 0, 0, width, height, hSrce, posX, posY, CopyPixelOperations.SourceCopy | CopyPixelOperations.CaptureBlt);

            try
            {
                if (b)
                {
                    var image = Image.FromHbitmap(hBmp);
                    if (captureCursor)
                    {
                        CURSORINFO cursorInfo;
                        cursorInfo.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
                        if (User32.GetCursorInfo(out cursorInfo) && cursorInfo.flags == Constants.CURSOR_SHOWING && User32.GetIconInfo(cursorInfo.hCursor, out var iconInfo))
                        {
                            using var graphics = Graphics.FromImage(image);
                            var x = cursorInfo.ptScreenPos.x - rectangle.Left - iconInfo.xHotspot;
                            var y = cursorInfo.ptScreenPos.y - rectangle.Top - iconInfo.yHotspot;
                            var hdc = graphics.GetHdc();
                            User32.DrawIconEx(hdc, x, y, cursorInfo.hCursor, 0, 0, 0, IntPtr.Zero, Constants.DI_NORMAL | Constants.DI_COMPAT);
                            User32.DestroyIcon(cursorInfo.hCursor);
                            Gdi32.DeleteObject(iconInfo.hbmMask);
                            Gdi32.DeleteObject(iconInfo.hbmColor);
                            graphics.ReleaseHdc();
                            Gdi32.DeleteDC(hdc);
                        }
                    }
                    return image;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                Gdi32.SelectObject(hDest, hOldBmp);
                Gdi32.DeleteObject(hBmp);
                Gdi32.DeleteDC(hDest);
                User32.ReleaseDC(hDesk, hSrce);
            }
        }

        public static void SetTransparency(IntPtr handle, int percent)
        {
            var opacity = (byte)Math.Round(255 * (100 - percent) / 100f, MidpointRounding.AwayFromZero);
            SetOpacity(handle, opacity);
        }


        public static void SetOpacity(IntPtr handle, byte opacity)
        {
            var exStyle = User32.GetWindowLong(handle, Constants.GWL_EXSTYLE);
            User32.SetWindowLong(handle, Constants.GWL_EXSTYLE, exStyle | Constants.WS_EX_LAYERED);
            User32.SetLayeredWindowAttributes(handle, 0, opacity, Constants.LWA_ALPHA);
        }

        private static string GetWindowText(IntPtr handle)
        {
            var builder = new StringBuilder(1024);
            User32.GetWindowText(handle, builder, builder.Capacity);
            var windowText = builder.ToString();
            return windowText;
        }

        private static string GetWmGettext(IntPtr handle)
        {
            var titleSize = User32.SendMessage(handle, Constants.WM_GETTEXTLENGTH, 0, 0);
            if (titleSize.ToInt32() == 0)
            {
                return string.Empty;
            }

            var title = new StringBuilder(titleSize.ToInt32() + 1);
            User32.SendMessage(handle, Constants.WM_GETTEXT, title.Capacity, title);
            return title.ToString();
        }

        private static string GetClassName(IntPtr handle)
        {
            var builder = new StringBuilder(1024);
            User32.GetClassName(handle, builder, builder.Capacity);
            var className = builder.ToString();
            return className;
        }

        private static string RealGetWindowClass(IntPtr handle)
        {
            var builder = new StringBuilder(1024);
            User32.RealGetWindowClass(handle, builder, builder.Capacity);
            var className = builder.ToString();
            return className;
        }

        private static string GetFontName(IntPtr handle)
        {
            var hFont = User32.SendMessage(handle, Constants.WM_GETFONT, 0, 0);
            if (hFont == IntPtr.Zero)
            {
                return "Default system font";
            }
            var font = Font.FromHfont(hFont);
            return font.Name;
        }

        private static Rect GetWindowSize(IntPtr handle)
        {
            User32.GetWindowRect(handle, out var size);
            return size;
        }

        private static Rect GetWindowClientSize(IntPtr handle)
        {
            User32.GetClientRect(handle, out var size);
            return size;
        }

        private static WINDOWPLACEMENT GetWindowPlacement(IntPtr handle)
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            User32.GetWindowPlacement(handle, ref placement);
            return placement;
        }

        private static Rect GetSizeWithFrameBounds(IntPtr handle)
        {
            Rect size;
            if (Environment.OSVersion.Version.Major < 6)
            {
                User32.GetWindowRect(handle, out size);
            }
            else if (Dwmapi.DwmGetWindowAttribute(handle, Constants.DWMWA_EXTENDED_FRAME_BOUNDS, out size, Marshal.SizeOf(typeof(Rect))) != 0)
            {
                User32.GetWindowRect(handle, out size);
            }
            return size;
        }

        private static Rect GetFrameBounds(IntPtr handle)
        {
            var withMargin = GetSizeWithFrameBounds(handle);
            var size = GetWindowSize(handle);
            return new Rect
            {
                Left = withMargin.Left - size.Left,
                Top = withMargin.Top - size.Top,
                Right = size.Right - withMargin.Right,
                Bottom = size.Bottom - withMargin.Bottom
            };
        }

        private static Process GetProcessByIdSafely(int pId)
        {
            try
            {
                return Process.GetProcessById(pId);
            }
            catch
            {
                return null;
            }
        }

        private static WmiProcessInfo GetWmiProcessInfo(int pId)
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE ProcessId = " + pId);
            using var objects = searcher.Get();
            var processInfo = new WmiProcessInfo();
            foreach (ManagementObject obj in objects)
            {
                var argList = new string[] { string.Empty, string.Empty };
                var returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    processInfo.Owner = argList[1] + "\\" + argList[0];
                    break;
                }
            }

            var baseObject = objects.Cast<ManagementBaseObject>().FirstOrDefault();
            if (baseObject != null)
            {
                processInfo.CommandLine = baseObject["CommandLine"] != null ? baseObject["CommandLine"].ToString() : "";
                processInfo.HandleCount = baseObject["HandleCount"] != null ? (uint)baseObject["HandleCount"] : 0;
                processInfo.ThreadCount = baseObject["ThreadCount"] != null ? (uint)baseObject["ThreadCount"] : 0;
                processInfo.VirtualSize = baseObject["VirtualSize"] != null ? (ulong)baseObject["VirtualSize"] : 0;
                processInfo.WorkingSetSize = baseObject["WorkingSetSize"] != null ? (ulong)baseObject["WorkingSetSize"] : 0;
            }

            return processInfo;
        }

        private static int EnumWindows(IntPtr handle, ref IntPtr lParam)
        {
            var result = 1;
            var className = new StringBuilder(1024);
            User32.GetClassName(handle, className, className.Capacity);
            if (string.Compare(className.ToString(), "Internet Explorer_Server") == 0)
            {
                lParam = handle;
                result = 0;
            }
            return result;
        }

        private static Color GetColorUnderCursor(Point cursorPosition)
        {
            using var bmp = new Bitmap(1, 1);
            using var graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(cursorPosition, Point.Empty, new Size(1, 1));
            var color = bmp.GetPixel(0, 0);
            return color;
        }

        private class WmiProcessInfo
        {
            public string CommandLine { get; set; }
 
            public uint HandleCount { get; set; }
            
            public uint ThreadCount { get; set; }
            
            public ulong VirtualSize { get; set; }
            
            public ulong WorkingSetSize { get; set; }
            
            public string Owner { get; set; }
        }
    }
}