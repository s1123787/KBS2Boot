using System.Drawing;

namespace KBSBoot.Model
{
    public class AddBoatEventArgs
    {
        public string BoatType { get; }
        public string BoatName { get; }
        public string BoatYoutubeUrl { get; }
        public Image BoatImage { get; }
        public int BoatTypeId { get; }
        public string FullName { get; }
        public int AccessLevel { get; }
        public int MemberId { get; }

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