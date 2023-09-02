using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowTextExtractor
{
    class WindowInformation
    {
        public IDictionary<string, string> CursorDetails { get; }

        public IDictionary<string, string> WindowDetails { get; }

        public IDictionary<string, string> ProcessDetails { get; }

        public WindowInformation() : this(new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, string>())
        {
        }

        public WindowInformation(IDictionary<string, string> cursorDetails, IDictionary<string, string> windowDetails, IDictionary<string, string> processDetails)
        {
            CursorDetails = cursorDetails;
            WindowDetails = windowDetails;
            ProcessDetails = processDetails;
        }

        public override string ToString()
        {
            const int paddingSize = 25;
            var builder = new StringBuilder(1024);

            if (CursorDetails.Keys.Any())
            {
                builder.AppendFormat($"Cursor Information {Environment.NewLine}");
            }

            foreach (var cursorDetailKey in CursorDetails.Keys)
            {
                builder.AppendFormat($"{cursorDetailKey.PadRight(paddingSize)}: {CursorDetails[cursorDetailKey]}{Environment.NewLine}");
            }

            if (WindowDetails.Keys.Any())
            {
                builder.AppendFormat($"Window Information {Environment.NewLine}");
            }
            
            foreach (var windowDetailKey in WindowDetails.Keys)
            {
                builder.AppendFormat($"{windowDetailKey.PadRight(paddingSize)}: {WindowDetails[windowDetailKey]}{Environment.NewLine}");
            }

            if (WindowDetails.Keys.Any())
            {
                builder.AppendFormat($"{Environment.NewLine}");
            }

            if (ProcessDetails.Keys.Any())
            {
                builder.AppendFormat($"Process Information {Environment.NewLine}");
            }

            foreach (var processDetailKey in ProcessDetails.Keys)
            {
                builder.AppendFormat($"{processDetailKey.PadRight(paddingSize)}: {ProcessDetails[processDetailKey]}{Environment.NewLine}");
            }

            return builder.ToString();
        }
    }
}
