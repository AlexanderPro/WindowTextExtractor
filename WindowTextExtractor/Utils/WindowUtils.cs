using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using System.Linq;
using System.IO;
using mshtml;
using WindowTextExtractor.Native;
using WindowTextExtractor.Extensions;

namespace WindowTextExtractor.Utils
{
    static class WindowUtils
    {
        public static IList<string> GetPasswordsFromHtmlPage(IntPtr hWnd)
        {
            var result = new List<string>();
            var proc = new NativeMethods.EnumProc(EnumWindows);
            NativeMethods.EnumChildWindows(hWnd, proc, ref hWnd);
            if (hWnd != IntPtr.Zero)
            {
                var message = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");
                if (message != 0)
                {
                    var messageResult = 0;
                    NativeMethods.SendMessageTimeout(hWnd, message, 0, 0, NativeConstants.SMTO_ABORTIFHUNG, 1000, out messageResult);
                    if (messageResult != 0)
                    {
                        IHTMLDocument2 document = null;
                        var iidIHtmlDocument = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");
                        NativeMethods.ObjectFromLresult(messageResult, ref iidIHtmlDocument, 0, ref document);
                        if (document != null)
                        {
                            foreach (var element in document.all)
                            {
                                var inputElement = element as IHTMLInputElement;
                                if (inputElement != null && inputElement.type != null && inputElement.type.ToLower() == "password" && !string.IsNullOrEmpty(inputElement.value))
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

        public static WindowInformation GetWindowInformation(IntPtr hWnd)
        {
            var text = GetWindowText(hWnd);
            var wmText = GetWmGettext(hWnd);
            var className = GetClassName(hWnd);
            var realWindowClass = RealGetWindowClass(hWnd);
            var hWndParent = NativeMethods.GetParent(hWnd);
            var size = GetWindowSize(hWnd);
            var threadId = NativeMethods.GetWindowThreadProcessId(hWnd, out var processId);
            var process = GetProcessByIdSafely(processId);

            var gwlStyle = NativeMethods.GetWindowLong(hWnd, NativeConstants.GWL_STYLE);
            var gwlExstyle = NativeMethods.GetWindowLong(hWnd, NativeConstants.GWL_EXSTYLE);
            var gwlUserData = NativeMethods.GetWindowLong(hWnd, NativeConstants.GWL_USERDATA);
            var gclStyle = NativeMethods.GetClassLong(hWnd, NativeConstants.GCL_STYLE);
            var gclWndproc = NativeMethods.GetClassLong(hWnd, NativeConstants.GCL_WNDPROC);
            var dwlDlgproc = NativeMethods.GetClassLong(hWnd, NativeConstants.DWL_DLGPROC);
            var dwlUser = NativeMethods.GetClassLong(hWnd, NativeConstants.DWL_USER);

            var windowDetailes = new Dictionary<string, string>();
            windowDetailes.Add("GetWindowText", text);
            windowDetailes.Add("WM_GETTEXT", wmText);
            windowDetailes.Add("GetClassName", className);
            windowDetailes.Add("RealGetClassName", realWindowClass);
            try
            {
                windowDetailes.Add("Font Name", GetFontName(hWnd));
            }
            catch
            {
            }

            windowDetailes.Add("Window Handle", $"0x{hWnd.ToInt64():X}");
            windowDetailes.Add("Parent Window Handle", $"0x{hWndParent.ToInt64():X}");
            windowDetailes.Add("Window Size", $"{size.Width} x {size.Height}");
            
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
                if (NativeMethods.GetWindowInfo(hWnd, ref windowInfo))
                {
                    windowDetailes.Add("WindowInfo.ExStyle", $"0x{windowInfo.dwExStyle:X}");
                }
            }
            catch
            {
            }
            
            try
            {
                uint key;
                Byte alpha;
                uint flags;
                var result = NativeMethods.GetLayeredWindowAttributes(hWnd, out key, out alpha, out flags);
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
                    var fullPath = NativeMethods.QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ? fileNameBuilder.ToString() : "";
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

            try
            {
                processDetailes.Add("Parent", Path.GetFileName(process.GetParentProcess().MainModule.FileName));
            }
            catch
            {
            }

            try
            {
                processDetailes.Add("Priority", process.GetProcessPriority().ToString());
            }
            catch
            {
            }

            processDetailes.Add("Process Id", processId.ToString());
            processDetailes.Add("Thread Id", threadId.ToString());
            
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
                var fileVersionInfo = process.MainModule.FileVersionInfo;
                processDetailes.Add("Product Name", fileVersionInfo.ProductName);
                processDetailes.Add("Copyright", fileVersionInfo.LegalCopyright);
                processDetailes.Add("File Version", fileVersionInfo.FileVersion);
                processDetailes.Add("Product Version", fileVersionInfo.ProductVersion);
            }
            catch
            {
            }

            return new WindowInformation(windowDetailes, processDetailes);
        }

        public static Bitmap PrintWindow(IntPtr hWnd)
        {
            Rect rect;
            NativeMethods.GetWindowRect(hWnd, out rect);
            var bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var hdc = graphics.GetHdc();
                NativeMethods.PrintWindow(hWnd, hdc, 0);
                graphics.ReleaseHdc(hdc);
            }
            return bitmap;
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            var builder = new StringBuilder(1024);
            NativeMethods.GetWindowText(hWnd, builder, builder.Capacity);
            var windowText = builder.ToString();
            return windowText;
        }

        private static string GetWmGettext(IntPtr hWnd)
        {
            var titleSize = NativeMethods.SendMessage(hWnd, NativeConstants.WM_GETTEXTLENGTH, 0, 0);
            if (titleSize.ToInt32() == 0)
            {
                return string.Empty;
            }

            var title = new StringBuilder(titleSize.ToInt32() + 1);
            NativeMethods.SendMessage(hWnd, NativeConstants.WM_GETTEXT, title.Capacity, title);
            return title.ToString();
        }

        private static string GetClassName(IntPtr hWnd)
        {
            var builder = new StringBuilder(1024);
            NativeMethods.GetClassName(hWnd, builder, builder.Capacity);
            var className = builder.ToString();
            return className;
        }

        private static string RealGetWindowClass(IntPtr hWnd)
        {
            var builder = new StringBuilder(1024);
            NativeMethods.RealGetWindowClass(hWnd, builder, builder.Capacity);
            var className = builder.ToString();
            return className;
        }

        private static string GetFontName(IntPtr hWnd)
        {
            var hFont = NativeMethods.SendMessage(hWnd, NativeConstants.WM_GETFONT, 0, 0);
            if (hFont == IntPtr.Zero)
            {
                return "Default system font";
            }
            var font = Font.FromHfont(hFont);
            return font.Name;
        }

        private static Rect GetWindowSize(IntPtr hWnd)
        {
            Rect size;
            NativeMethods.GetWindowRect(hWnd, out size);
            return size;
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
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE ProcessId = " + pId))
            using (var objects = searcher.Get())
            {
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
        }

        private static int EnumWindows(IntPtr hWnd, ref IntPtr lParam)
        {
            var result = 1;
            var className = new StringBuilder(1024);
            NativeMethods.GetClassName(hWnd, className, className.Capacity);
            if (string.Compare(className.ToString(), "Internet Explorer_Server") == 0)
            {
                lParam = hWnd;
                result = 0;
            }
            return result;
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