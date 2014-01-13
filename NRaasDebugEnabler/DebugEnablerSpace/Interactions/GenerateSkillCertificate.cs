using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.DebugEnablerSpace.Interfaces;
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
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class GenerateSkillCertificate : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is Sim)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                Sim sim = Target as Sim;
                if (sim != null)
                {
                    List<Item> allChoices = new List<Item>();

                    foreach (Skill skill in sim.SkillManager.List)
                    {
                        if (skill.IsHiddenSkill()) continue;

                        if (!skill.ReachedMaxLevel()) continue;

                        allChoices.Add(new Item(skill));
                    }

                    if (allChoices.Count == 0) return false;

                    CommonSelection<Item>.Results results = new CommonSelection<Item>(Common.Localize("GenerateSkillCertificate:MenuName"), allChoices).SelectMultiple();
                    if ((results == null) || (results.Count == 0)) return false;

                    foreach (Item item in results)
                    {
                        Certificate certificate = GlobalFunctions.CreateObjectOutOfWorld("CertificateReward") as Certificate;
                        certificate.OwnerSimDescription = sim.SimDescription;
                        certificate.SkillName = item.Value.Guid;

                        if (!Inventories.TryToMove(certificate, sim))
                        {
                            certificate.Destroy();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        public class Item : ValueSettingOption<Skill>
        {
            public Item(Skill skill)
                : base(skill, skill.Name, 0)
            { }
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<GenerateSkillCertificate>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("GenerateSkillCertificate:MenuName");
            }

            public override bool Test(IActor a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return true;
            }
        }
    }
}