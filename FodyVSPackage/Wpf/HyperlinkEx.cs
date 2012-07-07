using System.Diagnostics;
using System.Windows.Documents;

namespace Wpf
{
    public class HyperlinkEx : Hyperlink
    {
        public HyperlinkEx()
        {
            RequestNavigate += (sender, e) => Process.Start(e.Uri.ToString());
        }
    }
}