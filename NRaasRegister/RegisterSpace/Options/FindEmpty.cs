using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.RegisterSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RegisterSpace.Options
{
    public class FindEmpty : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "FindEmpty";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (Register.Settings.mAllowImmigration) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<Item> choices = new List<Item> ();

            foreach(IRoleGiver obj in Sims3.Gameplay.Queries.GetObjects<IRoleGiver>())
            {
                if (obj.CurrentRole != null) continue;

                if (obj.RoleType == Role.RoleType.None) continue;

                if (obj.HasBeenDestroyed) continue;

                if (!obj.InWorld) continue;

                if (obj.LotCurrent == null) continue;

                if (!obj.LotCurrent.IsCommunityLot) continue;

                if (obj is IRoleGiverOneNpcPerLot)
                {
                    IRoleGiverOneNpcPerLot objAsOnePerLot = obj as IRoleGiverOneNpcPerLot;
                    if ((objAsOnePerLot != null) && objAsOnePerLot.DoesRoleLotTypeMatch(objAsOnePerLot.LotCurrent))
                    {
                        bool found = false;

                        foreach (IRoleGiverOneNpcPerLot other in obj.LotCurrent.GetObjects<IRoleGiverOneNpcPerLot>())
                        {
                            if (other.CurrentRole == null) continue;

                            if (other == obj) continue;

                            if (other.RoleType == obj.RoleType)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found) continue;
                    }
                }

                choices.Add(new Item(obj));
            }

            if (choices.Count == 0)
            {
                Common.Notify(Common.Localize(GetTitlePrefix() + ":None"));
            }

            CommonSelection<Item>.Results results = new CommonSelection<Item>(Name, choices).SelectMultiple();
            if ((results == null) || (results.Count == 0)) return OptionResult.Failure;

            foreach (Item result in results)
            {
                if (CameraController.IsMapViewModeEnabled())
                {
                    Camera.ToggleMapView();
                }

                Camera.FocusOnGivenPosition(result.Value.Position, Camera.kDefaultLerpDuration);

                if (new RoleGiver.Select().Perform(new GameHitParameters<IGameObject>(parameters.mActor, result.Value, new GameObjectHit(GameObjectHitType.Object))) == OptionResult.Failure)
                {
                    return OptionResult.Failure;
                }
            }

            return OptionResult.SuccessClose;
        }

        public class Item : ValueSettingOption<IRoleGiver>
        {
            public Item(IRoleGiver giver)
                : base(giver, GetName(giver), 0, giver.GetThumbnailKey())
            { }

            public static string GetName(IRoleGiver giver)
            {
                return giver.GetLocalizedName() + " - " + giver.LotCurrent.Name;
            }
        }
    }
}