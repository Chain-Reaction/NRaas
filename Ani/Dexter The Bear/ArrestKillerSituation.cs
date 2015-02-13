using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System.Collections.Generic;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.ActorSystems;
using Sims3.UI;


public class ArrestSuspectSituation : RootSituation
{
    // Fields
    public Lot ArrestLocation;
    public Sim Cop;
    public CarService CopCar;
    public RabbitHole SimsLocation;
    public Sim SimToArrest;
    public IPoliceStation Station;

    // Methods
    private ArrestSuspectSituation()
    {
    }

    private ArrestSuspectSituation(Lot lot, Sim simToArrest)
        : base(lot)
    {
        this.SimToArrest = simToArrest;
        this.ArrestLocation = lot;
        this.SimsLocation = RabbitHole.GetRandomRabbitHoleOfType(RabbitHoleType.PoliceStation);
        base.SetState(new CreateCopAndRoute(this));
    }

    public static ArrestSuspectSituation Create(Lot lot, Sim simToArrest)
    {
        return new ArrestSuspectSituation(lot, simToArrest);
    }


    private SimDescription CreateNewCopFromPool()
    {
        List<SimDescription> randomList = new List<SimDescription>();
        Police instance = Police.Instance;
        if (instance != null)
        {
            foreach (SimDescription description in instance.Pool)
            {
                if (!Police.Instance.IsServiceAssigned(description))
                {
                    randomList.Add(description);
                }
            }
        }
        if (randomList.Count > 0)
        {
            return RandomUtil.GetRandomObjectFromList<SimDescription>(randomList);
        }
        return null;
    }

    public override void Dispose()
    {
        if (this.CopCar != null)
        {
            this.CopCar.Destroy();
            this.CopCar = null;
        }
        base.Dispose();
    }

    public override void OnParticipantDeleted(Sim participant)
    {
        if (participant == this.Cop)
        {
            base.Exit();
        }
    }

    // Nested Types
    private class CreateCopAndRoute : ChildSituation<ArrestSuspectSituation>
    {
        // Methods
        protected CreateCopAndRoute()
        {
        }

        public CreateCopAndRoute(ArrestSuspectSituation parent)
            : base(parent)
        {
        }

        public override void Init(ArrestSuspectSituation parent)
        {
            IPoliceStation objects = (IPoliceStation)RabbitHole.GetRandomRabbitHoleOfType(RabbitHoleType.PoliceStation);

            if (objects == null)
            {
                base.Exit();
            }
            else
            {
                IPoliceStation randomObjectFromList = RandomUtil.GetRandomObjectFromList<IPoliceStation>(objects);
                parent.Station = randomObjectFromList;
            }
            SimDescription description = parent.CreateNewCopFromPool();
            if (description == null)
            {
                base.Exit();
            }
            else
            {
                if (description.CreatedSim != null)
                {
                    parent.Cop = description.CreatedSim;
                    parent.Cop.SwitchToOutfitWithoutSpin(OutfitCategories.Career);
                }
                else
                {
                    SimOutfit outfit = description.GetOutfit(OutfitCategories.Career, 0);
                    parent.Cop = description.Instantiate(Vector3.OutOfWorld, outfit.Key);
                }
                parent.CopCar = CarNpcManager.Singleton.CreateServiceCar(ServiceType.Police);
                base.ForceSituationSpecificInteraction(parent.Cop, parent.Cop, new ArrestSuspectSituation.GoToCulpritsLocation.Definition(base.Parent), null, new Callback(this.OnCompletion), new Callback(this.OnFailure), new InteractionPriority(InteractionPriorityLevel.High));
            }
        }

        private void OnCompletion(Sim actor, float x)
        {
            base.Parent.SetState(new ArrestSuspectSituation.MakeArrest(base.Parent));
        }

        private void OnFailure(Sim actor, float x)
        {
            base.Exit();
        }
    }

    private class GetCulpritInCar : ChildSituation<ArrestSuspectSituation>
    {
        // Methods
        protected GetCulpritInCar()
        {
        }

        public GetCulpritInCar(ArrestSuspectSituation parent)
            : base(parent)
        {
        }

