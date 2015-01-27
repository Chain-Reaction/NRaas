using NRaas.CommonSpace.Helpers;
using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Helpers;
using NRaas.CareerSpace.Interfaces;
using NRaas.CareerSpace.Skills;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using Sims3.UI.Controller;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class Kill : Interaction<Sim, Sim>, Common.IAddInteraction
    {
        static Common.MethodStore sStoryProgressionAboutToDie = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AboutToDie", new Type[] { typeof(SimDescription) });

        public static InteractionDefinition Singleton = new Definition(false, true);
        public static InteractionDefinition MassSingleton = new Definition(true, true);

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public Kill()
        { }

        public static string LocalizeString(bool isFemale, string key, params object[] parameters)
        {
            return Common.LocalizeEAString(isFemale, "Gameplay/Excel/Socializing/Action:NRaas" + key, parameters);
        }

        public static void OnAccepted(Sim actor, Sim target, SimDescription.DeathType type)
        {
            InteractionInstance instance = (new Definition (false, false, type)).CreateInstance(target, actor, new InteractionPriority(InteractionPriorityLevel.Autonomous), false, true);

            actor.InteractionQueue.Add(instance);
        }

        protected static bool PrivateKill(Sim actor, Sim target, SimDescription.DeathType deathType)
        {
            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            if (!Assassination.CanBeKilled(target, ref greyedOutTooltipCallback))
            {
                return false;
            }

            if (deathType == SimDescription.DeathType.None)
            {
                new EraseKillTask(actor, target.SimDescription);
                return true;
            }
            else
            {
                sStoryProgressionAboutToDie.Invoke<bool>(new object[] { target.SimDescription });

                if (deathType == SimDescription.DeathType.Burn)
                {
                    LotLocation location = LotLocation.Invalid;
                    ulong lotLocation = World.GetLotLocation(target.PositionOnFloor, ref location);
                    if (lotLocation != 0)
                    {
                        Fire.CreateFire(lotLocation, location);
                    }
                }
                else if (deathType == SimDescription.DeathType.Meteor)
                {
                    new MeteorControl(actor, target);
                    return true;
                }
                else if (deathType == SimDescription.DeathType.Thirst)
                {
                    if (actor.SimDescription.IsVampire)
                    {
                        if (actor.TraitManager.HasElement(TraitNames.Vegetarian))
                        {
                            actor.BuffManager.AddElement(BuffNames.Nauseous, Origin.FromCarnivorousBehavior);
                        }
                        else
                        {
                            actor.BuffManager.AddElement(BuffNames.Sated, Origin.FromReceivingVampireNutrients);
                        }

                        actor.Motives.SetMax(CommodityKind.VampireThirst);

                        if (target.SimDescription.IsFairy)
                        {
                            actor.BuffManager.AddElement(BuffNames.DrankFromAFairy, Origin.FromReceivingVampireNutrients);
                        }

                        EventTracker.SendEvent(EventTypeId.kVampireDrankFromSim, actor, target);
                        EventTracker.SendEvent(new VampireLifetimeEvent(EventTypeId.kVampireLifetimeEvent, actor.SimDescription, false, target.SimDescription.SimDescriptionId));
                    }
                }

                return DelayedKill(actor, target, deathType);
            }
        }

        protected static bool DelayedKill(Sim actor, Sim target, SimDescription.DeathType deathType)
        {
            if (actor == target)
            {
                target.SimDescription.SetDeathStyle(deathType, true);

                Urnstone urnstone = Urnstone.CreateGrave(target.SimDescription, false, true);
                if (urnstone != null)
                {
                    if (!target.Inventory.TryToAdd(urnstone, false))
                    {
                        urnstone.Destroy();
                        return false;
                    }

                    urnstone.GhostSetup(target, true);
                }
            }
            else
            {
                List<IRabbitHolePartnershipDeed> list = Inventories.QuickDuoFind<IRabbitHolePartnershipDeed,GameObject>(target.Inventory);
                if ((list != null) && (list.Count > 0x0))
                {
                    Sim sim = null;
                    float minValue = float.MinValue;
                    foreach (Sim sim2 in Households.AllHumans(target.Household))
                    {
                        if (sim2 != target)
                        {
                            float liking = -100f;
                            Relationship relationship = Relationship.Get(target, sim2, false);
                            if (relationship != null)
                            {
                                liking = relationship.LTR.Liking;
                            }
                            if (liking > minValue)
                            {
                                minValue = liking;
                                sim = sim2;
                            }
                        }
                    }
                    foreach (IRabbitHolePartnershipDeed deed in list)
                    {
                        target.Inventory.RemoveByForce(deed);
                        if (sim != null)
                        {
                            sim.Inventory.TryToAdd(deed, false);
                        }
                        else
                        {
                            deed.Destroy();
                        }
                    }
                }

                InteractionInstance entry = Urnstone.KillSim.Singleton.CreateInstance(target, target, new InteractionPriority(InteractionPriorityLevel.MaxDeath, 0f), false, false);
                (entry as Urnstone.KillSim).simDeathType = deathType;
                target.InteractionQueue.Add(entry);
            }

            return true;
        }

        public override bool Run()
        {
            try
            {
                Definition definition = InteractionDefinition as Definition;
                if (definition.IsMassDeath)
                {
                    List<Sim> sims = new List<Sim>(Target.LotCurrent.GetObjects<Sim>());

                    foreach (Sim sim in sims)
                    {
                        if (sim == Actor) continue;

                        GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                        if (!Assassination.CanBeKilled(sim, ref greyedOutTooltipCallback)) continue;

                        Kill instance = (new Definition(false, false, definition.DeathType)).CreateInstance(sim, Actor, new InteractionPriority(InteractionPriorityLevel.MaxDeath), false, false) as Kill;

                        Actor.InteractionQueue.Add(instance);
                    }
                }
                else
                {
                    SimDescription target = Target.SimDescription;

                    if (PrivateKill(Actor, Target, definition.DeathType))
                    {
                        Assassination skill = Assassination.EnsureSkill(Actor);
                        if (skill != null)
                        {
                            skill.AddPotentialKill(target, false);
                        }

                        if ((Autonomous) && (Assassination.Settings.mShowAutonomousNotice))
                        {
                            Common.Notify(Actor, Common.Localize("AssassinationKill:Notice", Actor.IsFemale, Target.IsFemale, new object[] { Actor, Target }));
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public class MeteorControl : Common.AlarmTask
        {
            Sim mActor;

            Sim mTarget;

            Sims3.Gameplay.Objects.Miscellaneous.Meteor mMeteor;

            public MeteorControl(Sim actor, Sim target)
                : base(15, TimeUnit.Minutes)
            {
                mActor = actor;
                mTarget = target;

                Vector3 empty = Vector3.Empty;
                Vector3 fwd = Vector3.Empty;
                Jig jig = GlobalFunctions.CreateObject("MeteorImpactJig", ProductVersion.EP2, Vector3.OutOfWorld, 0x0, Vector3.UnitZ, "Sims3.Gameplay.Core.Jig", null) as Jig;

                FindGoodLocationBooleans booleans = FindGoodLocationBooleans.AllowOnBridges | FindGoodLocationBooleans.AllowOnSideWalks | FindGoodLocationBooleans.AllowOnStreets | FindGoodLocationBooleans.TemporaryPlacement;
                World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(target.PositionOnFloor);
                fglParams.BooleanConstraints = booleans;
                fglParams.PlacementCheckFunc = Sims3.Gameplay.Objects.Miscellaneous.Meteor.IsValidMeteorPosition;
                if (GlobalFunctions.FindGoodLocation(jig, fglParams, out empty, out fwd))
                {
                    mMeteor = GlobalFunctions.CreateObject("MeteorImpactShadow", ProductVersion.EP2, empty, 0x0, fwd, "Sims3.Gameplay.Objects.Miscellaneous.Meteor", null) as Sims3.Gameplay.Objects.Miscellaneous.Meteor;
                    mMeteor.SetHiddenFlags(HiddenFlags.Footprint);
                }

                jig.Destroy();

                if ((actor != null) && (mMeteor != null))
                {
                    mTarget.InteractionQueue.AddNext(Sims3.Gameplay.Objects.Miscellaneous.Meteor.ReactToMeteor.Singleton.CreateInstance(mMeteor, mTarget, mTarget.InheritedPriority(), false, true));

                    mActor.InteractionQueue.CancelAllInteractions();

                    Vector3 position = actor.Position;
                    Vector3 forwardVector = actor.ForwardVector;
                    if (GlobalFunctions.FindGoodLocationNearby(actor, ref position, ref forwardVector, Sims3.Gameplay.Objects.Miscellaneous.Meteor.kMeteorBlastZoneRadius, GlobalFunctions.FindGoodLocationStrategies.All, FindGoodLocationBooleans.None))
                    {
                        InteractionInstance interaction = Terrain.GoHere.GetSingleton(actor, position).CreateInstanceWithCallbacks(Terrain.Singleton, actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), true, true, null, null, OnGoHereFailure);

                        interaction.RequestWalkStyle(Sim.WalkStyle.Run);

                        actor.InteractionQueue.AddNext(interaction);
                    }
                    else
                    {
                        mActor.InteractionQueue.AddNext(Sims3.Gameplay.Objects.Miscellaneous.Meteor.ReactToMeteor.Singleton.CreateInstance(mMeteor, mActor, mActor.InheritedPriority(), false, true));
                    }
                }
            }

            protected void OnGoHereFailure(Sim actor, float x)
            {
                try
                {
                    Vector3 position = actor.Position;
                    Vector3 forwardVector = actor.ForwardVector;
                    if (GlobalFunctions.FindGoodLocationNearby(actor, ref position, ref forwardVector, Sims3.Gameplay.Objects.Miscellaneous.Meteor.kMeteorBlastZoneRadius, GlobalFunctions.FindGoodLocationStrategies.All, FindGoodLocationBooleans.None))
                    {
                        actor.SetPosition(position);
                        actor.SetForward(forwardVector);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(actor, e);
                }
            }

            protected override void OnPerform()
            {
                try
                {
                    if (mMeteor != null)
                    {
                        DelayedKill(mActor, mTarget, SimDescription.DeathType.Meteor);

                        VisualEffect effect = VisualEffect.Create("ep2meteorShower");
                        effect.SetPosAndOrient(mMeteor.Position, mMeteor.ForwardVector, mMeteor.UpVector);
                        effect.SubmitOneShotEffect(VisualEffect.TransitionType.SoftTransition);

                        mMeteor.MeteorCollisionCallback();
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mMeteor, e);
                }
            }
        }

        public class EraseKillTask : Common.AlarmTask
        {
            Sim mActor;

            SimDescription mTarget;

            public EraseKillTask(Sim actor, SimDescription target)
                : base(1, TimeUnit.Minutes)
            {
                mActor = actor;
                mTarget = target;

                if (mTarget.CreatedSim != null)
                {
                    mTarget.CreatedSim.InteractionQueue.CancelAllInteractions();
                }
            }

            public class GnomeData
            {
                public readonly ulong mInstance;

                public readonly ProductVersion mVersion;

                public GnomeData(ulong instance, ProductVersion version)
                {
                    mInstance = instance;
                    mVersion = version;
                }
            }

            protected override void OnPerform()
            {
                int roomId = 0;
                Lot lot = null;

                if (mTarget.CreatedSim != null)
                {
                    Vector3 location = mTarget.CreatedSim.PositionOnFloor;

                    lot = mTarget.CreatedSim.LotCurrent;
                    roomId = mTarget.CreatedSim.RoomId;

                    List<GnomeData> gnomes = new List<GnomeData>();

                    if (mTarget.TraitManager.HasElement(TraitNames.CultureChina))
                    {
                        gnomes.Add(new GnomeData (0x000000000098977D, ProductVersion.EP1));
                    }
                    else if ((mTarget.TraitManager.HasElement(TraitNames.CultureEgypt)) || (mTarget.IsMummy))
                    {
                        gnomes.Add(new GnomeData (0x000000000098977C, ProductVersion.EP1));
                    }
                    else if (mTarget.TraitManager.HasElement(TraitNames.CultureFrance))
                    {
                        gnomes.Add(new GnomeData (0x000000000098977E, ProductVersion.EP1));
                    }
                    else if (mTarget.IsVampire)
                    {
                        gnomes.Add(new GnomeData (0x000000000098A1C2, ProductVersion.EP3));
                    }
                    else if (mTarget.IsFrankenstein)
                    {
                        gnomes.Add(new GnomeData (0x0000000000989CD9, ProductVersion.EP2));
                    }
                    else if ((mTarget.IsCelebrity) || (mTarget.AssignedRole is RolePaparazzi))
                    {
                        gnomes.Add(new GnomeData(0x000000000098A1C4, ProductVersion.EP3));
                    }
                    else if (mTarget.Occupation is MagicianCareer)
                    {
                        gnomes.Add(new GnomeData(0x000000000098D215, ProductVersion.EP6)); // Magician
                    }
                    else if (mTarget.Occupation is SingerCareer)
                    {
                        gnomes.Add(new GnomeData(0x000000000098D214, ProductVersion.EP6)); // Singer
                    }
                    else if (mTarget.IsCat)
                    {
                        gnomes.Add(new GnomeData(0x000000000098AAD6, ProductVersion.EP5)); // Pet Cat
                    }
                    else if (mTarget.IsADogSpecies)
                    {
                        gnomes.Add(new GnomeData(0x000000000098AAD5, ProductVersion.EP5)); // Pet Dog
                    }
                    else if (mTarget.IsHorse)
                    {
                        gnomes.Add(new GnomeData(0x000000000098AAD4, ProductVersion.EP5)); // Pet Horse
                    }
                    else
                    {
                        gnomes.Add(new GnomeData(0x000000000000058B, ProductVersion.BaseGame)); // Normal

                        if (GameUtils.IsInstalled(ProductVersion.EP1))
                        {
                            gnomes.Add(new GnomeData(0x000000000098977D, ProductVersion.EP1)); // China
                            gnomes.Add(new GnomeData(0x000000000098977C, ProductVersion.EP1)); // Egypt
                            gnomes.Add(new GnomeData(0x000000000098977E, ProductVersion.EP1)); // France
                        }

                        if (GameUtils.IsInstalled(ProductVersion.EP2))
                        {
                            gnomes.Add(new GnomeData(0x0000000000989EEC, ProductVersion.EP2)); // Caveman
                            gnomes.Add(new GnomeData(0x0000000000989CD9, ProductVersion.EP2)); // Inventor
                            gnomes.Add(new GnomeData(0x0000000000989EDF, ProductVersion.EP2)); // Laundry
                            gnomes.Add(new GnomeData(0x0000000000989CF2, ProductVersion.EP2)); // Sculptor
                        }

                        if (GameUtils.IsInstalled(ProductVersion.EP3))
                        {
                            gnomes.Add(new GnomeData(0x000000000098A1C4, ProductVersion.EP3)); // Celebrity
                            gnomes.Add(new GnomeData(0x000000000098A1C2, ProductVersion.EP3)); // Vampire
                        }

                        if (GameUtils.IsInstalled(ProductVersion.EP5))
                        {
                            gnomes.Add(new GnomeData(0x000000000098AEB1, ProductVersion.EP5)); // Freezer Bunny
                            gnomes.Add(new GnomeData(0x000000000098AAD6, ProductVersion.EP5)); // Pet Cat
                            gnomes.Add(new GnomeData(0x000000000098AAD5, ProductVersion.EP5)); // Pet Dog
                            gnomes.Add(new GnomeData(0x000000000098AAD4, ProductVersion.EP5)); // Pet Horse
                        }

                        if (GameUtils.IsInstalled(ProductVersion.EP6))
                        {
                            gnomes.Add(new GnomeData(0x000000000098D215, ProductVersion.EP6)); // Magician
                            gnomes.Add(new GnomeData(0x000000000098D214, ProductVersion.EP6)); // Singer
                        }
                    }

                    if (gnomes.Count > 0)
                    {
                        GnomeData preferred = RandomUtil.GetRandomObjectFromList(gnomes);

                        MagicGnomeBase gnome = ObjectCreation.CreateObject(preferred.mInstance, preferred.mVersion, null) as MagicGnomeBase;
                        if (gnome != null)
                        {
                            NameComponent name = gnome.GetComponent<NameComponent>();
                            if (name != null)
                            {
                                name.mName = mTarget.FullName;
                            }

                            gnome.SetPosition(location);
                            gnome.AddToWorld();
                        }
                    }
                }

                Genealogy genealogy = mTarget.CASGenealogy as Genealogy;
                if (genealogy != null)
                {
                    genealogy.ClearAllGenealogyInformation();
                }

                foreach (SimDescription other in SimDescription.GetSimDescriptionsInWorld())
                {
                    MiniSimDescription miniOther = MiniSimDescription.Find(other.SimDescriptionId);
                    if (miniOther == null) continue;

                    miniOther.RemoveMiniRelatioship(mTarget.SimDescriptionId);
                }

                Annihilation.RemoveMSD(mTarget.SimDescriptionId);

                Relationship.RemoveSimDescriptionRelationships(mTarget);

                Urnstone urnstone = Urnstone.FindGhostsGrave(mTarget);

                if (urnstone != null)
                {
                    if ((urnstone.InInventory) && (urnstone.Parent != null) && (urnstone.Parent.Inventory != null))
                    {
                        urnstone.Parent.Inventory.RemoveByForce(urnstone);
                    }

                    urnstone.DestroyGrave();

                    try
                    {
                        urnstone.Dispose();
                    }
                    catch
                    { }
                }

                try
                {
                    mTarget.Dispose();

                    Assassination skill = Assassination.EnsureSkill(mActor);
                    if (skill != null)
                    {
                        skill.AddPotentialKill(mTarget, true);

                        bool witnessed = false;

                        if (lot != null)
                        {
                            witnessed = Assassination.WasWitnessed(lot, roomId, mActor.SimDescription, mTarget, new List<Sim>());
                        }

                        skill.AddActualKill(mTarget, false, witnessed);
                    }
                }
                catch
                { }
            }
        }

        public class Definition : InteractionDefinition<Sim, Sim, Kill>
        {
            bool mMassDeath;

            bool mDirect;

            SimDescription.DeathType mType;

            public Definition(bool massDeath, bool direct)
            {
                mMassDeath = massDeath;
                mDirect = direct;
            }
            public Definition(bool massDeath, bool direct, SimDescription.DeathType type)
                : this(massDeath, direct)
            {
                mType = type;
            }

            public bool IsMassDeath
            {
                get { return mMassDeath; }
            }

            public SimDescription.DeathType DeathType
            {
                get { return mType; }
            }

            public override string[] GetPath(bool isFemale)
            {
                if (mMassDeath)
                {
                    return new string[] { Common.Localize("MassKill:RootName") };
                }
                else
                {
                    return new string[] { Common.Localize("Kill:RootName") };
                }
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                foreach (SimDescription.DeathType type in Assassination.Types.Keys)
                {
                    results.Add(new InteractionObjectPair(new Definition(mMassDeath, mDirect, type), target));
                }
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return LocalizeString(actor.IsFemale, "Assassin" + mType, new object[0]);
            }
            /*
            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                InteractionTestResult result = base.Test(ref parameters, ref greyedOutTooltipCallback);

                Common.Notify(result.ToString());

                return result;
            }
            */
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((mDirect) || (mMassDeath))
                {
                    if (isAutonomous) return false;
                }

                return Assassination.Allow(actor, target, mType, isAutonomous, mDirect, mMassDeath, ref greyedOutTooltipCallback);
            }
        }
    }
}
