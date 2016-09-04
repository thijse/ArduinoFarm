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
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using Lextm.SharpSnmpLib;
using WeifenLuo.WinFormsUI.Docking;

using ArduinoWrapper;

namespace ArduinoStudio
{
    public partial class MainForm : Form
    {
        private ArduinoEnvironments _arduinoEnvironments;
        private ArduinoEnvironment _arduinoEnvironment;

        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;
        private SolutionExplorer m_solutionExplorer;
        private PropertyWindow m_propertyWindow;
        private Toolbox m_toolbox;
        private OutputWindow m_outputWindow;
        private TaskList m_taskList;
        private bool _showSplash;
        private SplashScreen _splashScreen;
        public MainForm()
        {
            _arduinoEnvironments = new ArduinoEnvironments();
            _arduinoEnvironment = _arduinoEnvironments.ArduinoIdeList[0];

            InitializeComponent();

            SetSplashScreen();
            CreateStandardControls();

            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            
            vS2012ToolStripExtender1.DefaultRenderer = _toolStripProfessionalRenderer;
            vS2012ToolStripExtender1.VS2012Renderer = _vs2012ToolStripRenderer;
            vS2012ToolStripExtender1.VS2013Renderer = _vs2013ToolStripRenderer;

            this.topBar.BackColor = this.bottomBar.BackColor = Color.FromArgb(0xFF, 41, 57, 85);

            //SetSchema(this.menuItemSchemaVS2013Blue, null);
            SetSchema();
        }

        #region Methods

        private IDockContent FindDocument(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                    if (content.DockHandler.TabText == text)
                        return content;

                return null;
            }
        }

        private Document CreateNewDocument()
        {
            Document dummyDoc = new Document();

            int count = 1;
            //string text = "C:\\MADFDKAJ\\ADAKFJASD\\ADFKDSAKFJASD\\ASDFKASDFJASDF\\ASDFIJADSFJ\\ASDFKDFDA" + count.ToString();
            string text = "Document" + count.ToString();
            while (FindDocument(text) != null)
            {
                count++;
                //text = "C:\\MADFDKAJ\\ADAKFJASD\\ADFKDSAKFJASD\\ASDFKASDFJASDF\\ASDFIJADSFJ\\ASDFKDFDA" + count.ToString();
                text = "Document" + count.ToString();
            }
            dummyDoc.Text = text;
            return dummyDoc;
        }

        private Document CreateNewDocument(string text)
        {
            Document dummyDoc = new Document();
            dummyDoc.Text = text;
            return dummyDoc;
        }

