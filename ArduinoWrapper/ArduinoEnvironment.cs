using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using IniParser.Parser;
using IWshRuntimeLibrary;
using Utilities;
using ErrorEventArgs = Utilities.ErrorEventArgs;
using File = System.IO.File;


namespace ArduinoWrapper
{
    //todo determine version based on revisions.txt

    public class ArduinoEnvironment : HandleExecutable
    {
        public string ArduinoBinPath { get; set; }
        public string ArduinoIdePath { get; set; }
        public string ArduinoCliPath { get; set; }
        public string ArduinoPrefsPath { get; set; }
        public string ArduinoSketchesPath { get; set; }
        //public IniData PreferenceData { get; set; }
        public ArduinoConfigReader PreferenceData { get; set; }
        private readonly string _userPath;
        private readonly Boards _boards;


        //private readonly HandleExecutable _handleExecutable;

        public ArduinoEnvironment(string arduinoIdePath) : base()
        {
            _userPath           = LocateUserPath();
            ArduinoIdePath      = arduinoIdePath;
            ArduinoBinPath      = Path.GetDirectoryName(ArduinoIdePath);
            ArduinoCliPath      = LocateCLIPath(arduinoIdePath);
            ArduinoPrefsPath    = LocatePreferences();
            ReadPreferences(ArduinoPrefsPath);
            ArduinoSketchesPath = LocateketchesPath();
            _boards             = new Boards(ArduinoBinPath, LocateApplicationPath());
        }

        


        private string LocateCLIPath(string arduinoIdePath)
        {
            var cli = arduinoIdePath.ToLowerInvariant().Replace("arduino.exe", "arduino_debug.exe");
            return (File.Exists(cli)) ? cli : arduinoIdePath;
        }


        private void ReadPreferences(string arduinoPrefsPath)
        {
            //var parser = new FileIniDataParser();
            //PreferenceData = parser.ReadFile(arduinoPrefsPath);
            PreferenceData = new ArduinoConfigReader(arduinoPrefsPath);
        }

        private string LocateketchesPath()
        {
            var arduinoSketchesPath = PreferenceValue("sketchbook.path");
            if (!string.IsNullOrEmpty(arduinoSketchesPath) && Directory.Exists(arduinoSketchesPath)) return arduinoSketchesPath;

            var locationList = new List<string>
            {
                FileUtils.Combine(_userPath, @"\Documents\Arduino")
            };

            foreach (var location in locationList)
            {
                if (Directory.Exists(location)) return location;
            }
            return "";
        }

   
        private string LocatePreferences()
        {
            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var subdirectoryEntries = Directory.GetDirectories(basepath);
            foreach (var subdirectoryEntry in subdirectoryEntries)
            {
                var t = (Path.GetFileName(subdirectoryEntry)??"").ToLowerInvariant();  // actually dirname
                if (t.Contains("arduino"))
                {
                    var prefs = FileUtils.Combine(subdirectoryEntry, "preferences.txt");
                    if (File.Exists(prefs)) return prefs;
                }
            }

            return "";
        }

        private string PreferenceValue(string key)
        {
            return PreferenceData.Dictionary["sketchbook.path"];
            //return PreferenceData.Global.GetKeyData("sketchbook.path").Value;
        }

        private string LocateUserPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

                private string LocateApplicationPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData );
        }

        public BoardPackages GetBoards()
        {
            return _boards.FindBoardFiles(this);
        } 




        public void Verify(string inoPath)
        {
            var arguments = string.Format("--verify \"{0}\"", inoPath);
            CallExecutable(ArduinoCliPath,arguments,false,true);       
        }


    }
}
