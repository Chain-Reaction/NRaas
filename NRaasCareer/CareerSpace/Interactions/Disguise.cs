using NRaas.CareerSpace.Skills;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Services;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class Disguise : Interaction<Sim, Sim>, Common.IAddInteraction
    {
        // Fields
        public static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public override bool Run()
        {
            Definition definition = InteractionDefinition as Definition;

            string type = definition.mType.ToString();

            string uniformName;
            ResourceKey key;

            if (definition.mType == ServiceType.GrimReaper)
            {
                uniformName = "YmDeath";

                key = ResourceKey.CreateOutfitKey(uniformName, 0);
            }
            else
            {
                if (!ServiceNPCSpecifications.TryGetUniform(type, Actor.SimDescription.Gender, out key, out uniformName))
                {
                    return false;
                }
            }

            SimOutfit uniform = new SimOutfit(key);
            if (uniform.IsValid)
            {
                SimOutfit outfit = new SimOutfit(OutfitUtils.ApplyUniformToOutfit(Actor.SimDescription.GetOutfit(OutfitCategories.Everyday, 0), uniform, Actor.SimDescription, "NRaas.Disguise.Run"));
                if (outfit.IsValid)
                {
                    Actor.SimDescription.AddOutfit(outfit, OutfitCategories.Career, true);

                    Actor.PushSwitchToOutfitInteraction(Sim.ClothesChangeReason.GoingToWork, OutfitCategories.Career);
                }
            }

            return true;
        }

        public class Definition : InteractionDefinition<Sim, Sim, Disguise>
        {
            public ServiceType mType;

            static List<ServiceType> sTypes = new List<ServiceType>();

            static Definition()
            {
                sTypes.Add(ServiceType.Burglar);
                sTypes.Add(ServiceType.Firefighter);
                sTypes.Add(ServiceType.GrimReaper);
                sTypes.Add(ServiceType.Magician);
                sTypes.Add(ServiceType.Maid);
                sTypes.Add(ServiceType.MailCarrier);
                sTypes.Add(ServiceType.PizzaDelivery);
                sTypes.Add(ServiceType.Police);
                sTypes.Add(ServiceType.Repairman);
                sTypes.Add(ServiceType.Repoman);
                sTypes.Add(ServiceType.SocialWorkerAdoption);
            }
            public Definition()
            { }
            public Definition(ServiceType type)
            {
                mType = type;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("Disguise:RootName") };
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                if (mType == ServiceType.None) return null;

                return Common.LocalizeEAString(actor.IsFemale, "Ui/Caption/Services/Service:" + mType.ToString(), new object[0]);
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                foreach (ServiceType type in sTypes)
                {
                    results.Add(new InteractionObjectPair(new Definition(type), iop.Target));
                }

                base.AddInteractions(iop, actor, target, results);
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;

                if (a != target) return false;

                try
                {
                    if (a.CurrentOutfitCategory == Sims3.SimIFace.CAS.OutfitCategories.Career)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

                OmniCareer career = a.Occupation as OmniCareer;

                Assassination skill = a.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                if (mType == ServiceType.GrimReaper)
                {
                    if (skill != null)
                    {
                        if (skill.IsReaper()) return true;
                    }

                    if (career != null)
                    {
                        if (career.CanUseReaper()) return true;
                    }
                }
                else
                {
                    if (skill != null)
                    {
                        if (skill.IsHitman()) return true;
                    }

                    if (career != null)
                    {
                        if (career.CanUseDisguise()) return true;
                    }
                }

                return false;
            }
        }
    }
}
