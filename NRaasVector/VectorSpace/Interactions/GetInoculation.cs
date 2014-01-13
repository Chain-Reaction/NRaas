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
    public class GetInoculation : RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>, Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, RabbitHole.VisitRabbitHole.Definition, Definition>(true);

            InteractionTuning tuning = Tunings.GetTuning<Hospital, Hospital.GetFluShot.Definition>();
            if (tuning != null)
            {
                tuning.Availability.AgeSpeciesAvailabilityFlags = Sims3.SimIFace.CAS.CASAGSAvailabilityFlags.None;
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Hospital>(Singleton);
        }

        public override void ConfigureInteraction()
        {
            base.ConfigureInteraction();

            TimedStage stage = new TimedStage(GetInteractionName(), Hospital.GetFluShot.kSimMinutesForFluShot, false, false, true);
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

                        foreach(VectorBooter.Data vector in VectorBooter.Vectors)
                        {
                            bool booster = false;
                            int cost = vector.InoculationCost;

                            DiseaseVector disease = Vector.Settings.GetVector(Actor, vector.Guid);
                            if (disease != null)
                            {
                                if (!disease.CanInoculate) continue;

                                if (disease.IsInoculated)
                                {
                                    if (vector.InoculationStrain >= vector.InoculationStrain) continue;

                                    cost /= 2;
                                    booster = true;
                                }
                            }
                            else
                            {
                                if (vector.InoculationCost <= 0) continue;
                            }

                            items.Add(new VectorBooter.Item(vector, Actor.IsFemale, cost, booster));
                        }

                        if (items.Count == 0)
                        {
                            Common.Notify(Actor, Common.Localize("GetInoculation:None", Actor.IsFemale));
                        }
                        else
                        {
                            CommonSelection<VectorBooter.Item>.Results choices = new CommonSelection<VectorBooter.Item>(GetInteractionName(), items, new CostColumn()).SelectMultiple();
                            if ((choices == null) || (choices.Count == 0)) return false;

                            int cost = 0;

                            foreach (VectorBooter.Item item in choices)
                            {
                                cost += item.Value.InoculationCost;
                            }

                            if (Actor.FamilyFunds < cost)
                            {
                                Common.Notify(Actor, Common.Localize("GetInoculation:Cost", Actor.IsFemale, new object[] { cost }));
                            }
                            else
                            {
                                foreach (VectorBooter.Item item in choices)
                                {
                                    DiseaseVector vector = Vector.Settings.GetVector(Actor, item.Value.Guid);
                                    if (vector != null)
                                    {
                                        vector.Inoculate(item.Value.InoculationStrain, true);
                                    }
                                }

                                Common.Notify(Actor, Common.Localize("GetInoculation:Success", Actor.IsFemale, new object[] { cost }));

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

        public class Definition : InteractionDefinition<Sim, RabbitHole, GetInoculation>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return Common.Localize("GetInoculation:MenuName", actor.IsFemale);
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

        public class CostColumn : ObjectPickerDialogEx.CommonHeaderInfo<VectorBooter.Item>
        {
            public CostColumn()
                : base("NRaas.Vector.Type:CostHeader", "NRaas.Vector.Type:CostTooltip", 30)
            { }

            public override ObjectPicker.ColumnInfo GetValue(VectorBooter.Item item)
            {
                return new ObjectPicker.TextColumn(EAText.GetMoneyString(item.Count));
            }
        }
    }
}
