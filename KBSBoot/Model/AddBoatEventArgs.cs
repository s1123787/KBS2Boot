using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class AddBoatEventArgs
    {
        public string boatType { get; set; }

        public string boatName { get; set; }


        public int boatOutOfService { get; set; }
        public string boatYoutubeUrl { get; set; }
        public System.Drawing.Image BoatImage { get; set; }

        public int boatTypeId { get; set; }

        public AddBoatEventArgs(string boatName, int boatOutOfService, string boatType, string boatYoutubeUrl, System.Drawing.Image boatImage, int boattypeId)
        {
            this.boatType = boatType;
            this.boatName = boatName;
            this.boatOutOfService = boatOutOfService;
            this.boatYoutubeUrl = boatYoutubeUrl;
            this.BoatImage = boatImage;
            this.boatTypeId = boattypeId;
        }
    }
}
