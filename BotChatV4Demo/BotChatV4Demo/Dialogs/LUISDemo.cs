using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.AI.Luis;

namespace BotChatV4Demo
{
    public class LUISDemo : ComponentDialog
    {
        static BotState _userState;
        static bool isFirstAsync = true;

        LuisApplication luisApplication = new LuisApplication("eeca8396-0c16-48b0-ad86-ee0ddf5a7219", "7003445329fe462db8b2024dd5e6dbc2", "https://westus.api.cognitive.microsoft.com");

        public LUISDemo(UserState userState) : base(nameof(LUISDemo))
        {
            _userState = userState;

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SwitchKBStepAsync,
                ReviewAnswerStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> SwitchKBStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userStateAccessors = _userState.CreateProperty<Order>("OrderStorage");
            var order = await userStateAccessors.GetAsync(stepContext.Context, () => new Order());

            var visitorName = string.IsNullOrEmpty(order?.Name) ? order.Name : "you";

            var message = isFirstAsync?"Hello " + visitorName + ", LUIS is ready! Type 'stop' to stop conversation with LUIS.": string.Empty;
            
            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(message) };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ReviewAnswerStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Text.ToLower() == "stop")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"LUIS is stopped!"));
                stepContext.Context.Activity.Text = "reset";
                return await stepContext.EndDialogAsync();
            }

            var recognizer = new LuisRecognizer(luisApplication);
            var recognizerResult = await recognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            var (intent, score) = recognizerResult.GetTopScoringIntent();
            
            var result = "Intent: " + intent + Environment.NewLine + "Score: " + score;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(result));

            isFirstAsync = false;

            //loop step
            stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
            return await stepContext.NextAsync();
        }
    }
}
