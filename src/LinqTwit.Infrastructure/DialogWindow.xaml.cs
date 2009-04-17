using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LinqTwit.Infrastructure
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
        }


        public object DialogContent
        {
            get
            {
                return this.content.Content;
            }
            set
            {
                this.content.Content = value;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }
    }
}
