using Sims3.Metadata;
using System.Collections.Generic;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Fishing;

namespace ani_StoreRestockItem
{
    public enum ItemType
    {
        Buy,
        Fish,
        Ingredient,
        Craftable,
        Herb,
        Metal,
        Gem,
        Nectar,
        AlchemyPotion,
        Bug,
        Food,
        BookGeneral_,
        BookSkill_,
        BookRecipe_,
        SheetMusic_,
        BookToddler_,
        BookFish_,
        BookAlchemyRecipe_,
        AcademicTextBook_,
        BookComic_,
        Flowers,
		JamJar
    }
    [Persistable]
    public class RestockInfo
    {
        public ResourceKey Key;

        public ItemType Type;

        public float Rotation;

        public string Name;

        public int Price;

        //Buy mode
        public string DesignPreset;

        //Ingredient
        // public RIIngredientData IngData;
        public string IngredientKey;
        public IngredientData IngData;

        //Fish
        public FishType FType;

        //Rock and Metal
        public RockGemMetal RockData;

        //Herbs
        public PlantableNonIngredientData PlantData;

        //Nectar 
        public int NectarAge;
        public uint NectarFruitHash;
        public NectarBottle.NectarBottleGlassInfo NectarData;
        public List<NectarMaker.IngredientNameCount> NectarIngredients;

        //Book
        public string BookId;
        public Sims3.Gameplay.Objects.BookData.BookType BookType;

        //Bug
        public InsectType BugType;

        //Food
       // public Recipe FoodRecipe;
       // public Sims3.Gameplay.Objects.FoodObjects.Recipe.MealQuantity FoodQuantity;

        public CookingProcess cookingProcess;

        //Flowers
        public Wildflower.WildflowerType TypeOfWildFlower;

		//JamJar
		public Quality JamQuality;
		public bool JamIsPreserve;

        public RestockInfo()
        {
            Type = ItemType.Buy;
            DesignPreset = string.Empty;
            //enabledStencils = new SortedList<string, bool>();
            //patterns = new SortedList<string, Complate>();
        }
    }
}