        private void CloseAllDocuments()
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    form.Close();
            }
            else
            {
                foreach (IDockContent document in dockPanel.DocumentsToArray())
                {
                    document.DockHandler.Close();
                }
            }
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(SolutionExplorer).ToString())
                return m_solutionExplorer;
            else if (persistString == typeof(PropertyWindow).ToString())
                return m_propertyWindow;
            else if (persistString == typeof(Toolbox).ToString())
                return m_toolbox;
            else if (persistString == typeof(OutputWindow).ToString())
                return m_outputWindow;
            else if (persistString == typeof(TaskList).ToString())
                return m_taskList;
            else
            {
                // DummyDoc overrides GetPersistString to add extra information into persistString.
                // Any DockContent may override this value to add any needed information for deserialization.

                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length != 3)
                    return null;

                if (parsedStrings[0] != typeof(Document).ToString())
                    return null;

                Document dummyDoc = new Document();
                if (parsedStrings[1] != string.Empty)
                    dummyDoc.FileName = parsedStrings[1];
                if (parsedStrings[2] != string.Empty)
                    dummyDoc.Text = parsedStrings[2];

                return dummyDoc;
            }
        }

        private void CloseAllContents()
        {
            // we don't want to create another instance of tool window, set DockPanel to null
            m_solutionExplorer.DockPanel = null;
            m_propertyWindow.DockPanel = null;
            m_toolbox.DockPanel = null;
            m_outputWindow.DockPanel = null;
            m_taskList.DockPanel = null;

            // Close all other document windows
            CloseAllDocuments();
        }

        private readonly ToolStripRenderer _toolStripProfessionalRenderer = new ToolStripProfessionalRenderer();
        private readonly ToolStripRenderer _vs2012ToolStripRenderer = new VS2012ToolStripRenderer();
        private readonly ToolStripRenderer _vs2013ToolStripRenderer = new Vs2013ToolStripRenderer();


        private void SetSchema()
        {
            this.dockPanel.Theme = this.vS2013BlueTheme1;
            this.EnableVSRenderer(VSToolStripExtender.VsVersion.Vs2013);
        }

        //private void SetSchema(object sender, System.EventArgs e)
        //{
        //    // Persist settings when rebuilding UI
        //    string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.temp.config");

        //    dockPanel.SaveAsXml(configFile);
        //    CloseAllContents();


        //    if (sender == this.menuItemSchemaVS2013Blue)
        //    {
        //        this.dockPanel.Theme = this.vS2013BlueTheme1;
        //        this.EnableVSRenderer(VSToolStripExtender.VsVersion.Vs2013);
        //    }

        //    menuItemSchemaVS2005.Checked = (sender == menuItemSchemaVS2005);
        //    menuItemSchemaVS2003.Checked = (sender == menuItemSchemaVS2003);
        //    menuItemSchemaVS2012Light.Checked = (sender == menuItemSchemaVS2012Light);
        //    this.menuItemSchemaVS2013Blue.Checked = (sender == this.menuItemSchemaVS2013Blue);
        //    this.topBar.Visible = this.bottomBar.Visible = (sender == this.menuItemSchemaVS2013Blue);

        //    if (File.Exists(configFile))
        //        dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
        //}

        private void EnableVSRenderer(VSToolStripExtender.VsVersion version)
        {
            vS2012ToolStripExtender1.SetStyle(this.mainMenu, version);
            vS2012ToolStripExtender1.SetStyle(this.toolBar, version);
            vS2012ToolStripExtender1.SetStyle(this.statusBar, version);
        }

        //private void SetDocumentStyle(object sender, System.EventArgs e)
        //{
        //    DocumentStyle oldStyle = dockPanel.DocumentStyle;
        //    DocumentStyle newStyle;
        //    if (sender == menuItemDockingMdi)
        //        newStyle = DocumentStyle.DockingMdi;
        //    else if (sender == menuItemDockingWindow)
        //        newStyle = DocumentStyle.DockingWindow;
        //    else if (sender == menuItemDockingSdi)
        //        newStyle = DocumentStyle.DockingSdi;
        //    else
        //        newStyle = DocumentStyle.SystemMdi;

        //    if (oldStyle == newStyle)
        //        return;

        //    if (oldStyle == DocumentStyle.SystemMdi || newStyle == DocumentStyle.SystemMdi)
        //        CloseAllDocuments();

        //    dockPanel.DocumentStyle = newStyle;
        //    menuItemLayoutByCode.Enabled = (newStyle != DocumentStyle.SystemMdi);
        //    menuItemLayoutByXml.Enabled = (newStyle != DocumentStyle.SystemMdi);

        //}

        private AutoHideStripSkin _autoHideStripSkin;
        private DockPaneStripSkin _dockPaneStripSkin;

        private void SetDockPanelSkinOptions(bool isChecked)
        {
            if (isChecked)
            {
                // All of these options may be set in the designer.
                // This is not a complete list of possible options available in the skin.

                AutoHideStripSkin autoHideSkin = new AutoHideStripSkin();
                autoHideSkin.DockStripGradient.StartColor = Color.AliceBlue;
                autoHideSkin.DockStripGradient.EndColor = Color.Blue;
                autoHideSkin.DockStripGradient.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
                autoHideSkin.TabGradient.StartColor = SystemColors.Control;
                autoHideSkin.TabGradient.EndColor = SystemColors.ControlDark;
                autoHideSkin.TabGradient.TextColor = SystemColors.ControlText;
                autoHideSkin.TextFont = new Font("Showcard Gothic", 10);

                _autoHideStripSkin = dockPanel.Skin.AutoHideStripSkin;
                dockPanel.Skin.AutoHideStripSkin = autoHideSkin;

                DockPaneStripSkin dockPaneSkin = new DockPaneStripSkin();
                dockPaneSkin.DocumentGradient.DockStripGradient.StartColor = Color.Red;
                dockPaneSkin.DocumentGradient.DockStripGradient.EndColor = Color.Pink;

                dockPaneSkin.DocumentGradient.ActiveTabGradient.StartColor = Color.Green;
                dockPaneSkin.DocumentGradient.ActiveTabGradient.EndColor = Color.Green;
                dockPaneSkin.DocumentGradient.ActiveTabGradient.TextColor = Color.White;

                dockPaneSkin.DocumentGradient.InactiveTabGradient.StartColor = Color.Gray;
                dockPaneSkin.DocumentGradient.InactiveTabGradient.EndColor = Color.Gray;
                dockPaneSkin.DocumentGradient.InactiveTabGradient.TextColor = Color.Black;

                dockPaneSkin.TextFont = new Font("SketchFlow Print", 10);

                _dockPaneStripSkin = dockPanel.Skin.DockPaneStripSkin;
                dockPanel.Skin.DockPaneStripSkin = dockPaneSkin;
            }
            else
            {
                if (_autoHideStripSkin != null)
                {
                    dockPanel.Skin.AutoHideStripSkin = _autoHideStripSkin;
                }

                if (_dockPaneStripSkin != null)
                {
                    dockPanel.Skin.DockPaneStripSkin = _dockPaneStripSkin;
                }
            }

            menuItemLayoutByXml_Click(menuItemLayoutByXml, EventArgs.Empty);
        }

        #endregion

        #region Event Handlers

        private void menuItemExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void menuItemSolutionExplorer_Click(object sender, System.EventArgs e)
        {
            m_solutionExplorer.Show(dockPanel);
        }

        private void menuItemPropertyWindow_Click(object sender, System.EventArgs e)
        {
            m_propertyWindow.Show(dockPanel);
        }

        private void menuItemToolbox_Click(object sender, System.EventArgs e)
        {
            m_toolbox.Show(dockPanel);
        }

        private void menuItemOutputWindow_Click(object sender, System.EventArgs e)
        {
            m_outputWindow.Show(dockPanel);
        }

        private void menuItemTaskList_Click(object sender, System.EventArgs e)
        {
            m_taskList.Show(dockPanel);
        }

        private void menuItemAbout_Click(object sender, System.EventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog(this);
        }

        private void menuItemNew_Click(object sender, System.EventArgs e)
        {
            Document dummyDoc = CreateNewDocument();
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                dummyDoc.MdiParent = this;
                dummyDoc.Show();
            }
            else
                dummyDoc.Show(dockPanel);
        }

        private void menuItemOpen_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.InitialDirectory = Application.ExecutablePath;
            openFile.Filter = "rtf files (*.rtf)|*.rtf|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fullName = openFile.FileName;
                string fileName = Path.GetFileName(fullName);

                if (FindDocument(fileName) != null)
                {
                    MessageBox.Show("The document: " + fileName + " has already opened!");
                    return;
                }

                Document dummyDoc = new Document();
                dummyDoc.Text = fileName;
                if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
                {
                    dummyDoc.MdiParent = this;
                    dummyDoc.Show();
                }
                else
                    dummyDoc.Show(dockPanel);
                try
                {
                    dummyDoc.FileName = fullName;
                }
                catch (Exception exception)
                {
                    dummyDoc.Close();
                    MessageBox.Show(exception.Message);
                }

            }
        }

        private void menuItemFile_Popup(object sender, System.EventArgs e)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                menuItemClose.Enabled = 
                    menuItemCloseAll.Enabled =
                    menuItemCloseAllButThisOne.Enabled = (ActiveMdiChild != null);
            }
            else
            {
                menuItemClose.Enabled = (dockPanel.ActiveDocument != null);
                menuItemCloseAll.Enabled =
                    menuItemCloseAllButThisOne.Enabled = (dockPanel.DocumentsCount > 0);
            }
        }

        private void menuItemClose_Click(object sender, System.EventArgs e)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
                ActiveMdiChild.Close();
            else if (dockPanel.ActiveDocument != null)
                dockPanel.ActiveDocument.DockHandler.Close();
        }

        private void menuItemCloseAll_Click(object sender, System.EventArgs e)
        {
            CloseAllDocuments();
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            if (File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                dockPanel.SaveAsXml(configFile);
            else if (File.Exists(configFile))
                File.Delete(configFile);
        }

        private void menuItemToolBar_Click(object sender, System.EventArgs e)
        {
            toolBar.Visible = menuItemToolBar.Checked = !menuItemToolBar.Checked;
        }

        private void menuItemStatusBar_Click(object sender, System.EventArgs e)
        {
            statusBar.Visible = menuItemStatusBar.Checked = !menuItemStatusBar.Checked;
        }

        private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == toolBarButtonNew)
                menuItemNew_Click(null, null);
            else if (e.ClickedItem == toolBarButtonOpen)
                menuItemOpen_Click(null, null);
            else if (e.ClickedItem == toolBarButtonSolutionExplorer)
                menuItemSolutionExplorer_Click(null, null);
            else if (e.ClickedItem == toolBarButtonPropertyWindow)
                menuItemPropertyWindow_Click(null, null);
            else if (e.ClickedItem == toolBarButtonToolbox)
                menuItemToolbox_Click(null, null);
            else if (e.ClickedItem == toolBarButtonOutputWindow)
                menuItemOutputWindow_Click(null, null);
            else if (e.ClickedItem == toolBarButtonTaskList)
                menuItemTaskList_Click(null, null);



        }

        private void menuItemNewWindow_Click(object sender, System.EventArgs e)
        {
            MainForm newWindow = new MainForm();
            newWindow.Text = newWindow.Text + " - New";
            newWindow.Show();
        }

        //private void menuItemTools_Popup(object sender, System.EventArgs e)
        //{
        //    menuItemLockLayout.Checked = !this.dockPanel.AllowEndUserDocking;
        //}

        //private void menuItemLockLayout_Click(object sender, System.EventArgs e)
        //{
        //    dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
        //}

        private void menuItemLayoutByCode_Click(object sender, System.EventArgs e)
        {
            dockPanel.SuspendLayout(true);

            CloseAllContents();

            CreateStandardControls();

            m_solutionExplorer.Show(dockPanel, DockState.DockRight);
            m_propertyWindow.Show(m_solutionExplorer.Pane, m_solutionExplorer);
            m_toolbox.Show(dockPanel, new Rectangle(98, 133, 200, 383));
            m_outputWindow.Show(m_solutionExplorer.Pane, DockAlignment.Bottom, 0.35);
            m_taskList.Show(m_toolbox.Pane, DockAlignment.Left, 0.4);

            Document doc1 = CreateNewDocument("Document1");
            Document doc2 = CreateNewDocument("Document2");
            Document doc3 = CreateNewDocument("Document3");
            Document doc4 = CreateNewDocument("Document4");
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

            Timer _timer = new Timer();
            _timer.Tick += (sender, e) =>
            {
                _splashScreen.Visible = false;
                _timer.Enabled = false;
                _showSplash = false;
            };
            _timer.Interval = 4000;
            _timer.Enabled = true;
        }

        private void ResizeSplash()
        {
            if (_showSplash) {
                
            var centerXMain = (this.Location.X + this.Width) / 2.0;
            var LocationXSplash = Math.Max(0, centerXMain - (_splashScreen.Width / 2.0));

            var centerYMain = (this.Location.Y + this.Height) / 2.0;
            var LocationYSplash = Math.Max(0, centerYMain - (_splashScreen.Height / 2.0));

            _splashScreen.Location = new Point((int)Math.Round(LocationXSplash), (int)Math.Round(LocationYSplash));
            }
        }

        private void CreateStandardControls()
        {
            m_solutionExplorer = new SolutionExplorer();
            m_propertyWindow = new PropertyWindow();
            m_toolbox = new Toolbox();
            m_outputWindow = new OutputWindow();
            m_taskList = new TaskList();
        }

        private void menuItemLayoutByXml_Click(object sender, System.EventArgs e)
        {
            dockPanel.SuspendLayout(true);

            // In order to load layout from XML, we need to close all the DockContents
            CloseAllContents();

            CreateStandardControls();

            Assembly assembly = Assembly.GetAssembly(typeof(MainForm));
            Stream xmlStream = assembly.GetManifestResourceStream("DockSample.Resources.DockPanel.xml");
            if (xmlStream != null)
            {
                dockPanel.LoadFromXml(xmlStream, m_deserializeDockContent);
                xmlStream.Close();
            }

            dockPanel.ResumeLayout(true, true);
        }

        private void menuItemCloseAllButThisOne_Click(object sender, System.EventArgs e)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                Form activeMdi = ActiveMdiChild;
                foreach (Form form in MdiChildren)
                {
                    if (form != activeMdi)
                        form.Close();
                }
            }
            else
            {
                foreach (IDockContent document in dockPanel.DocumentsToArray())
                {
                    if (!document.DockHandler.IsActivated)
                        document.DockHandler.Close();
                }
            }
        }

        //private void menuItemShowDocumentIcon_Click(object sender, System.EventArgs e)
        //{
        //    dockPanel.ShowDocumentIcon = menuItemShowDocumentIcon.Checked = !menuItemShowDocumentIcon.Checked;
        //}

        //private void showRightToLeft_Click(object sender, EventArgs e)
        //{
        //    CloseAllContents();
        //    if (showRightToLeft.Checked)
        //    {
        //        this.RightToLeft = RightToLeft.No;
        //        this.RightToLeftLayout = false;
        //    }
        //    else
        //    {
        //        this.RightToLeft = RightToLeft.Yes;
        //        this.RightToLeftLayout = true;
        //    }
        //    m_solutionExplorer.RightToLeftLayout = this.RightToLeftLayout;
        //    showRightToLeft.Checked = !showRightToLeft.Checked;
        //}

        private void exitWithoutSavingLayout_Click(object sender, EventArgs e)
        {
            m_bSaveLayout = false;
            Close();
            m_bSaveLayout = true;
        }

        #endregion

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            ResizeSplash();
        }

        private void testBoardConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var boards = _arduinoEnvironment.GetBoards();
            BoardManagement.BoardSelector bs = new BoardManagement.BoardSelector(boards);
            bs.ShowDialog();
        }
    }
}