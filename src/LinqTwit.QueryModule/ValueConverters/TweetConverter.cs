using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;

namespace LinqTwit.QueryModule.ValueConverters
{
    public class TweetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            FlowDocument doc = new FlowDocument();
            doc.FontSize = 12;
            var paragraph = new Paragraph(new Run(text));
            doc.Blocks.Add(paragraph);

            return doc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
