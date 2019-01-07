namespace KBSBoot.Model
{
    public class ReservationEventArgs
    {
        public int MemberId { get; set; }

        public ReservationEventArgs(int MemberId)
        {
            this.MemberId = MemberId;
        }
    }
}