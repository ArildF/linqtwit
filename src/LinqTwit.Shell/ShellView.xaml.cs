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

namespace LinqTwit.Shell
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IShellView
    {
        public ShellView()
        {
            InitializeComponent();

            CommandManager.AddPreviewCanExecuteHandler(this, Handler);
        }

        private void Handler(object sender, CanExecuteRoutedEventArgs args)
        {
            
        }

        public void ShowView()
        {
            this.Show();
        }
    }
}
