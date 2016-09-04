using System;
using System.Collections.Generic;
using System.Management;

/// <summary>
/// Class that matches boards to COM ports. Example usage:
/// 
/// Identify all boards (serial ports) in the system and return them as a list of BoardIdentifiers. The
/// 'ToString()' implementation will show "(COMnn): (Board description)"
/// List<BoardIdentifier> boards = BoardWizard.ListBoards();
/// 
/// Board identifiers are stored by the user, together with e.g. configuration parameters for the boards.
/// The board identifiers can later be matched to (possibly changed) com ports using:
/// string[] coms = BoardWizard.BoardsToPorts(boards);
/// 
/// 'coms' is an array of strings with 'COMnn' port identifiers, or null when no match was found. Additionally,
/// the 'CurrentCOM' property of each BoardIdentifier in boards is updated (null or updated value).
/// </summary>
namespace ArduinoWrapper
{    
    /// <summary>
    /// Class containing fields that (more-or-less) uniquely define a port.
    /// </summary>
    public class BoardIdentifier
    {
        // Device description: should be a perfect match
        public string Description { get; }
        // Manufacturer: should be a perfect match (note: is also encoded in DeviceId as VID_.... (vendor id))
        public string Manufacturer { get; }
        // DeviceId: some combination of device type, vendor id, some code. For well-behaved (having a UniqueId) devices this Id is preserved when
        // changing the USB port of the device. For less-well behaved devices, the last part changes (some windows-assigned unique id). And the
        // COM port number changes then also...
        public string DeviceId { get; }
        /// <summary>
        /// Last known COM port
        /// </summary>
        public string LastKnownCOM { get; set; }
        /// <summary>
        /// CURRENT COM port
        /// </summary>
        public string CurrentCOM { get; set;  }

        /// <summary>
        /// Constructor. Parameters are obvious...
        /// </summary>
        /// <param name="description"></param>
        /// <param name="com_port"></param>
        /// <param name="manufacturer"></param>
        /// <param name="device_id"></param>
        public BoardIdentifier(string description, string com_port, string manufacturer, string device_id)
        {
            Description = description;
            CurrentCOM = com_port;
            LastKnownCOM = com_port;
            Manufacturer = manufacturer;
            DeviceId = device_id;            
        }


