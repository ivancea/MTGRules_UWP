using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace MTGRules {
    public static class TextBlockExtension {
        public static string GetFormattedText(DependencyObject obj) {
            return (string)obj.GetValue(FormattedTextProperty);
        }

        public static void SetFormattedText(DependencyObject obj, string value) {
            obj.SetValue(FormattedTextProperty, value);
        }

        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.Register("FormattedText", typeof(string), typeof(TextBlockExtension),
            new PropertyMetadata(string.Empty, (sender, e) =>
            {
                string text = e.NewValue as string;
                var textBlock = sender as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Inlines.Clear();
                    Regex regx = new Regex(@"(\d{3}(?:\.\d+[a-z]?)?)", RegexOptions.IgnoreCase);
                    var str = regx.Split(text);
                    for (int i = 0; i < str.Length; i++)
                        if (i % 2 == 0)
                            textBlock.Inlines.Add(new Run { Text = str[i] });
                        else
                        {
                            Hyperlink link = new Hyperlink();
                            link.Click += Rule_Click;

                            link.Inlines.Add(new Run { Text = str[i] });
                            textBlock.Inlines.Add(link);

                        }
                }
            }));

        private static void Rule_Click(Hyperlink sender, HyperlinkClickEventArgs args) {
            if (MainPage.ActualInstance != null)
            {
                MainPage.ActualInstance.OnHyperlinkRuleClick(((Run)sender.Inlines.First()).Text);
            }
        }
    }
}
