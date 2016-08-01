using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Utilities
{

    // todo: http://msdn.microsoft.com/en-us/library/system.diagnostics.process.exitcode(v=vs.110).aspx
    //http://stackoverflow.com/questions/2279181/catch-another-process-unhandled-exception
    // http://social.msdn.microsoft.com/Forums/vstudio/en-US/62259e21-3280-4d10-a27c-740d35efe51c/catch-another-process-unhandled-exception?forum=csharpgeneral
    public class ProgressEventArgs : EventArgs
    {
        public float Progress { get; private set; }
        public ProgressEventArgs(float progress)
        {
            Progress = progress;
        }
    }

    public class ErrorEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public ErrorEventArgs(string message)
        {
            Message = message;
        }
    }

    public class ProcessStatusArgs : EventArgs
    {
        public int ExitCode { get; private set; }
        public ProcessStatusArgs(int exitCode)
        {
            ExitCode = exitCode;
        }
    }

    public class HandleExecutable {
        public DataReceivedEventHandler OutputHandler;
        public DataReceivedEventHandler ErrorOutputHandler;
        public event EventHandler<ProgressEventArgs> ProgressHandler;
        public event EventHandler<ProcessStatusArgs> ExitHandler;
        public event EventHandler<ProcessStatusArgs> NotRespondingHandler;
        public event EventHandler<ErrorEventArgs> ErrorHandler;

        private readonly BackgroundWorker _processWatcher;

        public HandleExecutable()
        {
            _processWatcher = new BackgroundWorker { WorkerSupportsCancellation = true };
            _processWatcher.DoWork += WatchProcess;
            _processWatcher.RunWorkerCompleted += WatchProcessCompleted;
        }

        public void CallExecutable(string executable, string args)
        {
            CallExecutable(executable, args, true, false);
        }

        public void CallExecutable(string executable, bool waitForExit, bool runInDir)
        {
            var execVars = executable.Split(new char[] { ' ' }, 2);
            CallExecutable(execVars[0], execVars[1], waitForExit, runInDir);
        }

        public void CallExecutable(string executable, string[] args, bool waitForExit, bool runInDir)
        {
            var argString = String.Join(" ", args);
            CallExecutable(executable, argString, waitForExit, runInDir);
        }

        public void CallExecutable(string executable, string args, bool waitForExit, bool runInDir)
        {           
            var commandLine = executable;
            Trace.WriteLine("Running command: " + executable + " " + args);
            var psi = new ProcessStartInfo(commandLine)
            {
                UseShellExecute = false,
                LoadUserProfile = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                Arguments = args  
            };
            if (runInDir)
            {
                psi.WorkingDirectory = Path.GetDirectoryName(executable);
            }
            Process = new Process {StartInfo = psi};
            try
            {
                Process.Start();
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();                
                Process.OutputDataReceived += Output;
                Process.ErrorDataReceived  += OutputError;
                
                // Watch process for not reponding
                _processWatcher.RunWorkerAsync();

                if (waitForExit)
                {
                    Process.WaitForExit();
                    Process.Close();
                    Process.Dispose();
                }
                else
                {
                    Process.Exited += ProcessExited;
                    
                }
            }
            catch (Exception ex)
            {
                if (ErrorHandler != null) ErrorHandler(this, new ErrorEventArgs(ex.Message));
                if (!ProcessUtils.ProcessRunning(executable))
                {
                    if (ExitHandler != null) ExitHandler(this, new ProcessStatusArgs(-1));
                }
                
            }
        }

        void ProcessExited(object sender, EventArgs e)
        {
            if (ExitHandler != null) ExitHandler(sender, new ProcessStatusArgs(Process.ExitCode));
            _processWatcher.CancelAsync();
        }

        public Process Process { get; private set; }
        public void Close()
        {
            Process.Close();
            Process.Dispose();
        }

        private void WatchProcess(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            if (worker == null)
            {
                e.Cancel = true;
                return;
            }

            while (!worker.CancellationPending && !Process.HasExited)
            {
                try
                {
                    if (!Process.Responding)
                    {
                        if (NotRespondingHandler != null) NotRespondingHandler(sender, new ProcessStatusArgs(0));
                        Process.WaitForExit(1000);
                    }
                }
                catch{}
            }
            e.Cancel = true;
        }

       
        private void WatchProcessCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Error != null))
                Console.WriteLine(@"Unexpected error");            
        }

        private void Output(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {            
            if (string.IsNullOrEmpty(dataReceivedEventArgs.Data)) return;

            var output = dataReceivedEventArgs.Data;
            if (output.StartsWith("Progress:", System.StringComparison.CurrentCultureIgnoreCase))
            {
                output = output.Replace("Progress:", "");
                float progress;
                var result = float.TryParse(output, out progress);
                if (result && (progress >= 0.0f && progress <= 1.0f))
                {
                    // Fire progress event
                    var progressEventArgs = new ProgressEventArgs(progress);
                    if (ProgressHandler != null) ProgressHandler(sender, progressEventArgs);
                    return;
                }
            }            
            // Fire Output event
            if (OutputHandler != null) OutputHandler(sender, dataReceivedEventArgs);            
        }

        private void OutputError(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (String.IsNullOrEmpty(dataReceivedEventArgs.Data)) return;
            // Fire OutputError event
            if (ErrorOutputHandler != null) ErrorOutputHandler(sender, dataReceivedEventArgs);
        }
    }


} 
