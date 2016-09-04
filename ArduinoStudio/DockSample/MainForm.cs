using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using ArduinoWrapper;

namespace ArduinoStudio
{
    public partial class MainForm : Form
    {
        private ArduinoEnvironments _arduinoEnvironments;
        private ArduinoEnvironment _arduinoEnvironment;
        private bool _showSplash;
        private SplashScreen _splashScreen;
        private bool _bSaveLayout = true;
        private readonly DeserializeDockContent _deserializeDockContent;
        private OutputWindow _outputWindow;
        private PropertyWindow _propertyWindow;
        private SolutionExplorer _solutionExplorer;
        private TaskList _taskList;
        private Toolbox _toolbox;

        public MainForm()
        {
            _arduinoEnvironments = new ArduinoEnvironments();
            //_arduinoEnvironment = _arduinoEnvironments.ArduinoIdeList[0];

            InitializeComponent();

            SetSplashScreen();
            CreateStandardControls();

            _deserializeDockContent = GetContentFromPersistString;


            vS2012ToolStripExtender1.VS2013Renderer = _vs2013ToolStripRenderer;

            topBar.BackColor = bottomBar.BackColor = Color.FromArgb(0xFF, 41, 57, 85);

            SetSchema();
        }

        private void MainFormSizeChanged(object sender, EventArgs e)
        {
            ResizeSplash();
        }

        #region Methods

        private IDockContent FindSketch(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (var form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            foreach (var content in dockPanel.Documents)
                if (content.DockHandler.TabText == text)
                    return content;

            return null;
        }

        private DocumentWindow CreateNewDocument()
        {
            var dummyDoc = new DocumentWindow();

            var count = 1;
            var text = "Document" + count;
            while (FindSketch(text) != null)
            {
                count++;
                text = "Document" + count;
            }
            dummyDoc.Text = text;
            return dummyDoc;
        }

        private DocumentWindow CreateNewDocument(string text)
        {
            var dummyDoc = new DocumentWindow();
            dummyDoc.Text = text;
            return dummyDoc;
        }

        private void CloseAllDocuments()
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (var form in MdiChildren)
                    form.Close();
            }
            else
            {
                foreach (var document in dockPanel.DocumentsToArray())
                {
                    document.DockHandler.Close();
                }
            }
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(SolutionExplorer).ToString())
                return _solutionExplorer;
            if (persistString == typeof(PropertyWindow).ToString())
                return _propertyWindow;
            if (persistString == typeof(Toolbox).ToString())
                return _toolbox;
            if (persistString == typeof(OutputWindow).ToString())
                return _outputWindow;
            if (persistString == typeof(TaskList).ToString())
                return _taskList;
            // DummyDoc overrides GetPersistString to add extra information into persistString.
            // Any DockContent may override this value to add any needed information for deserialization.

            var parsedStrings = persistString.Split(',');
            if (parsedStrings.Length != 3)
                return null;

            if (parsedStrings[0] != typeof(DocumentWindow).ToString())
                return null;

            var dummyDoc = new DocumentWindow();
            if (parsedStrings[1] != string.Empty)
                dummyDoc.FileName = parsedStrings[1];
            if (parsedStrings[2] != string.Empty)
                dummyDoc.Text = parsedStrings[2];

