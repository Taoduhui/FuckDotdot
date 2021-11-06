using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuckWxDotdot
{
    class FileHelper
    {
        List<string> FileTypes = new List<string>();
        public Dictionary<string, string> ToNewFile = new Dictionary<string, string>();
        public List<string> Exclude = new List<string>();
        public FileHelper(List<string> fileTypes)
        {
            FileTypes = fileTypes;
        }

        public FileHelper(string fileType)
        {
            FileTypes.Add(fileType);
        }

        public string GetTargetFile(string filepath)
        {
            FileInfo info = new FileInfo(filepath);
            string name = info.Name.Remove(info.Name.IndexOf("."));
            string exname = "*" + info.Name.Remove(0, info.Name.IndexOf("."));
            string TargetExname = ToNewFile.ContainsKey(exname) ? ToNewFile[exname] : exname;
            return (info.DirectoryName + "/" + name + (TargetExname.Replace("*", ""))).Replace("\\", "/");
        }

        public FileHelper(string fileTypes, string toNewFile)
        {
            FileTypes.Add(fileTypes);
            ToNewFile.Add(fileTypes, toNewFile);
        }

        public List<string> GetAllTargetFiles()
        {
            List<string> TargetFiles = new List<string>();
            foreach (string FileType in FileTypes)
            {
                string[] res = Directory.GetFiles(Directory.GetCurrentDirectory(), FileType, SearchOption.AllDirectories);
                foreach (string path in res)
                {
                    bool skip = false;
                    foreach (string exc in Exclude)
                    {
                        if (path.Contains(exc))
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (!skip)
                    {
                        TargetFiles.Add(path);
                    }
                }

            }
            return TargetFiles;
        }


        public int GetDirectoryDepth(string filename)
        {
            filename = filename.Replace("\\", "/");
            int Left = filename.Where(r => r == '/').Count();
            return Left;
        }

        public string GenerateDotdot(int depth)
        {
            string Dotdot = "";
            for (int i = 0; i < depth; i++)
            {
                Dotdot += "../";
            }
            return Dotdot;
        }

        public string GenerateDotdot(string filename, string targetDir)
        {
            filename = filename.Replace("\\", "/");
            targetDir = targetDir.Replace("\\", "/");
            string dotdot = "";
            string srcDir = new FileInfo(filename).DirectoryName;
            string SameParent = "";
            for (int i = 0; i < filename.Length && i < targetDir.Length; i++)
            {
                if (filename[i] != targetDir[i])
                {
                    break;
                }
                SameParent += filename[i];
            }
            if (SameParent[SameParent.Length - 1] != '/')
            {
                targetDir += "/";
                SameParent += "/";
            }
            int SameParentDepth = GetDirectoryDepth(SameParent);
            int srcDepth = GetDirectoryDepth(filename);
            dotdot += GenerateDotdot(srcDepth - SameParentDepth);
            if (dotdot == "") { dotdot = "./"; }
            string InPath = targetDir.Remove(0, SameParent.Length);
            if (InPath != "")
            {
                dotdot += InPath;
            }
            else
            {
                dotdot = dotdot.Remove(dotdot.Length - 1);
            }
            return dotdot;
        }
    }
}
