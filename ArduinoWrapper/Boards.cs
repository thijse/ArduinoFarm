using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace ArduinoWrapper
{

    public class BoardPackages : List<BoardPackage> 
    {      
        public BoardPackage this[string name]
        {
           get { return this.FirstOrDefault(b => b.Name == name); }
        }
    }

    public class BoardPackage
    {
        public string Name { get; set; }
        public BoardArchitectures BoardArchitectures { get; set; }

        public BoardPackage()
        {
            BoardArchitectures = new BoardArchitectures();
        }
    }

    public class BoardArchitectures : List<BoardArchitecture>
    {
        public BoardArchitecture this[string name]
        {
           get { return this.FirstOrDefault(b => b.Name == name); }
        }
    }

    public class BoardArchitecture
    {
        public string Name { get; set; }
         public string FileName { get; set; }
        public List<BoardDescription>  BoardDescriptions { get; set; }

         public BoardArchitecture()
         {
             BoardDescriptions = new BoardDescriptions();
         }
    }


    public class BoardDescriptions :  List<BoardDescription>
    {
        public BoardDescription this[string name]
        {
           get { return this.FirstOrDefault(b => b.Name == name); }
        }
    }

    public class BoardDescription
    {
        public string Name { get; set; }
    } 

    public class Boards
    {
        private readonly string _rootPath;
        private readonly string _userPath;
        private ArduinoConfigReader _arduinoConfigReader;
        
        public BoardPackages  BoardPackages { get; set; }

        public Boards(string rootPath, string userPath)
        {              
            _rootPath = rootPath;
            _userPath = userPath;
            BoardPackages = new BoardPackages();
            var t = FindBoardFiles();
            _arduinoConfigReader = new ArduinoConfigReader();
        }

        public List<BoardPackage> FindBoardFiles()
        {
            BoardPackages.Clear();
            AddExeBoardFiles(_rootPath);
            AddUserBoardFiles(_userPath);
            return BoardPackages;
        }


        public void AddExeBoardFiles(string rootPath)
        {
            //var boardsFiles = new List<string>();
                 
            // All boards dirs 
            var boardsPath = FileUtils.Combine(rootPath, "hardware");

            var potentialPackageDirs = Directory.GetDirectories(boardsPath);
            foreach (var potentialPackageDir in potentialPackageDirs)
            {
                BoardPackage currentPackage = null;
                var potentialArchitectureDirs = Directory.GetDirectories(potentialPackageDir);
                foreach (var potentialArchitectureDir in potentialArchitectureDirs)
                {
                    
                    var boardFile = FileUtils.Combine(potentialArchitectureDir, "boards.txt");
                    if (!File.Exists(boardFile)) continue;

                    if (currentPackage == null)
                    {
                        var packageName = FileUtils.GetBaseName(potentialPackageDir);
                        currentPackage = BoardPackages[packageName];
                        if (currentPackage == null)
                        {
                            currentPackage = new BoardPackage {Name = packageName};
                            BoardPackages.Add(currentPackage);
                        }
                    }
                    var architectureName = FileUtils.GetBaseName(potentialArchitectureDir);
                    var currentArchitecture = currentPackage.BoardArchitectures[architectureName];
                    if (currentArchitecture == null)
                    {
                        currentArchitecture = new BoardArchitecture {Name = architectureName};
                        currentPackage.BoardArchitectures.Add(currentArchitecture);
                        currentArchitecture.FileName = boardFile;
                    }
                    AddBoards(currentArchitecture);
                }
            }           
        }

        public void AddUserBoardFiles(string userPath)
        {
            //var boardsFiles = new List<string>();
                 
            // All boards dirs 
            var boardsPath = FileUtils.Combine(userPath, "Arduino15\\packages");

            var potentialPackageDirs = Directory.GetDirectories(boardsPath);
            foreach (var potentialPackageDir in potentialPackageDirs)
            {
                BoardPackage currentPackage = null;
                var packageDir = FileUtils.Combine(potentialPackageDir, "hardware");
                


                    var potentialArchitectureDirs = Directory.GetDirectories(packageDir);


                    foreach (var potentialArchitectureDir in potentialArchitectureDirs)
                    {

                    var versionDirs = Directory.GetDirectories(potentialArchitectureDir);
                                    foreach (var versionDir in versionDirs)
                {


                        var boardFile = FileUtils.Combine(versionDir, "boards.txt");
                        if (!File.Exists(boardFile)) continue;

                        if (currentPackage == null)
                        {
                            var packageName = FileUtils.GetBaseName(potentialPackageDir);
                            currentPackage = BoardPackages[packageName];
                            if (currentPackage == null)
                            {
                                currentPackage = new BoardPackage {Name = packageName};
                                BoardPackages.Add(currentPackage);
                            }

                        }
                        var architectureName = FileUtils.GetBaseName(potentialArchitectureDir);
                        var currentArchitecture = currentPackage.BoardArchitectures[architectureName];
                        if (currentArchitecture == null)
                        {
                            currentArchitecture = new BoardArchitecture {Name = architectureName};
                            currentPackage.BoardArchitectures.Add(currentArchitecture);
                            currentArchitecture.FileName = boardFile;

                        }

                        AddBoards(currentArchitecture);
                    }
                }
            }
            
        }

        private void AddBoards(BoardArchitecture boardArchitecture)
        {
            
        }
    }
}
