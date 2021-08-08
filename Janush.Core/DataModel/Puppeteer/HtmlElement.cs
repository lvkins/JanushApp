
using PuppeteerSharp;

namespace Janush.Core
{
    public class HtmlElement
    {
        public bool HasChildNodes { get; set; }
        public string TagName { get; set; }
        public string TextContent { get; set; }
        public ElementAttributes[] Attributes { get; set; }
    }
}
