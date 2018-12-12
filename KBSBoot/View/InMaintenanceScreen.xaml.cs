using KBSBoot.DAL;
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
    /// Interaction logic for InMaintenanceScreen.xaml
    /// </summary>
    public partial class InMaintenanceScreen : UserControl
    {
        private int BoatID;
        public string FullName;
        public int AccessLevel;
        public int MemberId;
        public DateTime selectedDateFrom;
        public DateTime selectedDateUntill;


        public InMaintenanceScreen(string FullName, int AccessLevel, int BoatId, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            this.BoatID = BoatId;
            InitializeComponent();
        }

        private void DatePickerUntill_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //check if untill date is before start date, then clear untill date
            if(DatePickerUntill.SelectedDate <= DatePicker.SelectedDate)
                DatePickerUntill.SelectedDate = null;

            //datepicker starts from today
            selectedDateFrom = DatePicker.SelectedDate.Value;
            DatePickerUntill.DisplayDateStart = selectedDateFrom;
            DatePickerUntill.IsEnabled = true;
        }

        private void DidLoad(object sender, RoutedEventArgs e)
        {
            if (AccessLevel == 1)
            {
                AccessLevelButton.Content = "Lid";
            }
            else if (AccessLevel == 2)
            {
                AccessLevelButton.Content = "Wedstrijdcommissaris";
            }
            else if (AccessLevel == 3)
            {
                AccessLevelButton.Content = "Materiaalcommissaris";
            }
            else if (AccessLevel == 4)
            {
                AccessLevelButton.Content = "Administrator";
            }

            //datepicker starts from today
            DatePicker.DisplayDateStart = DateTime.Today;

            //set boat text
            SetBoatDetailsOnView();
        }

        private void SetBoatDetailsOnView()
        {
            string boatName = "";
            string boatDescr = "";
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 where b.boatId == BoatID
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatName = b.boatName,
                                     boatTypeDescription = bt.boatTypeDescription
                                 });

                foreach (var b in tableData)
                {
                    boatName = b.boatName;
                    boatDescr = b.boatTypeDescription;
                }
            }

            boatIdLabel.Content = boatName;
            boatDescrLabel.Content = boatDescr;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new DamageReportsScreen(FullName, AccessLevel, MemberId));
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
        }

        private void InMaintenance_Click(object sender, RoutedEventArgs e)
        {
            //startdate is not empty

            //enddate is not empty

            //startdate < enddate

            //save to boatMaintenance

            //update boats, set boatMaintenanceId = boatMaintenance insertId
        }
    }
}
