using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class OutfitChanger : Common.IWorldLoadFinished, Common.IWorldQuit
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kRoomChanged, OnRoomChanged);
        }

        public void OnWorldQuit()
        {
            Woohooer.Settings.mOutfitChangers.Clear();
        }

        protected static void OnRoomChanged(Event e)
        {
            Sim obj = e.Actor as Sim;
            if (obj != null)
            {
                if (Woohooer.Settings.NeedsChange(obj))
                {
                    if (Woohooer.Settings.mChangeRoomClothings)
                    {
                        bool needsChange = false;

                        try
                        {
                            if (obj.CurrentOutfitCategory == Sims3.SimIFace.CAS.OutfitCategories.Naked)
                            {
                                needsChange = (SkinnyDipClothingPile.FindClothingPile(obj) == null);
                            }
                        }
                        catch
                        {
                        }

                        if (needsChange)
                        {
                            if (!InteractionsEx.HasInteraction<Shower.TakeShower.Definition>(obj))
                            {
                                Sim.ClothesChangeReason reason = Sim.ClothesChangeReason.GoingToBed;
                                if (obj.IsOutside)
                                {
                                    reason = Sim.ClothesChangeReason.GoingOutside;
                                }
                                else if (Woohooer.Settings.mSwitchToEverydayAfterNakedWoohoo)
                                {
                                    reason = Sim.ClothesChangeReason.LeavingRoom;
                                }

                                SwitchOutfits.SwitchNoSpin(obj, reason);
                            }
                        }
                    }

                    Woohooer.Settings.RemoveChange(obj);
                }
            }
        }
    }
}
