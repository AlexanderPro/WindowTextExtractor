using System;
using System.Runtime.InteropServices;
using mshtml;

namespace WindowTextExtractor.Native
{
    static class Oleacc
    {
        [DllImport("OLEACC.dll")]
        public static extern int ObjectFromLresult(int lResult, ref Guid riid, int wParam, ref IHTMLDocument2 ppvObject);
    }
}
