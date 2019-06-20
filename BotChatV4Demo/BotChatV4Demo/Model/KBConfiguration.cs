using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class KBConfiguration
    {
        public string CODE { get; set; }
        public QnAConfig QnAInfo { get; set; }
        public bool isCurrent { get; set; }

        public KBConfiguration(string code = "", QnAConfig qna = null, bool isCurrent = false)
        {
            this.CODE = code;
            this.QnAInfo = qna;
            this.isCurrent = isCurrent;
        }

        public KBConfiguration() { }
    }
}
