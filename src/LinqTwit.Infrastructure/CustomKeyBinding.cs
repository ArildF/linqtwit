using System;
using System.Windows;
using System.Windows.Input;

namespace LinqTwit.Infrastructure
{
    public class CustomKeyBinding : FrameworkElement
    {

        static CustomKeyBinding()
        {
            //register dependency property
            FrameworkPropertyMetadata mdText = new FrameworkPropertyMetadata("", TextPropertyChanged);
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomKeyBinding), mdText);

            FrameworkPropertyMetadata mdKey = new FrameworkPropertyMetadata(Key.None, KeyPropertyChanged);
            KeyProperty = DependencyProperty.Register("Key", typeof(Key), typeof(CustomKeyBinding), mdKey);

            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(null, CommandPropertyChanged);
            CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CustomKeyBinding), metadata);           
        }

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty TextProperty;


        /// <summary>
        /// A property wrapper for the <see cref="TextProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty CommandProperty;

        public static readonly DependencyProperty KeyProperty;


        /// <summary>
        /// A property wrapper for the <see cref="CommandProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        /// <summary>
        /// Handles changes on the <see cref="CommandProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="Command"/> property wrapper, updates should be handled here.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void CommandPropertyChanged(DependencyObject d,
                                             DependencyPropertyChangedEventArgs
                                                 e)
        {
            CustomKeyBinding owner = (CustomKeyBinding) d;
            ICommand newValue = (ICommand) e.NewValue;
        }

        private static void TextPropertyChanged(DependencyObject d,
                                             DependencyPropertyChangedEventArgs
                                                 e)
        {
            CustomKeyBinding owner = (CustomKeyBinding) d;
            string newValue = (string) e.NewValue;
        }

        private static void KeyPropertyChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs
                                         e)
        {
            CustomKeyBinding owner = (CustomKeyBinding)d;
            Key newValue = (Key)e.NewValue;
        }



        public bool Handle(string text)
        {
            return text.Equals(this.Text) && this.DoHandle();
        }

        public bool Handle(Key key)
        {
            return key == this.Key && DoHandle();
        }

        private bool DoHandle()
        {
            if (this.Command == null || !this.Command.CanExecute(null))
            {
                return false;
            }

            this.Command.Execute(null);
            return true;
        }
    }
}