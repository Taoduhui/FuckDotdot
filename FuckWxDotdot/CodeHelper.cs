using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuckWxDotdot
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
                bool affect = false;
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
                                affect = true;
                            }
                        }
                    }
                }
                if (affect)
                {
                    File.Delete(Bitch);
                    using (Stream s = File.OpenWrite(Bitch))
                    {
                        using (StreamWriter sr = new StreamWriter(s))
                        {
                            sr.WriteLine(code);
                        }
                    }
                }
            }
            return Cnt;
        }
    }
}
