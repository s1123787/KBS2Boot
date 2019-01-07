using System;

namespace KBSBoot.Model
{
    public class HomePageEventArgs : EventArgs
    {
        public int TypeMember { get; set; }
        public string FullName { get; set; }
        public int MemberId { get; set; }

        public HomePageEventArgs(int typeMember, string fullName, int memberId)
        {
            TypeMember = typeMember;
            FullName = fullName;
            MemberId = memberId;
        }
    }
}