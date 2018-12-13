using KBSBoot.DAL;
using KBSBoot.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace KBSBoot.Model
{
    public class Member
    {
        public int memberId { get; set; }
        public string memberUsername { get; set; }
        public string memberName { get; set; }
        public int memberAccessLevelId { get; set; }
        public int memberRowLevelId { get; set; }
        public DateTime? memberSubscribedUntill { get; set; }
        public string InputUserName;
        public int SortUser;
        public delegate void NewHomePage(object source, HomePageEventArgs e);
        public event NewHomePage OnNewHomePage;
        public bool Correct;

        public void OnLoginButtonIsPressed(object source, LoginEventArgs e)
        {
            InputUserName = e.Name;
            LoginScreen Source = (LoginScreen)source;

            OnNewHomePage += Source.OnNewHomePage;
            using (var context = new BootDB())
            {
                //all usernames of members who are active in database in a list
                var members = (from m in context.Members where m.memberSubscribedUntill > DateTime.Now || m.memberAccessLevelId == 4 select m).ToList<Member>();

                //check if username exist in list from database
                if (members.Any(i => i.memberUsername == InputUserName))
                {
                    //getting all data from database that usefull for logging in
                    var AccessLevelCollection = (from m in context.Members where m.memberUsername == InputUserName select m.memberAccessLevelId).ToList<int>();
                    var AccessLevel = AccessLevelCollection[0];
                    var AccessDiscriptionCollection = (from a in context.Accesslevel where a.accessLevelId == AccessLevel select a.description).ToList<string>();
                    var AccessDiscription = AccessDiscriptionCollection[0];
                    var idCollection = (from m in context.Members where m.memberUsername == InputUserName select m.memberId).ToList<int>();
                    var id = idCollection[0];
                    var FullNameCollection = (from m in context.Members where m.memberUsername == InputUserName select m.memberName).ToList<string>();
                    var FullName = FullNameCollection[0];
                    //homepage is made and switch to so user can do something with the app
                    OnNewHomePageMade(AccessLevel, FullName, id);
                    SortUser = AccessLevel;
                }
                else //username doesn't exist
                {
                    //show message on window that username doesn't exist
                    MessageBox.Show("Inloggen is niet mogelijk", "Inloggen niet mogelijk", MessageBoxButton.OK, MessageBoxImage.Information);
                    Correct = false;
                }
            }
        }

        public void OnRegisterOKButtonIsPressed(Object source, RegisterEventArgs e)
        {
            string NameInput = e.Name;
            string UsernameInput = e.Username;

            //check all textboxes are filled
            if (!IsNullOrWhiteSpace(NameInput, UsernameInput))
            {

                //check if name has special characters
                if (!NameHasSpecialChars(NameInput))
                {

                    //if username doesn't exists and a correct username is filled in, add user to database
                    if (CheckUsername(UsernameInput))
                    {
                        AddNewUserToDB(NameInput, UsernameInput);

                        MessageBox.Show("Gebruiker is succesvol toegevoegd.", "Gebruiker toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
                        Switcher.Switch(new LoginScreen());
                    }
                }//if name has special chars, error message
                else
                {
                    MessageBox.Show("De naam kan alleen bestaan uit letters!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            } //messagebox if a field is empty
            else
            {
                MessageBox.Show("Vul beide velden in!", "Leeg veld", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void AddNewUserToDB(string NameInput, string UsernameInput)
        {
            //add user as member, but not active yet
            if (CheckAmountUsers() != 0)
            {
                using (var context = new BootDB())
                {
                    var member = new Member { memberUsername = UsernameInput, memberName = NameInput, memberAccessLevelId = 1, memberRowLevelId = 1 };
                    context.Members.Add(member);
                    context.SaveChanges();
                }
            }//if system has no members, first one to register is admin
            else
            {
                using (var context = new BootDB())
                {
                    var member = new Member { memberUsername = UsernameInput, memberName = NameInput, memberAccessLevelId = 4, memberRowLevelId = 1 };
                    context.Members.Add(member);
                    context.SaveChanges();
                }
            }
        }

        public bool IsNullOrWhiteSpace(string Name, string Username)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Username))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool CheckUsername(string Username)
        {
            //check if it only has letters                
            if (!HasSpecialChars(Username))
            {
                //check if username already exists
                if (!UsernameExists(Username))
                {
                    return true;
                } //if it has special characters 
                else
                {
                    MessageBox.Show("De ingevoerde gebruikersnaam is al in gebruik!", "Gebruikersnaam bestaat al", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            } // if username already exists 
            else
            {
                MessageBox.Show("De gebruikersnaam kan alleen bestaan uit letters en cijfers!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        //check if username exists
        public bool UsernameExists(string Username)
        {
            using (var context = new BootDB())
            {
                var usernames = from u in context.Members
                                where u.memberUsername == Username
                                select u;

                if (usernames.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //Method used to check if the entered name and user name contain any invalid characters
        public static void CheckForInvalidCharacters(string str)
        {
            var regexItem = new Regex("^[a-zA-Z0-9ÅåǺǻḀḁẚĂăẶặẮắẰằẲẳẴẵȂȃÂâẬậẤấẦầẪẫẨẩẢảǍǎȺⱥȦȧǠǡẠạÄäǞǟÀàȀȁÁáĀāĀ̀ā̀ÃãĄąĄ́ą́Ą̃ą̃ᶏĔĕḜḝȆȇÊêÊ̄ê̄Ê̌ê̌ỀềẾếỂểỄễỆệẺẻḘḙĚěɆɇĖėĖ́ė́Ė̃ė̃ẸẹËëÈèÈ̩è̩ȄȅÉéÉ̩é̩ĒēḔḕḖḗẼẽḚḛĘęĘ́ę́Ę̃ę̃ȨȩE̩e̩ᶒØøǾǿÖöȪȫÓóÒòÔôỐốỒồỔổỖỗỘộǑǒŐőŎŏȎȏȮȯȰȱỌọƟɵƠơỚớỜờỠỡỢợỞởỎỏŌōṒṓṐṑÕõȬȭṌṍṎṏǪǫȌȍO̩o̩Ó̩ó̩Ò̩ò̩ǬǭŬŭɄʉᵾᶶỤụÜüǛǜǗǘǙǚǕǖṲṳÚúÙùÛûṶṷǓǔȖȗŰűŬŭƯưỨứỪừỬửỰựỮỮỦủŪūŪ̀ū̀Ū́ū́ṺṻŪ̃ū̃ŨũṸṹṴṵᶙŲųŲ́ų́Ų̃ų̃ȔȕŮůỊịĬĭÎîǏǐƗɨÏïḮḯÍíÌìȈȉĮįĮ́Į̃ĪīĪ̀ī̀ᶖỈỉȊȋĨĩḬḭᶤ ]*$");

            if (!regexItem.IsMatch(str))
                throw new FormatException();
        }
        #region SpecialCharChecks
        //check for special characters, digits are allowed
        public static bool HasSpecialChars(string stString)
        {
            if (stString.Any(ch => !Char.IsLetterOrDigit(ch)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //check if name has spacial chars
        public static bool NameHasSpecialChars(string stString)
        {
            string s = stString.Replace(" ", string.Empty);
            if (s.Any(ch => !Char.IsLetter(ch)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        protected virtual void OnNewHomePageMade(int type, string FullName, int memberId)
        {
            OnNewHomePage?.Invoke(this, new HomePageEventArgs(type, FullName, memberId));
        }

        private int CheckAmountUsers()
        {
            using (var context = new BootDB())
            {
                var data = (from m in context.Members
                            select m).ToList();
                return data.Count;
            }
        }
    }
}
