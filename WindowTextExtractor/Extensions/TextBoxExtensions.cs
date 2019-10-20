using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowTextExtractor.Extensions
{
    public static class TextBoxExtensions
    {
        public static void ScrollTextToEnd(this TextBox textBox)
        {
            var length = textBox.Text.Length;
            if (length > 0)
            {
                textBox.Select(length - 1, 1);
                textBox.ScrollToCaret();
            }
        }
    }
}
