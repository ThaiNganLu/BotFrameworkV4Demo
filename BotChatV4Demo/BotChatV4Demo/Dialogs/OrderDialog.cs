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

namespace BotChatV4Demo
{
    public class OrderDialog : SupportDialog
    {
        private static UserState _userState;
        private static IStatePropertyAccessor<Order> localOrderAccessor;
        private static IStatePropertyAccessor<Order> globalOrderAccessor;
        private static Order localOrder = new Order();
        private static Order globalOrder = new Order();

        public OrderDialog(UserState userState) : base(nameof(OrderDialog))
        {
            _userState = userState;
            localOrderAccessor = _userState.CreateProperty<Order>(nameof(Order));
            globalOrderAccessor = _userState.CreateProperty<Order>("OrderStorage");


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NameStepAsync,
                PhoneStepAsync,
                QuantityStepAsync,
                ReviewOrderStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            globalOrder = await globalOrderAccessor.GetAsync(stepContext.Context, () => new Order());

            if (!string.IsNullOrEmpty(globalOrder?.Name))
                return await stepContext.NextAsync();

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> PhoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            globalOrder = await globalOrderAccessor.GetAsync(stepContext.Context, () => new Order());

            localOrder = await localOrderAccessor.GetAsync(stepContext.Context, () => new Order());

            if (!string.IsNullOrEmpty(globalOrder?.Phone))
                return await stepContext.NextAsync();

            localOrder.Name = (string)stepContext.Result;

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Please enter your phone.") };

            // Ask the user to enter their age.
            return await stepContext.PromptAsync(nameof(NumberPrompt<long>), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> QuantityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            localOrder = await localOrderAccessor.GetAsync(stepContext.Context, () => new Order());

            localOrder.Phone = ((int)stepContext.Result).ToString();

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Please enter quantity of product.") };

            // Ask the user to enter their age.
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ReviewOrderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //validate quantity
            if (!Validator.ValidateQuantity(stepContext.Result.ToString(), out int quantity, out string message))
            {
                await stepContext.Context.SendActivityAsync(message);
                //loop step when error
                stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
                return await stepContext.NextAsync();
            }

            globalOrder = await globalOrderAccessor.GetAsync(stepContext.Context, () => new Order());
            localOrder = await localOrderAccessor.GetAsync(stepContext.Context, () => new Order());

            localOrder.Cart[localOrder.Cart.Count - 1].Quantity = (int)stepContext.Result;
            await localOrderAccessor.SetAsync(stepContext.Context, localOrder);

            //save order to global order
            if (string.IsNullOrEmpty(globalOrder?.Name))
                globalOrder = localOrder;
            else
                globalOrder.Cart.AddRange(localOrder.Cart);
            await globalOrderAccessor.SetAsync(stepContext.Context, globalOrder);

            //clear local data
            await localOrderAccessor.DeleteAsync(stepContext.Context, cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Your Order: "
                + Environment.NewLine
                + "Name: " + localOrder.Name
                + Environment.NewLine
                + "Phone number: " + localOrder.Phone
                + Environment.NewLine
                + "Your order" + ": " + localOrder.Cart[localOrder.Cart.Count - 1].Name
                + Environment.NewLine
                + "Quantity: " + localOrder.Cart[localOrder.Cart.Count - 1].Quantity
                ),
                Choices = new List<Choice> { new Choice("Back to Main Menu"), new Choice("Stop") }
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync();
        }
    }
}
