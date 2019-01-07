using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for InMaintenanceScreen.xaml
    /// </summary>
    public partial class InMaintenanceScreen : UserControl
    {
        private readonly int BoatID;
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private DateTime SelectedDateFrom;
        private DateTime SelectedDateUntil;
        private string BoatName;


        public InMaintenanceScreen(string fullName, int accessLevel, int boatId, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            BoatID = boatId;
            InitializeComponent();
        }

        private void DatePickerUntil_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //check if until date is before start date, then clear until date
            if(DatePickerUntil.SelectedDate <= DatePicker.SelectedDate)
                DatePickerUntil.SelectedDate = null;

            //datepicker starts from today
            SelectedDateFrom = DatePicker.SelectedDate.Value;
            DatePickerUntil.DisplayDateStart = SelectedDateFrom;
            DatePickerUntil.IsEnabled = true;
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

            //disable the dates that are not possible to take a boat into maintenance
            DisableDatePickerDates();
        }

        private void DisableDatePickerDates()
        {
            //select all disabled dates from boat
            using (var context = new BootDB())
            {
                var tableData = (from bm in context.BoatInMaintenances
                                 where bm.boatId == BoatID
                                 select new
                                 {
                                     boatId = bm.boatId,
                                     startDate = bm.startDate,
                                     endDate = bm.endDate
                                 });

                foreach (var b in tableData)
                {
                    //disable dates in datepicker
                    DatePicker.BlackoutDates.Add(new CalendarDateRange((DateTime) b.startDate, (DateTime) b.endDate));
                }
            }
        }

        private void SetBoatDetailsOnView()
        {
            var boatName = "";
            var boatDescription = "";
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
                    boatDescription = b.boatTypeDescription;
                }
            }

            BoatName = boatName;
            boatIdLabel.Content = boatName;
            boatDescrLabel.Content = boatDescription;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new DamageReportsScreen(FullName, AccessLevel, MemberId));
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }
        
        private void InMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var valid = true;
            var from = DatePicker.SelectedDate;
            var until = DatePickerUntil.SelectedDate;
            
            //start date is not empty
            if (from == null)
            {
                valid = false;
                MessageBox.Show("Vult u de start datum in.");
            }
            else if (until == null) //end date is not empty
            {
                valid = false;
                MessageBox.Show("Vult u de eind datum in.");
            }
            else if (from > until) //startDate > endDates
            {
                valid = false;
                MessageBox.Show("Start datum mag niet voorbij eind datum zijn.");
            }

            

            //save to boatMaintenance
            if (!valid) return;
            int insertId;

            //set endDate time to 23:59:59 from day
            var now = (DateTime) until;
            var newUntil = now.AddHours(23).AddMinutes(59).AddSeconds(59);


            using (var context = new BootDB())
            {
                var inmain = new BoatInMaintenances()
                {
                    boatId = this.BoatID,
                    startDate = @from,
                    endDate = newUntil
                };

                //save to boat in maintenances
                context.BoatInMaintenances.Add(inmain);
                context.SaveChanges();

                insertId = inmain.boatInMaintenanceId;

                //find reservation id
                int reservId;
                var query = context.Reservations
                    .FirstOrDefault(x => x.memberId == MemberId && x.date >= @from && x.date <= newUntil);

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
            MessageBox.Show($"Boot \"{BoatName}\" is in onderhoud genomen van {@from?.ToString("dd-MM-yyyy")} t/m {until?.ToString("dd-MM-yyyy")}.");
            Switcher.Switch(new DamageReportsScreen(FullName, AccessLevel, MemberId));
        }
    }
}