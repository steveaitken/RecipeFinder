using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using RecipeFinder.Model;
using RecipeFinder.Dialogs;

namespace RecipeFinder
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                return await Conversation.SendAsync(message, MakeRoot);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        internal static IDialog<Recipe> MakeRoot()
        {
            return Chain.From(() => new RecipeFinderDialog(Recipe.BuildForm)).Do(async (context, recipe) =>
            {
                try
                {
                    var completed = await recipe;
                    // Actually process the recipe...
                    await context.PostAsync("Processed your recipe!");
                }
                catch (FormCanceledException<Recipe> e)
                {
                    string reply;
                    if (e.InnerException == null)
                    {
                        reply = $"You quit on {e.Last}--maybe you can finish next time!";
                    }
                    else
                    {
                        reply = "Sorry, I've had a short circuit.  Please try again.";
                    }
                    await context.PostAsync(reply);
                }
            }); ;
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}