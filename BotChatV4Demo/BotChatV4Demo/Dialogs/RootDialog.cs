using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace BotChatV4Demo
{
    public class RootDialog : ComponentDialog
    {
        private readonly UserState _userState;

        public RootDialog(UserState userState)
            : base(nameof(RootDialog))
        {
            _userState = userState;

            AddDialog(new MainMenuDialog(userState));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(MainMenuDialog), cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Text == "reset")
            {
                return await stepContext.ReplaceDialogAsync(nameof(RootDialog));
            }

            var userStateAccessors = _userState.CreateProperty<Order>(nameof(Order));
            var order = await userStateAccessors.GetAsync(stepContext.Context, () => new Order());

            var visitorName = string.IsNullOrEmpty(order?.Name) ? order.Name : "you";
            string status = "Thank " + visitorName + ", see you!";

            await stepContext.Context.SendActivityAsync(status);
            
            return await stepContext.EndDialogAsync(cancellationToken);
        }

        public override Task<DialogTurnResult> ResumeDialogAsync(DialogContext dc, DialogReason reason, object result = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }
    }
}
