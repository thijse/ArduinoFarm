using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArduinoWrapper;


namespace ArduinoTestbenchBasic
{
    public partial class Form1 : Form
    {
        private ArduinoEnvironments _arduinoEnvironments;
        private ArduinoEnvironment _arduinoEnvironment;

        public Form1()
        {
            InitializeComponent();
            _arduinoEnvironments = new ArduinoEnvironments();
            _arduinoEnvironment = _arduinoEnvironments.ArduinoIdeList[0];
            _arduinoEnvironment.OutputHandler += ArduinoEnvironmentOutput;
        }

        private void ArduinoEnvironmentOutput(object sender, DataReceivedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(() => { LogMessage(e.Data); }));
                
            }
            else
            {
                LogMessage(e.Data);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _arduinoEnvironment.Verify(@"C:\Program Files (x86)\Arduino\examples\01.Basics\Blink\Blink.ino");

        }

        private void LogMessage(string message)
        {
            listBox1.AddEntry(message);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //new ArduinoConfigReader(@"E:\ProgramFiles\Arduino\arduino-1.0.1\hardware\arduino\boards.txt");

            var test = _arduinoEnvironment.GetBoards();

        }
    }
}