        public override void Init(ArrestSuspectSituation parent)
        {
            Sim simToArrest = parent.SimToArrest;
            Sim cop = parent.Cop;
            CarService copCar = parent.CopCar;
            base.ForceSituationSpecificInteraction(base.Parent.CopCar, base.Parent.Cop, new ArrestSuspectSituation.GetInPoliceCar.Definition(base.Parent), null, new Callback(this.OnCompletion), new Callback(this.OnFailure), new InteractionPriority(InteractionPriorityLevel.High));
        }

        private void OnCompletion(Sim actor, float x)
        {
            base.Parent.SetState(new ArrestSuspectSituation.ThrowInJail(base.Parent));
        }

        private void OnFailure(Sim actor, float x)
        {
            base.Exit();
        }
    }

    public sealed class GetInPoliceCar : Interaction<Sim, IGameObject>
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        // Methods
        public override void ConfigureInteraction()
        {
            base.CancellableByPlayer = false;
        }

        public override bool Run()
        {
            Definition interactionDefinition = base.InteractionDefinition as Definition;
            ArrestSuspectSituation arrestSituation = interactionDefinition.ArrestSituation;
            CarService copCar = arrestSituation.CopCar;
            Sim simToArrest = arrestSituation.SimToArrest;
            if (base.Actor == simToArrest)
            {
                copCar.RouteToAndGetIn(simToArrest, false);
                InteractionInstance instance = PoliceSituation.Wait.Singleton.CreateInstance(simToArrest, simToArrest, new InteractionPriority(InteractionPriorityLevel.High), false, false);
                simToArrest.InteractionQueue.PushAsContinuation(instance, true);
            }
            else
            {
                copCar.RouteToAndGetIn(arrestSituation.Cop, true);
                CarNpcManager.Singleton.DriveToLotInCar(copCar, arrestSituation.Cop, arrestSituation.Station.LotCurrent, false);
            }
            return true;
        }

        // Nested Types
        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, IGameObject, ArrestSuspectSituation.GetInPoliceCar>
        {
            // Fields
            public ArrestSuspectSituation ArrestSituation;

            // Methods
            public Definition()
            {
            }

            public Definition(ArrestSuspectSituation situation)
            {
                this.ArrestSituation = situation;
            }

            public override string GetInteractionName(Sim a, IGameObject target, InteractionObjectPair interaction)
            {
                return Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/RabbitHoles/PoliceStation/GoToJail:InteractionName", new object[0]);
            }

            public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }

    public sealed class GoToCulpritsLocation : Interaction<Sim, IGameObject>
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        // Methods
        public override bool Run()
        {
            Definition interactionDefinition = base.InteractionDefinition as Definition;
            Sim cop = interactionDefinition.arrestSituation.Cop;
            CarService copCar = interactionDefinition.arrestSituation.CopCar;
            Lot lotCurrent = interactionDefinition.arrestSituation.Lot;//.SimsLocation.LotCurrent;
                    
            if ((copCar == null) || !CarNpcManager.Singleton.DriveToLotInServiceCarWithinTime(copCar, cop, lotCurrent, 20f, true))
            {
                return false;
            }
            copCar.GetOut(cop);
            return true;
        }

