using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace RecipeFinder.Dialogs
{
    [LuisModel("f856bce2-68f1-47ca-a306-9250d54180dc", "63038e9502b5404e8e448e6e25eafb14")]
    [Serializable]
    public class RecipeFinderDialog : LuisDialog<object>
    {
        #region public methods

        [LuisIntent("FindRecipeByEndProduct")]
        public async Task FindRecipeByEndProduct(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("did not find any recipes");
            context.Wait(MessageReceived);
        }

        [LuisIntent("FindRecipeByIngredients")]
        public async Task FindRecipeByIngredients(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("did not find any recipes");
            context.Wait(MessageReceived);
        }

        #endregion


        #region private methods



        #endregion

    }
}