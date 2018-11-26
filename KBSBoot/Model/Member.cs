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
        public DateTime memberSubscribedUntill { get; set; }
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
                var members = (from m in context.Members where m.memberSubscribedUntill > DateTime.Now select m).ToList<Member>();

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
                    OnNewHomePageMade(AccessLevel, FullName);
                    SortUser = AccessLevel;
                }
                else //username doesn't exist
                {
                    //show message on window that username doesn't exist
                    Source.UpdateLabel("Geen geldige gebruikersnaam");
                    Correct = false;
                }
            }
        }
        
        protected virtual void OnNewHomePageMade(int type, string FullName)
        {
            OnNewHomePage?.Invoke(this, new HomePageEventArgs(type, FullName));
        } 
    }
}
