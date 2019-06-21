using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;

namespace BotChatV4Demo
{
    public class ReviewOrderDialog : SupportDialog
    {
        private static BotState _userState;

        public ReviewOrderDialog(UserState userState) : base(nameof(ReviewOrderDialog))
        {
            _userState = userState;

            AddDialog(new OrderDialog(userState));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ViewOrderStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ViewOrderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userStateAccessors = _userState.CreateProperty<Order>("OrderStorage");
            var orderProfile = await userStateAccessors.GetAsync(stepContext.Context, () => new Order());
            var promptOptions = new PromptOptions();

            if (string.IsNullOrEmpty(orderProfile?.Name))
            {
                promptOptions.Prompt = MessageFactory.Text($"You don't have ordered!");
            }
            else
            {
                var reply = stepContext.Context.Activity.CreateReply();
                reply.Attachments = new List<Attachment> { CardService.CreateOrder(orderProfile).ToAttachment() };
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            }

            promptOptions.Choices = new List<Choice> { new Choice("Back to Main menu"), new Choice("Stop") };
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //loop step when error
            stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
            return await stepContext.NextAsync();
        }
    }
}
