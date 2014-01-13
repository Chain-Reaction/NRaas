using NRaas.CommonSpace.Helpers;
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
    public class GoForWalkWithDogEx : Sim.GoForWalkWithDog, Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.GoForWalkWithDog.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.GoForWalkWithDog.Definition>(Singleton);
        }

        private new class Definition : Sim.GoForWalkWithDog.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoForWalkWithDogEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                try
                {
                    IActor actor = parameters.Actor;
                    if (!mbWalkHome)
                    {
                        NumSelectableRows = 0x1;
                        headers = new List<ObjectPicker.HeaderInfo>();
                        headers.Add(new ObjectPicker.HeaderInfo("Gameplay/Actors/Sim/GoForWalkWithDog:ObjectPickerColumnHeaderLot", "Gameplay/Actors/Sim/GoForWalkWithDog:ObjectPickerColumnHeaderDescriptionLot", 0xfa));
                        listObjs = new List<ObjectPicker.TabInfo>();
                        listObjs.Add(new ObjectPicker.TabInfo(string.Empty, string.Empty, new List<ObjectPicker.RowInfo>()));

                        foreach(GoToVenue.Item choice in GoToVenue.GetChoices(null))
                        {
                            Lot lot = choice.Value;

                            GameObject item = lot;
                            List<ObjectPicker.ColumnInfo> columnInfo = new List<ObjectPicker.ColumnInfo>();
                            string name = lot.Name;
                            if (lot == actor.LotHome)
                            {
                                name = Common.LocalizeEAString(actor.IsFemale, "Gameplay/Actors/Sim/GoForWalkWithDog:WalkHome");
                            }
                            else if (name == string.Empty)
                            {
                                name = lot.Address;
                                if (name == string.Empty)
                                {
                                    name = (Responder.Instance.EditTownModel as EditTownModel).CommercialSubTypeLocalizedName(lot.CommercialLotSubType);
                                    if (name == null)
                                    {
                                        name = string.Empty;
                                    }
                                }
                            }

                            columnInfo.Add(new ObjectPicker.ThumbAndTextColumn(item.GetThumbnailKey(), name));
                            listObjs[0x0].RowInfo.Add(new ObjectPicker.RowInfo(item, columnInfo));
                        }

                        if (listObjs.Count == 0x0)
                        {
                            NumSelectableRows = 0x0;
                            listObjs = null;
                            headers = null;
                        }
                    }
                    else
                    {
                        NumSelectableRows = 0x0;
                        listObjs = null;
                        headers = null;
                    }
                }
                catch (Exception e)
                {
                    NumSelectableRows = 0x0;
                    listObjs = null;
                    headers = null;

                    Common.Exception(parameters.Actor, parameters.Target, e);
                }
            }
        }
    }
}
