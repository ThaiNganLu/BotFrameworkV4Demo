using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace BotChatV4Demo
{
    public class SwitchKB : SupportDialog
    {
        static BotState _userState;

        public SwitchKB(UserState userState) : base(nameof(SwitchKB))
        {
            _userState = userState;

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SwitchKBStepAsync,
                ReviewSelectionStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> SwitchKBStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Please choose your bot!"), cancellationToken);
            var choiceList = new List<Choice>();

            foreach(var choiceItem in KBList.KBConfigurationsList)
            {
                choiceList.Add(new Choice(choiceItem.CODE));
            }

            var promptOptions = new PromptOptions()
            {
                Choices = choiceList
            };
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ReviewSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            FoundChoice result = (FoundChoice)stepContext.Result;
            var choice = (string)result.Value;

            foreach (var qnaconfig in KBList.KBConfigurationsList)
            {
                if (qnaconfig.CODE == choice)
                {
                    //set current QnA config
                    KBList.KBConfigurationsList[KBList.KBConfigurationsList.IndexOf(qnaconfig)].isCurrent = true;
                }
                else
                {
                    KBList.KBConfigurationsList[KBList.KBConfigurationsList.IndexOf(qnaconfig)].isCurrent = false;
                }
            }

            //reset qnaMaker in SupportDialog
            qnaMaker = null;

            await stepContext.Context.SendActivityAsync(choice + $" is current knowlegde base!", cancellationToken: cancellationToken);

            var promptOptions = new PromptOptions();
            promptOptions.Choices = new List<Choice> { new Choice("Back to Main menu"), new Choice("Stop") };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }
        
    }
}
