
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema.Teams;

using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBotCLU;
using CoreBotCLU.Dialogs;
using Microsoft.BotBuilderSamples.Dialogs;
using Microsoft.Bot.Schema;
using System.Net.Sockets;
using Microsoft.BotBuilderSamples;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using AdaptiveCards;



namespace CoreBotCLU.Dialogs

{
    public class TicketCreation : CancelAndHelpDialog
    {
        public TicketCreation() : base(nameof(TicketCreation))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
           {
                TitleStepAsync,
                DescriptionStepAsync,
                PriorityStepAsync,
                TenantStepAsync,
                EnvStepAsync,
                CreationDateStepAsync,
                ConfirmStepAsync,
                //FinalStepAsync,

            }));



            InitialDialogId = nameof(WaterfallDialog);
        }

       

        private async Task<DialogTurnResult> TitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var TicketDetails = (ticket)stepContext.Options;
            
            if (TicketDetails.Title == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("please enter the title of the ticket that you want to submit ")
                }, cancellationToken);
            }
            return await stepContext.NextAsync(TicketDetails.Title, cancellationToken);
        }

        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var TicketDetails = (ticket)stepContext.Options;
            TicketDetails.Title = (string)stepContext.Result;
            if (TicketDetails.Description == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("please enter the description of the ticket that you want to submit ")
                }, cancellationToken);
            }
            return await stepContext.NextAsync(TicketDetails.Description, cancellationToken);
        }

        private async Task<DialogTurnResult> PriorityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var TicketDetails = (ticket)stepContext.Options;
            TicketDetails.Description = (string)stepContext.Result;
            if (TicketDetails.priority == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("please enter the priority of the ticket that you want to submit ")
                }, cancellationToken);
            }
            return await stepContext.NextAsync(TicketDetails.priority, cancellationToken);
        }
        private  async  Task<DialogTurnResult> TenantStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var TicketDetails = (ticket)stepContext.Options;
            TicketDetails.priority = (string)stepContext.Result;
            if (TicketDetails.Tenant == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("please enter the tenant of the ticket that you want to submit ")
                }, cancellationToken);
            }
            return await stepContext.NextAsync(TicketDetails.Tenant, cancellationToken);
        }

        private async Task<DialogTurnResult> EnvStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var TicketDetails = (ticket)stepContext.Options;
            TicketDetails.Tenant = (string)stepContext.Result;
            if (TicketDetails.Environment == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("please enter the environment of the ticket that you want to submit ")
                }, cancellationToken);
            }
            return await stepContext.NextAsync(TicketDetails.Environment, cancellationToken);
        }
        private async Task<DialogTurnResult> CreationDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var TicketDetails = (ticket)stepContext.Options;
            TicketDetails.Environment = (string)stepContext.Result;
            if (TicketDetails.CreatedDate == null)
            {
                return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), TicketDetails.CreatedDate, cancellationToken);
            }
            return await stepContext.NextAsync(TicketDetails.CreatedDate, cancellationToken);
        }
        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var TicketDetails = (ticket)stepContext.Options;
            TicketDetails.CreatedDate = (string)stepContext.Result;
            //var messageText = $"Please confirm, you want to create ticket with Title {TicketDetails.Title} .\r\n Description : {TicketDetails.Description} . \r\n Priority :{TicketDetails.priority} . \r\n tenant : {TicketDetails.Tenant}. \r\n Environment : {TicketDetails.Environment} . \r\n and on {TicketDetails.CreatedDate} \r\n Is this correct?";
            //var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = "TICKET DETAILS ",
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Medium,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = $"Title : {TicketDetails.Title}",
                        Size = AdaptiveTextSize.Medium,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = $"Description : {TicketDetails.Description}",
                        Size = AdaptiveTextSize.Medium,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = $"Priority : {TicketDetails.priority}",
                        Size = AdaptiveTextSize.Medium,
                    },
                     new AdaptiveTextBlock
                    {
                        Text = $"Tenant : {TicketDetails.Tenant}",
                        Size = AdaptiveTextSize.Medium,
                    },
                      new AdaptiveTextBlock
                    {
                        Text = $"environment : {TicketDetails.Environment}",
                        Size = AdaptiveTextSize.Medium,
                    },
                       new AdaptiveTextBlock
                    {
                        Text = $"Date : {TicketDetails.CreatedDate}",
                        Size = AdaptiveTextSize.Medium,
                    },

                },
                BackgroundImage = new AdaptiveBackgroundImage
                {
                    Url = new Uri("https://th.bing.com/th/id/R.7903d6fd0397080d0a98a32f807ae615?rik=HKwL0PdyG1lyRA&riu=http%3a%2f%2fwww.solidbackgrounds.com%2fimages%2f2880x1800%2f2880x1800-light-sky-blue-solid-color-background.jpg&ehk=Gm141oxIDKzlHNSvQvPwdQZUoD8UZBc7%2fXMYhC4TmRs%3d&risl=&pid=ImgRaw&r=0"), // Replace with your image URL
                    FillMode = AdaptiveImageFillMode.Cover,
                },
            };

            // Prompt the user with the ticket choices
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(card),
                }),
            },cancellationToken);
        }

        //    return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //}
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //if ((bool)stepContext.Result)
            //{
            //    var ticketDetails = (ticket)stepContext.Options;

            //    return await stepContext.EndDialogAsync(ticketDetails, cancellationToken);
            //}
            var ticketDetails = (ticket)stepContext.Options;
            return await stepContext.EndDialogAsync(ticketDetails, cancellationToken);

        }
    }
}
