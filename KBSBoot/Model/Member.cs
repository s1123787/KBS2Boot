using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override string ToString()
        {
            return $"ID: {memberId} \n" +
                   $"Username: {memberUsername}\n" +
                   $"Name: {memberName} \n" +
                   $"Accesslevel: {memberAccessLevelId} \n" +
                   $"Row level: {memberRowLevelId} \n" +
                   $"Subscribed until: {memberSubscribedUntill:dd-MM-yyyy} \n";
        }
    }
}
