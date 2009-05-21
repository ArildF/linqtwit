using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace LinqTwit.Infrastructure
{
    public class InputBindingBehavior
    {
        public static readonly DependencyProperty CommandProperty =
           DependencyProperty.RegisterAttached(
               "Command",
               typeof(ICommand),
               typeof(InputBindingBehavior),
               new PropertyMetadata(new PropertyChangedCallback(CommandChanged)));

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InputBinding binding = d as InputBinding;
            if (binding != null)
            {
                binding.Command = e.NewValue as ICommand;
            }
        }

        public static void SetCommand(InputBinding obj,
                                                  bool value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static bool GetCommand(InputBinding obj)
        {
            return (bool)obj.GetValue(CommandProperty);
        }

    }
}
