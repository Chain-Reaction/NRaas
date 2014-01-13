using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Helpers
{
    public class WerewolfOutfitControl
    {
        static Dictionary<ulong, bool> sRippedParts = new Dictionary<ulong, bool>();

        static List<CASParts.Wrapper> sParts = null;

        static string sWerewolfOutfitKey = "NRaasWerewolfTransformOutfit";

        static WerewolfOutfitControl()
        {
            sRippedParts.Add(0x2FACD37EAFAF7E44, true); // Elder Male
            sRippedParts.Add(0x179718762C8B8E16, true);
            sRippedParts.Add(0xAA85E8757AB40AD9, true);
            sRippedParts.Add(0x57C9DFFB55771B50, true); // Elder Female
            sRippedParts.Add(0x36D595D1B9444F89, true);
            sRippedParts.Add(0x9DB8854BF0DC74E6, true);
            sRippedParts.Add(0x26B4BCFD24064ED0, true); // Adult Male
            sRippedParts.Add(0x470B0A372B672422, true);
            sRippedParts.Add(0xA69B3DF15ED1BDBD, true);
            sRippedParts.Add(0x59BFC78526834A8D, true); // Adult Female
            sRippedParts.Add(0x5984B54BC38391CC, true);
            sRippedParts.Add(0xAB612493984710EA, true);
            sRippedParts.Add(0xD529CE55ED351216, true); // Teen Male
            sRippedParts.Add(0x7177BBE6329D4CE5, true);
            sRippedParts.Add(0x2C27A751046366F3, true);
            sRippedParts.Add(0x639A2CBE0D4300BC, true); // Teen Female
            sRippedParts.Add(0x0BD41D5C6F0C0931, true);
            sRippedParts.Add(0x9C6AB2D32A08F555, true);
        }

        protected static bool PartMatches(CASParts.Wrapper part)
        {
            if (part.mPart.Key.GroupId != 0x70000000) return false;

            return sRippedParts.ContainsKey(part.mPart.Key.InstanceId);
        }

        protected static void OnBuff(Event e)
        {
            HasGuidEvent<BuffNames> buffEvent = e as HasGuidEvent<BuffNames>;
            if (buffEvent == null) return;

            if (buffEvent.Guid != BuffNames.Werewolf) return;

            Transform(e.Actor.SimDescription);
        }

        public static void Transform(SimDescription sim)
        {
            if (!Hybrid.Settings.mSpecialWerewolfOutfit) return;

            SimOutfit outfit = sim.GetSpecialOutfit(sWerewolfOutfitKey);
            if (outfit == null)
            {
                SimOutfit sourceOutfit = sim.GetOutfit(OutfitCategories.Everyday, 0);

                foreach (CASPart part in sourceOutfit.Parts)
                {
                    if (part.BodyType == BodyTypes.FullBody)
                    {
                        return;
                    }
                }

                if (sParts == null)
                {
                    sParts = CASParts.GetParts(PartMatches);
                }

                List<CASParts.PartPreset> parts = new List<CASParts.PartPreset>();
                foreach (CASParts.Wrapper part in sParts)
                {
                    if (!part.ValidFor(sim)) continue;

                    if (RandomUtil.CoinFlip()) continue;

                    CASParts.PartPreset preset = part.GetRandomPreset();
                    if (preset == null) continue;

                    parts.Add(preset);
                }

                if (parts.Count > 0)
                {
                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, new CASParts.Key(sWerewolfOutfitKey), sourceOutfit))
                    {
                        foreach (CASParts.PartPreset part in parts)
                        {
                            builder.Builder.RemoveParts(new BodyTypes[] { part.mPart.BodyType });
                            builder.ApplyPartPreset(part);
                        }
                    }
                }

                outfit = sim.GetSpecialOutfit(sWerewolfOutfitKey);
                if (outfit == null) return;
            }
            
            SwitchOutfits.SwitchNoSpin(sim.CreatedSim, new CASParts.Key(sWerewolfOutfitKey));
        }
    }
}
