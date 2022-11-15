using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace UdemyAspNetCore.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MailJetSettings MailJetSettings { get; set; }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            MailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();

            var client = new MailjetClient(MailJetSettings.ApiKey, MailJetSettings.SecretKey)
            {
                Version = ApiVersion.V3_1
            };

            var request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
     new JObject {
      {
       "From",
       new JObject {
        {"Email", "temchik1997@yandex.ru"},
        {"Name", "Artem"}
       }
      }, {
       "To",
       new JArray {
        new JObject {
         {
          "Email",
          email
         }, {
          "Name",
          "DotNetMastery"
         }
        }
       }
      }, {
       "Subject",
       subject
      }, 
         {
       "HTMLPart",
       body
      },
     }
             });
            
            await client.PostAsync(request);
        }
    }
}
