using KBSBoot.DAL;
using KBSBoot.Model;
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
        private string boatName;


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

            this.boatName = boatName;
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
            bool valid = true;
            DateTime? from = DatePicker.SelectedDate;
            DateTime? untill = DatePickerUntill.SelectedDate;
            
            //start date is not empty
            if (from == null)
            {
                valid = false;
                MessageBox.Show("Vult u de start datum in.");
            }
            else if (untill == null) //end date is not empty
            {
                valid = false;
                MessageBox.Show("Vult u de eind datum in.");
            }
            else if (from > untill) //startdate > enddates
            {
                valid = false;
                MessageBox.Show("Start datum mag niet voorbij eind datum zijn.");
            }

            

            //save to boatMaintenance
            if (valid == true)
            {
                int insertId;

                //set enddate time to 23:59:59 from day
                DateTime nu = (DateTime) untill;
                DateTime newUntill = nu.AddHours(23).AddMinutes(59).AddSeconds(59);


                using (var context = new BootDB())
                {
                    var inmain = new BoatInMaintenances()
                    {
                        boatId = this.BoatID,
                        startDate = from,
                        endDate = newUntill
                    };

                    //save to boat in maintenances
                    context.BoatInMaintenances.Add(inmain);
                    context.SaveChanges();

                    insertId = inmain.boatInMaintenanceId;

                    //find reservation id
                    int reservId;
                    var query = context.Reservations
                       .Where(x => x.memberId == MemberId && x.date >= from && x.date <= newUntill)
                       .FirstOrDefault<Reservations>();

                    if(query != null)
                    { 
                        reservId = query.reservationId;
                    
                        //remove records from reservations
                        context.Reservations.RemoveRange(context.Reservations.Where(x => x.reservationId == reservId && x.memberId == MemberId));

                        //remove records from Resevervation_boats
                        context.Reservation_Boats.RemoveRange(context.Reservation_Boats.Where(x => x.reservationId == reservId));
                        context.SaveChanges();
                    }
                }

                

                MessageBox.Show($"Boot \"{this.boatName}\" is in onderhoud genomen van {from?.ToString("dd-MM-yyyy")} t/m {untill?.ToString("dd-MM-yyyy")}.\n LET OP: alle reserveringen voor deze boot op deze dagen zijn verwijderd!");
                Switcher.Switch(new DamageReportsScreen(FullName, AccessLevel, MemberId));
            }
            
        }
    }
}
