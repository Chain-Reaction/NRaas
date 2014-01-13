using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Interactions
{
    public class GetResistance : RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>, Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, RabbitHole.VisitRabbitHole.Definition, Definition>(true);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Hospital>(Singleton);
        }

        public override void ConfigureInteraction()
        {
            base.ConfigureInteraction();

            TimedStage stage = new TimedStage(GetInteractionName(), 15, false, false, true);
            Stages = new List<Stage>(new Stage[] { stage });
            ActiveStage = stage;
        }

        public override bool InRabbitHole()
        {
            try
            {
                StartStages();
                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    if (succeeded)
                    {
                        List<VectorBooter.Item> items = new List<VectorBooter.Item>();

                        foreach (DiseaseVector vector in Vector.Settings.GetVectors(Actor))
                        {
                            if (!vector.CanBoostResistance) continue;

                            items.Add(new VectorBooter.Item(vector.Data, Actor.IsFemale, vector.ResistanceCost, vector.HasAnyResistance));
                        }

                        if (items.Count == 0)
                        {
                            Common.Notify(Actor, Common.Localize("GetInoculation:None", Actor.IsFemale));
                        }
                        else
                        {
                            CommonSelection<VectorBooter.Item>.Results choices = new CommonSelection<VectorBooter.Item>(GetInteractionName(), items, new GetInoculation.CostColumn()).SelectMultiple();
                            if ((choices == null) || (choices.Count == 0)) return false;

                            int cost = 0;

                            foreach (VectorBooter.Item item in choices)
                            {
                                cost += item.Value.ResistanceCost;
                            }

                            if (Actor.FamilyFunds < cost)
                            {
                                Common.Notify(Actor, Common.Localize("BuyResistance:Cost", Actor.IsFemale, new object[] { cost }));
                            }
                            else
                            {
                                foreach (VectorBooter.Item item in choices)
                                {
                                    foreach (DiseaseVector vector in Vector.Settings.GetVectors(Actor))
                                    {
                                        if (vector.Guid == item.Value.Guid)
                                        {
                                            vector.AlterResistance(Vector.Settings.mResistanceBoost);
                                        }
                                    }
                                }

                                Common.Notify(Actor, Common.Localize("BuyResistance:Success", Actor.IsFemale, new object[] { cost }));

                                Actor.ModifyFunds(-cost);
                            }
                        }
                    }
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }
                return succeeded;
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

        public class Definition : InteractionDefinition<Sim, RabbitHole, GetResistance>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return Common.Localize("BuyResistance:MenuName", actor.IsFemale);
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("Virologist:RootName", isFemale) };
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
