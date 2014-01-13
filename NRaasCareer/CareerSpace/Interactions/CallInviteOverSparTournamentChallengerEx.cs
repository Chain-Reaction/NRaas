using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class CallInviteOverSparTournamentChallengerEx : Phone.CallInviteOverSparTournamentChallenger, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Phone, Phone.CallInviteOverSparTournamentChallenger.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP1)) return;

            interactions.Replace<Phone, Phone.CallInviteOverSparTournamentChallenger.Definition>(Singleton);
        }

        protected new class Definition : Phone.Call.CallDefinition<CallInviteOverSparTournamentChallengerEx>
        {
            public override string[] GetPath(bool isFemale)
            {
                return base.CallSimPath();
            }

            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Phone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Base fail");
                    return false;
                }

                MartialArts skill = a.SkillManager.GetSkill<MartialArts>(SkillNames.MartialArts);
                if ((skill == null) || !skill.CanParticipateInTournaments)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("CanParticipateInTournaments fail");
                    return false;
                }

                SimDescription tournamentChallenger = skill.TournamentChallenger;
                if (tournamentChallenger == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("tournamentChallenger fail");
                    return false;
                }
                /* Removed
                if (GameUtils.GetCurrentWorld() != WorldName.China)
                {
                    Phone.CallInviteOverSparTournamentChallenger.Definition.GreyedOutTooltipHelper helper = new Phone.CallInviteOverSparTournamentChallenger.Definition.GreyedOutTooltipHelper(a.SimDescription, tournamentChallenger, "CanSparOnlyInChina");
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(helper.Callback);
                    return false;
                }
                */
                if (tournamentChallenger.Household == a.Household)
                {
                    Phone.CallInviteOverSparTournamentChallenger.Definition.GreyedOutTooltipHelper helper2 = new Phone.CallInviteOverSparTournamentChallenger.Definition.GreyedOutTooltipHelper(a.SimDescription, tournamentChallenger, "ChallengerPartOfHousehold");
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(helper2.Callback);
                    return false;
                }
                if (!CanSimInviteOver(a, isAutonomous) || !CanInviteOverToLot(a.LotCurrent, isAutonomous))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sims3.Gameplay.Visa.TravelUtil.LocalizeString(a.IsFemale, "CannotInviteOver", new object[] { a }));
                    return false;
                }
                if ((tournamentChallenger.CreatedSim != null) && (tournamentChallenger.CreatedSim.LotCurrent == a.LotCurrent))
                {
                    Phone.CallInviteOverSparTournamentChallenger.Definition.GreyedOutTooltipHelper helper3 = new Phone.CallInviteOverSparTournamentChallenger.Definition.GreyedOutTooltipHelper(a.SimDescription, tournamentChallenger, "ChallengerOnTheSameLot");
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(helper3.Callback);
                    return false;
                }
                return true;
            }
        }
    }
}