        /// <summary>
        /// Human-readable version: COM port: description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", (CurrentCOM == null) ? "???" : CurrentCOM, Description);
        }
    }


    /// <summary>
    /// Static class to:
    /// - List all boards (serial ports) in the system
    /// - Match previously found ports (as identified by BoardIdentifier) to current COM ports
    /// </summary>
    public static class BoardWizard
    {
        /// <summary>
        /// Private: maximum cost value for mis-matching devices
        /// </summary>
        private static int MaxCost = 10000;

        /// <summary>
        /// Return list of all serial (com) ports in the system. Uses WMI queries. Note that Win32_SerialPort does not list all
        /// (USB) serial ports in the system. I am using a 'hack' from https://dariosantarelli.wordpress.com/2010/10/18/c-how-to-programmatically-find-a-com-port-by-friendly-name/
        /// to get all ports by browsing through 'Win32_PnPEntity and looking for devices with 'COM' in the caption. This seems
        /// to return the complete list.
        /// </summary>
        /// <returns>
        /// List of PortIdentifiers. The 'ToString()' method returns a human-readable port number - device description combination. 
        /// </returns>
        public static List<BoardIdentifier> ListBoards()
        {
            List<BoardIdentifier> devices = new List<BoardIdentifier>();
                        
            ConnectionOptions options = ProcessConnection.ProcessConnectionOptions();
            ManagementScope connectionScope = ProcessConnection.ConnectionScope(Environment.MachineName, options, @"\root\CIMV2");
            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, objectQuery);
            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject device in collection)
            {
                if (device == null) continue;
                object cap = device["Caption"];
                if (cap == null) continue;
                string caption = cap.ToString();
                if (!caption.Contains("(COM")) continue;
                string port = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);

                PropertyDataCollection pd = device.Properties;
                devices.Add(new BoardIdentifier(device["Description"].ToString(), port, device["Manufacturer"].ToString(), device["DeviceID"].ToString()));
            }
            return devices;
        }


        /// <summary>
        /// Match a single port. Less powerfull than using a list of identifiers, with multiple close matches we may not be able to guess the best match.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static string MatchBoard(BoardIdentifier identifier)
        {
            string[] result = BoardsToPorts(new BoardIdentifier[] { identifier });
            return result[0];
        }


        /// <summary>
        /// Find best match between given port identifiers and current ports. This function
        /// changes the BoardIdentifiers:
        /// - CurrentCOM is updated to newly found com port (null or COMnn)
        /// - LastKnownCOM is updated ONLY when a new com port is found
        /// </summary>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        public static string[] BoardsToPorts(IList<BoardIdentifier> identifiers)
        {
            // Get current port list;
            List<BoardIdentifier> p_current = BoardWizard.ListBoards();
            // Try to find best matches by a cost-minimization algorithm
            int na = identifiers.Count;
            int nb = p_current.Count;
            int n = Math.Max(na, nb);
            int[,] cost = new int[n, n];                        
            // Create cost matrix. Making impossible matches (na>nb, nb>na) is maximum unattractive. And
            // we kick out any 'MaxCost' results anyway.
            for (int a = 0; a < n; a++)
            {
                for (int b = 0; b < n; b++)
                {
                    cost[a, b] = (a < na && b < nb) ? CostFunction(identifiers[a], p_current[b]) : BoardWizard.MaxCost;
                }
            }
            // Find matches
            HungarianAlgorithm ha = new HungarianAlgorithm(cost);
            int[] matches = ha.Run();
            // Create output
            string[] result = new string[na];
            for (int a = 0; a < na; a++)
            {
                // Find matching index in 'p_current'
                int b = matches[a];
                // Get comport from p_current
                string com = (cost[a, b] != BoardWizard.MaxCost) ? p_current[b].CurrentCOM : null;
                // Export result in string array
                result[a] = com;
                // Update BoardIdentifier in 'identifiers'
                identifiers[a].CurrentCOM = com;
                if (com != null)
                {
                    identifiers[a].LastKnownCOM = com;
                }
            }
            // Done!
            return result;
        }


        /// <summary>
        /// Determine goodness of match between two port identifiers. Logic:
        /// - Name and manufacturerer should match, otherwise maximum penalty
        /// - COM port may not match, then +1 penalty
        /// - Any non-matching characters in DeviceId count as cost. May not be very good, as long names
        ///   may have more mismatching characters than short names...
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int CostFunction(BoardIdentifier a, BoardIdentifier b)
        {
            // Check for matching description an manufacturer
            if (a.Description != b.Description || a.Manufacturer != b.Manufacturer)
            {   // Total mismatch!
                return BoardWizard.MaxCost;
            }
            // Get maximum string length
            int max_len = Math.Max(a.DeviceId.Length, b.DeviceId.Length);
            return
                ((a.LastKnownCOM != b.CurrentCOM) ? 1 : 0) +            // Cost 1 for mismatching COM port
                (max_len - FirstMismatch(a.DeviceId, b.DeviceId));
        }


        /// <summary>
        /// Find index of first mismatch (may be end of string)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int FirstMismatch(string a, string b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                if (i >= a.Length || a[i] != b[i])
                {
                    return i;
                }
            }
            return b.Length;
        }


        /// <summary>
        /// From https://dariosantarelli.wordpress.com/2010/10/18/c-how-to-programmatically-find-a-com-port-by-friendly-name/.
        /// Connection options to WMI interface.
        /// </summary>
        internal class ProcessConnection
        {

            public static ConnectionOptions ProcessConnectionOptions()
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Impersonation = ImpersonationLevel.Impersonate;
                options.Authentication = AuthenticationLevel.Default;
                options.EnablePrivileges = true;
                return options;
            }

            public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options, string path)
            {
                ManagementScope connectScope = new ManagementScope();
                connectScope.Path = new ManagementPath(@"\\" + machineName + path);
                connectScope.Options = options;
                connectScope.Connect();
                return connectScope;
            }
        }



    }
}
