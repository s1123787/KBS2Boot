using KBSBoot.DAL;
using KBSBoot.View;
using System;
using System.Linq;
using System.Text.RegularExpressions;
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
        private string InputUserName;
        public int SortUser;
        public delegate void NewHomePage(object source, HomePageEventArgs e);
        public event NewHomePage OnNewHomePage;
        public bool Correct;

        public void OnLoginButtonIsPressed(object source, LoginEventArgs e)
        {
            InputUserName = e.Name;
            var Source = (LoginScreen)source;

            OnNewHomePage += Source.OnNewHomePage;
            using (var context = new BootDB())
            {
                //all usernames of members who are active in database in a list
                var today = DateTime.Now.Date;
                var members = (from m in context.Members where m.memberSubscribedUntill >= today || m.memberAccessLevelId == 4 select m).ToList();

                //check if username exist in list from database
                if (members.Any(i => i.memberUsername == InputUserName))
                {
                    //getting all data from database that is useful for logging in
                    var accessLevelCollection = (from m in context.Members where m.memberUsername == InputUserName select m.memberAccessLevelId).ToList<int>();
                    var accessLevel = accessLevelCollection[0];
                    var accessDescriptionCollection = (from a in context.Accesslevel where a.AccessLevelId == accessLevel select a.Description).ToList<string>();
                    var accessDescription = accessDescriptionCollection[0];
                    var idCollection = (from m in context.Members where m.memberUsername == InputUserName select m.memberId).ToList<int>();
                    var id = idCollection[0];
                    var fullNameCollection = (from m in context.Members where m.memberUsername == InputUserName select m.memberName).ToList<string>();
                    var fullName = fullNameCollection[0];
                    //homepage is made and switch to so user can do something with the app
                    OnNewHomePageMade(accessLevel, fullName, id);
                    SortUser = accessLevel;
                    Correct = true;
                }
                else //username doesn't exist
                {
                    //show message on window that username doesn't exist
                    MessageBox.Show("Inloggen is niet mogelijk", "Inloggen niet mogelijk", MessageBoxButton.OK, MessageBoxImage.Information);
                    Correct = false;
                }
            }
        }

        public static void OnRegisterOkButtonIsPressed(object source, RegisterEventArgs e)
        {
            var nameInput = e.Name;
            var usernameInput = e.Username;

            //check all textboxes are filled
            if (!IsNullOrWhiteSpace(nameInput, usernameInput))
            {
                //check if name has special characters
                if (!NameHasSpecialChars(nameInput))
                {
                    //if username doesn't exists and a correct username is filled in, add user to database
                    if (!CheckUsername(usernameInput)) return;
                    AddNewUserToDb(nameInput, usernameInput);

                    MessageBox.Show("Gebruiker is succesvol toegevoegd.", "Gebruiker toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
                    Switcher.Switch(new LoginScreen());
                }
                //if name has special chars, error message
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

        public static void AddNewUserToDb(string nameInput, string usernameInput)
        {
            //add user as member, but not active yet
            if (CheckAmountUsers() != 0)
            {
                using (var context = new BootDB())
                {
                    var member = new Member { memberUsername = usernameInput, memberName = nameInput, memberAccessLevelId = 1, memberRowLevelId = 1 };
                    context.Members.Add(member);
                    context.SaveChanges();
                }
            }//if system has no members, first one to register is admin
            else
            {
                using (var context = new BootDB())
                {
                    var member = new Member { memberUsername = usernameInput, memberName = nameInput, memberAccessLevelId = 4, memberRowLevelId = 1 };
                    context.Members.Add(member);
                    context.SaveChanges();
                }
            }
        }

        public static bool IsNullOrWhiteSpace(string name, string username)
        {
            return string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(username);
        }

        public static bool CheckUsername(string username)
        {
            //check if it only has letters                
            if (!HasSpecialChars(username))
            {
                //check if username already exists
                if (!UsernameExists(username))
                {
                    return true;
                } 
                //if it has special characters 
                MessageBox.Show("De ingevoerde gebruikersnaam is al in gebruik!", "Gebruikersnaam bestaat al", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            // if username already exists
            MessageBox.Show("De gebruikersnaam kan alleen bestaan uit letters en cijfers!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        //check if username exists
        public static bool UsernameExists(string username)
        {
            using (var context = new BootDB())
            {
                var usernames = from u in context.Members
                                where u.memberUsername == username
                                select u;

                return usernames.Any();
            }
        }
        
        //Method to check if user already exists
        public static void CheckIfMemberExists(Member member)
        {
            using (var context = new BootDB())
            {
                var members = from m in context.Members
                    where m.memberUsername == member.memberUsername
                    select m;

                if (members.ToList().Count > 0)
                    throw new Exception("Gebruiker bestaat al");
            }
        }

        //Method to add member to the database
        public static void AddMemberToDb(Member member)
        {
            using (var context = new BootDB())
            {
                context.Members.Add(member);
                context.SaveChanges();
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
            return stString.Any(ch => !char.IsLetterOrDigit(ch));
        }

        //check if name has spacial chars
        public static bool NameHasSpecialChars(string stString)
        {
            var s = stString.Replace(" ", string.Empty);
            return s.Any(ch => !char.IsLetter(ch));
        }
        #endregion


        protected virtual void OnNewHomePageMade(int type, string fullName, int memberId)
        {
            OnNewHomePage?.Invoke(this, new HomePageEventArgs(type, fullName, memberId));
        }

        private static int CheckAmountUsers()
        {
            using (var context = new BootDB())
            {
                var data = (from m in context.Members
                            select m).ToList();
                return data.Count;
            }
        }

        //for unit tests
        public static void RemoveLastAddedMember()
        {
            using (var context = new BootDB())
            {
                var data = (from m in context.Members
                            select m).ToList().Last();
                context.Members.Attach(data);
                context.Members.Remove(data);
                context.SaveChanges();
            }
        }
    }
}