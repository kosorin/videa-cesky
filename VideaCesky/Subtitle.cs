using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace VideaCesky
{
    public class Subtitle
    {
        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public string Text { get; set; }

        #region FormattedText
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.RegisterAttached(
            "FormattedText",
            typeof(string),
            typeof(Subtitle),
            new PropertyMetadata("", FormattedTextPropertyChanged));

        public static void SetFormattedText(DependencyObject d, string value)
        {
            d.SetValue(FormattedTextProperty, value);
        }

        public static string GetFormattedText(DependencyObject d)
        {
            return (string)d.GetValue(FormattedTextProperty);
        }

        private static void FormattedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock)
            {
                TextBlock tb = (TextBlock)d;
                tb.Inlines.Clear();

                if (e.NewValue != null)
                {
                    XDocument doc = XDocument.Parse("<root>" + (string)e.NewValue + "</root>");
                    foreach (XNode node in doc.Root.Nodes())
                    {
                        InlineNode(tb.Inlines, node);
                    }
                }
            }
        }

        private static void InlineNode(InlineCollection ic, XNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                XElement el = (XElement)node;
                Span inline;
                if (el.Name == "i")
                {
                    inline = new Italic();
                }
                else if (el.Name == "b")
                {
                    inline = new Bold();
                }
                else if (el.Name == "u")
                {
                    inline = new Underline();
                }
                else
                {
                    inline = new Span();
                }

                foreach (XNode childNode in el.Nodes())
                {
                    InlineNode(inline.Inlines, childNode);
                }
                ic.Add(inline);
            }
            else if (node.NodeType == XmlNodeType.Text)
            {
                ic.Add(new Run() { Text = node.ToString() });
            }
        }
        #endregion
    }
}
