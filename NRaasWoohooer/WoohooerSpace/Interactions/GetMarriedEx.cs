using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class GetMarriedEx : WeddingArch.GetMarried, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<WeddingArch, WeddingArch.GetMarried.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<WeddingArch, WeddingArch.GetMarried.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public new class Definition : WeddingArch.GetMarried.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new GetMarriedEx();
                result.Init(ref parameters);
                return result;
            }

            public override bool Test(Sim a, WeddingArch target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    SimDescription partner = a.Partner;
                    if (partner == null)
                    {
                        return false;
                    }
                    Sim createdSim = partner.CreatedSim;
                    if ((createdSim == null) || !a.IsEngaged)
                    {
                        return false;
                    }
                    if (createdSim.LotCurrent != target.LotCurrent)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(WeddingArch.LocalizeString(a.IsFemale, "FianceNotOnLot", new object[] { createdSim }));
                        return false;
                    }

                    string reason;
                    if (!CommonSocials.CanGetRomantic(a, createdSim, false, false, true, ref greyedOutTooltipCallback, out reason))
                    {
                        return false;
                    }

                    return CommonSocials.CanGetMarriedNow(a, createdSim, isAutonomous, false, ref greyedOutTooltipCallback);
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
