using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

namespace FuckWxDotdot
{
    class Program
    {
        static FileSystemWatcher watcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
        static int DelayParm = 0;
        static void Main(string[] args)
        {
            if (args.Length >= 1 && args[0] == "-w")
            {
                if (args.Length >= 2)
                {
                    DelayParm = Convert.ToInt32(args[1].Replace("-", ""));
                }
                else
                {
                    DelayParm = 100;
                }
                DelayTime = DelayParm;
                Console.WriteLine("Start watching...");
                InitWatcher();
                while (true) { Console.ReadKey(); };
            }
            else
            {
                Run();
            }
        }



        private static void InitWatcher()
        {
            watcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = true;
            watcher.Created += Watcher_Notification;
            watcher.Changed += Watcher_Notification;
            watcher.Renamed += Watcher_Notification;
            watcher.EnableRaisingEvents = true;
        }

        static int DelayTime = 20;
        private static bool delay()
        {
            if (DelayTime != DelayParm)
            {
                DelayTime = DelayParm - 1;
                return false;
            }
            else
            {
                Console.WriteLine("Waiting For Completed");
                DelayTime = DelayParm;
                while (DelayTime != 0)
                {
                    DelayTime--;
                    Thread.Sleep(1);
                }
                DelayTime = DelayParm;
                return true;
            }

        }

        private static void Watcher_Notification(object sender, object e)
        {
            if (!delay()) { return; }
            Console.Clear();
            watcher.EnableRaisingEvents = false;
            Console.WriteLine("Detect Change");
            int err = 0;
            while (err >= 0 && err <= 3)
            {
                try
                {
                    Run();
                    err = -1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    err++;
                }
            }

            InitWatcher();
        }

        private static void Run()
        {
            string CurrentDir = Directory.GetCurrentDirectory() + "\\";
            string Config = "";
            XmlDocument xmlDocument = new XmlDocument();
            using (Stream s = File.OpenRead(CurrentDir + "FuckDotdot.xml"))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    Config = sr.ReadToEnd();
                }
            }
            xmlDocument.LoadXml(Config);
            //xmlDocument.Load(CurrentDir + "FuckDotdot.xml");
            XmlNodeList GlobalKeys = xmlDocument.SelectNodes("//GlobalAlias");
            Dictionary<string, string> GlobalAlias = new Dictionary<string, string>();
            foreach (XmlNode node in GlobalKeys)
            {
                GlobalAlias.Add(node.Attributes["key"].Value, CurrentDir + node.InnerText);
            }

            XmlNodeList ExcludeKeys = xmlDocument.SelectNodes("//GlobalAlias");
            List<string> Exclude = new List<string>();
            foreach (XmlNode node in ExcludeKeys)
            {
                Exclude.Add(CurrentDir + node.InnerText);
            }


            XmlNodeList FileTypes = xmlDocument.SelectNodes("//FileType");
            foreach (XmlNode node in FileTypes)
            {
                string FileType = node.Attributes["type"].Value;
                FileHelper fileHelper = node.Attributes["to"] == null ? new FileHelper(FileType) : new FileHelper(FileType, node.Attributes["to"].Value);
                fileHelper.Exclude = Exclude;
                XmlNodeList keys = node.SelectNodes(".//Alias");
                Dictionary<string, string> Alias = new Dictionary<string, string>();
                foreach (XmlNode key in keys)
                {
                    Alias.Add(key.Attributes["key"].Value, CurrentDir + key.InnerText);
                }
                foreach (KeyValuePair<string, string> global in GlobalAlias)
                {
                    Alias.Add(global.Key, global.Value);
                }
                CodeHelper codeHelper = new CodeHelper(fileHelper, Alias);
                int affect = codeHelper.Fuck(fileHelper.GetAllTargetFiles());
                Console.WriteLine("{0}\tAffect:{1}", FileType, affect);
            }
        }
    }
}
