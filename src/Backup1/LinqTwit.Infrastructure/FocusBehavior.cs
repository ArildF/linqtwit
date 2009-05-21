using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace LinqTwit.Infrastructure
{
    public class FocusBehavior : DependencyObject
    {
        /// <summary>
        /// Command Dependency Property.
        /// </summary>
        public static readonly DependencyProperty BindableFocusProperty =
            DependencyProperty.RegisterAttached(
                "BindableFocus",
                typeof (bool),
                typeof (FocusBehavior),
                new PropertyMetadata(new PropertyChangedCallback(BindableFocusChanged)));

        public static void SetBindableFocus(UIElement obj,
                                bool value)
        {
            HookEvents(obj);
            obj.SetValue(BindableFocusProperty, value);
        }

        public static bool GetBindableFocus(UIElement obj)
        {
            HookEvents(obj);
            return (bool) obj.GetValue(BindableFocusProperty);
        }

        private static void BindableFocusChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs e)
        {
            UIElement c = d as UIElement;
            if (c != null && (bool)e.NewValue)
            {
                if (!c.IsEnabled)
                {
                    c.IsEnabledChanged += FocusElement;
                }
                else
                {
                    Keyboard.Focus(c);
                }
              
                HookEvents(c);
            }
        }

        private static void FocusElement(object sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element != null)
            {
                element.IsEnabledChanged -= FocusElement;
                Keyboard.Focus(element);
            }
            
        }

        private static void HookEvents(UIElement c)
        {
            c.LostKeyboardFocus -= LostKeyboardFocus;
            c.LostKeyboardFocus += LostKeyboardFocus;

            c.GotKeyboardFocus -= GotKeyboardFocus;
            c.GotKeyboardFocus += GotKeyboardFocus;
        }

        static void GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SetBindableFocus(sender as UIElement, true);
        }

        static void LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SetBindableFocus(sender as UIElement, false);
        }
    }
}
