using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using RecipeFinder.Model;

namespace RecipeFinder.Dialogs
{
    [LuisModel("f856bce2-68f1-47ca-a306-9250d54180dc", "63038e9502b5404e8e448e6e25eafb14")]
    [Serializable]
    public class RecipeFinderDialog : LuisDialog<Recipe>
    {
        #region variables

        private readonly BuildForm<Recipe> MakeRecipeForm;
        public const string Entity_Ingredient = "";
        public const string Entity_EndProduct = "";
        public const string Entity_Dietary_Restriction = "";

        #endregion


        #region constructors

        public RecipeFinderDialog(BuildForm<Recipe> makeRecipeForm, ILuisService service = null)
            : base(service)
        {
            MakeRecipeForm = makeRecipeForm;
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
            endProductList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.EndProduct.ToString()).Select(f => f.Entity));

            // check filters
            dietaryRestrictionList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.DietaryRestriction.ToString()).Select(f => f.Entity));
            dietList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.Diet.ToString()).Select(f => f.Entity));


            // check score

            // get filters

            // if need more info, prompt user for more info

            // make api call?

            // display results based on response
            var pizzaForm = new FormDialog<Recipe>(new Recipe(), this.MakeRecipeForm, FormOptions.PromptInStart, result.Entities);
            context.Call<Recipe>(pizzaForm, RecipeFormComplete);
        }

        [LuisIntent("FindRecipeByIngredients")]
        public async Task FindRecipeByIngredients(IDialogContext context, LuisResult result)
        {
            var ingredientList = new List<string>();
            var dietaryRestrictionList = new List<string>();
            var dietList = new List<string>();

            // get ingredients
            ingredientList.AddRange(result.Entities.Where(f => f.Type == UtteranceType.Ingredient.ToString()).Select(f => f.Entity));

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

        private async Task RecipeFormComplete(IDialogContext context, IAwaitable<Recipe> result)
        {
            Recipe recipe = null;
            try
            {
                recipe = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                return;
            }

            if (recipe != null)
            {
                await context.PostAsync("Your recipe: " + recipe.ToString());
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            context.Wait(MessageReceived);
        }

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