using NRaas.MasterControllerSpace.Sims.Advanced;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class EditSimInCASEx : ImmediateInteraction<Sim,Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        protected static CASMode OnGetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref Sims.CASBase.EditType editType)
        {
            return CASMode.Full;
        }

        public override bool Run()
        {
            try
            {
                return EditInCAS.Perform(Target.SimDescription, OnGetMode);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return false;
        }

        public class Definition : ActorlessInteractionDefinition<Sim, Sim, EditSimInCASEx>
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Cheats.LocalizeString("EditSimInCAS", new object[0]);
            }
            
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    return new EditInCAS().Test(target.SimDescription);
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
