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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class GoHereAnd : GoHereBase
    {
        public static InteractionDefinition sSameLotSingleton = new SameLotDefinition();
        public static InteractionDefinition sOtherLotSingleton = new OtherLotDefinition();
        public static InteractionDefinition sOtherLotWithCarriedChildSingleton = new OtherLotWithCarriedChildDefinition();

        public override void OnPreLoad()
        {
            Tunings.Inject<Terrain, Terrain.GoHere.OtherLotDefinition, OtherLotDefinition>(false);
            Tunings.Inject<Terrain, Terrain.GoHere.OtherLotWithCarriedChildDefinition, OtherLotWithCarriedChildDefinition>(false);
            Tunings.Inject<Terrain, Terrain.GoHere.SameLotDefinition, SameLotDefinition>(false);
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Terrain>(sOtherLotSingleton);
            interactions.Add<Terrain>(sOtherLotWithCarriedChildSingleton);
            interactions.Add<Terrain>(sSameLotSingleton);
        }

        public override bool Run()
        {
            try
            {
                List<InteractionObjectPair> allInteractions = Actor.GetAllInteractionsForActor(Actor);
                if (allInteractions == null) return false;

                List<Item> items = new List<Item>();

                foreach (InteractionObjectPair pair in allInteractions)
                {
                    if (pair.InteractionDefinition is ISoloInteractionDefinition)
                    {
                        try
                        {
                            InteractionInstanceParameters p = new InteractionInstanceParameters(pair, Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), Autonomous, CancellableByPlayer);

                            GreyedOutTooltipCallback callBack = null;
                            if (!IUtil.IsPass(pair.InteractionDefinition.Test(ref p, ref callBack))) continue;

                            items.Add(new Item(pair.InteractionDefinition.GetInteractionName(ref p), pair));
                        }
                        catch (Exception e)
                        {
                            Common.Exception(Actor, null, pair.InteractionDefinition.GetType().ToString(), e);
                        }
                    }
                }

                Item choice = new CommonSelection<Item>(GetInteractionName(), items).SelectSingle();
                if (choice == null) return false;

                InteractionInstance instance = choice.Value.InteractionDefinition.CreateInstance(Actor, Actor, GetPriority(), Autonomous, CancellableByPlayer);

                Actor.InteractionQueue.Add(instance);

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

        protected static void PopulatePicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
        {
            NumSelectableRows = 0x1;
            headers = new List<ObjectPicker.HeaderInfo>();
            headers.Add(new ObjectPicker.HeaderInfo("NRaas.GoHere.OptionList:InteractionTitle", "NRaas.GoHere.OptionList:InteractionTooltip", 0xfa));

            listObjs = new List<ObjectPicker.TabInfo>();
            listObjs.Add(new ObjectPicker.TabInfo(string.Empty, string.Empty, new List<ObjectPicker.RowInfo>()));
        }

        public class Item : ValueSettingOption<InteractionObjectPair>
        {
            public Item(string name, InteractionObjectPair pair)
                : base(pair, name, 0)
            { }
        }

        private new class OtherLotDefinition : Terrain.GoHere.OtherLotDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoHereAnd();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Terrain target, InteractionObjectPair iop)
            {
                return Common.Localize("GoHereAnd:MenuName", actor.IsFemale);
            }
        }

        private new class OtherLotWithCarriedChildDefinition : Terrain.GoHere.OtherLotWithCarriedChildDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoHereAnd();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Terrain target, InteractionObjectPair iop)
            {
                return Common.Localize("GoHereAnd:MenuName", actor.IsFemale);
            }
        }

        public new class SameLotDefinition : Terrain.GoHere.SameLotDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoHereAnd();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(IActor actor, Terrain target, InteractionObjectPair iop)
            {
                return Common.Localize("GoHereAnd:MenuName", actor.IsFemale);
            }
        }
    }
}
