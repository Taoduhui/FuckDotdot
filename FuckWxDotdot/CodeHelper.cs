using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiyiAsm
{
    class CodeHelper
    {
        Dictionary<string, string> Alias = new Dictionary<string, string>();
        FileHelper Helper;
        public CodeHelper(FileHelper fileHelper, Dictionary<string, string> alias)
        {
            Helper = fileHelper;
            Alias = alias;
        }


        public int Fuck(List<string> Bitches)
        {
            int Cnt = 0;
            List<string> keys = Alias.Keys.ToList();
            foreach (string Bitch in Bitches)
            {
                string code = "";
                using (Stream s = File.OpenRead(Bitch))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        code = sr.ReadToEnd();
                        foreach (string key in keys)
                        {
                            if (code.Contains(key))
                            {
                                string TargetDir = Alias[key];
                                string Dotdot = Helper.GenerateDotdot(Bitch, TargetDir);
                                code = code.Replace(key, Dotdot);
                                Cnt++;
                            }
                        }
                    }
                }
                string TargetFile = Helper.GetTargetFile(Bitch);
                File.Delete(TargetFile);
                using (Stream s = File.OpenWrite(TargetFile))
                {
                    using (StreamWriter sr = new StreamWriter(s))
                    {
                        sr.Write(code);
                    }
                }
            }
            return Cnt;
        }

        public void AsmCode(List<string> TargetFile, Dictionary<string, string> Asms)
        {
            foreach (string filepath in TargetFile)
            {
                string src = "";
                using (Stream s = File.OpenRead(filepath))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        src = sr.ReadToEnd();
                        foreach (string key in Asms.Keys)
                        {
                            Console.WriteLine(key);
                            if (src.Contains(key))
                            {
                                FileInfo file = new FileInfo(filepath);
                                using (Stream s_1 = File.OpenRead(file.DirectoryName + Asms[key]))
                                {
                                    using (StreamReader sr_1 = new StreamReader(s_1))
                                    {
                                        src = src.Replace(key, sr_1.ReadToEnd());
                                        Console.WriteLine("\t{0}", Asms[key]);
                                    }
                                }

                            }

                        }
                    }
                }
                string DestFile = Helper.GetTargetFile(filepath);
                File.Delete(DestFile);
                using (Stream s = File.OpenWrite(DestFile))
                {
                    using (StreamWriter sr = new StreamWriter(s))
                    {
                        sr.Write(src);
                    }
                }
            }
        }
    }
}
