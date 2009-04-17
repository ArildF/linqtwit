using System.Windows.Controls;

namespace LinqTwit.QueryModule.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, ILoginView
    {
        public LoginView()
        {
            InitializeComponent();

            this.LostFocus += new System.Windows.RoutedEventHandler(LoginView_LostFocus);
            this.GotFocus += new System.Windows.RoutedEventHandler(LoginView_GotFocus);

        }

        void LoginView_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        void LoginView_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
        }

       

    }
}

