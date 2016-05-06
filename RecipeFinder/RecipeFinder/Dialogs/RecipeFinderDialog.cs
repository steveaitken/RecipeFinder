using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RecipeFinder.Dialogs
{
    [LuisModel("f856bce2-68f1-47ca-a306-9250d54180dc", "63038e9502b5404e8e448e6e25eafb14")]
    [Serializable]
    public class RecipeFinderDialog : LuisDialog<object>
    {
        #region variables

        public const string Entity_Ingredient = "";
        public const string Entity_EndProduct = "";
        public const string Entity_Dietary_Restriction = "";

        #endregion


        #region constructors

        public RecipeFinderDialog(ILuisService service = null)
            : base(service)
        {
        }

        #endregion


        #region public methods

        [LuisIntent("FindRecipeByEndProduct")]
        public async Task FindRecipeByEndProduct(IDialogContext context, LuisResult result)
        {

            // check intent score
            var endProductList = new List<string>();
            var dietaryRestrictionList = new List<string>();
            var dietList = new List<string>();

            // get ingredients
            endProductList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.Ingredient.ToString()).Select(f => f.Entity));

            //not sure if this is correct
            if (endProductList.Count == 0)
                await context.PostAsync("Please specify at least one End Product.");

            // check filters
            dietaryRestrictionList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.DietaryRestriction.ToString()).Select(f => f.Entity));
            dietList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.Diet.ToString()).Select(f => f.Entity));


            // check score

            // get filters

            // if need more info, prompt user for more info

            // make api call?

            // display results based on response

            await context.PostAsync("did not find any recipes");
            context.Wait(MessageReceived);
        }

        [LuisIntent("FindRecipeByIngredients")]
        public async Task FindRecipeByIngredients(IDialogContext context, LuisResult result)
        {
            var ingredientList = new List<string>();
            var dietaryRestrictionList = new List<string>();
            var dietList = new List<string>();

            // get ingredients
            ingredientList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.Ingredient.ToString()).Select(f => f.Entity));

            //not sure if this is correct
            if (ingredientList.Count == 0)
                await context.PostAsync("Please specify at least one ingredient.");

            // check filters
            dietaryRestrictionList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.DietaryRestriction.ToString()).Select(f => f.Entity));
            dietList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.Diet.ToString()).Select(f => f.Entity));

            // if need more info, prompt user for more info

            // make api call?

            // display results based on response
            await context.PostAsync("did not find any recipes");
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        #endregion


        #region private methods



        #endregion

    }

    public enum UtteranceType
    {
        EndProduct,
        Ingredient,
        DietaryRestriction,
        Diet
    }
}