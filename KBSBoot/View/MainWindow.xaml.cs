using KBSBoot.View;
using System.Windows;
using System.Windows.Controls;


namespace KBSBoot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserControl content;

        public MainWindow()
        {            
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Switcher.pageSwitcher = this;
            Switcher.Switch(new LoginScreen());  //initial page              
        }

        public void Navigate(UserControl nextPage)
        {
            Content = nextPage;
        }       
    }
}
