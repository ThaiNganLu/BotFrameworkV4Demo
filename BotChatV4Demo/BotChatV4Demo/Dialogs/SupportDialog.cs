using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace BotChatV4Demo
{
    public class SupportDialog : ComponentDialog
    {
        //qna maker
        protected static QnAMaker qnaMaker;

        public SupportDialog(string id)
            : base(id)
        {
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            try
            {
                string choice;
                if (innerDc.Context.Activity.Type == ActivityTypes.Message)
                {
                    var text = innerDc.Context.Activity.Text.ToLowerInvariant();

                    var currentQnAConfig = KBList.KBConfigurationsList[KBList.KBConfigurationsList.IndexOf(KBList.KBConfigurationsList.Find(kb => kb.isCurrent == true))].QnAInfo;
                    
                    if (qnaMaker == null)
                    {
                        qnaMaker = new QnAMaker(new QnAMakerEndpoint()
                        {
                            KnowledgeBaseId = currentQnAConfig.KnowledgeBaseId,
                            EndpointKey = currentQnAConfig.EndpointKey,
                            Host = currentQnAConfig.Host
                        });
                    }

                    if (innerDc.Parent.ActiveDialog.Id == nameof(MainMenuDialog))
                        return null;


                    var response = await qnaMaker.GetAnswersAsync(innerDc.Context);
                    try
                    {
                        if (response == null)
                            return null;
                        choice = response[0].Answer;
                        switch (choice.ToLower())
                        {
                            //options case
                            case "food":
                            case "drink":
                            case "view order":
                                break;
                            case "reset":
                                await innerDc.Context.SendActivityAsync($"I see, type anything to get started!", cancellationToken: cancellationToken);
                                //await innerDc.CancelAllDialogsAsync(cancellationToken);
                                innerDc.Context.Activity.Text = null;
                                return await innerDc.CancelAllDialogsAsync(cancellationToken);
                            case "stop":
                                await innerDc.Context.SendActivityAsync($"I see!", cancellationToken: cancellationToken);
                                await innerDc.Context.SendActivityAsync(MessageFactory.Text($"Thanks, see you!"));
                                return await innerDc.CancelAllDialogsAsync();
                            default:
                                await innerDc.Context.SendActivityAsync(choice, cancellationToken: cancellationToken);
                                if(innerDc.Parent.ActiveDialog.Id != nameof(MainMenuDialog))
                                    await innerDc.Context.SendActivityAsync($"Continue order, pls!", cancellationToken: cancellationToken);
                                break;
                        }
                    }
                    catch (System.Exception)
                    {
                        return null;
                    }
                }

                return null;
            }
            catch (System.Exception ex)
            {
                return await innerDc.CancelAllDialogsAsync();
            }

        }
    }
}
