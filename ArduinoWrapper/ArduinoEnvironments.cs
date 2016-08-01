using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IniParser.Model;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace ArduinoWrapper
{
    public class ArduinoEnvironments
    {
        public string UserPath { get; set; }

        public List<ArduinoEnvironment> ArduinoIdeList { get; set; }

        public ArduinoEnvironments()
        {
            ArduinoIdeList = new List<ArduinoEnvironment>();
            var arduinoLocations = LocateArduinoIde();
            foreach (var arduinoLocation in arduinoLocations)
            {
                Add(arduinoLocation);
            }
        }

        public void Add(string location)
        {
            ArduinoIdeList.Add(new ArduinoEnvironment(location));
        }

        private List<string> LocateArduinoIde()
        {
            var locations = new List<string>();
            
            var location = LocateArduinoIdeFromStartMenu();
            if (!string.IsNullOrEmpty(location)) locations.Add(location);
            locations.AddRange(LocateArduinoIdeFromDefaultLocation());
            return locations.Distinct().ToList();
        }
        private string LocateArduinoIdeFromStartMenu()
        {
            const string linkPathName = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Arduino.lnk";
            if (File.Exists(linkPathName))
            {             
                var shell = new WshShell(); //Create a new WshShell Interface
                var link = (IWshShortcut)shell.CreateShortcut(linkPathName); //Link the interface to our shortcut
                var path = link.TargetPath.ToLowerInvariant();
                return (File.Exists(path) && Path.GetExtension(path)==".exe")? path : ""; 
            }
            return "";
        }

        private List<string>  LocateArduinoIdeFromDefaultLocation()
        {
            var locations = new List<string>();

            var locationList = new List<string>
            {
                @"C:\Program Files (x86)\Arduino\Arduino.exe",
                @"C:\Program Files\Arduino\Arduino.exe",
                @"D:\Program Files (x86)\Arduino\Arduino.exe",
                @"D:\Program Files\Arduino\Arduino.exe",
                @"E:\Program Files (x86)\Arduino\Arduino.exe",
                @"E:\Program Files\Arduino\Arduino.exe",
            };

            foreach (var location in locationList)
            {
                var locationLower = location.ToLowerInvariant();
                if (File.Exists(locationLower))  locations.Add(locationLower);
            }
            return locations;
        }

        private string LocateUserPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

    }
}
