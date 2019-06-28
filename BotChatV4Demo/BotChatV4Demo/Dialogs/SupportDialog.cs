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

                    if (innerDc.Stack.Count != 0)
                        if (innerDc.Stack[0].Id == nameof(LUISDemo))
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
                                innerDc.Context.Activity.Text = "food";
                                break;
                            case "drink":
                                innerDc.Context.Activity.Text = "drink";
                                break;
                            case "view order":
                                innerDc.Context.Activity.Text = "view order";
                                break;
                            case "switch kb":
                                innerDc.Context.Activity.Text = "switch kb";
                                break;
                            case "luis demo":
                                innerDc.Context.Activity.Text = "luis demo";
                                break;
                            case "card demo":
                                innerDc.Context.Activity.Text = "card demo";
                                break;
                            case "KB1":
                                innerDc.Context.Activity.Text = "KB1";
                                break;
                            case "KB2":
                                innerDc.Context.Activity.Text = "KB2";
                                break;
                            case "reset":
                                if(innerDc.Stack.Count != 0)
                                    if (innerDc.Stack[0].Id == nameof(MainMenuDialog))
                                        return null;
                                innerDc.Context.Activity.Text = "reset";
                                await innerDc.Context.SendActivityAsync($"I see, conversation reset!", cancellationToken: cancellationToken);
                                return await innerDc.CancelAllDialogsAsync(cancellationToken);
                            case "stop":
                                await innerDc.Context.SendActivityAsync($"I see, conversation stop!", cancellationToken: cancellationToken);
                                return await innerDc.CancelAllDialogsAsync(cancellationToken);
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
