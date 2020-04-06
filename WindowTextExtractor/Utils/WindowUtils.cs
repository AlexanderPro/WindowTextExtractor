using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

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
    }
}