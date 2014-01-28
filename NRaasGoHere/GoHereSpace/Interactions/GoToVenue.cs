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
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class GoToVenue : Interaction<Sim, Sim>, Common.IAddInteraction
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
                Item choice = GetChoices(Actor, Common.Localize("GoToVenue:MenuName", Target.IsFemale));
                if (choice == null) return false;

                InteractionInstance interaction = null;
                if (choice.Value.IsCommunityLot)
                {
                    interaction = VisitCommunityLotEx.Singleton.CreateInstance(choice.Value, Target, GetPriority(), Autonomous, CancellableByPlayer);
                }
                else
                {
                    interaction = VisitLotEx.Singleton.CreateInstance(choice.Value, Target, GetPriority(), Autonomous, CancellableByPlayer);
                }

                Target.InteractionQueue.Add(interaction);
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

        public static List<Item> GetChoices(Sim actor)
        {
            List<Item> choices = new List<Item>();

            foreach (Lot lot in LotManager.Lots)
            {
                if (lot.IsWorldLot) continue;
                if (lot == actor.LotCurrent) continue;

                if (lot.IsCommunityLot)
                {                  
                    switch (lot.CommercialLotSubType)
                    {
                        case CommercialLotSubType.kCommercialUndefined:
                        case CommercialLotSubType.kMisc_NoVisitors:
                            continue;
                    }
                }
                else
                {
                    if (lot != actor.LotHome) continue;
                }

                choices.Add(new Item(lot));
            }

            return choices;
        }

        public static Item GetChoices(Sim actor, string title)
        {
            List<Item> choices = GetChoices(actor);
            if (choices.Count == 0) return null;

            bool okayed;
            Item result = new CommonSelection<Item>(title, choices, new AuxillaryColumn()).SelectSingle(out okayed);

            if (!okayed) return null;

            if (result == null)
            {
                result = RandomUtil.GetRandomObjectFromList(choices);
            }

            return result;
        }

        public class Item : ValueSettingOption<Lot>
        {
            public readonly bool mOpen;

            public Item(Lot lot)
                : base(lot, VenueName(lot.CommercialLotSubType) + ": " + lot.Name, 0, lot.GetThumbnailKey())
            {
                float startTime, endTime;
                Bartending.GetRoleTimes(out startTime, out endTime, lot.GetMetaAutonomyType);

                if (startTime == endTime)
                {
                    mOpen = true;
                }
                else
                {
                    mOpen = SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, startTime, endTime);
                }
            }

            protected static string VenueName(CommercialLotSubType type)
            {
                if (type == CommercialLotSubType.kCommercialUndefined) return null;

                string result = (Responder.Instance.EditTownModel as EditTownModel).CommercialSubTypeLocalizedName(type);
                if (result == null)
                {
                    result = type.ToString();
                }

                return result;
            }
        }

        public class AuxillaryColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
        {
            public AuxillaryColumn()
                : base("NRaas.GoHere.OptionList:OpenTitle", "NRaas.GoHere.OptionList:OpenTooltip", 40)
            { }

            public override ObjectPicker.ColumnInfo GetValue(Item item)
            {
                if (item.mOpen)
                {
                    return new ObjectPicker.TextColumn(Common.Localize("Boolean:True"));
                }
                else
                {
                    return new ObjectPicker.TextColumn("");
                }
            }
        }

        private class Definition : SoloSimInteractionDefinition<GoToVenue>
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.Localize("GoToVenue:MenuName", target.IsFemale);
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor != target) return false;

                return base.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
