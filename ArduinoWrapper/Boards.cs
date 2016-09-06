using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public ArduinoEnvironment Parent { get; set; }
        public string Name { get; set; }
        public BoardArchitectures BoardArchitectures { get; set; }

        public BoardPackage()
        {
            BoardArchitectures = new BoardArchitectures();
        }

        public override string ToString()
        {
            return Name;
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
        public BoardPackage Parent { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public List<BoardDescription>  BoardDescriptions { get; set; }

         public BoardArchitecture()
         {
             BoardDescriptions = new BoardDescriptions();
         }

        public override string ToString()
        {
            return Name;
        }
    }


    public class BoardDescriptions :  List<BoardDescription>
    {
        public BoardDescription this[string name]
        {
           get { return this.FirstOrDefault(b => b.Name == name); }
        }
    }

    /// <summary>
    /// All info pertaining to the given board (Arduino physical device) needed for compilation
    /// </summary>
    public class BoardDescription
    {
        public BoardArchitecture Parent { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BoardOptionSet BoardOptions{ get; set; }
        public string UploadSpeed { get; set; }

        public BoardDescription()
        {
            BoardOptions = new BoardOptionSet();
        }

        public override string ToString()
        {
            return Description;
        }

        public string CompileArguments()
        {
            // Extent to handling options!
            return string.Format("{0}:{1}:{2}", Parent.Parent.Name, Parent.Name, Name);
        }
    }


    /// <summary>
    /// List with all board options
    /// </summary>
    public class BoardOptionSet : List<BoardOption>
    {
        public BoardOption this[string name]
        {            
            get { return this.FirstOrDefault(b => b.Name == name); }            
        }
    }

    /// <summary>
    /// Single board option (e.g. cpu, speed, ...). Note that 'Name' is also the key in the BoardOptionSet dictionary.
    /// </summary>
    public class BoardOption : List<BoardOptionValue>
    {
        public string Name { get; }
        public string Description { get; set; }

        public BoardOption(string _name, string _description)
        {
            Name = _name;
            Description = _description;
        }

        public BoardOptionValue this[string value]
        {
            get { return this.FirstOrDefault(b => b.Value == value); }
        }

        public override string ToString()
        {
            return Description;
        }
    }

    /// <summary>
    /// Option value, including a possible upload speed setting connected to this option.
    /// </summary>
    public class BoardOptionValue
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string UploadSpeed { get; set; }
        public BoardOption Parent { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }



    public class Boards
    {
        private readonly string _rootPath;
        private readonly string _userPath;
        private ArduinoConfigReader _arduinoConfigReader;

        public BoardPackages BoardPackages { get; set; }

        public Boards(string rootPath, string userPath)
        {
            _rootPath = rootPath;
            _userPath = userPath;
            BoardPackages = new BoardPackages();
            //var t = FindBoardFiles();
            _arduinoConfigReader = new ArduinoConfigReader();
        }

        public BoardPackages FindBoardFiles(ArduinoEnvironment arduinoEnvironment)
        {
            BoardPackages.Clear();
            AddExeBoardFiles(_rootPath, arduinoEnvironment);
            AddUserBoardFiles(_userPath, arduinoEnvironment);

            // Board name as used by compiler
            var boardname = BoardPackages[0].BoardArchitectures[0].BoardDescriptions[0].ToString();
            return BoardPackages;
        }


        public void AddExeBoardFiles(string rootPath, ArduinoEnvironment arduinoEnvironment)
        {
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
                            currentPackage = new BoardPackage {Name = packageName, Parent = arduinoEnvironment};
                            BoardPackages.Add(currentPackage);
                        }
                    }
                    var architectureName = FileUtils.GetBaseName(potentialArchitectureDir);
                    var currentArchitecture = currentPackage.BoardArchitectures[architectureName];
                    if (currentArchitecture == null)
                    {
                        currentArchitecture = new BoardArchitecture
                        {
                            Name = architectureName,
                            Parent = currentPackage
                        };
                        currentPackage.BoardArchitectures.Add(currentArchitecture);
                        currentArchitecture.FileName = boardFile;
                    }
                    AddBoards(currentArchitecture);
                }
            }
        }

        public void AddUserBoardFiles(string userPath, ArduinoEnvironment arduinoEnvironment)
        {
            // All boards dirs 
            var boardsPath = FileUtils.Combine(userPath, "Arduino15\\packages");
            if (Directory.Exists(boardsPath))
            {

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
                                currentArchitecture = new BoardArchitecture
                                {
                                    Name = architectureName,
                                    Parent = currentPackage,

                                };
                                currentPackage.BoardArchitectures.Add(currentArchitecture);
                                currentArchitecture.FileName = boardFile;
                            }
                            AddBoards(currentArchitecture);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Add boards in config file to the architecture description
        /// </summary>
        /// <param name="boardArchitecture"></param>
        private void AddBoards(BoardArchitecture boardArchitecture)
        {
            // Read config file
            var boardsConfig = new ArduinoConfigReader(boardArchitecture.FileName);
            // Get description of all available menu options from config file
            Dictionary<string, string> menu_options = GetMenuOptions(boardsConfig);
            // Extract boards and their extra options
            foreach (var keyvalue in boardsConfig.Dictionary)
            {
                var key = keyvalue.Key;
                var value = keyvalue.Value;
                if (key.Path[1] == "name")
                {
                    // Create board
                    var board = new BoardDescription
                    {
                        Description = value,
                        Name = key.Path[0],
                        Parent = boardArchitecture,
                        UploadSpeed = boardsConfig.Dictionary[string.Format("{0}.upload.speed", key.Path[0])]
                    };
                    // Add it to the description list
                    boardArchitecture.BoardDescriptions.Add(board);
                    // Add extra options to board
                    AddBoardOptions(boardsConfig, board, menu_options);
                }
            }
        }


        /// <summary>
        /// Get menu options and their descriptions from the config file
        /// </summary>
        /// <param name="boardsConfig"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetMenuOptions(ArduinoConfigReader boardsConfig)
        {
            Dictionary<string, string> menu_options = new Dictionary<string, string>();
            Regex menu_opts = new Regex(@"^menu\.(.*)$", RegexOptions.Compiled);
            foreach (var keyvalue in boardsConfig.Dictionary)
            {
                Match m = menu_opts.Match(keyvalue.Key.Key);
                if (m.Success)
                {
                    menu_options.Add(m.Groups[1].Value, keyvalue.Value);
                }
            } // EOF get descriptions of all available menu options
            return menu_options;
        }


        /// <summary>
        /// Find and add options for the given board
        /// </summary>
        /// <param name="boardsConfig"></param>
        /// <param name="board"></param>
        /// <param name="menu_options"></param>
        private void AddBoardOptions(ArduinoConfigReader boardsConfig, BoardDescription board,
            Dictionary<string, string> menu_options)
        {
            // RegEx to find [board name].menu.[option].[option value] keys
            Regex opts = new Regex(string.Format(@"^{0}\.menu\.([^\.]+)\.([^\.]+)$", board.Name), RegexOptions.Compiled);
            foreach (var keyvalue in boardsConfig.Dictionary)
            {
                Match m = opts.Match(keyvalue.Key.Key);
                if (m.Success)
                {
                    // Create BoardOption when needed, add option value.
                    BoardOption board_option;
                    string name = m.Groups[1].Value;
                    board_option = board.BoardOptions[name];
                    if (board_option == null)
                    {
                        // Create board option, including description of option (if available)
                        board_option = new BoardOption(name, menu_options.ContainsKey(name) ? menu_options[name] : name);
                        board.BoardOptions.Add(board_option);
                    }
                    // Add option value, including a potentially defined upload speed
                    board_option.Add(new BoardOptionValue
                    {
                        Value = m.Groups[2].Value,
                        Description = keyvalue.Value,
                        UploadSpeed = boardsConfig.Dictionary[string.Format("{0}.upload.speed", keyvalue.Key.Key)],
                        Parent = board_option
                    });
                }
            }
        } // EOF AddBoardOptions
    }
}

