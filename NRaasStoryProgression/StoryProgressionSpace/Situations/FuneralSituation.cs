using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Situations
{
    public class FuneralSituation : HousePartySituation
    {
        public SimDescription mWhoDied;

        public FuneralSituation(Lot lot, SimDescription deadSim, Sim host, List<SimDescription> guests, OutfitCategories clothingStyle, DateAndTime startTime) 
            : base(lot, host, guests, clothingStyle, startTime)
        {
            mWhoDied = deadSim;
        }

        // Properties
        public override bool AllowPartyAnimalCheer
        {
            get { return false; }
        }

        public override Party.PartyType TypeOfParty
        {
            get
            {
                return Party.PartyType.Funeral;
            }
        }
 
        public override bool IsGreatParty
        {
            get { return false; }
        }

        public override CommodityKind BeAtHostedSituationMotive(Sim sim)
        {
            return CommodityKind.BeGuestAtFuneral;
        }

        public override PartyParams GetParams()
        {
            return Funeral.kFuneralParams;
        }

        public override Party.PartyOutcomes GetPartyOutcome()
        {
            return Party.PartyOutcomes.kNeutralParty;
        }

        public override CommodityKind HostMotive()
        {
            return CommodityKind.BeHostAtFuneral;
        }

        public override void OnHappening()
        {
            foreach (Sim sim in base.Guests)
            {
                ActiveTopic.AddToSim(sim, "Funeral", mWhoDied);
            }
            foreach (Sim sim2 in base.OtherHosts)
            {
                ActiveTopic.AddToSim(sim2, "Funeral", mWhoDied);
            }
            ActiveTopic.AddToSim(base.Host, "Funeral", mWhoDied);
        }

        public override void OnPreparation()
        { }

        public override CommodityKind PreparationMotive()
        {
            return CommodityKind.PrepareForFuneral;
        }

        public override void ShowPartyEndTNS(Party.PartyOutcomes partyResult)
        {
            string str;
            switch (partyResult)
            {
                case Party.PartyOutcomes.kBadParty:
                    str = Common.LocalizeEAString(Host.IsFemale, "Gameplay/Notifications/Party:BadFuneral", new object[] { Host.SimDescription });
                    break;

                case Party.PartyOutcomes.kGoodParty:
                    str = Common.LocalizeEAString(Host.IsFemale, "Gameplay/Notifications/Party:GoodFuneral", new object[] { Host.SimDescription });
                    break;

                default:
                    str = Common.LocalizeEAString(Host.IsFemale, "Gameplay/Notifications/Party:NormalFuneral", new object[] { Host.SimDescription });
                    break;
            }
            StyledNotification.Format format = new StyledNotification.Format(str, ObjectGuid.InvalidObjectGuid, Host.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
            StyledNotification.Show(format, "w_party");
        }
    }
}

