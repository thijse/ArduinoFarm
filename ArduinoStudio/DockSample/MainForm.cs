using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ArduinoStudio
{
    public partial class MainForm : Form
    {
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
            InitializeComponent();

            SetSplashScreen();
            CreateStandardControls();

            _deserializeDockContent = GetContentFromPersistString;

            //    vS2012ToolStripExtender1.DefaultRenderer = _toolStripProfessionalRenderer;
            //    vS2012ToolStripExtender1.VS2012Renderer = _vs2012ToolStripRenderer;
            vS2012ToolStripExtender1.VS2013Renderer = _vs2013ToolStripRenderer;

            topBar.BackColor = bottomBar.BackColor = Color.FromArgb(0xFF, 41, 57, 85);

            //SetSchema(this.menuItemSchemaVS2013Blue, null);
            SetSchema();
        }

        private void MainFormSizeChanged(object sender, EventArgs e)
        {
            ResizeSplash();
        }

        #region Methods

        private IDockContent FindDocument(string text)
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

        private Document CreateNewDocument()
        {
            var dummyDoc = new Document();

            var count = 1;
            var text = "Document" + count;
            while (FindDocument(text) != null)
            {
                count++;
                text = "Document" + count;
            }
            dummyDoc.Text = text;
            return dummyDoc;
        }

        private Document CreateNewDocument(string text)
        {
            var dummyDoc = new Document();
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

            if (parsedStrings[0] != typeof(Document).ToString())
                return null;

            var dummyDoc = new Document();
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

        //    private readonly ToolStripRenderer _toolStripProfessionalRenderer = new ToolStripProfessionalRenderer();
        //    private readonly ToolStripRenderer _vs2012ToolStripRenderer = new VS2012ToolStripRenderer();
        private readonly ToolStripRenderer _vs2013ToolStripRenderer = new Vs2013ToolStripRenderer();


        private void SetSchema()
        {
            dockPanel.Theme = vS2013BlueTheme1;
            EnableVSRenderer(VSToolStripExtender.VsVersion.Vs2013);
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
        //        dockPanel.LoadFromXml(configFile, _deserializeDockContent);
        //}

        private void EnableVSRenderer(VSToolStripExtender.VsVersion version)
        {
            vS2012ToolStripExtender1.SetStyle(mainMenu, version);
            vS2012ToolStripExtender1.SetStyle(toolBar, version);
            vS2012ToolStripExtender1.SetStyle(statusBar, version);
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

        //private AutoHideStripSkin _autoHideStripSkin;
        //private DockPaneStripSkin _dockPaneStripSkin;

        //private void SetDockPanelSkinOptions(bool isChecked)
        //{
        //    if (isChecked)
        //    {
        //        // All of these options may be set in the designer.
        //        // This is not a complete list of possible options available in the skin.

        //        var autoHideSkin = new AutoHideStripSkin();
        //        autoHideSkin.DockStripGradient.StartColor = Color.AliceBlue;
        //        autoHideSkin.DockStripGradient.EndColor = Color.Blue;
        //        autoHideSkin.DockStripGradient.LinearGradientMode = LinearGradientMode.ForwardDiagonal;
        //        autoHideSkin.TabGradient.StartColor = SystemColors.Control;
        //        autoHideSkin.TabGradient.EndColor = SystemColors.ControlDark;
        //        autoHideSkin.TabGradient.TextColor = SystemColors.ControlText;
        //        autoHideSkin.TextFont = new Font("Showcard Gothic", 10);

        //        _autoHideStripSkin = dockPanel.Skin.AutoHideStripSkin;
        //        dockPanel.Skin.AutoHideStripSkin = autoHideSkin;

        //        var dockPaneSkin = new DockPaneStripSkin();
        //        dockPaneSkin.DocumentGradient.DockStripGradient.StartColor = Color.Red;
        //        dockPaneSkin.DocumentGradient.DockStripGradient.EndColor = Color.Pink;

        //        dockPaneSkin.DocumentGradient.ActiveTabGradient.StartColor = Color.Green;
        //        dockPaneSkin.DocumentGradient.ActiveTabGradient.EndColor = Color.Green;
        //        dockPaneSkin.DocumentGradient.ActiveTabGradient.TextColor = Color.White;

        //        dockPaneSkin.DocumentGradient.InactiveTabGradient.StartColor = Color.Gray;
        //        dockPaneSkin.DocumentGradient.InactiveTabGradient.EndColor = Color.Gray;
        //        dockPaneSkin.DocumentGradient.InactiveTabGradient.TextColor = Color.Black;

        //        dockPaneSkin.TextFont = new Font("SketchFlow Print", 10);

        //        _dockPaneStripSkin = dockPanel.Skin.DockPaneStripSkin;
        //        dockPanel.Skin.DockPaneStripSkin = dockPaneSkin;
        //    }
        //    else
        //    {
        //        if (_autoHideStripSkin != null)
        //        {
        //            dockPanel.Skin.AutoHideStripSkin = _autoHideStripSkin;
        //        }

        //        if (_dockPaneStripSkin != null)
        //        {
        //            dockPanel.Skin.DockPaneStripSkin = _dockPaneStripSkin;
        //        }
        //    }

        //    menuItemLayoutByXml_Click(menuItemLayoutByXml, EventArgs.Empty);
        //}

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
            var openFile = new OpenFileDialog();

            openFile.InitialDirectory = Application.ExecutablePath;
            openFile.Filter = "rtf files (*.rtf)|*.rtf|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var fullName = openFile.FileName;
                var fileName = Path.GetFileName(fullName);

                if (FindDocument(fileName) != null)
                {
                    MessageBox.Show("The document: " + fileName + " has already opened!");
                    return;
                }

                var dummyDoc = new Document {Text = fileName};
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

        //private void menuItemTools_Popup(object sender, System.EventArgs e)
        //{
        //    menuItemLockLayout.Checked = !this.dockPanel.AllowEndUserDocking;
        //}

        //private void menuItemLockLayout_Click(object sender, System.EventArgs e)
        //{
        //    dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
        //}

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
        //    _solutionExplorer.RightToLeftLayout = this.RightToLeftLayout;
        //    showRightToLeft.Checked = !showRightToLeft.Checked;
        //}

        //private void ExitWithoutSavingLayoutClick(object sender, EventArgs e)
        //{
        //    _bSaveLayout = false;
        //    Close();
        //    _bSaveLayout = true;
        //}

        #endregion
    }
}