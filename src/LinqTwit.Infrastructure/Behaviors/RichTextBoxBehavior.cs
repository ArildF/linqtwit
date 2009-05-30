using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LinqTwit.Infrastructure.Behaviors
{
    public class RichTextBoxBehavior
    {
        /// <summary>
        /// BindableDocument Dependency Property.
        /// </summary>
        public static readonly DependencyProperty BindableDocumentProperty =
            DependencyProperty.RegisterAttached(
                "BindableDocument",
                typeof (FlowDocument),
                typeof (RichTextBoxBehavior),
                new PropertyMetadata(new PropertyChangedCallback(BindableDocumentChanged)));

        public static void SetBindableDocument(RichTextBox obj,
                                FlowDocument value)
        {
            obj.SetValue(BindableDocumentProperty, value);
        }

        public static FlowDocument GetBindableDocument(RichTextBox obj)
        {
            return (FlowDocument)obj.GetValue(BindableDocumentProperty);
        }

        private static void BindableDocumentChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs e)
        {
            RichTextBox box = d as RichTextBox;
            FlowDocument doc = e.NewValue as FlowDocument;
            if (box != null && doc != null)
            {
                box.Document = doc;
            }
        }
    }
}
