using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;

namespace ArduinoTestbenchBasic
{
    public class OutputLogger
    {
        private readonly LoggingView _loggingView;

        public OutputLogger(LoggingView loggingView)
        {
            _loggingView = loggingView;
        }

        public void LogMessage(string message)
        {
            _loggingView.AddEntry(message);
        }
    }
}
