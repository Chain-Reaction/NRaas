using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class MassVisitLot : VisitCommunityLot
    {
        public MassVisitLot()
        { }

        [DoesntRequireTuning]
        public new class Definition : MetaInteractionScoredByVenue<Sim, Lot, MassVisitLot>
        {
            public List<Sim> mFollowers = null;

            public Definition(List<Sim> followers)
            {
                mFollowers = followers;
            }

            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                string name = target.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    return VisitCommunityLot.LocalizeString(actor.IsFemale, "VisitNamedLot", new object[] { name });
                }

                string entryKey = "Gameplay/Core/VisitCommunityLot:" + target.GetMetaAutonomyVenueType().ToString();

                if (Localization.HasLocalizationString(entryKey))
                {
                    return Common.LocalizeEAString(actor.IsFemale, entryKey, new object[0x0]);
                }

                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Core/VisitLot:VisitLot", new object[0x0]);
            }

            public override bool Test(Sim a, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }

        public override bool Run()
        {
            Definition definition = base.InteractionDefinition as Definition;

            mFollowers = definition.mFollowers;

            return base.Run();
        }
    }
}

