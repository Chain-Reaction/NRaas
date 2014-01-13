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
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
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
    public class PlayJuicePongEx : PingPongTable.PlayJuicePong, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<PingPongTable, PingPongTable.PlayJuicePong.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<PingPongTable>(Singleton);
        }

        public new class Definition : PingPongTable.PlayJuicePong.Definition
        {
            public Definition()
            { }
            public Definition(string text, bool playType)
                : base(text, playType)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new PlayJuicePongEx();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim actor, PingPongTable target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, PingPongTable target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(PingPongTable.LocalizeString("PlayJuicePong", new object[0]) + Localization.Ellipsis, false), iop.Target));
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 1;
                PopulateSimPicker(ref parameters, out listObjs, out headers, PingPongTable.GetPotentialPlayers(parameters.Actor as Sim, parameters.Target.LotCurrent, !Woohooer.Settings.mUnlockTeenActions, true), false);
            }

            public override bool Test(Sim a, PingPongTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.InUse)
                {
                    if (!target.IsHaunted)
                    {
                        if (target.IsActorUsingMe(a))
                        {
                            return true;
                        }

                        greyedOutTooltipCallback = delegate
                        {
                            return PingPongTable.LocalizeString("PingPongTableInUse", new object[0]);
                        };
                    }

                    return false;
                }

                if (!Woohooer.Settings.mUnlockTeenActions)
                {
                    if (a.SimDescription.TeenOrBelow)
                    {
                        return false;
                    }
                }

                if (PingPongTable.GetPotentialPlayers(a, target.LotCurrent, !Woohooer.Settings.mUnlockTeenActions, true).Count == 0)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/Toys/PlayCatchObject:NoOneToPlayWith", new object[0]));
                    return false;
                }

                return true;
            }
        }
    }
}
