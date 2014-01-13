using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class VisitCommunityLotWithEx : VisitCommunityLotEx
    {
        static InteractionDefinition sOldVisitWithSingleton;

        public override void OnPreLoad()
        {
            Tunings.Inject<Lot, VisitCommunityLotWith.VisitWithDefinition, Definition>(false);

            sOldVisitWithSingleton = VisitCommunityLotWith.VisitWithSingleton;
            VisitCommunityLotWith.VisitWithSingleton = new Definition();
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Lot, VisitCommunityLotWith.VisitWithDefinition>(VisitCommunityLotWith.VisitWithSingleton);
        }

        public override bool Run()
        {
            try
            {
                mFollowers = Terrain.GoHereWith.GetFollowersFromSelectedObjects(Actor, SelectedObjects);
                return base.Run();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : VisitCommunityLotWith.VisitWithDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new VisitCommunityLotWithEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    if (!GoHere.Settings.AllowPush(a, target))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Allow Push Fail");
                        return false;
                    }
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
