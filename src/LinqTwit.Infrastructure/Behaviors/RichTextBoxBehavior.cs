using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

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

        /// <summary>
        /// PreventTextInput Dependency Property.
        /// </summary>
        public static readonly DependencyProperty PreventTextInputProperty =
            DependencyProperty.RegisterAttached(
                "PreventTextInput",
                typeof (bool),
                typeof (RichTextBoxBehavior),
                new PropertyMetadata(new PropertyChangedCallback(PreventTextInputChanged)));

        private static CommandBinding _binding;

        public static void SetPreventTextInput(RichTextBox obj,
                                bool value)
        {
            obj.SetValue(PreventTextInputProperty, value);
        }

        public static bool GetPreventTextInput(RichTextBox obj)
        {
            return (bool) obj.GetValue(PreventTextInputProperty);
        }

        private static void PreventTextInputChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs e)
        {
            RichTextBox box = d as RichTextBox;
            bool val = (bool) e.NewValue;

            if (box != null)
            {
                box.PreviewTextInput -= RichTextBoxOnPreviewTextInput;

                if (val)
                {
                    box.PreviewTextInput += RichTextBoxOnPreviewTextInput;
                    if (_binding == null)
                    {
                        _binding = new CommandBinding(ApplicationCommands.Paste, (sender, args) => { });
                        _binding.PreviewCanExecute += BindingPreviewCanExecute;

                        CommandManager.RegisterClassCommandBinding(typeof(RichTextBox),
                                       _binding);

                        _binding = new CommandBinding(ApplicationCommands.Cut, (sender, args) => {});
                        _binding.PreviewCanExecute += BindingPreviewCanExecute;

                        CommandManager.RegisterClassCommandBinding(typeof(RichTextBox),
                                       _binding);
                    }
                }
            }
        }

        static void BindingPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool inputDisabled = GetPreventTextInput((RichTextBox)sender);
            if (inputDisabled)
            {
                e.CanExecute = false;
                e.Handled = true;
            }
        }

        private static void PasteCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
          
        }

        private static void RichTextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs args)
        {
            args.Handled = true;
        }
    }
}
