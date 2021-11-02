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
            string CurrentDir = Directory.GetCurrentDirectory()+"\\";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(CurrentDir + "FuckDotdot.xml");
            XmlNodeList keys = xmlDocument.SelectNodes("//Alias");
            Dictionary<string, string> Alias = new Dictionary<string, string>();
            foreach (XmlNode node in keys)
            {
                var a = node.Attributes["key"];
                Alias.Add(node.Attributes["key"].Value, CurrentDir + node.InnerText);
            }
            List<string> TargetFileTypes = new List<string>();
            XmlNodeList FileTypes = xmlDocument.SelectNodes("//FileType");
            foreach (XmlNode node in FileTypes)
            {
                TargetFileTypes.Add(node.InnerText);
            }
            FileHelper fileHelper = new FileHelper(TargetFileTypes);
            CodeHelper codeHelper = new CodeHelper(fileHelper, Alias);
            codeHelper.Fuck(fileHelper.GetAllTargetFiles());
        }


    }
}
