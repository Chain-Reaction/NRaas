using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Replacers;
using NRaas.OverwatchSpace.Helpers;
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
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Helpers
{
    public class CommonSocials : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP10))
            {
                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Invite to VIP Room", "InviteToVIP"));
            }
        }

        public static void InviteToVIP(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                ResortManagerCareerProtection protection = null;

                try
                {
                    Lot lotCurrent = actor.LotCurrent;
                    if (!lotCurrent.ResortManager.IsCheckedIn(target))
                    {
                        DateAndTime time2 = lotCurrent.ResortManager.GetCheckOutDateAndTime(actor) - SimClock.CurrentTime();
                        int days = ((int)SimClock.ConvertFromTicks(time2.Ticks, TimeUnit.Days)) + 0x1;

                        List<Sim> simsInGroup = new List<Sim>();
                        simsInGroup.Add(target);

                        protection = new ResortManagerCareerProtection(simsInGroup, days);
                    }

                    SocialCallback.InviteToVIP(actor, target, interaction, topic, i);
                }
                finally
                {
                    if (protection != null)
                    {
                        protection.Dispose();
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
