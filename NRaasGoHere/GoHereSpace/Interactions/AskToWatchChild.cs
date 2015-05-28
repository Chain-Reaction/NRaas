using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class AskToWatchChild : SocialInteraction, Common.IAddInteraction
    {
        static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (SelectedObjects.Count > 0)
                {
                    foreach (object sim in SelectedObjects)
                    {
                        Sim sim2 = sim as Sim;
                        if (sim2 != null)
                        {
                            sim2.Household.Remove(sim2.SimDescription);
                            Target.Household.Add(sim2.SimDescription);
                            sim2.SimDescription.IsNeverSelectable = true;

                            Relationship rel = sim2.GetRelationship(Target, true);

                            if (sim2.SimDescription.Baby)
                            {
                                if (rel != null)
                                {
                                    if (rel.CurrentLTRLiking < 30)
                                    {
                                        rel.LTR.UpdateLiking(30f);
                                    }
                                }
                            }

                            if (sim2.SimDescription.ToddlerOrBelow)
                            {
                                GoHereEx.Teleport.Perform(sim2, Target.LotHome, false);
                            }
                            else
                            {
                                if (sim2.LotCurrent == Actor.LotHome)
                                {
                                    Sim.MakeSimGoHome(sim2, false);
                                }
                            }
                        }
                    }

                    Common.Notify(Target, Common.Localize("AskToWatchChildren:Success", Actor.IsFemale));
                }
                
                return true;
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

        public static List<Sim> GetChoices(Sim actor, Sim target)
        {
            List<Sim> results = new List<Sim>();
            if (actor == null || target == null || actor.LotHome == null || target.LotHome == null)
            {
                return results;
            }

            foreach (SimDescription sim2 in actor.Household.SimDescriptions)
            {
                if (sim2.CreatedSim == null) continue;

                if (sim2.TeenOrAbove) continue;

                if (sim2.CreatedSim.IsAtWork) continue;

                if (!Relationships.IsCloselyRelated(actor.SimDescription, sim2, false)) continue;

                results.Add(sim2.CreatedSim);
            }

            return results;
        }

        private class Definition : InteractionDefinition<Sim, Sim, AskToWatchChild>
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.Localize("AskToWatchChild:MenuName", target.IsFemale);
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                Sim actor = parameters.Actor as Sim;
                Sim target = parameters.Target as Sim;

                if ((actor.IsInGroupingSituation()) || (actor != target))
                {
                    listObjs = null;
                    headers = null;
                    NumSelectableRows = 0x0;
                }
                else
                {
                    NumSelectableRows = -1;
                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, GetChoices(actor, target), false);
                }
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor == null)
                {
                    if (actor.SimDescription.ChildOrBelow) return false;

                    if (target.SimDescription.ChildOrBelow) return false;

                    if (target.LotHome == null) return false;

                    IMiniRelationship rel = target.SimDescription.GetMiniRelationship(actor.SimDescription.GetMiniSimDescription());
                    if (rel != null)
                    {
                        if (rel.AreFriends() || rel.AreRomantic())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}