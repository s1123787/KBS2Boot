using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        public string UserName;
        public int UserId;
        public int AccessLevelId;
        public string AccessLevelDescription;

        public HomePage(string UserName, int UserId, int AccessLevelId, string AccessLevelDescription)
        {
            //all the information that is needed
            this.UserName = UserName;
            this.UserId = UserId;
            this.AccessLevelId = AccessLevelId;
            this.AccessLevelDescription = AccessLevelDescription;
            InitializeComponent();
        }

        private void DidLoaded(object sender, RoutedEventArgs e)
        {
            //show the username of the user
            label.Content = UserName + " ";
            //if user is an administrator
            if (AccessLevelId == 4)
            {
                Button btn = new Button();
                btn.Content = "Beheren van gebruikers";
                btn.Name = "ButtonTest";
                btn.Height = 150;
                btn.Width = 200;
                //Canvas.SetLeft(btn, 5);
                //Canvas.SetTop(btn, 5);
                btn.Background = Brushes.AliceBlue;
                HomeScreen.Children.Add(btn);

                label.Content += "Je mag alles doen";
            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }
    }
}
