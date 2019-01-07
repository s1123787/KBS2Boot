using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.VisualBasic.CompilerServices;

namespace KBSBoot.View
{
    public static class Switcher
    {
        public static MainWindow pageSwitcher;

        public static void Switch(UserControl newPage)
        {
            pageSwitcher.Navigate(newPage);
        }

        public static void Logout()
        {
            Switch(new LoginScreen());
        }

        public static void BackToHomePage(int accessLevel, string fullName, int memberId)
        {
            switch (accessLevel)
            {
                case 1:
                    Switch(new HomePageMember(fullName, accessLevel, memberId));
                    break;
                case 2:
                    Switch(new HomePageMatchCommissioner(fullName, accessLevel, memberId));
                    break;
                case 3:
                    Switch(new HomePageMaterialCommissioner(fullName, accessLevel, memberId));
                    break;
                case 4:
                    Switch(new HomePageAdministrator(fullName, accessLevel, memberId));
                    break;
            }
        }
    }
}
