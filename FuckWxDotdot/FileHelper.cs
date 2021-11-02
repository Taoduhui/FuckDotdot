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

        public FileHelper(List<string> fileTypes)
        {
            FileTypes = fileTypes;
        }

        public List<string> GetAllTargetFiles()
        {
            List<string> TargetFiles = new List<string>();
            foreach (string FileType in FileTypes)
            {
                string[] res = Directory.GetFiles(Directory.GetCurrentDirectory(), FileType, SearchOption.AllDirectories);
                TargetFiles.AddRange(new List<string>(res));
            }
            return TargetFiles;
        }

        public int GetDirectoryDepth(string filename)
        {
            int Left = filename.Where(r => r == '/').Count();
            int Right = filename.Where(r => r == '\\').Count();
            if (Left != 0 && Right != 0)
            {
                throw new Exception("Path invaild!");
            }
            return Math.Max(Left, Right);
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

        public string GenerateDotdot(string filename,string targetDir)
        {
            string dotdot = "";
            string srcDir = new FileInfo(filename).DirectoryName;
            string SameParent = "";
            for (int i = 0; i < filename.Length && i<targetDir.Length; i++)
            {
                if (filename[i] != targetDir[i])
                {
                    break;
                }
                SameParent += filename[i];
            }
            int SameParentDepth = GetDirectoryDepth(SameParent);
            int srcDepth = GetDirectoryDepth(filename);
            dotdot += GenerateDotdot(srcDepth - SameParentDepth);
            dotdot += targetDir.Remove(0, SameParent.Length);
            return dotdot;
        }
    }
}
