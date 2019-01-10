namespace KBSBoot.Model
{
    public class ReservationEventArgs
    {
        private int MemberId { get; }

        public ReservationEventArgs(int MemberId)
        {
            this.MemberId = MemberId;
        }
    }
}