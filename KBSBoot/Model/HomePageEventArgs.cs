using System;

namespace KBSBoot.Model
{
    public class HomePageEventArgs : EventArgs
    {
        public int TypeMember { get; }
        public string FullName { get; }
        public int MemberId { get; }

        public HomePageEventArgs(int typeMember, string fullName, int memberId)
        {
            TypeMember = typeMember;
            FullName = fullName;
            MemberId = memberId;
        }
    }
}