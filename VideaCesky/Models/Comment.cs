using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VideaCesky.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace VideaCesky.Models
{
    public class Comment
    {
        public string Author { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public int KarmaUp { get; set; }

        public int KarmaDown { get; set; }

        public List<Comment> Children { get; set; }

        public bool IsPopular { get; set; }

        public Comment()
        {
            Children = new List<Comment>();
        }

        #region CommentText
        public static readonly DependencyProperty CommentTextProperty = DependencyProperty.RegisterAttached(
            "CommentText",
            typeof(string),
            typeof(Comment),
            new PropertyMetadata("", CommentTextPropertyChanged));

        public static void SetCommentText(DependencyObject d, string value)
        {
            d.SetValue(CommentTextProperty, value);
        }

        public static string GetCommentText(DependencyObject d)
        {
            return (string)d.GetValue(CommentTextProperty);
        }

        private static void CommentTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock)
            {
                TextBlock tb = (TextBlock)d;
                tb.Inlines.Clear();

                if (e.NewValue != null)
                {
                    string[] lines = ((string)e.NewValue).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        tb.Inlines.Add(new Run() { Text = lines[i] });
                        if (i != lines.Length - 1)
                        {
                            tb.Inlines.Add(new LineBreak());
                        }
                    }
                }
            }
        }
        #endregion
    }
}
