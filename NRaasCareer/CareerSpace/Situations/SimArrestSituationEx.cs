using NRaas.CareerSpace.Skills;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Situations
{
    public class SimArrestSituationEx : SimArrestSituation
    {
        // Methods
        private SimArrestSituationEx()
        { }

        private SimArrestSituationEx(Lot lot, Sim simToArrest, Sim officer) 
        {
            // Situation
            mChecks = new Hashtable();
            LotChecks = new Hashtable();
            mDesires = new ArrayList();
            mInteractions = new List<InteractionInstance>();
            mSimsWithInteractions = new Dictionary<Sim, bool>();
            mForcedInteractions = new List<Pair<Sim, ulong>>();
            sAllSituations.Add(this);

            // RootSituation
            mLot = lot;
            mLotId = (lot != null) ? lot.LotId : ((ulong)0x0L);

            SimToArrest = simToArrest;

            Cop = officer;
            if (Cop == null)
            {
                SetState(new CreateCopAndRoute(this));
            }
            else
            {
                SetState(new MakeArrest(this, 0));
            }
        }

        public override void Dispose()
        {
            if (CopCar != null)
            {
                List<Sim> actors = new List<Sim>(CopCar.ActorsUsingMe);
                foreach (Sim actor in actors)
                {
                    ResetSimTask.Perform(actor, true);
                }
            }

            base.Dispose();
        }

        public static SimArrestSituationEx Create(Sim simToArrest)
        {
            if (simToArrest == null) return null;

            if (Assassination.StaticGuid == SkillNames.None) return null;

            if (!Assassination.Settings.mAllowArrest) return null;

            if (Sims3.Gameplay.Queries.CountObjects<PoliceStation>() == 0) return null;

            foreach (Situation situation in Situation.sAllSituations)
            {
                if (situation is SimArrestSituationEx)
                {
                    if (situation.Lot == simToArrest.LotCurrent) return null;
                }
            }

            Sim officer = null;
            foreach (Sim sim in simToArrest.LotCurrent.GetSims())
            {
                if (sim.Service is Police)
                {
                    officer = sim;
                    break;
                }
            }

            return new SimArrestSituationEx(simToArrest.LotCurrent, simToArrest, officer);
        }

        // Nested Types
        private new class CreateCopAndRoute : ChildSituation<SimArrestSituationEx>
        {
            // Methods
            protected CreateCopAndRoute()
            { }
            public CreateCopAndRoute(SimArrestSituationEx parent) 
                : base(parent)
            { }

            public override void Init(SimArrestSituationEx parent)
            {
                IPoliceStation[] objects = Sims3.Gameplay.Queries.GetObjects<IPoliceStation>();
                if (objects.Length == 0x0)
                {
                    Exit();
                }
                else
                {
                    IPoliceStation randomObjectFromList = RandomUtil.GetRandomObjectFromList<IPoliceStation>(objects);
                    parent.Station = randomObjectFromList;
                }

                SimDescription description = CreateNewCopFromPool();
                if (description == null)
                {
                    Exit();
                }
                else
                {
                    if (description.CreatedSim != null)
                    {
                        parent.Cop = description.CreatedSim;
                        SwitchOutfits.SwitchNoSpin(parent.Cop, new CASParts.Key(OutfitCategories.Career, 0));
                    }
                    else
                    {
                        parent.Cop = Instantiation.PerformOffLot (description, parent.Lot, description.GetOutfit(OutfitCategories.Career, 0x0), null);
                    }
                    parent.CopCar = CarNpcManager.Singleton.CreateServiceCar(ServiceType.Police);
                    ForceSituationSpecificInteraction(parent.Cop, parent.Cop, new GoToCulpritsLocation.Definition(Parent), null, OnCompletion, OnFailure, new InteractionPriority(InteractionPriorityLevel.High));
                }
            }

            private void OnCompletion(Sim actor, float x)
            {
                Parent.SetState(new MakeArrest(Parent, 0));
            }

            private void OnFailure(Sim actor, float x)
            {
                Exit();
            }
        }

        public new class GoToCulpritsLocation : Interaction<Sim, IGameObject>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                Definition interactionDefinition = InteractionDefinition as Definition;
                Sim cop = interactionDefinition.arrestSituation.Cop;
                CarService copCar = interactionDefinition.arrestSituation.CopCar;
                Lot lotCurrent = interactionDefinition.arrestSituation.Lot;
                if ((copCar == null) || !CarNpcManager.Singleton.DriveToLotInServiceCarWithinTime(copCar, cop, lotCurrent, 10f, true))
                {
                    return false;
                }
                copCar.GetOut(cop);
                return true;
            }

            // Nested Types
            [DoesntRequireTuning]
            public sealed class Definition : InteractionDefinition<Sim, IGameObject, GoToCulpritsLocation>
            {
                // Fields
                public SimArrestSituationEx arrestSituation;

                // Methods
                public Definition()
                {
                }

                public Definition(SimArrestSituationEx situation)
                {
                    arrestSituation = situation;
                }

                public override string GetInteractionName(Sim a, IGameObject target, InteractionObjectPair interaction)
                {
                    return "PickUpCulprit";
                }

                public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }

        public new class MakeArrest : ChildSituation<SimArrestSituationEx>
        {
            // Fields
            private AlarmHandle mAlarmHandle;
            private int mNumTries;

            // Methods
            public MakeArrest()
            {
                mAlarmHandle = AlarmHandle.kInvalidHandle;
            }

            public MakeArrest(SimArrestSituationEx parent, int numTries)
                : base(parent)
            {
                mAlarmHandle = AlarmHandle.kInvalidHandle;
                mNumTries = numTries;
            }

            public override void CleanUp()
            {
                base.CleanUp();

                Parent.Cop.RemoveAlarm(mAlarmHandle);
                mAlarmHandle = AlarmHandle.kInvalidHandle;
            }

            private void DoArrest()
            {
                Parent.SetState(new MakeArrest(Parent, mNumTries));
            }

            public override void Init(SimArrestSituationEx parent)
            {
                if (parent.SimToArrest.LotCurrent != parent.Lot)
                {
                    if (parent.SimToArrest.Parent == Parent.CopCar)
                    {
                        OnArrestFinished(parent.Cop, 1);
                    }
                    else
                    {
                        Exit();
                    }
                }

                ActiveTopic.AddToSim(parent.Cop, "Policeman Arrest Burglar", null);

                RequestWalkStyle(parent.Cop, Sim.WalkStyle.Run);

                SituationSocial.Definition i = new SituationSocial.Definition("Policeman Arrest Burglar", new string[0x0], null, false);
                ForceSituationSpecificInteraction(parent.SimToArrest, parent.Cop, i, null, OnArrested, OnArrestFailed);
            }

            private void OnArrested(Sim sim, float x)
            {
                UnrequestWalkStyle(Parent.Cop, Sim.WalkStyle.Run);

                ForceSituationSpecificInteraction(Parent.CopCar, Parent.SimToArrest, new GetInPoliceCar.Definition(Parent), null, OnArrestFinished, OnArrestFailed, new InteractionPriority(InteractionPriorityLevel.High));
            }

            private void OnArrestFinished(Sim sim, float x)
            {
                Parent.SetState(new GetCulpritInCar(Parent));
            }

            private void OnArrestFailed(Sim sim, float x)
            {
                UnrequestWalkStyle(Parent.Cop, Sim.WalkStyle.Run);

                if (++mNumTries <= 0x3)
                {
                    Parent.Cop.RemoveAlarm(mAlarmHandle);
                    mAlarmHandle = Parent.Cop.AddAlarm(1f, TimeUnit.Minutes, DoArrest, "Arrest retry alarm", AlarmType.DeleteOnReset);
                }
                else
                {
                    Parent.SetState(new RideOffIntoTheSunset(Parent));
                }
            }
        }
    }
}
