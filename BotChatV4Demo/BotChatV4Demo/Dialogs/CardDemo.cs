using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class CardDemo:SupportDialog
    {
        public CardDemo() : base(nameof(CardDemo))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ViewCardDemoStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ViewCardDemoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Here is all card template!"), cancellationToken);

            var attachmentList = new List<Attachment>();
            attachmentList.Add(CardService.CreateAdaptiveCardAttachment());
            attachmentList.Add(CardService.GetAnimationCard().ToAttachment());
            attachmentList.Add(CardService.GetAudioCard().ToAttachment());
            attachmentList.Add(CardService.GetHeroCard().ToAttachment());
            attachmentList.Add(CardService.GetReceiptCard().ToAttachment());
            attachmentList.Add(CardService.GetSigninCard().ToAttachment());
            attachmentList.Add(CardService.GetThumbnailCard().ToAttachment());
            attachmentList.Add(CardService.GetVideoCard().ToAttachment());


            var reply = stepContext.Context.Activity.CreateReply();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = attachmentList;

            

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            var promptOptions = new PromptOptions();
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
