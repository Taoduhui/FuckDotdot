using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FuckWxDotdot
{
    class Program
    {
        static void Main(string[] args)
        {
            string CurrentDir = Directory.GetCurrentDirectory() + "\\";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(CurrentDir + "FuckDotdot.xml");
            XmlNodeList GlobalKeys = xmlDocument.SelectNodes("//GlobalAlias");
            Dictionary<string, string> GlobalAlias = new Dictionary<string, string>();
            foreach (XmlNode node in GlobalKeys)
            {
                GlobalAlias.Add(node.Attributes["key"].Value, CurrentDir + node.InnerText);
            }

            XmlNodeList FileTypes = xmlDocument.SelectNodes("//FileType");
            foreach (XmlNode node in FileTypes)
            {
                List<string> TargetFileTypes = new List<string>();
                TargetFileTypes.Add(node.Attributes["type"].Value);
                FileHelper fileHelper = new FileHelper(TargetFileTypes);
                XmlNodeList keys = node.SelectNodes(".//Alias");
                Dictionary<string, string> Alias = new Dictionary<string, string>();
                foreach (XmlNode key in keys)
                {
                    Alias.Add(key.Attributes["key"].Value, CurrentDir + node.InnerText);
                }
                foreach (KeyValuePair<string, string> global in GlobalAlias)
                {
                    Alias.Add(global.Key, global.Value);
                }
                CodeHelper codeHelper = new CodeHelper(fileHelper, Alias);
                int affect = codeHelper.Fuck(fileHelper.GetAllTargetFiles());
                Console.WriteLine("{0}\tAffect:{1}", TargetFileTypes[0], affect);
            }
        }


    }
}
