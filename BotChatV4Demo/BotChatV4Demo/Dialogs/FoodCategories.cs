using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace BotChatV4Demo
{
    public class FoodCategories : SupportDialog
    {
        static BotState _userState;

        public FoodCategories(UserState userState) : base(nameof(FoodCategories))
        {
            _userState = userState;

            AddDialog(new OrderDialog(userState));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FoodMenuStepAsync,
                ReviewSelectionStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> FoodMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var attachmentList = new List<Attachment>();
            foreach (var food in ProductList.Foods)
            {
                attachmentList.Add(CardService.CreateProductThumbnailCardMenu(food).ToAttachment());
            }
            var reply = stepContext.Context.Activity.CreateReply();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = attachmentList;

            var promptOptions = new PromptOptions()
            {
                RetryPrompt = reply
            };

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ReviewSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userStateAccessors = _userState.CreateProperty<Order>(nameof(Order));
            var order = await userStateAccessors.GetAsync(stepContext.Context, () => new Order());

            var foundChoice = (string)stepContext.Result;

            try
            {
                order.Cart = new List<Product>();
                order.Cart.Add(ProductList.Foods[ProductList.Foods.IndexOf(ProductList.Foods.First(food => food.Code == foundChoice))]);
            }
            catch (Exception)
            {
                //loop step when error
                stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
                return await stepContext.NextAsync();
            }


            return await stepContext.BeginDialogAsync(nameof(OrderDialog), order, cancellationToken);
        }
        public override Task<DialogTurnResult> ResumeDialogAsync(DialogContext dc, DialogReason reason, object result = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync();
        }
    }
}
