using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Iu.Windows;

namespace IuSpy
{
    public partial class StylesEditForm : Form
    {
        private struct ControlStruct
        {
            public uint value;
            public int row, col;
            public ControlStruct(uint value, int row, int column)
            {
                this.value = value;
                this.row = row;
                this.col = column;
            }
        };
        CWindow curWin = 0;
        bool exStyle = false;
        int prevStyles = 0;
        int sLeft = 8, sTop = 14, sHeight = 20;
        Dictionary<CheckBox, ControlStruct> conts = new Dictionary<CheckBox, ControlStruct>();
        bool updating = false;
        public StylesEditForm(CWindow window, bool extendedStyle)
        {
            InitializeComponent();
            curWin = window;
            exStyle = extendedStyle;
            if (exStyle)
                CreateExControls();
            else
                CreateControls();
        }
        private void CreateControls()
        {
            CreateCheckBox("0x00000001", 0x1, true, 0, 0);
            CreateCheckBox("0x00000002", 0x2, true, 0, 1);
            CreateCheckBox("0x00000004", 0x4, true, 0, 2);
            CreateCheckBox("0x00000008", 0x8, true, 0, 3);

            CreateCheckBox("0x00000010", 0x10, true, 1, 0);
            CreateCheckBox("0x00000020", 0x20, true, 1, 1);
            CreateCheckBox("0x00000040", 0x40, true, 1, 2);
            CreateCheckBox("0x00000080", 0x80, true, 1, 3);

            CreateCheckBox("0x00000100", 0x100, true, 2, 0);
            CreateCheckBox("0x00000200", 0x200, true, 2, 1);
            CreateCheckBox("0x00000400", 0x400, true, 2, 2);
            CreateCheckBox("0x00000800", 0x800, true, 2, 3);

            CreateCheckBox("0x00001000", 0x1000, true, 3, 0);
            CreateCheckBox("0x00002000", 0x2000, true, 3, 1);
            CreateCheckBox("0x00004000", 0x4000, true, 3, 2);
            CreateCheckBox("0x00008000", 0x8000, true, 3, 3);

            CreateCheckBox("WS_MAXIMIZEBOX", WindowStyles.WS_MAXIMIZEBOX, true, 4, 0);
            CreateCheckBox("WS_MINIMIZEBOX", WindowStyles.WS_MINIMIZEBOX, true, 4, 1);
            CreateCheckBox("WS_THICKFRAME", WindowStyles.WS_THICKFRAME, true, 4, 2);
            CreateCheckBox("WS_SYSMENU", WindowStyles.WS_SYSMENU, true, 4, 3);

            CreateCheckBox("WS_HSCROLL", WindowStyles.WS_HSCROLL, true, 5, 0);
            CreateCheckBox("WS_VSCROLL", WindowStyles.WS_VSCROLL, true, 5, 1);
            CreateCheckBox("WS_DLGFRAME", WindowStyles.WS_DLGFRAME, true, 5, 2);
            CreateCheckBox("WS_BORDER", WindowStyles.WS_BORDER, true, 5, 3);

            CreateCheckBox("WS_MAXIMIZE", WindowStyles.WS_MAXIMIZE, true, 6, 0);
            CreateCheckBox("WS_CLIPCHILDREN", WindowStyles.WS_CLIPCHILDREN, true, 6, 1);
            CreateCheckBox("WS_CLIPSIBLINGS", WindowStyles.WS_CLIPSIBLINGS, true, 6, 2);
            CreateCheckBox("WS_DISABLED", WindowStyles.WS_DISABLED, true, 6, 3);

            CreateCheckBox("WS_VISIBLE", WindowStyles.WS_VISIBLE, true, 7, 0);
            CreateCheckBox("WS_MINIMIZE", WindowStyles.WS_MINIMIZE, true, 7, 1);
            CreateCheckBox("WS_CHILD", WindowStyles.WS_CHILD, true, 7, 2);
            CreateCheckBox("WS_POPUP", WindowStyles.WS_POPUP, true, 7, 3);


            CreateCheckBox("WS_CAPTION", WindowStyles.WS_CAPTION, false, 0, 0, "WS_BORDER|WS_DLGFRAME = 0x00C00000");
            CreateCheckBox("WS_OVERLAPPEDWINDOW", WindowStyles.WS_OVERLAPPEDWINDOW, false, 0, 1, "WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX = 0x00cf0000");
            CreateCheckBox("WS_POPUPWINDOW", WindowStyles.WS_POPUPWINDOW, false, 0, 2, "WS_POPUP | WS_BORDER | WS_SYSMENU = 0x80880000");

            AdjustCoordinates();
        }
        private void CreateExControls()
        {
            CreateCheckBox("WS_EX_DLGMODALFRAME", WindowExStyles.WS_EX_DLGMODALFRAME, true, 0, 0);
            CreateCheckBox("0x00000002", 0x2, true, 0, 1);
            CreateCheckBox("WS_EX_NOPARENTNOTIFY", WindowExStyles.WS_EX_NOPARENTNOTIFY, true, 0, 2);
            CreateCheckBox("WS_EX_TOPMOST", WindowExStyles.WS_EX_TOPMOST, true, 0, 3);

            CreateCheckBox("WS_EX_ACCEPTFILES", WindowExStyles.WS_EX_ACCEPTFILES, true, 1, 0);
            CreateCheckBox("WS_EX_TRANSPARENT", WindowExStyles.WS_EX_TRANSPARENT, true, 1, 1);
            CreateCheckBox("WS_EX_MDICHILD", WindowExStyles.WS_EX_MDICHILD, true, 1, 2);
            CreateCheckBox("WS_EX_TOOLWINDOW", WindowExStyles.WS_EX_TOOLWINDOW, true, 1, 3);

            CreateCheckBox("WS_EX_WINDOWEDGE", WindowExStyles.WS_EX_WINDOWEDGE, true, 2, 0);
            CreateCheckBox("WS_EX_CLIENTEDGE", WindowExStyles.WS_EX_CLIENTEDGE, true, 2, 1);
            CreateCheckBox("WS_EX_CONTEXTHELP", WindowExStyles.WS_EX_CONTEXTHELP, true, 2, 2);
            CreateCheckBox("0x00000800", 0x800, true, 2, 3);

            CreateCheckBox("WS_EX_RIGHT", WindowExStyles.WS_EX_RIGHT, true, 3, 0);
            CreateCheckBox("WS_EX_RTLREADING", WindowExStyles.WS_EX_RTLREADING, true, 3, 1);
            CreateCheckBox("WS_EX_LEFTSCROLLBAR", WindowExStyles.WS_EX_LEFTSCROLLBAR, true, 3, 2);
            CreateCheckBox("0x00008000", 0x8000, true, 3, 3);

            CreateCheckBox("WS_EX_CONTROLPARENT", WindowExStyles.WS_EX_CONTROLPARENT, true, 4, 0);
            CreateCheckBox("WS_EX_STATICEDGE", WindowExStyles.WS_EX_STATICEDGE, true, 4, 1);
            CreateCheckBox("WS_EX_APPWINDOW", WindowExStyles.WS_EX_APPWINDOW, true, 4, 2);
            CreateCheckBox("WS_EX_LAYERED", WindowExStyles.WS_EX_LAYERED, true, 4, 3);

            CreateCheckBox("WS_EX_NOINHERITLAYOUT", WindowExStyles.WS_EX_NOINHERITLAYOUT, true, 5, 0);
            CreateCheckBox("0x00200000", 0x200000, true, 5, 1);
            CreateCheckBox("WS_EX_LAYOUTRTL", WindowExStyles.WS_EX_LAYOUTRTL, true, 5, 2);
            CreateCheckBox("0x00800000", 0x800000, true, 5, 3);
            
            CreateCheckBox("0x01000000", 0x1000000, true, 6, 0);
            CreateCheckBox("WS_EX_COMPOSITED", WindowExStyles.WS_EX_COMPOSITED, true, 6, 1);
            CreateCheckBox("0x04000000", 0x4000000, true, 6, 2);
            CreateCheckBox("WS_EX_NOACTIVATE", WindowExStyles.WS_EX_NOACTIVATE, true, 6, 3);
            
            CreateCheckBox("0x10000000", 0x10000000, true, 7, 0);
            CreateCheckBox("0x20000000", 0x20000000, true, 7, 1);
            CreateCheckBox("0x40000000", 0x40000000, true, 7, 2);
            CreateCheckBox("0x80000000", 0x80000000, true, 7, 3);

            CreateCheckBox("WS_EX_OVERLAPPEDWINDOW", WindowExStyles.WS_EX_OVERLAPPEDWINDOW, false, 0, 0, "WS_EX_WINDOWEDGE|WS_EX_CLIENTEDGE = 0x00000300");
            CreateCheckBox("WS_EX_PALETTEWINDOW", WindowExStyles.WS_EX_PALETTEWINDOW, false, 0, 1, "WS_EX_WINDOWEDGE|WS_EX_TOOLWINDOW|WS_EX_TOPMOST = 0x00000188");

            AdjustCoordinates();         
        }
        private void AdjustCoordinates()
        {
            List<CheckBox> list;
            int width = 0, left = sLeft;
            for (int i = 0; i < 4 ; i++)
            {
                list = SelectColumnControls(i,true);
                AdjustListCoordinates(list, left);
                width = GetOveralColumnWidth(list);
                left += width;
            }
            Primitive.Width = left + 1;
            left = sLeft;
            for (int i = 0; i < 4 ; i++)
            {
                list = SelectColumnControls(i, false);
                if (list.Count < 1)
                    break;
                AdjustListCoordinates(list, left);
                width = GetOveralColumnWidth(list);
                left += width;
            }
            if (Primitive.Width < left+1)
                Primitive.Width = left + 1;
            Derived.Width = Primitive.Width;
            Derived.Left = Primitive.Left;
            Primitive.Height = Primitive.PreferredSize.Height - 15;
            Derived.Height = Derived.PreferredSize.Height - 15;
            Derived.Top = Primitive.Bottom+3;
            this.ClientSize = new Size(Primitive.Width + Primitive.Left + 2, Derived.Bottom+37);
        }
        private List<CheckBox> SelectColumnControls(int column,bool primitive)
        {
            List<CheckBox> list = new List<CheckBox>();
            foreach (CheckBox ch in conts.Keys)
            {
                if (primitive)
                {
                    if (ch.Parent == Derived)
                        continue;
                }
                else
                    if (ch.Parent == Primitive)
                        continue;

                if (conts[ch].col == column)
                    list.Add(ch);
            }
            return list;
        }
        private int GetOveralColumnWidth(List<CheckBox> controls)
        {
            int width = 0;
            foreach (CheckBox ch in controls)
            {
                if (ch.Width > width)
                    width = ch.Width;
            }
            return width;
        }
        private void AdjustListCoordinates(List<CheckBox> controls,int left)
        {
            foreach (CheckBox ch in controls)
            {
                ch.Left = left;
            }
        }
        private void CreateCheckBox(string name, WindowStyles style, bool primitive, int row, int column)
        {
            CreateCheckBox(name, style, primitive, row, column, null);
        }
        private void CreateCheckBox(string name, WindowStyles style, bool primitive, int row, int column, string toolTip)
        {
            CreateCheckBox(name, (uint)style, primitive, row, column, toolTip);
        }
        private void CreateCheckBox(string name, WindowExStyles style, bool primitive, int row, int column)
        {
            CreateCheckBox(name, style, primitive, row, column, null);
        }
        private void CreateCheckBox(string name, WindowExStyles style, bool primitive, int row, int column, string toolTip)
        {
            CreateCheckBox(name, (uint)style, primitive, row, column, toolTip);
        }
        private void CreateCheckBox(string name, uint value, bool primitive, int row, int column)
        {
            CreateCheckBox(name, value, primitive, row, column, null);
        }
        private void CreateCheckBox(string name, uint value, bool primitive, int row, int column, string toolTip)
        {
            CheckBox ch = new CheckBox();
            if (primitive)
                Primitive.Controls.Add(ch);
            else
                Derived.Controls.Add(ch);
            ch.AutoSize = true;
            ch.Text = ch.Name = name;
            ch.Top = sTop + sHeight * row;
            ch.CheckedChanged += new EventHandler(CheckBox_CheckedChanged);
            conts.Add(ch, new ControlStruct(value,row,column));
            uint tmp;
            if(toolTip != null)
                toolTip1.SetToolTip(ch, toolTip);
            else
            if (uint.TryParse(name.Substring(2,name.Length-2),out tmp))
                toolTip1.SetToolTip(ch, "Undeclared style.");
            else
                toolTip1.SetToolTip(ch, "0x"+value.ToString("X").PadLeft(8,'0'));
        }
        void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (updating)
                return;
            if (!exStyle)
                curWin.SetStyle((WindowStyles)conts[(CheckBox)sender].value, ((CheckBox)sender).Checked);
            else
                curWin.SetExStyle((WindowExStyles)conts[(CheckBox)sender].value, ((CheckBox)sender).Checked);
        }
        private void StylesEditForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (!exStyle)
                    prevStyles = curWin.Styles;
                else
                    prevStyles = curWin.ExStyles;
                FillStyles();
            }
        }
        private void FillStyles()
        {
            updating = true;
            int styles = 0;
            if (!exStyle)
                styles = curWin.Styles;
            else
                styles = curWin.ExStyles;
            foreach (CheckBox ch in conts.Keys)
            {
                if ((styles & conts[ch].value) == conts[ch].value)
                    ch.Checked = true;
                else
                    ch.Checked = false;
            }
            updating = false;
        }
        private void CancelB_Click(object sender, EventArgs e)
        {
            if (!exStyle)
                curWin.Styles = this.prevStyles;
            else
                curWin.ExStyles = this.prevStyles;
            curWin.UpdateStyles();
            this.Close();
        }
        private void OkB_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            FillStyles();
        }
    }
}