using ani_DonationBox;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Decorations.Mimics.ani_DonationBox
{
    public class DonationBox : Painting
    {
        public DonationBoxInfo info;

        public override void OnCreation()
        {
            //  base.OnCreation();
            info = new DonationBoxInfo();

        }

        #region Mailbox Stuff
        public static MailboxDoor[] sMailboxDoors = new MailboxDoor[]
        {
	        new MailboxDoor(Slots.Hash("Front"))
        };

        public virtual MailboxDoor GetDoorOfSim(Sim sim)
        {
            return Mailbox.sMailboxDoors[0];
        }

        #endregion

        public override void OnStartup()
        {
            //Settings
            base.AddInteraction(ChangeName.Singleton);
            base.AddInteraction(EditFunds.Singleton);
            base.AddInteraction(DonationMoodValue.Singleton);

            //Building
            base.AddInteraction(SaveLotValue.Singleton);
            base.AddInteraction(SubstractFromFunds.Singleton);

            //Sim
            base.AddInteraction(WithdrawFunds.Singleton);
            base.AddInteraction(Donate.Singleton);
        }

        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {
            return new SimpleTextTooltip(this.info.Name + ": " + this.info.Funds + "§");
        }

       
        public override AbstractArtObject.ViewTuning TuningView
        {
            get
            {
                return PaintingMission.kViewTuning;
            }
        }
    }
}