        // Nested Types
        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, IGameObject, ArrestSuspectSituation.GoToCulpritsLocation>
        {
            // Fields
            public ArrestSuspectSituation arrestSituation;

            // Methods
            public Definition()
            {
            }

            public Definition(ArrestSuspectSituation situation)
            {
                this.arrestSituation = situation;
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

    private class MakeArrest : ChildSituation<ArrestSuspectSituation>
    {
        // Fields
        public ArrestSuspectSituation arrestSituation;

        // Methods
        public MakeArrest()
        {
        }

        public MakeArrest(ArrestSuspectSituation parent)
            : base(parent)
        {
            this.arrestSituation = parent;
        }

        public void ArrestFailure(Sim actor, float x)
        {
            base.UnrequestWalkStyle(actor, Sim.WalkStyle.Run);
            base.Parent.SetState(new ArrestSuspectSituation.RideOffIntoTheSunset(base.Parent));
        }

        public void ArrestFinished(Sim actor, float x)
        {
            InteractionDefinition definition = new ArrestSuspectSituation.GetInPoliceCar.Definition(this.arrestSituation);
            InteractionInstance entry = definition.CreateInstance(this.arrestSituation.CopCar, this.arrestSituation.SimToArrest, new InteractionPriority(InteractionPriorityLevel.High), false, false);
            this.arrestSituation.SimToArrest.InteractionQueue.Add(entry);
            while (((this.arrestSituation.SimToArrest.GetDistanceToObject(this.arrestSituation.CopCar) + 3f) >= this.arrestSituation.Cop.GetDistanceToObject(this.arrestSituation.CopCar)) && !(this.arrestSituation.SimToArrest.Posture is SittingInVehicle))

            {
                Simulator.Sleep(30);
            }
            base.UnrequestWalkStyle(actor, Sim.WalkStyle.Run);
            base.Parent.SetState(new ArrestSuspectSituation.GetCulpritInCar(base.Parent));
        }

        public new void CleanUp()
        {
            base.UnrequestWalkStyle(base.Parent.Cop, Sim.WalkStyle.Run);
            base.CleanUp();
        }

        public override void Init(ArrestSuspectSituation parent)
        {
            if (base.Parent.SimToArrest != null)
            {
                SituationSocial.Definition i = new SituationSocial.Definition("Policeman Express Disappointment", new string[0], null, false);
                InteractionInstance instance = base.ForceSituationSpecificInteraction(parent.SimToArrest, parent.Cop, i, null, new Callback(this.ArrestFinished), new Callback(this.ArrestFailure), InteractionPriorityLevel.High);
                if (instance != null)
                {
                    instance.CancellableByPlayer = false;
                }
            }
        }


    }

    private class RideOffIntoTheSunset : ChildSituation<ArrestSuspectSituation>
    {
        // Methods
        protected RideOffIntoTheSunset()
        {
        }

        public RideOffIntoTheSunset(ArrestSuspectSituation parent)
            : base(parent)
        {
        }

        private void ExitSituation(Sim actor, float x)
        {
            base.Exit();
        }

        public override void Init(ArrestSuspectSituation parent)
        {
            base.ForceSituationSpecificInteraction(base.Parent.Lot, base.Parent.Cop, new DriveAwayInServiceCar.Definition(base.Parent.CopCar), null, new Callback(this.ExitSituation), new Callback(this.ExitSituation));
        }
    }

    private class ThrowInJail : ChildSituation<ArrestSuspectSituation>
    {
        // Methods
        protected ThrowInJail()
        {
        }

        public ThrowInJail(ArrestSuspectSituation parent)
            : base(parent)
        {
        }

        public override void Init(ArrestSuspectSituation parent)
        {
            Sim simToArrest = parent.SimToArrest;
            Sim cop = parent.Cop;
            CarService copCar = parent.CopCar;
            IPoliceStation target = parent.Station;
            InteractionInstance entry = target.GetGoToJailDefinition().CreateInstance(target, simToArrest, new InteractionPriority(InteractionPriorityLevel.High), false, false);
            simToArrest.AddExitReason(ExitReason.CanceledByScript);
            simToArrest.InteractionQueue.AddNext(entry);
            copCar.GetOut(cop);
            InteractionDefinition i = new RabbitHole.ArrestSimInRabbitHole.Definition();//.Definition(SimArrestSituation.Create(parent.Lot, parent.SimToArrest, parent.SimsLocation), false);

           // InteractionDefinition i = new RabbitHole.ArrestSimInRabbitHole.Definition(parent, false);
            base.ForceSituationSpecificInteraction(target, cop, i, null, new Callback(this.OnCompletion), new Callback(this.OnFailure), new InteractionPriority(InteractionPriorityLevel.High));
        }

        private void OnCompletion(Sim actor, float x)
        {
            base.Parent.SetState(new ArrestSuspectSituation.RideOffIntoTheSunset(base.Parent));
        }

        private void OnFailure(Sim actor, float x)
        {
            base.Parent.SetState(new ArrestSuspectSituation.RideOffIntoTheSunset(base.Parent));
        }
    }
}




