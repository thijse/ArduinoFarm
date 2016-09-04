using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArduinoWrapper
{

    public class ArduinoKey
    {
        private string _key;
        private string[] _path;
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                _path = _key.Split('.');
            } 
        }

        public string[] Path
        {
            get { return _path; }
            set
            {
                _path = value;
                _key = string.Join(".",_path);
            } 
        }


        public ArduinoKey(string key)
        {
            Key = key;
        }

        public ArduinoKey(string[] path)
        {
            Path = path;
        }

        public static explicit operator ArduinoKey(string key)
        {
            return new ArduinoKey(key);
        }

        public static explicit operator ArduinoKey(string[] key)
        {
            return new ArduinoKey(key);
        }

        public static explicit operator string(ArduinoKey arduinoKey)
        {
            return arduinoKey.Key;
        }

        public static explicit operator string[] (ArduinoKey arduinoKey)
        {
            return arduinoKey.Path;
        }
    }

    public class ArduinoConfig : Dictionary<ArduinoKey, string>
    {
        public string this[string key]
        {
            // The get accessor.
            get
            {
                return this.FirstOrDefault(i => i.Key.Key == key).Value;
            }

            // The set accessor.
            set
            {
                var arduinoKey = new ArduinoKey(key);
                this[arduinoKey] = value;
            }
        }


        public string this[string[] path]
        {
            // The get accessor.
            get
            {
                var key = string.Join(".", path);
                return this.First(i => i.Key.Key == key).Value;
            }

            // The set accessor.
            set
            {
                var arduinoKey = new ArduinoKey(path);
                this[arduinoKey] = value;
            }
        }
    }

    public class ArduinoConfigReader 
    {
        public ArduinoConfig Dictionary { get; private set; }

        private const char CommentDelimiter  = '#';
        private const char KeyValueSeparator = '=';

        public ArduinoConfigReader()
        {
            Dictionary = new ArduinoConfig();
        }

        public ArduinoConfigReader(string file)
        {
            Dictionary = new ArduinoConfig();
            Load(file);
        }

        public void Load(string file)
        {
            Dictionary.Clear();

            if (string.IsNullOrEmpty(file) || !File.Exists(file)) return;

            using (var fileStream = new StreamReader(file))
            {
                string line;
                while ((line = fileStream.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line)) continue;                                                             // skip empty line
                    if (line[0] == CommentDelimiter) continue;                                                            // skip if line starts with comment
                    line = (line.Split(new []{CommentDelimiter}, 2, StringSplitOptions.RemoveEmptyEntries)[0]).Trim();    // remove comment from line
                    if (string.IsNullOrEmpty(line)) continue;                                                             // skip empty line
                    var keyvaluepair = line.Split(new []{KeyValueSeparator}, 2, StringSplitOptions.RemoveEmptyEntries);   // split in key & value
                    var key = keyvaluepair[0].Trim();                                                                     // trim key
                    if (string.IsNullOrEmpty(key)) continue;                                                              // skip if key is empty
                    var value =(keyvaluepair.Length==2)?keyvaluepair[1].Trim():"";                                        // add empty string value if not existent
                    //Console.WriteLine("key:\"{0}\", value:\"{1}\"",key,value);
                    Dictionary.Add((ArduinoKey)key,value);                                                                // add to dictionary
                }
            }
        }
    }
}
