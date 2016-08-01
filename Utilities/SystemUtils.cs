using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace Utilities
{
    public class SystemUtils
    {
        static public string GetUserNameUsingWmi()
        {
            var searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            return (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
        }

        static public string GetUserName()
        {
            var userName = GetUserNameUsingWmi();
            userName = RemoveDomainOrComputerName(userName);
            userName = userName.Replace(" ", "_");
            return userName;
        }

        static private string RemoveDomainOrComputerName(string userName)
        {
            userName = Regex.Replace(userName, ".*\\\\(.*)", "$1", RegexOptions.None);
            return userName;
        }

    }
}
