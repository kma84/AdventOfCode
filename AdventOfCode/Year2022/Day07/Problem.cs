using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day07
{
    [Problem(Year = 2022, Day = 7, ProblemName = "No Space Left On Device")]
    internal class Problem : IProblem
    {
        private readonly int MAX_SIZE = 100000;
        private readonly int TOTAL_DISK_SPACE = 70000000;
        private readonly int UPDATE_REQUIRED_SPACE = 30000000;

        public bool Debug => false;

        public string Part1(string input) => GetDirectories(input.GetLines()).Select(d => d.GetDirectorySize())
                                                                             .Where(s => s <= MAX_SIZE).Sum()
                                                                             .ToString();
        
        public string Part2(string input)
        {
            IEnumerable<long> dirsSize = GetDirectories(input.GetLines()).Select(d => d.GetDirectorySize());
            long requiredFreeSpace = UPDATE_REQUIRED_SPACE - TOTAL_DISK_SPACE + dirsSize.First();
            
            return dirsSize.Where(s => s > requiredFreeSpace).Order().First().ToString();
        }

        private static List<DirectoryNode> GetDirectories(string[] lines)
        {
            DirectoryNode rootDir = new() { Name = "/" };
            DirectoryNode currentDir = rootDir;
            List<DirectoryNode> dirs = new() { rootDir };

            foreach (string line in lines)
                currentDir = ProcessLine(line, currentDir, dirs);

            return dirs;
        }

        private static DirectoryNode ProcessLine(string line, DirectoryNode currentDir, List<DirectoryNode> dirs) => line.Split() switch
        {
            ["$", "cd", ".." or "/"]            => currentDir.Parent ?? currentDir,
            ["$", "cd", string dirName]         => currentDir.Dirs.First(d => d.Name == dirName),
            ["$", "ls"]                         => currentDir,
            ["dir", string dirName]             => AddDir(currentDir, dirName, dirs),
            [string sizeStr, string fileName]   => AddFile(currentDir, fileName, long.Parse(sizeStr)),
            _                                   => currentDir,
        };

        private static DirectoryNode AddDir(DirectoryNode currentDir, string newDirName, List<DirectoryNode> dirs)
        {
            DirectoryNode newDir = new() { Name = newDirName, Parent = currentDir };
            currentDir.Dirs.Add(newDir);
            dirs.Add(newDir);

            return currentDir;
        }

        private static DirectoryNode AddFile(DirectoryNode currentDir, string fileName, long size)
        {
            currentDir.Files.Add(new FileRecord(fileName, size));
            return currentDir;
        }


        private class DirectoryNode
        {
            public required string Name { get; set; }
            public DirectoryNode? Parent { get; set; }
            public List<DirectoryNode> Dirs { get; set; } = new();
            public List<FileRecord> Files { get; set; } = new();

            public long GetDirectorySize()
            {
                long size = 0;

                foreach (DirectoryNode node in Dirs)
                    size += node.GetDirectorySize();

                size += Files.Sum(fr => fr.Size);

                return size;
            }
        }

        private record FileRecord(string Name, long Size);
    }
}