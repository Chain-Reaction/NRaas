using NRaas.CareerSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
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
    public static class SparChallenge
    {
        public static bool CallbackTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (GameUtils.GetCurrentWorld() == WorldName.China)
                {
                    return false;
                }

                MartialArts skill = actor.SkillManager.GetSkill<MartialArts>(SkillNames.MartialArts);
                MartialArts arts2 = target.SkillManager.GetSkill<MartialArts>(SkillNames.MartialArts);
                if (((skill != null) && skill.CanParticipateInTournaments) && ((arts2 != null) && arts2.CanParticipateInTournaments))
                {
                    SimDescription tournamentChallenger = skill.TournamentChallenger;
                    if ((tournamentChallenger != null) && (tournamentChallenger == target.SimDescription))
                    {
                        if (MartialArts.IsSimsMotiveSparWorthy(actor))
                        {
                            return true;
                        }
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Common.LocalizeEAString(actor.IsFemale, "Gameplay/Objects/HobbiesSkills/BoardBreaker:CannotBeFatigued", new object[] { actor }));
                    }
                }

                greyedOutTooltipCallback = Common.DebugTooltip("CanParticipateInTournaments fail");
                return false;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }
    }
}
