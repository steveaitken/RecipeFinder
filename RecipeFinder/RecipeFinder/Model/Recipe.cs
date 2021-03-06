﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YummyProvider;
using RecipeFinder.ComponentModel;
using System.Text;
using System.Reflection;
using Microsoft.Bot.Connector;
#pragma warning disable 649

namespace RecipeFinder.Model
{
    //for now using LUIS to identify the ingredients as search terms "q"
    public enum AllowedIngredient { }

    public enum ExcludedIngredient { }

    public enum AllowedDietaryRestriction
    {
        None,
        [Terms(new string[] { "dairy free", "dairy-free", "dairy - free" })]
        DairyFree,
        [Terms(new string[] { "egg free", "egg-free", "egg - free" })]
        EggFree,
        [Terms(new string[] { "gluten free", "gluten-free", "gluten - free" })]
        GlutenFree,
        [Terms(new string[] { "peanut free", "peanut-free", "peanut - free" })]
        PeanutFree,
        [Terms(new string[] { "seafood free", "seafood-free", "seafood - free" })]
        SeadfoodFree,
        [Terms(new string[] { "sesame free", "sesame-free", "sesame - free" })]
        SesameFree,
        [Terms(new string[] { "soy free", "soy-free", "soy - free" })]
        SoyFree,
        [Terms(new string[] { "sulfite free", "sulfite-free", "sulfite - free" })]
        SulfiteFree,
        [Terms(new string[] { "tree nut free", "tree nut-free", "tree nut - free" })]
        TreeNutFree,
        [Terms(new string[] { "wheat free", "wheat-free", "wheat - free" })]
        WheatFree
    }

    public enum AllowedDiet
    {
        None,
        [Terms(new string[] { "lacto vegetarian" })]
        LactoVegetarian,
        [Terms(new string[] { "ovo vegetarian" })]
        OvoVegetarian,
        [Terms(new string[] { "pescetarian" })]
        Pescetarian,
        [Terms(new string[] { "vegan" })]
        Vegan,
        [Terms(new string[] { "lacto-ovo vegetarian", "lacto ovo vegetarian", "lacto ovo" })]
        LactoOvoVegetarian,
        [Terms(new string[] { "paleo" })]
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
        static YummlyProvider yp;

        public Recipe(List<string> searchTerms)
        {
            yp = yp ?? new YummlyProvider();
            this.SearchTerms = searchTerms;
        }
        
        [Optional]
        [Prompt("Are you interested in a specific diet? {||}")]
        [Template(TemplateUsage.NotUnderstood, "What does \"{0}\" mean???")]
        [Describe("Allowed diets")]
        [Template(TemplateUsage.NoPreference, "None")]
        public AllowedDiet Diet;

        [Optional]
        [Prompt("Do you have any allergy? {||}")]
        [Template(TemplateUsage.NotUnderstood, "What does \"{0}\" mean???")]
        [Describe("Dietary Restriction")]
        [Template(TemplateUsage.NoPreference, "None")]
        public AllowedDietaryRestriction DietaryRestriction;

        [Prompt("What do you want to search?")]
        private List<string> SearchTerms;

        public static IForm<Recipe> BuildForm()
        {
            OnCompletionAsyncDelegate<Recipe> processOrder = async (context, state) =>
            {
                List<YummyRequestCondition> conditions = new List<YummyRequestCondition>();
                conditions.Add(new YummyRequestCondition(SearchParameterType.Search, string.Join(" ", state.SearchTerms)));


                if (state.Diet != AllowedDiet.None)
                {
                    object[] attributes = typeof(AllowedDiet).GetMember(state.Diet.ToString())[0].GetCustomAttributes(typeof(TermsAttribute), false);
                    if (attributes != null)
                        conditions.Add(new YummyRequestCondition(SearchParameterType.AllowedDiet, ((TermsAttribute)attributes[0]).Alternatives[0]));
                }

                if (state.DietaryRestriction != AllowedDietaryRestriction.None)
                {
                    object[] attributes = typeof(AllowedDietaryRestriction).GetMember(state.DietaryRestriction.ToString())[0].GetCustomAttributes(typeof(TermsAttribute), false);
                    if (attributes != null)
                        conditions.Add(new YummyRequestCondition(SearchParameterType.AllowedAllergy, ((TermsAttribute)attributes[0]).Alternatives[1]));
                }

                try
                {
                    YummyRecipesResponse yrs = await yp.GetRecipes(new YummyRequest(conditions.ToArray()));
                    if (yrs.matches.Any())
                    {
                        var message = context.MakeMessage();
                        message.Attachments = new List<Attachment>();
                        var actions = new List<Microsoft.Bot.Connector.Action>();

                        foreach (var recipeResult in yrs.matches)
                        {
                       
                                YummyRecipeResponse yr = await yp.GetRecipe(recipeResult.id);

                                var sbIngredients = new StringBuilder();
                                foreach (var ingredient in yr.ingredientLines)
                                {
                                    sbIngredients.AppendFormat("- {0}", ingredient);
                                    sbIngredients.AppendLine();
                                }

                                actions.Add(new Microsoft.Bot.Connector.Action
                                {
                                    Image = yr.images[0].hostedMediumUrl,
                                    Url = yr.source.sourceRecipeUrl,
                                    Title = string.Format("**{0}**", yr.name),
                                    Message = sbIngredients.ToString()
                                    //Message = string.Format("![recipe]({0})",yr.images[0].hostedMediumUrl)
                                });
                        }

                        message.Attachments.Add(new Attachment
                        {
                            Title = "Results:",
                            Actions = actions
                        });

                        await context.PostAsync(message);
                    }
                    else
                    {
                        await context.PostAsync("Nothing found");
                    }
                }
                catch (Exception)
                {
                    await context.PostAsync("An error occurred while searching for a recipe. Please try it again later. ");
                    //todo: add log
                }
            };

            return new FormBuilder<Recipe>()
                        .Message("Welcome to the search recipe bot!")
                        .Field(nameof(Diet))
                        .Field(nameof(DietaryRestriction))
                        .AddRemainingFields()
                        //.Message("Searching recipes using filters for diet {Diet} and allergies {Allergies}.")
                        //.Confirm("Do you want to order your {Length} {Sandwich} on {Bread} {&Bread} with {[{Cheese} {Toppings} {Sauces}]} to be sent to {DeliveryAddress} {?at {DeliveryTime:t}}?")
                        .AddRemainingFields()
                        .Message("Searching your recipe...")
                        .OnCompletionAsync(processOrder)
                        .Build();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Dietary Restirctions: {0} ", string.Join(",", DietaryRestriction));
            builder.AppendLine();
            builder.AppendFormat("Diet: {0} ", Diet.ToString());
            builder.AppendLine();
            builder.AppendFormat("Search terms: {0} ", string.Join(" ", SearchTerms));
            return builder.ToString();
        }
    };
}
