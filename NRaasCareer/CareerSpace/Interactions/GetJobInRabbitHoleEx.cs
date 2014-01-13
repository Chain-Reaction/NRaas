using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class GetJobInRabbitHole : Sims3.Gameplay.Careers.GetJobInRabbitHole, Common.IPreLoad, Common.IAddInteraction
    {
        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<RabbitHole, Sims3.Gameplay.Careers.GetJobInRabbitHole.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomCareerInjector());
            interactions.AddCustom(new CustomSchoolInjector());
        }

        public override bool InRabbitHole()
        {
            try
            {
                Definition definition = InteractionDefinition as Definition;

                CareerLocation careerLocation = definition.mLocation;
                if (careerLocation == null)
                {
                    return false;
                }

                TryDisablingCameraFollow(Actor);

                return SchoolBooter.Enroll(Actor, definition.mName.Second, careerLocation);
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

        public new class Definition : InteractionDefinition<Sim, RabbitHole, GetJobInRabbitHole>
        {
            public readonly CareerLocation mLocation;

            public readonly Pair<string, string> mName;

            public Definition()
            { }
            public Definition(string s, CareerLocation location)
            {
                mName = new Pair<string,string>(s,s);
                mLocation = location;
            }
            public Definition(Pair<string, string> s, CareerLocation location)
            {
                mName = s;
                mLocation = location;
            }

            public override string[] GetPath(bool isFemale)
            {
                if ((mLocation != null) && (mLocation.Career is NRaas.Gameplay.Careers.Unemployed))
                {
                    return new string[] { GetJobInRabbitHole.LocalizeString(isFemale, "JoinCareer" + mLocation.Career.Guid.ToString(), new object[] { mLocation.Career.Name }) };
                }
                else
                {
                    return base.GetPath(isFemale);
                }
            }

            public override string GetInteractionName(Sim a, RabbitHole target, InteractionObjectPair interaction)
            {
                return mName.First;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, RabbitHole target, List<InteractionObjectPair> results)
            {
                try
                {
                    if ((actor != null) && (actor.CareerManager != null) && (target.CareerLocations != null))
                    {
                        foreach (CareerLocation location in target.CareerLocations.Values)
                        {
                            if (location.Career == null) continue;

                            if (location.Career is School)
                            {
                                School school = actor.CareerManager.School;
                                if ((school == null) ||
                                    ((school.CareerLoc != location) && (school.Guid != location.Career.Guid)))
                                {
                                    string name = GetJobInRabbitHole.LocalizeString(actor.IsFemale, "JoinCareer" + location.Career.Guid, new object[] { location.Career.Name });
                                    if (string.IsNullOrEmpty(name))
                                    {
                                        name = "JoinCareer" + location.Career.Guid;
                                    }

                                    results.Add(new InteractionObjectPair(new Definition(name, location), target));
                                }
                            }
                            else
                            {
                                if ((actor.Occupation == null) || 
                                    ((actor.Occupation.CareerLoc != location) && (actor.Occupation.Guid != location.Career.Guid)))
                                {
                                    if (location.Career is NRaas.Gameplay.Careers.Unemployed)
                                    {
                                        NRaas.Gameplay.Careers.Unemployed unemployed = location.Career as NRaas.Gameplay.Careers.Unemployed;

                                        foreach (Pair<string, string> name in unemployed.GetLocalizedTitles(actor.IsFemale))
                                        {
                                            results.Add(new InteractionObjectPair(new Definition(name, location), target));
                                        }
                                    }
                                    else
                                    {
                                        string name = GetJobInRabbitHole.LocalizeString(actor.IsFemale, "JoinCareer" + location.Career.Guid, new object[] { location.Career.Name });
                                        if (string.IsNullOrEmpty(name))
                                        {
                                            name = "JoinCareer" + location.Career.Guid;
                                        }

                                        results.Add(new InteractionObjectPair(new Definition(name, location), target));
                                    }
                                    continue;
                                }
                                else if ((actor.Occupation.CareerLoc != location) && (actor.Occupation.Guid == location.Career.Guid))
                                {
                                    results.Add(new InteractionObjectPair(new Definition(GetJobInRabbitHole.LocalizeString(actor.IsFemale, "TransferJob", new object[0x0]), location), target));
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(actor, target, e);
                }
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a.SimDescription.IsEnrolledInBoardingSchool())
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Boarding School");
                        return false;
                    }

                    if (GameUtils.GetCurrentWorldType() == WorldType.Vacation)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("On Vacation");
                        return false;
                    }

                    if (a.SimDescription.ToddlerOrBelow)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("ToddlerOrBelow");
                        return false;
                    }

                    if (mLocation == null)
                    {
                        return true;
                    }
                    else if (mLocation.Career is SchoolElementary)
                    {
                        if (!a.SimDescription.Child)
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Elementary Not Child");
                            return false;
                        }
                    }
                    else if (mLocation.Career is SchoolHigh)
                    {
                        if (!a.SimDescription.Teen)
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("High Not Teen");
                            return false;
                        }
                    }
                    else if (mLocation.Career is School)
                    {
                        if ((!a.SimDescription.Child) && (!a.SimDescription.Teen))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("School Not Child Or Teen");
                            return false;
                        }
                    }
                    else if (a.School != null)
                    {
                        if (a.SimDescription.Teen && AfterschoolActivity.DoesJobConflictWithActivities(a, mLocation.Career))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("DoesJobConflictWithActivities");
                            return false;
                        }
                    }

                    if (!mLocation.Career.CareerAgeTest(a.SimDescription))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("CareerAgeTest");
                        return false;
                    }

                    Career occupationAsCareer = a.OccupationAsCareer;
                    if ((occupationAsCareer != null) && (occupationAsCareer.Guid == mLocation.Career.Guid))
                    {
                        if (occupationAsCareer.CareerLoc == mLocation)
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Already Working There");
                            return false;
                        }
                    }
                    else if (!mLocation.Career.CanAcceptCareer(a.ObjectId, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }

        public class CustomCareerInjector : Common.InteractionReplacer<RabbitHole, Sims3.Gameplay.Careers.GetJobInRabbitHole.Definition>
        {
            public CustomCareerInjector()
                : base(Singleton, true)
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                if (obj is ComboRabbitHole) return false;

                return base.Perform(obj, definition, existing);
            }
        }

        public class CustomSchoolInjector : Common.InteractionInjector<RabbitHole>
        {
            public CustomSchoolInjector()
                : base(Sims3.Gameplay.Careers.GoToSchoolInRabbitHole.Singleton)
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                if (obj is ComboRabbitHole) return false;

                if (obj is SchoolRabbitHole) return false;

                return base.Perform(obj, definition, existing);
            }
        }
    }
}
