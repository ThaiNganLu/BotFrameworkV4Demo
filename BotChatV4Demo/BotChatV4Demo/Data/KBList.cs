using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class KBList
    {
        public static List<KBConfiguration> KBConfigurationsList = new List<KBConfiguration>() {
            new KBConfiguration("KB1", new QnAConfig("aaa5c939-7f21-42cf-94ff-7d5840aca57a", "446108d5-7dbd-42c2-b3ed-152378a841ed", "https://qnachatbottest.azurewebsites.net/qnamaker"), true),
            new KBConfiguration("KB2", new QnAConfig("37dce89f-1c8b-49c4-b891-8ef2240e6cde", "9519801e-30db-4a9d-a2cb-b3e10f03f88e", "https://switchbottest.azurewebsites.net/qnamaker"), false)
        };
    }
}