            return dummyDoc;
        }

        private void CloseAllContents()
        {
            // we don't want to create another instance of tool window, set DockPanel to null
            _solutionExplorer.DockPanel = null;
            _propertyWindow.DockPanel = null;
            _toolbox.DockPanel = null;
            _outputWindow.DockPanel = null;
            _taskList.DockPanel = null;

            // Close all other document windows
            CloseAllDocuments();
        }


        private readonly ToolStripRenderer _vs2013ToolStripRenderer = new Vs2013ToolStripRenderer();


        private void SetSchema()
        {
            dockPanel.Theme = vS2013BlueTheme1;
            EnableVSRenderer(VSToolStripExtender.VsVersion.Vs2013);
        }


        private void EnableVSRenderer(VSToolStripExtender.VsVersion version)
        {
            vS2012ToolStripExtender1.SetStyle(mainMenu, version);
            vS2012ToolStripExtender1.SetStyle(toolBar, version);
            vS2012ToolStripExtender1.SetStyle(statusBar, version);
        }

   
        #endregion

        #region Event Handlers

        private void MenuItemExitClick(object sender, EventArgs e)
        {
            Close();
        }

        private void MenuItemSolutionExplorerClick(object sender, EventArgs e)
        {
            _solutionExplorer.Show(dockPanel);
        }

        private void MenuItemPropertyWindowClick(object sender, EventArgs e)
        {
            _propertyWindow.Show(dockPanel);
        }

        private void MenuItemToolboxClick(object sender, EventArgs e)
        {
            _toolbox.Show(dockPanel);
        }

        private void MenuItemOutputWindowClick(object sender, EventArgs e)
        {
            _outputWindow.Show(dockPanel);
        }

        private void MenuItemTaskListClick(object sender, EventArgs e)
        {
            _taskList.Show(dockPanel);
        }

        private void MenuItemAboutClick(object sender, EventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog(this);
        }

        private void MenuItemNewClick(object sender, EventArgs e)
        {
            var dummyDoc = CreateNewDocument();
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                dummyDoc.MdiParent = this;
                dummyDoc.Show();
            }
            else
                dummyDoc.Show(dockPanel);
        }

        private void MenuItemOpenClick(object sender, EventArgs e)
        {
            //Todo this is an initial test.
            // In the future:
            // - A sketch shall not be directly compiled, but added to to project
            // - A window shall not be opened based on sketch, but per device
            // - re-investigate if MDI setup should be used

            var openFile = new OpenFileDialog();

            openFile.InitialDirectory = Application.ExecutablePath;
            openFile.Filter = "Arduino sketches (*.ino)|*.ino|Arduino sketches pre-1.0 (*.pde)|*.txt|All files (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var fullName = openFile.FileName;
                var fileName = Path.GetFileName(fullName);

                // todo management of multiple sketches should not happen in UI
                if (FindSketch(fileName) != null)
                {
                    MessageBox.Show("The sketch: " + fileName + " has already been opened!");
                    return;
                }


                // Create window
                var compileUploadWindow = new CompileUploadWindow() {Text = fileName};
                compileUploadWindow.Show(dockPanel);
                try
                {
                    // Try interaction with window. May already been closed?
                    //dummyDoc.FileName = fullName;
                }
                catch (Exception exception)
                {
                    compileUploadWindow.Close();
                    MessageBox.Show(exception.Message);
                }


                // Create compiler environment
                if (_arduinoEnvironments.ArduinoIdeList.Count > 0)
                {
                    _arduinoEnvironment = _arduinoEnvironments.ArduinoIdeList[0];
                    _arduinoEnvironment.OutputHandler += delegate(object o, DataReceivedEventArgs args)
                    {
                        if (this.InvokeRequired)
                        {
                            Invoke(new MethodInvoker(() => { compileUploadWindow.LogMessage(args.Data); }));

                        }
                        else
                        {
                            compileUploadWindow.LogMessage(args.Data);
                        }
                    };
                }

                // Start Compilation (or "Verify" in Arduino-speak)
                _arduinoEnvironment.Verify(fullName);

            }
        }

        private void ArduinoEnvironmentOutput(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuItemFilePopup(object sender, EventArgs e)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                menuItemClose.Enabled =
                    menuItemCloseAll.Enabled =
                        menuItemCloseAllButThisOne.Enabled = ActiveMdiChild != null;
            }
            else
            {
                menuItemClose.Enabled = dockPanel.ActiveDocument != null;
                menuItemCloseAll.Enabled =
                    menuItemCloseAllButThisOne.Enabled = dockPanel.DocumentsCount > 0;
            }
        }

        private void MenuItemCloseClick(object sender, EventArgs e)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
                ActiveMdiChild.Close();
            else if (dockPanel.ActiveDocument != null)
                dockPanel.ActiveDocument.DockHandler.Close();
        }

        private void MenuItemCloseAllClick(object sender, EventArgs e)
        {
            CloseAllDocuments();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            var configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            if (File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, _deserializeDockContent);
        }

        private void MainFormClosing(object sender, CancelEventArgs e)
        {
            var configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (_bSaveLayout)
                dockPanel.SaveAsXml(configFile);
            else if (File.Exists(configFile))
                File.Delete(configFile);
        }

        private void MenuItemToolBarClick(object sender, EventArgs e)
        {
            toolBar.Visible = menuItemToolBar.Checked = !menuItemToolBar.Checked;
        }

        private void MenuItemStatusBarClick(object sender, EventArgs e)
        {
            statusBar.Visible = menuItemStatusBar.Checked = !menuItemStatusBar.Checked;
        }

        private void ToolBarButtonClick(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == toolBarButtonNew)
                MenuItemNewClick(null, null);
            else if (e.ClickedItem == toolBarButtonOpen)
                MenuItemOpenClick(null, null);
            else if (e.ClickedItem == toolBarButtonSolutionExplorer)
                MenuItemSolutionExplorerClick(null, null);
            else if (e.ClickedItem == toolBarButtonPropertyWindow)
                MenuItemPropertyWindowClick(null, null);
            else if (e.ClickedItem == toolBarButtonToolbox)
                MenuItemToolboxClick(null, null);
            else if (e.ClickedItem == toolBarButtonOutputWindow)
                MenuItemOutputWindowClick(null, null);
            else if (e.ClickedItem == toolBarButtonTaskList)
                MenuItemTaskListClick(null, null);
        }

        private void MenuItemNewWindowClick(object sender, EventArgs e)
        {
            var newWindow = new MainForm();
            newWindow.Text = newWindow.Text + " - New";
            newWindow.Show();
        }


        private void MenuItemLayoutByCodeClick(object sender, EventArgs e)
        {
            dockPanel.SuspendLayout(true);

            CloseAllContents();

            CreateStandardControls();

            _solutionExplorer.Show(dockPanel, DockState.DockRight);
            _propertyWindow.Show(_solutionExplorer.Pane, _solutionExplorer);
            _toolbox.Show(dockPanel, new Rectangle(98, 133, 200, 383));
            _outputWindow.Show(_solutionExplorer.Pane, DockAlignment.Bottom, 0.35);
            _taskList.Show(_toolbox.Pane, DockAlignment.Left, 0.4);

            var doc1 = CreateNewDocument("Document1");
            var doc2 = CreateNewDocument("Document2");
            var doc3 = CreateNewDocument("Document3");
            var doc4 = CreateNewDocument("Document4");
            doc1.Show(dockPanel, DockState.Document);
            doc2.Show(doc1.Pane, null);
            doc3.Show(doc1.Pane, DockAlignment.Bottom, 0.5);
            doc4.Show(doc3.Pane, DockAlignment.Right, 0.5);

            dockPanel.ResumeLayout(true, true);
        }

        private void SetSplashScreen()
        {
            _showSplash = true;
            _splashScreen = new SplashScreen();

            ResizeSplash();
            _splashScreen.Visible = true;
            _splashScreen.TopMost = true;

            var timer = new Timer();
            timer.Tick += (sender, e) =>
            {
                _splashScreen.Visible = false;
                timer.Enabled = false;
                _showSplash = false;
            };
            timer.Interval = 4000;
            timer.Enabled = true;
        }

        private void ResizeSplash()
        {
            if (_showSplash)
            {
                var centerXMain = (Location.X + Width)/2.0;
                var locationXSplash = Math.Max(0, centerXMain - _splashScreen.Width/2.0);

                var centerYMain = (Location.Y + Height)/2.0;
                var locationYSplash = Math.Max(0, centerYMain - _splashScreen.Height/2.0);

                _splashScreen.Location = new Point((int) Math.Round(locationXSplash), (int) Math.Round(locationYSplash));
            }
        }

        private void CreateStandardControls()
        {
            _solutionExplorer = new SolutionExplorer();
            _propertyWindow = new PropertyWindow();
            _toolbox = new Toolbox();
            _outputWindow = new OutputWindow();
            _taskList = new TaskList();
        }

        private void MenuItemLayoutByXmlClick(object sender, EventArgs e)
        {
            dockPanel.SuspendLayout(true);

            // In order to load layout from XML, we need to close all the DockContents
            CloseAllContents();

            CreateStandardControls();

            var assembly = Assembly.GetAssembly(typeof(MainForm));
            var xmlStream = assembly.GetManifestResourceStream("DockSample.Resources.DockPanel.xml");
            if (xmlStream != null)
            {
                dockPanel.LoadFromXml(xmlStream, _deserializeDockContent);
                xmlStream.Close();
            }

            dockPanel.ResumeLayout(true, true);
        }

        private void MenuItemCloseAllButThisOneClick(object sender, EventArgs e)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                var activeMdi = ActiveMdiChild;
                foreach (var form in MdiChildren)
                {
                    if (form != activeMdi)
                        form.Close();
                }
            }
            else
            {
                foreach (var document in dockPanel.DocumentsToArray())
                {
                    if (!document.DockHandler.IsActivated)
                        document.DockHandler.Close();
                }
            }
        }


        #endregion
    }
}