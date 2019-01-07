using System.Drawing;

namespace KBSBoot.Model
{
    public class AddBoatEventArgs
    {
        public string BoatType { get; set; }
        public string BoatName { get; set; }
        public string BoatYoutubeUrl { get; set; }
        public Image BoatImage { get; set; }
        public int BoatTypeId { get; set; }
        public string FullName { get; set; }
        public int AccessLevel { get; set; }
        public int MemberId { get; set; }

        public AddBoatEventArgs(string boatName, string boatType, string boatYoutubeUrl, System.Drawing.Image boatImage, int boatTypeId, string fullName, int accessLevel, int memberId)
        {
            BoatType = boatType;
            BoatName = boatName;
            BoatYoutubeUrl = boatYoutubeUrl;
            BoatImage = boatImage;
            BoatTypeId = boatTypeId;
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
        }
    }
}