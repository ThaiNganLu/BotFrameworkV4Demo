using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class QnAConfig
    {
        public string KnowledgeBaseId { get; set; }
        public string EndpointKey { get; set; }
        public string Host { get; set; }

        public QnAConfig(string kbID = "", string endpointKey = "", string host = "")
        {
            this.KnowledgeBaseId = kbID;
            this.EndpointKey = endpointKey;
            this.Host = host;
        }

        public QnAConfig() { }
    }
}
