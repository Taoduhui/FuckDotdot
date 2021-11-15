using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

namespace ShiyiAsm
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
            using (Stream s = File.OpenRead(CurrentDir + "ShiyiAsm.xml"))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    Config = sr.ReadToEnd();
                }
            }
            xmlDocument.LoadXml(Config);
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
                Console.WriteLine("{0}\tAliasAffect:{1}", FileType, affect);

                XmlNodeList Asmkeys = node.SelectNodes(".//Asm");
                Dictionary<string, string> Asms = new Dictionary<string, string>();
                foreach (XmlNode key in Asmkeys)
                {
                    Asms.Add(key.Attributes["key"].Value, key.InnerText.Replace("{{Key}}", key.Attributes["key"].Value.Replace("{{", "").Replace("}}", "")));
                }
                codeHelper.AsmCode(fileHelper.GetAllTargetFiles(), Asms);
            }

            XmlNodeList AfterCmdKeys = xmlDocument.SelectNodes("//AfterCmd");
            foreach (XmlNode node in AfterCmdKeys)
            {
                RunCmd(node.InnerText);
            }
        }

        public static void RunCmd(string str)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Directory.GetCurrentDirectory();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(str + "&exit");

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();

            //StreamReader reader = p.StandardOutput;
            //string line=reader.ReadLine();
            //while (!reader.EndOfStream)
            //{
            //    str += line + "  ";
            //    line = reader.ReadLine();
            //}

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();


            Console.WriteLine(output);
        }

    }
}
