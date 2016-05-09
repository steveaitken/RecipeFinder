using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YummyProvider;
using RecipeFinder.ComponentModel;
using System.Text;
#pragma warning disable 649

namespace RecipeFinder.Model
{
    //for now using LUIS to identify the ingredients as search terms "q"
    public enum AllowedIngredient { }

    public enum ExcludedIngredient { }

    public enum AllowedDietaryRestriction
    {
        None,
        [SearchValue("396^Dairy-Free")]
        [Terms(new string[] { "dairy free", "dairy-free", "dairy - free" })]
        DairyFree,
        [SearchValue("397^Egg-Free")]
        [Terms(new string[] { "egg free", "egg-free", "egg - free" })]
        EggFree,
        [SearchValue("393^Gluten-Free")]
        [Terms(new string[] { "gluten free", "gluten-free", "gluten - free" })]
        GlutenFree,
        [SearchValue("394^Peanut-Free")]
        [Terms(new string[] { "peanut free", "peanut-free", "peanut - free" })]
        PeanutFree,
        [SearchValue("398^Seafood-Free")]
        [Terms(new string[] { "seafood free", "seafood-free", "seafood - free" })]
        SeadfoodFree,
        [SearchValue("399^Sesame-Free")]
        [Terms(new string[] { "sesame free", "sesame-free", "sesame - free" })]
        SesameFree,
        [SearchValue("400^Soy-Free")]
        [Terms(new string[] { "soy free", "soy-free", "soy - free" })]
        SoyFree,
        [SearchValue("401^Sulfite-Free")]
        [Terms(new string[] { "sulfite free", "sulfite-free", , "sulfite - free" })]
        SulfiteFree,
        [SearchValue("395^Tree Nut-Free")]
        [Terms(new string[] { "tree nut free", "tree nut-free", "tree nut - free" })]
        TreeNutFree,
        [SearchValue("392^Wheat-Free")]
        [Terms(new string[] { "wheat free", "wheat-free", "wheat - free" })]
        WheatFree
    }

    public enum AllowedDiet
    {
        None,
        [SearchValue("388^Lacto vegetarian")]
        [Terms(new string[] { "lacto vegetarian" })]
        LactoVegetarian,
        [SearchValue("389^Ovo vegetarian")]
        [Terms(new string[] { "ovo vegetarian" })]
        OvoVegetarian,
        [SearchValue("390^Pescetarian")]
        [Terms(new string[] { "pescetarian" })]
        Pescetarian,
        [SearchValue("386^Vegan")]
        [Terms(new string[] { "vegan" })]
        Vegan,
        [SearchValue("387^Lacto-ovo vegetarian")]
        [Terms(new string[] { "lacto ovo vegetarian", "lacto-ovo vegetarian", "lacto ovo" })]
        LactoOvoVegetarian,
        [SearchValue("403^Paleo")]
        [Terms(new string[] { "paleo", "steve" })]
        Paleo
    }

    public enum AllowedCuisine { }

    public enum ExcludedCuisine { }

    public enum AllowedCourse { }

    public enum ExcludedCourse { }

    public enum AllowedHoliday { }

    public enum ExcludedHoliday { }

    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")]
    public class Recipe
    {
        [Optional]
        [Prompt("Are you interested in a specific diet? {||}")]
        [Template(TemplateUsage.NotUnderstood, "What does \"{0}\" mean???")]
        [Describe("Allowed diets")]
        [Template(TemplateUsage.NoPreference, "None")]
        public AllowedDiet Diet;

        [Optional]
        [Prompt("Do you have any allergy? {||}")]
        [Template(TemplateUsage.NotUnderstood, "What does \"{0}\" mean???")]
        [Describe("Allowed diets")]
        [Template(TemplateUsage.NoPreference, "None")]
        public AllowedDietaryRestriction DietaryRestriction;

        public static IForm<Recipe> BuildForm()
        {
            OnCompletionAsyncDelegate<Recipe> processOrder = async (context, state) =>
            {
                await context.PostAsync("We are currently processing your search. We will message you the results.");
            };

            return new FormBuilder<Recipe>()
                        .Message("Welcome to the search recipe bot!")
                        .Field(nameof(Diet))
                        .Field(nameof(DietaryRestriction))
                        //.Message("Searching recipes using filters for diet {Diet} and allergies {Allergies}.")
                        //.Confirm("Do you want to order your {Length} {Sandwich} on {Bread} {&Bread} with {[{Cheese} {Toppings} {Sauces}]} to be sent to {DeliveryAddress} {?at {DeliveryTime:t}}?")
                        //.AddRemainingFields()
                        .Message("Searching your recipe...")
                        .OnCompletionAsync(processOrder)
                        .Build();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Allergies: {0}", string.Join(",", DietaryRestriction));
            builder.AppendFormat("Diet: {0}", Diet.ToString());
            return builder.ToString();
        }
    };
}
