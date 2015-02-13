using Sims3.SimIFace;

namespace ani_DonationBox
{
    [Persistable]
    public class DonationBoxInfo
    {
        public string Name;
        public int Funds;
        public int LotValue;
        public int DonationMoodValue;

        public DonationBoxInfo()
        {
            Name = "DonationBox";
            Funds = 0;
            LotValue = 0;
            DonationMoodValue = 10;
        }
    }
}
