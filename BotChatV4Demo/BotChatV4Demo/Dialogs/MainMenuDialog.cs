using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace BotChatV4Demo
{
    public class MainMenuDialog : SupportDialog
    {
        static Order orderInfo = new Order();

        static BotState _userState;

        public MainMenuDialog(UserState userState) : base(nameof(MainMenuDialog))
        {
            _userState = userState;
            
            AddDialog(new TextPrompt(nameof(TextPrompt)));  
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new FoodCategories(userState));
            AddDialog(new DrinkCategories(userState));
            AddDialog(new ReviewOrderDialog(userState));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                MainMenuStepAsync,
                ReviewSelectionStepAsync,
                MainMenuFinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private static async Task<DialogTurnResult> MainMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Welcome to Order Bot!"), cancellationToken);

            var attachmentList = new List<Attachment>();
            foreach (var menuItem in MenuList.Menu)
            {
                var menuItemCard = CardService.CreateMainMenuHeroCardMenu(menuItem);
                attachmentList.Add(menuItemCard.ToAttachment());
            }
            
            var reply = stepContext.Context.Activity.CreateReply();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = attachmentList;

            var promptOptions = new PromptOptions
            {
                RetryPrompt = reply
            };

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);

        }

        private async Task<DialogTurnResult> ReviewSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                //qna test
                var response = await qnaMaker.GetAnswersAsync(stepContext.Context);
                var choice = response[0].Answer;
                //var choice = "view order";

                switch (choice.ToLower())
                {
                    case "food":
                        return await stepContext.BeginDialogAsync(nameof(FoodCategories), null, cancellationToken);
                    case "drink":
                        return await stepContext.BeginDialogAsync(nameof(DrinkCategories), null, cancellationToken);
                    case "view order":
                        return await stepContext.BeginDialogAsync(nameof(ReviewOrderDialog), null, cancellationToken);
                    case "reset":
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Conversation reset..."));
                        return await stepContext.ReplaceDialogAsync(nameof(MainMenuDialog), cancellationToken);
                    default:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sorry, I don't understand that!"));
                        return await stepContext.ReplaceDialogAsync(nameof(MainMenuDialog), cancellationToken);
                }
            }
            catch (Exception)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sorry, I dont understand that! :("));
                //loop step when error
                return await stepContext.ReplaceDialogAsync(nameof(MainMenuDialog), cancellationToken);
            }

        }

        private async Task<DialogTurnResult> MainMenuFinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //var userStateAccessors = _userState.CreateProperty<Order>("OrderStorage");
            //var orderProfile = await userStateAccessors.GetAsync(stepContext.Context, () => new Order());

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank " + (string.IsNullOrEmpty(orderProfile?.Name) ? "you" : orderProfile.Name) + ", see you!"));
            return await stepContext.EndDialogAsync();
        }

        public override Task<DialogTurnResult> ResumeDialogAsync(DialogContext dc, DialogReason reason, object result = null, CancellationToken cancellationToken = default(CancellationToken))
        {

            return null;
        }
    }
}
