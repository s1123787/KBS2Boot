using KBSBoot.DAL;
using KBSBoot.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string TextFieldInput;
        

        public void OnLoginButtonIsPressed(object source, LoginEventArgs e)
        {
            TextFieldInput = e.Name;

            using(var context = new BootDB())
            {
                var members = (from m in context.Members
                              select m).ToList<Member>();

                var test = members.Count;

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
            using (var context = new BootDB())
            {
                var member = new Member { memberUsername = UsernameInput, memberName = NameInput, memberAccessLevelId = 1, memberRowLevelId = 1, };
                context.Members.Add(member);
                context.SaveChanges();
            }
        }

        public bool IsNullOrWhiteSpace(string Name, string Username)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Username))
            {
                return true;
            }else
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

                if(usernames.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #region SpecialCharChecks
        //check for special characters, digits are allowed
        public bool HasSpecialChars(string stString)
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
        public bool NameHasSpecialChars(string stString)
        {
            string s = stString.Replace(" ", string.Empty);
            if(s.Any(ch => !Char.IsLetter(ch)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


    }
}
