using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace utilities.Windows
{
    public class CWindow
    {
        protected bool Equals(CWindow other)
        {
            return _handle.Equals(other._handle);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CWindow) obj);
        }

        public override int GetHashCode()
        {
            return _handle.GetHashCode();
        }

        private IntPtr _handle;
        #region Constructors/Operators
        public CWindow(IntPtr handle)
        {
            this._handle = handle;
        }
        public static implicit operator CWindow(IntPtr handle)
        {
            return new CWindow(handle);
        }
        public static implicit operator CWindow(int handle)
        {
            return new CWindow((IntPtr)handle);
        }
        public static implicit operator IntPtr(CWindow window)
        {
            return window._handle;
        }
        public static bool operator ==(CWindow a, CWindow b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a._handle == b._handle;
        }
        public static bool operator !=(CWindow a, CWindow b)
        {
            return !(a == b);
        }
        public static bool operator ==(CWindow a, IntPtr b)
        {
            if ((object)a == null)
                return false;
            return a._handle == b;
        }
        public static bool operator !=(CWindow a, IntPtr b)
        {
            return !(a == b);
        }
        #endregion Constructors/Operators

        #region Static
        /// <summary>
        /// Gets/sets the window at the top of the Z order.
        /// </summary>
        public static CWindow TopWindow
        {
            get
            {
                return GetTopWindow(IntPtr.Zero);
            }
            set
            {
                value.BringToFront();
            }
        }
        /// <summary>
        /// Gets/sets whether windows animation, when minimizing and maximizing, is enabled.
        /// </summary>
        public static bool StateChangeAnimation
        {
            get
            {
                ANIMATIONINFO info = new ANIMATIONINFO(0);
                SystemParametersInfo(SPI_GETANIMATION, info.cbSize, ref info, 0);
                return info.iMinAnimate != 0;
            }
            set
            {
                ANIMATIONINFO info = new ANIMATIONINFO(value ? 1 : 0);
                SystemParametersInfo(SPI_SETANIMATION, info.cbSize, ref info, 0);
            }
        }
        /// <summary>
        /// Gets/sets the currently active window.
        /// </summary>
        public static CWindow ActiveWindow
        {
            get
            {
                return GetForegroundWindow();
            }
            set
            {
                value.ForceActivate();
            }
        }
        /// <summary>
        /// Gets/sets the window that has the keyboard focus.
        /// </summary>
        public static CWindow FocusedWindow
        {
            get
            {
                return GetFocus();
            }
            set
            {
                value.Focus();
            }
        }
        /// <summary>
        /// Gets the window (if any) that has captured the mouse.
        /// </summary>
        public static CWindow Captured
        {
            get
            {
                return GetCapture();
            }
        }
        /// <summary>
        /// Gets the desktop window. The desktop window covers the entire screen. 
        /// The desktop window is the area on top of which other windows are painted. 
        /// </summary>
        public static CWindow DesktopWindow
        {
            get { return GetDesktopWindow(); }
        }
        /// <summary>
        /// Gets the collection of all the top-level windows on the screen.
        /// </summary>
        public static List<CWindow> TopLevelWindows
        {
            get
            {
                List<CWindow> list = new List<CWindow>();
                GCHandle listHandle = GCHandle.Alloc(list);
                try
                {
                    EnumWindows_(new EnumWindowsProc(EnumChildrenProc), GCHandle.ToIntPtr(listHandle));
                }
                catch { }
                finally
                {
                    if (listHandle.IsAllocated)
                        listHandle.Free();
                }
                return list;
            }
        }
        /// <summary>
        /// Retrieves the window at a specified location.
        /// </summary>
        public static CWindow FromPoint(Point pt)
        {
            return WindowFromPoint(pt);
        }
        /// <summary>
        /// Retrieves the top-level window whose class name and window name match the specified strings. 
        /// This function does not search child windows. This function does not perform a case-sensitive search.
        /// </summary>
        public static CWindow FindWindow(string className, string windowName)
        {
            return FindWindow_(className, windowName);
        }
        /// <summary>
        /// Retrieves the child windows whose class name and window name match the specified strings, 
        /// beginning with the one following the specified child window.
        /// This function does not perform a case-sensitive search.
        /// </summary>
        public static CWindow FindWindow(CWindow windowAfter, string className, string windowName)
        {
            return FindWindowEx(IntPtr.Zero, windowAfter._handle, className, windowName);
        }
        /// <summary>
        /// Enumerates all top-level windows on the screen by passing the handle to each window, in turn, 
        /// to an application-defined callback function. EnumWindows continues until the last top-level window is enumerated 
        /// or the callback function returns FALSE. For details see EnumWindows in MSDN.
        /// </summary>
        /// <param name="lParam">Specifies an application-defined value to be passed to the callback function.</param>
        public static bool EnumWindows(EnumWindowsProc proc, IntPtr lParam)
        {
            return EnumWindows_(proc, lParam);
        }
        /// <summary>
        /// Enumerates all nonchild windows associated with the specified thread by passing the handle to each window, 
        /// in turn, to an application-defined callback function. Continues until the last window is enumerated 
        /// or the callback function returns FALSE. For details see EnumThreadWindows in MSDN.
        /// </summary>
        /// <param name="lParam">Specifies an application-defined value to be passed to the callback function.</param>
        public static bool EnumThreadWindows(uint threadId, EnumWindowsProc proc, IntPtr lParam)
        {
            return EnumThreadWindows_(threadId, proc, lParam);
        }
        /// <summary>
        /// Retrieves the collection of the top-level windows contained within the specified thread.
        /// </summary>
        public static List<CWindow> GetThreadWindows(uint threadId)
        {
            List<CWindow> list = new List<CWindow>();
            GCHandle listHandle = GCHandle.Alloc(list);
            try
            {
                EnumThreadWindows(threadId, new EnumWindowsProc(EnumChildrenProc), GCHandle.ToIntPtr(listHandle));
            }
            catch { }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return list;
        }
        private static bool EnumChildrenProc(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<CWindow> list = gch.Target as List<CWindow>;
            if (list == null)
                return false;
            list.Add(handle);
            return true;
        }
        #endregion Static

        #region Window Relations
        /// <summary>
        /// Gets/sets window's parent or owner, and changes the parent window. For more information, see GetParent function in MSDN
        /// </summary>
        public CWindow Parent
        {
            get
            {
                return GetParent(this._handle);
            }
            set
            {
                SetParent(_handle, value._handle);
            }
        }
        /// <summary>
        /// Gets owner window, if any. For more information, see "Window Features"/"Owned Windows", GetWindow in MSDN.
        /// </summary>
        public CWindow Owner
        {
            get
            {
                return GetNextWindow(GetWindow_Cmd.GW_OWNER);
            }
        }
        /// <summary>
        /// Retrieves the root window by walking the chain of parent windows. See Control.TopLevelControl in MSDN also. 
        /// </summary>
        public CWindow TopLevelWindow
        {
            get
            {
                return GetAncestor(_handle, AncestorFlags.GA_ROOT);
            }
        }
        /// <summary>
        /// Gets a value indicating whether the window is a top-level window.
        /// </summary>
        public bool TopLevel
        {
            get { return this.TopLevelWindow._handle == this._handle; }
        }
        /// <summary>
        /// If current window is a top-level window, checks if it is at the top of the Z order.
        /// If the window is a child window, checks if it is at the top of it parent's Z order.
        /// </summary>
        public bool IsTopWindow
        {
            get
            {
                return GetTopWindow(Parent._handle) == this._handle;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the window should be displayed as a topmost window.
        /// </summary>
        public bool TopMost
        {
            get
            {
                return ExStyles.Check(WindowExStyles.WS_EX_TOPMOST);
            }
            set
            {
                SetWindowPos(_handle, (value ? WindowPosAfter.HWND_TOPMOST : WindowPosAfter.HWND_NOTOPMOST), 0, 0, 0, 0,
                    WinPosFlags.SWP_NOSIZE | WinPosFlags.SWP_NOMOVE | WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets a value indicating whether the window contains one or more child windows.
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return GetNextWindow(GetWindow_Cmd.GW_CHILD) != IntPtr.Zero;
            }
        }
        /// <summary>
        /// Gets the collection of windows contained within the window. It contains only immediate child windows.
        /// </summary>
        public List<CWindow> Children
        {
            get
            {
                List<CWindow> list = new List<CWindow>();
                GCHandle listHandle = GCHandle.Alloc(list);
                try
                {
                    EnumChildWindows(new EnumWindowsProc(EnumChildrenProc), GCHandle.ToIntPtr(listHandle));
                }
                catch { }
                if (listHandle.IsAllocated)
                    listHandle.Free();
                int cur = 0;
                while (cur < list.Count)
                    if (GetAncestor(list[cur]._handle, AncestorFlags.GA_PARENT) != this._handle)
                        list.RemoveAt(cur);
                    else
                        cur++;
                return list;
            }
        }
        /// <summary>
        /// Gets the child window at the top of the Z order, if this window is a parent window; 
        /// otherwise, returns null-window. Examines only child windows of this window (does not examine descendant windows).
        /// </summary>
        public CWindow TopChild
        {
            get
            {
                return GetNextWindow(GetWindow_Cmd.GW_CHILD);
            }
        }
        /// <summary>
        /// Gets the collection of the top-level windows contained within the window's thread.
        /// </summary>
        public List<CWindow> ThreadWindows
        {
            get
            {
                return CWindow.GetThreadWindows(this.ThreadId);
            }
        }
        /// <summary>
        /// Sets the control as the top-level control.
        /// </summary>
        public void SetTopLevel()
        {
            this.Parent = IntPtr.Zero;
        }
        /// <summary>
        /// Retrieves a value indicating whether the specified window is a child of this window.
        /// </summary>
        public bool Contains(CWindow wind)
        {
            return IsChild(this._handle, wind);
        }
        /// <summary>
        /// Retrieves the child window at a specified location.
        /// </summary>
        /// <param name="skipValue">Determining whether to ignore child controls of a certain type.</param>
        public CWindow GetChildAtPoint(Point pt, System.Windows.Forms.GetChildAtPointSkip skipValue)
        {
            CWindow descendant = ChildWindowFromPointEx(this._handle, pt, (int)skipValue);
            if (descendant._handle != this._handle)
                return descendant;
            return null;
        }
        /// <summary>
        /// Retrieves the child window at a specified location.
        /// </summary>
        public CWindow GetChildAtPoint(Point pt)
        {
            return GetChildAtPoint(pt, System.Windows.Forms.GetChildAtPointSkip.None);
        }
        /// <summary>
        /// Retrieves the window whose class name and window name match the specified strings. 
        /// The function searches child windows, beginning with the one following the specified child window. 
        /// This function does not perform a case-sensitive search. 
        /// </summary>
        /// <param name="childAfter">The search begins with the next child window in the Z order. 
        /// The child window must be a direct child window, not just a descendant window. </param>
        public CWindow FindChildWindow(CWindow childAfter, string className, string windowName)
        {
            return FindWindowEx(this._handle, childAfter._handle, className, windowName);
        }
        public CWindow FindChildWindow(string className, string windowName)
        {
            return FindWindowEx(this._handle, IntPtr.Zero, className, windowName);
        }
        /// <summary>
        /// Retrieves a window that has the specified relationship (Z-Order or owner) to this window. 
        /// </summary>
        /// <param name="flags">Specifies the relationship between this window and the window which is retrieved.</param>
        public CWindow GetNextWindow(GetWindow_Cmd flags)
        {
            return GetWindow(this._handle, (int)flags);
        }
        /// <summary>
        /// Inserts this window after the specified in the Z order.
        /// </summary>
        /// <param name="win">A window to precede the positioned window in the Z order.</param>
        public void InsertAfter(CWindow insertAfter)
        {
            SetWindowPos(this._handle, (WindowPosAfter)insertAfter.Handle, 0, 0, 0, 0, WinPosFlags.SWP_NOSIZE | WinPosFlags.SWP_NOMOVE |
                WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
        }
        /// <summary>
        /// Enumerates the child windows that belong to this window by passing the handle to each child window, 
        /// in turn, to an application-defined callback function. Continues until the last child window is enumerated 
        /// or the callback function returns false. For details see EnumChildWindows in MSDN.
        /// </summary>
        /// <param name="lParam">Specifies an application-defined value to be passed to the callback function.</param>
        public bool EnumChildWindows(EnumWindowsProc proc, IntPtr lParam)
        {
            return EnumChildWindows(this._handle, proc, lParam);
        }
        /// <summary>
        /// Enumerates all nonchild windows associated with the current thread by passing the handle to each window, 
        /// in turn, to an application-defined callback function. Continues until the last window is enumerated 
        /// or the callback function returns FALSE. For details see EnumThreadWindows in MSDN.
        /// </summary>
        /// <param name="lParam">Specifies an application-defined value to be passed to the callback function.</param>
        public bool EnumThreadWindows(EnumWindowsProc proc, IntPtr lParam)
        {
            return EnumThreadWindows_(this.ThreadId, proc, lParam);
        }
        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        public bool BringToFront()
        {
            return SetWindowPos(_handle, WindowPosAfter.HWND_TOP, 0, 0, 0, 0, WinPosFlags.SWP_NOSIZE | WinPosFlags.SWP_NOMOVE |
                WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
        }
        /// <summary>
        /// Places the window at the bottom of the Z order. If if current window is topmost window, 
        /// the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        public void SendToBack()
        {
            SetWindowPos(_handle, WindowPosAfter.HWND_BOTTOM, 0, 0, 0, 0, WinPosFlags.SWP_NOSIZE | WinPosFlags.SWP_NOMOVE |
                WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
        }
        private bool EnumModalProc(IntPtr handle, IntPtr pointer)
        {
            if (handle == this._handle)
                return true;
            CWindow cWin = handle;
            WindowStyle styles = cWin.Styles;
            if (!styles.Check(WindowStyles.WS_VISIBLE))
                return true;
            CWindow res = GCHandle.FromIntPtr(pointer).Target as CWindow;
            if (!styles.Check(WindowStyles.WS_DISABLED))
            {
                res._handle = IntPtr.Zero;
                return false;
            }
            res._handle = this._handle;
            return true;
        }
        #endregion Window Relations

        #region Coordinats
        /// <summary>
        /// Gets or sets the size and location of the window on the Windows desktop.
        /// </summary>
        public Rectangle DesktopBounds
        {
            get
            {
                RECT rect = new RECT();
                GetWindowRect(_handle, out rect);
                return rect;
            }
            set
            {
                Point locat = this.Parent.PointToClient(value.Location);
                SetWindowPos(_handle, 0, locat.X, locat.Y, value.Width, value.Height, WinPosFlags.SWP_NOZORDER |
                    WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets or sets the location of the window on the Windows desktop.
        /// </summary>
        public Point DesktopLocation
        {
            get
            {
                return this.DesktopBounds.Location;
            }
            set
            {
                Point locat = this.Parent.PointToClient(value);
                SetWindowPos(_handle, 0, locat.X, locat.Y, 0, 0, WinPosFlags.SWP_NOZORDER | WinPosFlags.SWP_NOSIZE |
                    WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets or sets the size and location of the window including its nonclient elements, in pixels, relative to the parent window.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                CWindow parent = this.Parent;
                if (parent != IntPtr.Zero)
                {
                    RECT rect = DesktopBounds;
                    return new Rectangle(parent.PointToClient(rect.Location), rect.Size);
                }
                return DesktopBounds;
            }
            set
            {
                SetWindowPos(_handle, 0, value.X, value.Y, value.Width, value.Height, WinPosFlags.SWP_NOZORDER |
              WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets or sets the size of the window.
        /// </summary>
        public Size Size
        {
            get { return this.Bounds.Size; }
            set
            {
                SetWindowPos(_handle, 0, 0, 0, value.Width, value.Height, WinPosFlags.SWP_NOZORDER | WinPosFlags.SWP_NOMOVE |
              WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of the window relative to the upper-left corner of its parent.
        /// </summary>
        public Point Location
        {
            get
            {
                return this.Bounds.Location;
            }
            set
            {
                SetWindowPos(_handle, 0, value.X, value.Y, 0, 0, WinPosFlags.SWP_NOZORDER | WinPosFlags.SWP_NOSIZE |
                    WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        public int Width
        {
            get { return this.Bounds.Width; }
            set
            {
                SetWindowPos(_handle, 0, 0, 0, value, this.Bounds.Height, WinPosFlags.SWP_NOZORDER | WinPosFlags.SWP_NOMOVE |
                  WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        public int Height
        {
            get { return this.Bounds.Height; }
            set
            {
                SetWindowPos(_handle, 0, 0, 0, this.Bounds.Width, value, WinPosFlags.SWP_NOZORDER | WinPosFlags.SWP_NOMOVE |
                  WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets/sets the distance, in pixels, between the top edge of the window and the top edge of its parent's client area.
        /// </summary>
        public int Top
        {
            get { return this.Bounds.Top; }
            set
            {
                SetWindowPos(_handle, 0, this.Bounds.Left, value, 0, 0, WinPosFlags.SWP_NOZORDER | WinPosFlags.SWP_NOSIZE |
                  WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets/sets the distance, in pixels, between the bottom edge of the window and the top edge of its parent's client area.
        /// </summary>
        public int Bottom
        {
            get { return this.Bounds.Bottom; }
            set
            {
                Rectangle bounds = this.Bounds;
                SetWindowPos(_handle, 0, 0, 0, bounds.Width, value - bounds.Top, WinPosFlags.SWP_NOZORDER |
                    WinPosFlags.SWP_NOMOVE | WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets/sets or sets the distance, in pixels, between the left edge of the window and the left edge of its parent's client area.
        /// </summary>
        public int Left
        {
            get { return this.Bounds.Left; }
            set
            {
                SetWindowPos(_handle, 0, value, Bounds.Top, 0, 0, WinPosFlags.SWP_NOZORDER | WinPosFlags.SWP_NOSIZE |
              WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets/sets the distance, in pixels, between the right edge of the window and the left edge of its parent's client area.
        /// </summary>
        public int Right
        {
            get { return this.Bounds.Right; }
            set
            {
                Rectangle bounds = this.Bounds;
                SetWindowPos(_handle, 0, 0, 0, value - bounds.Left, bounds.Height, WinPosFlags.SWP_NOZORDER |
                    WinPosFlags.SWP_NOMOVE | WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
            }
        }
        /// <summary>
        /// Gets the rectangle that represents the client area of the window.
        /// </summary>
        public Rectangle ClientRectangle
        {
            get
            {
                RECT rect = new RECT();
                GetClientRect(_handle, out rect);
                return rect;
            }
        }
        /// <summary>
        /// Gets/sets the size of the client area of the window.
        /// </summary>
        public Size ClientSize
        {
            get { return this.ClientRectangle.Size; }
            set
            {
                RECT rect = new RECT(0, 0, value.Width, value.Height);
                AdjustWindowRect(ref rect, this.Styles, Menu != IntPtr.Zero);
                this.Size = rect.Size;
            }
        }
        /// <summary>
        /// Gets the maximum size the window can be resized to. Be careful with this property, check IsHung first.
        /// </summary>
        public Size MaximumSize
        {
            get
            {
                MINMAXINFO inf = new MINMAXINFO();
                SendMessage(_handle, WM_GETMINMAXINFO, IntPtr.Zero, ref inf);
                return (Size)inf.ptMaxTrackSize;
            }
        }
        /// <summary>
        /// Gets the minimum size the window can be resized to. Be careful with this property, check IsHung first.
        /// </summary>
        public Size MinimumSize
        {
            get
            {
                MINMAXINFO inf = new MINMAXINFO();
                SendMessage(_handle, WM_GETMINMAXINFO, IntPtr.Zero, ref inf);
                return (Size)inf.ptMinTrackSize;
            }
        }
        /// <summary>
        /// Gets the size of the window when it is maximized. Be careful with this property, check IsHung first.
        /// </summary>
        public Rectangle MaximizedBounds
        {
            get
            {
                MINMAXINFO inf = new MINMAXINFO();
                SendMessage(_handle, WM_GETMINMAXINFO, IntPtr.Zero, ref inf);
                return new Rectangle(inf.ptMaxPosition, (Size)inf.ptMaxSize);
            }
        }
        /// <summary>
        /// Gets/sets the location and size of the window in its normal window state.
        /// </summary>
        public Rectangle RestoreBounds
        {
            get
            {
                WINDOWPLACEMENT plac = new WINDOWPLACEMENT();
                plac.length = Marshal.SizeOf(plac);
                GetWindowPlacement(_handle, ref plac);
                return new Rectangle(plac.rcNormalPosition_left, plac.rcNormalPosition_top,
                    plac.rcNormalPosition_right - plac.rcNormalPosition_left, plac.rcNormalPosition_bottom - plac.rcNormalPosition_top);
            }
            set
            {
                WINDOWPLACEMENT plac = new WINDOWPLACEMENT();
                plac.length = Marshal.SizeOf(plac);
                plac.rcNormalPosition_left = value.Left;
                plac.rcNormalPosition_top = value.Top;
                plac.rcNormalPosition_right = value.Left + value.Width;
                plac.rcNormalPosition_bottom = value.Top + value.Height;
                SetWindowPlacement(_handle, ref plac);
            }
        }
        /// <summary>
        /// Computes the location of the specified screen point into client coordinates.
        /// </summary>
        public Point PointToClient(Point screenPoint)
        {
            POINT pt = screenPoint;
            ScreenToClient(_handle, ref pt);
            return pt;
        }
        /// <summary>
        /// Computes the location of the specified client point into screen coordinates.
        /// </summary>
        public Point PointToScreen(Point clientPoint)
        {
            POINT pt = clientPoint;
            ClientToScreen(_handle, ref pt);
            return pt;
        }
        /// <summary>
        /// Computes the size and location of the specified screen rectangle in client coordinates.
        /// </summary>
        public Rectangle RectangleToClient(Rectangle screenRect)
        {
            RECT rect = screenRect;
            MapWindowPoints(IntPtr.Zero, this._handle, ref rect, 2);
            return rect;
        }
        /// <summary>
        /// Computes the size and location of the specified client rectangle in screen coordinates.
        /// </summary>
        public Rectangle RectangleToScreen(Rectangle clientRect)
        {
            RECT rect = clientRect;
            MapWindowPoints(this._handle, IntPtr.Zero, ref rect, 2);
            return rect;
        }
        #endregion Coordinats

        #region State
        /// <summary>
        /// Gets/sets the window's visible state. Implements the same as Form.WindowState.
        /// Sometimes it differs from real window state. For changing current state use WindowState.
        /// </summary>
        public System.Windows.Forms.FormWindowState WindowVisibleState
        {
            get
            {
                WINDOWPLACEMENT plac = new WINDOWPLACEMENT();
                plac.length = Marshal.SizeOf(plac);
                GetWindowPlacement(_handle, ref plac);
                if (plac.showCmd == (int)SW.SW_SHOWMINIMIZED)
                    return System.Windows.Forms.FormWindowState.Minimized;
                if (plac.showCmd == (int)SW.SW_SHOWMAXIMIZED)
                    return System.Windows.Forms.FormWindowState.Maximized;
                return System.Windows.Forms.FormWindowState.Normal;

            }
            set
            {
                WINDOWPLACEMENT plac = new WINDOWPLACEMENT();
                plac.length = Marshal.SizeOf(plac);
                GetWindowPlacement(_handle, ref plac);
                switch (value)
                {
                    case System.Windows.Forms.FormWindowState.Maximized:
                        ShowWindow(_handle, SW.SW_MAXIMIZE);
                        break;
                    case System.Windows.Forms.FormWindowState.Minimized:
                        ShowWindow(_handle, SW.SW_MINIMIZE);
                        break;
                    case System.Windows.Forms.FormWindowState.Normal:
                        ShowWindow(_handle, SW.SW_NORMAL);
                        break;
                }
                //SetWindowPlacement(handle, ref plac);
            }
        }
        /// <summary>
        /// Gets/sets the window's state. See WindowVisibleState also.
        /// </summary>
        public System.Windows.Forms.FormWindowState WindowState
        {
            get { return WindowVisibleState; }
            set
            {
                switch (value)
                {
                    case System.Windows.Forms.FormWindowState.Maximized:
                        SysCommand(SC.SC_MAXIMIZE, 0);
                        break;
                    case System.Windows.Forms.FormWindowState.Minimized:
                        SysCommand(SC.SC_MINIMIZE, 0);
                        break;
                    case System.Windows.Forms.FormWindowState.Normal:
                        this.RestoreToMaximized = false;
                        SysCommand(SC.SC_RESTORE, 0);
                        break;
                }
            }
        }
        /// <summary>
        /// Gets/sets a value indicating whether the window is maximized in restored state.
        /// This setting is only valid the next time the window is restored. It does not change the default restoration behavior. 
        /// This property is only valid when the window is minimized.
        /// </summary>
        public bool RestoreToMaximized
        {
            get
            {
                WINDOWPLACEMENT plac = new WINDOWPLACEMENT();
                plac.length = Marshal.SizeOf(plac);
                GetWindowPlacement(_handle, ref plac);
                if (plac.showCmd != (int)SW.SW_SHOWMINIMIZED || plac.flags != WPF_RESTORETOMAXIMIZED)
                    return false;
                return true;
            }
            set
            {
                WINDOWPLACEMENT plac = new WINDOWPLACEMENT();
                plac.length = Marshal.SizeOf(plac);
                GetWindowPlacement(_handle, ref plac);
                if (plac.showCmd != (int)SW.SW_SHOWMINIMIZED)
                    return;
                if (value)
                    plac.flags |= WPF_RESTORETOMAXIMIZED;
                else
                    plac.flags &= ~WPF_RESTORETOMAXIMIZED;
                SetWindowPlacement(_handle, ref plac);
            }
        }
        /// <summary>
        /// Minimizes the window.
        /// </summary>
        public void Minimize()
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        }
        /// <summary>
        /// Restores the window from minimized to its previous state.
        /// </summary>
        public void Restore()
        {
            if (this.WindowState != System.Windows.Forms.FormWindowState.Minimized)
                return;
            if (!this.RestoreToMaximized)
                this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            else
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
        #endregion State

        #region Visibility
        /// <summary>
        /// Gets/sets a value indicating whether the opacity of the window can be adjusted.
        /// </summary>
        public bool AllowTransparency
        {
            get
            {
                return this.ExStyles.Check(WindowExStyles.WS_EX_LAYERED);
            }
            set
            {
                if (this.AllowTransparency == value)
                    return;
                this.SetExStyle(WindowExStyles.WS_EX_LAYERED, value);
            }
        }
        /// <summary>
        /// Gets/sets the color that will represent transparent areas of the window.
        /// </summary>
        public Color TransparencyKey
        {
            get
            {
                int col;
                byte t1; int flags;
                GetLayeredWindowAttributes(this._handle, out col, out t1, out flags);
                if ((flags & LWA_COLORKEY) == LWA_COLORKEY)
                    return ColorTranslator.FromWin32(col);
                return Color.Empty;
            }
            set
            {
                SetLayeredAttributesInternal(ColorTranslator.ToWin32(value), 0, LWA_COLORKEY);
            }
        }
        /// <summary>
        /// Gets/sets the opacity level of the window.
        /// </summary>
        public double Opacity
        {
            get
            {
                byte opac;
                int t1, flags;
                GetLayeredWindowAttributes(this._handle, out t1, out opac, out flags);
                if ((flags & LWA_ALPHA) == LWA_ALPHA)
                    return (double)opac / 255;
                return 1;
            }
            set
            {
                if (value > 1.0)
                    value = 1.0;
                else if (value < 0.0)
                    value = 0.0;
                byte opacity = (byte)(0xff * value);
                SetLayeredAttributesInternal(0, opacity, LWA_ALPHA);
            }
        }
        /// <summary>
        /// Gets/sets a value indicating whether the window and is visible to user.
        /// </summary>
        public bool Visible
        {
            get
            {
                return IsWindowVisible(_handle);
            }
            set
            {
                if (value)
                    Show();
                else
                    Hide();
            }
        }
        /// <summary>
        /// Gets/sets the window region associated with the window.
        /// </summary>
        public Region Region
        {
            get
            {
                IntPtr hRgn = CreateRectRgn(0, 0, 0, 0);
                GetWindowRgn(this._handle, hRgn);
                RECT rect = new RECT();
                if (GetRgnBox(hRgn, out rect) == 1)
                    return null;
                return Region.FromHrgn(hRgn);
            }
            set
            {
                IntPtr hrgn = value.GetHrgn(Graphics.FromHdc(GetDC(this._handle)));
                SetWindowRgn(this._handle, hrgn, true);
            }
        }
        /// <summary>
        /// Gets/sets weather the window is considered to be transparent. Can be used for hit testing or for drawing underlayered windows.
        /// </summary>
        public bool Transparent
        {//If Transparent specified and no layout attributes, then flags are set to LWA_ALPHA and alpha to opaque.
            get
            {
                return this.ExStyles.Check(WindowExStyles.WS_EX_TRANSPARENT);
            }
            set
            {
                SetExStyle(WindowExStyles.WS_EX_TRANSPARENT, value);
                SetLayeredAttributesInternal(0, 0, 0);
            }
        }
        /// <summary>
        /// Conceals the control from the user.
        /// </summary>
        public bool Hide()
        {
            return ShowWindow(_handle, SW.SW_HIDE);
        }       
        /// <summary>
        /// Displays the window. 
        /// </summary>
        public bool Show()
        {
            return Show(true);
        }
        /// <summary>
        /// Displays the window. 
        /// </summary>
        /// <param name="activate">Whether the window have to be activated. </param>
        public bool Show(bool activate)
        {
            if (activate)
                return ShowWindow(_handle, SW.SW_SHOW);
            else
                return ShowWindow(_handle, SW.SW_SHOWNA);
        }
        /// <summary>
        /// Causes the window to redraw the invalidated regions within its client area.
        /// </summary>
        public bool Update()
        {
            return UpdateWindow(_handle);
        }
        /// <summary>
        /// Invalidates a specific region of the window and causes a paint message to be sent to the window.
        /// </summary>
        public void Invalidate(bool invalidateChildren)
        {
            if (invalidateChildren)
                RedrawWindow(_handle, IntPtr.Zero, IntPtr.Zero, RDW.RDW_ERASE | RDW.RDW_INVALIDATE | RDW.RDW_ALLCHILDREN);
            else
                InvalidateRect(_handle, IntPtr.Zero, Opacity != 1);
        }
        /// <summary>
        /// Invalidates the window and causes a paint message to be sent to the window.
        /// </summary>
        public void Invalidate()
        {
            this.Invalidate(false);
        }
        /// <summary>
        /// Forces the window to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        public void Refresh()
        {
            Invalidate(true);
            Update();
        }
        /// <summary>
        /// Sets window's layered attributes in according to the specified values an Transparent state.
        /// For more information, see SetLayeredWindowAttributes in MSDN.
        /// </summary>
        private bool SetLayeredAttributesInternal(int crKey, byte bAlpha, int dwFlags)
        {
            int key; byte alpha; int flags;
            bool changed = false;
            GetLayeredWindowAttributes(this._handle, out key, out alpha, out flags);
            if (((dwFlags & LWA_ALPHA) == LWA_ALPHA))
            {
                changed = true;
                alpha = bAlpha;
                if (alpha < 255)
                    flags |= LWA_ALPHA;
                else
                    flags &= ~LWA_ALPHA;
            }
            if (((dwFlags & LWA_COLORKEY) == LWA_COLORKEY))
            {
                changed = true;
                key = crKey;
                if (key != 0)
                    flags |= LWA_COLORKEY;
                else
                    flags &= ~LWA_COLORKEY;
            }
            if (flags == 0 && !this.Transparent)
                AllowTransparency = false;
            else
                AllowTransparency = true;
            if (flags == 0 && this.Transparent)
            {//If Transparent specified and no layout attributes, then set flags to LWA_ALPHA and alpha to opaque.
                changed = true;
                alpha = 255;
                flags = LWA_ALPHA;
            }
            else
                if(alpha==255&&((flags & LWA_ALPHA) == LWA_ALPHA))                
                    flags &= ~LWA_ALPHA;
            if (changed)
                return SetLayeredWindowAttributes(this._handle, key, alpha, flags);
            else
                return true;
        }
        #endregion Visibility

        #region Styles
        /// <summary>
        /// Get/Set current window styles. Remember, that changing the object, returned with this property does't change window's styles.
        /// Set: Styles just set, but don't apply. Call UpdateStyles() to apply them to the window.
        /// </summary>
        public WindowStyle Styles
        {
            get
            {
                return (WindowStyle)GetWindowLong(_handle, GWL_STYLE);
            }
            set
            {
                SetWindowLong(_handle, GWL_STYLE, value);
            }
        }
        /// <summary>
        /// Get/Set current window extended styles. Remember, that changing the object, returned with this property does't change window's styles.
        /// Set: Styles just set, but don't apply. Call UpdateExStyles() to apply them to the window.
        /// </summary>
        public WindowExStyle ExStyles
        {
            get
            {
                return (WindowExStyles)GetWindowLong(_handle, GWL_EXSTYLE);
            }
            set
            {
                SetWindowLong(_handle, GWL_EXSTYLE, value);
            }
        }
        /// <summary>
        /// Forces the assigned styles to be reapplied to the window.
        /// </summary>
        public void UpdateStyles()
        {
            SetWindowPos(this._handle, 0, 0, 0, 0, 0, WinPosFlags.SWP_FRAMECHANGED | WinPosFlags.SWP_NOZORDER |
                WinPosFlags.SWP_NOSIZE | WinPosFlags.SWP_NOMOVE | WinPosFlags.SWP_NOACTIVATE | WinPosFlags.SWP_NOOWNERZORDER);
        }
        /// <summary>
        /// Retrieves the value of the specified window styles for the window.
        /// </summary>
        /// <param name="styles">Checked styles. Could be specified more than one.</param>
        public bool GetStyle(WindowStyles styles)
        {
            return this.Styles.Check(styles);
        }
        /// <summary>
        /// Sets the specified styles to the specified value and applies them.
        /// </summary>
        /// <param name="styles">Styles to set. Could be specified more than one.</param>
        /// <param name="value">true to set specified styles to the window; false - to reset. </param>
        public void SetStyle(WindowStyles styles, bool value)
        {
            SetStyle(styles, value, true);
        }
        /// <summary>
        /// Sets the specified styles to the specified value.
        /// </summary>
        /// <param name="styles">Styles to set. Could be specified more than one.</param>
        /// <param name="value">true to set specified styles to the window; false - to reset. </param>
        /// <param name="applyStyles">true to apply specified styles to the window; false - just to set.</param>
        public void SetStyle(WindowStyles styles, bool value, bool applyStyles)
        {
            this.Styles = this.Styles.Set(styles, value);
            if (applyStyles)
                this.UpdateStyles();
        }
        /// <summary>
        /// Forces the assigned extended styles to be reapplied to the window.
        /// </summary>
        public void UpdateExStyles()
        {
            UpdateStyles();
        }
        /// <summary>
        /// Retrieves the value of the specified window extended styles for the window.
        /// </summary>
        /// <param name="styles">Checked styles. Could be specified more than one.</param>
        public bool GetExStyle(WindowExStyles styles)
        {
            return this.ExStyles.Check(styles);
        }
        /// <summary>
        /// Sets the specified extended styles to the specified value and applies them.
        /// </summary>
        /// <param name="styles">Styles to set. Could be specified more than one.</param>
        /// <param name="value">true to set specified styles to the window; false - to reset.</param>
        public void SetExStyle(WindowExStyles styles, bool value)
        {
            SetExStyle(styles, value, true);
        }
        /// <summary>
        /// Sets the specified styles to the specified value.
        /// </summary>
        /// <param name="styles">Styles to set. Could be specified more than one.</param>
        /// <param name="value">true to set specified styles to the window; false - to reset.</param>
        /// <param name="applyStyles">true to apply specified styles to the window; false - just to set.</param>
        public void SetExStyle(WindowExStyles styles, bool value, bool applyStyles)
        {
            this.ExStyles = this.ExStyles.Set(styles, value);
            if (applyStyles)
                this.UpdateExStyles();
        }
        #endregion Styles

        #region Window Info
        /// <summary>
        /// Gets the window handle.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return _handle;
            }
        }
        /// <summary>
        /// Determines whether this object identifies an existing window.
        /// </summary>
        public bool Exists
        {
            get
            {
                return IsWindow(_handle);
            }
        }
        /// <summary>
        /// Gets/sets the text associated with this window. Be careful with this property, check IsHung first.
        /// </summary>
        public string TextUnsafe
        {
            get
            {
                int textLength = (int)SendMessage(this._handle, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
                StringBuilder text = new StringBuilder(textLength+1);
                int lengthReaded = (int)SendMessage(this._handle, WM_GETTEXT, (IntPtr)text.Capacity, text);
                return text.ToString();
            }
            set
            {
                SendMessage(this._handle, WM_SETTEXT, IntPtr.Zero, new StringBuilder(value));
            }
        }

        /// <summary>
        /// Gets/sets the text associated with this window. 
        /// This property cann't get/set the text of the most window types. See TextUnsafe also.
        /// </summary>
        public string Text
        {
            get
            {
                StringBuilder text = new StringBuilder(260);
                int lengthReaded = GetWindowText(this.Handle, text, text.Capacity);
                return text.ToString();
            }
            set
            {
                SetWindowText(this.Handle, value);
            }
        }
        /// <summary>
        /// Gets/sets a handle to the menu assigned to the window.
        /// </summary>
        public IntPtr Menu
        {
            get
            {
                IntPtr pt = GetMenu(_handle);
                if (IsMenu(pt))
                    return pt;
                else
                    return IntPtr.Zero;
            }
            set
            {
                if (IsMenu(value))
                    SetMenu(_handle, value);
            }
        }
        /// <summary>
        /// Gets/sets a value indicating whether the window can respond to user interaction.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return IsWindowEnabled(_handle);
            }
            set
            {
                EnableWindow(_handle, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether the window is displayed modally.
        /// </summary>
        public bool Modal
        {
            get
            {
                WindowStyle styles = this.Styles;
                if (styles.Check(WindowStyles.WS_DISABLED) || !styles.Check(WindowStyles.WS_VISIBLE))
                    return false;
                CWindow res = 0;
                GCHandle hRes = GCHandle.Alloc(res);
                try
                {
                    EnumThreadWindows(new EnumWindowsProc(EnumModalProc), GCHandle.ToIntPtr(hRes));
                }
                catch { }
                if (hRes.IsAllocated)
                    hRes.Free();
                return res._handle == this._handle;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the window can receive focus.
        /// </summary>
        public bool CanFocus
        {
            get
            {
                return (Visible && Enabled);
            }
        }
        /// <summary>
        /// Gets a value indicating whether the window has input focus.
        /// </summary>
        public bool Focused
        {
            get
            {
                return GetFocus() == _handle;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the window, or one of its child windows, currently has the input focus.
        /// </summary>
        public bool ContainsFocus
        {
            get
            {
                IntPtr focus = GetFocus();
                if (focus == IntPtr.Zero)
                    return false;
                return ((focus == this._handle) || IsChild(this._handle, focus));
            }
        }
        /// <summary>
        /// Gets/sets a value indicating whether the window has captured the mouse.
        /// </summary>
        public bool Capture
        {
            get
            {
                return GetCapture() == _handle;
            }
            set
            {
                if (Capture != value)
                {
                    if (value)
                    {
                        SetCapture(_handle);
                    }
                    else
                    {
                        ReleaseCapture();
                    }
                }
            }
        }
        /// <summary>
        /// Gets the identifier of the thread that created this window.
        /// </summary>
        public uint ThreadId
        {
            get
            {
                uint prId = 0;
                return GetWindowThreadProcessId(this._handle, out prId);
            }
        }
        /// <summary>
        /// Gets the identifier of the process that created the window. 
        /// </summary>
        public uint ProcessId
        {
            get
            {
                uint prId = 0;
                GetWindowThreadProcessId(this._handle, out prId);
                return prId;
            }
        }
        /// <summary>
        /// Determines whether the specified window is a native Unicode window. For more information, see IsWindowUnicode in MSDN.
        /// </summary>
        public bool IsUnicodeWindow
        {
            get
            {
                return IsWindowUnicode(this._handle);
            }
        }
        /// <summary>
        /// Determine if Microsoft Windows considers that a specified application is not responding.
        /// For more information, see IsHungAppWindow in MSDN.
        /// </summary>
        public bool IsHung
        {
            get
            {
                return IsHungAppWindow(this._handle);
            }
        }
        /// <summary>
        /// Gets the name of the class to which the specified window belongs.
        /// For more information, see GetClassName in MSDN.
        /// </summary>
        public string ClassName
        {
            get
            {
                StringBuilder cName = new StringBuilder(261);
                int len = GetClassName(this._handle, cName, 260);
                return cName.ToString(0,len);
            }
        }
        /// <summary>
        /// Gets a string that specifies the window type.
        /// For more information, see RealClassName in MSDN.
        /// </summary>
        public string RealClassName
        {
            get
            {
                StringBuilder cName = new StringBuilder(261);
                RealGetWindowClass(this._handle, cName, 260);
                return cName.ToString();
            }
        }
        /// <summary>
        /// Gets a value indicating whether the window is displayed in the Windows taskbar. 
        /// </summary>
        public bool ShowInTaskbar
        {
            get
            {
                return this.ExStyles.Check(WindowExStyles.WS_EX_APPWINDOW);
            }
        }
        private Icon TestIcon(Icon ic)
        {
            if (ic != null)
                if (ic.Height > 0 && ic.Width > 0)
                    return ic;
            return null;
        }
        /// <summary>
        /// Gets the large icon for the window. Be careful with this property, check IsHung first.
        /// </summary>
        public Icon Icon
        {
            get
            {
                IntPtr icon = SendMessage(this._handle, WM_GETICON, (IntPtr)ICON_BIG, IntPtr.Zero);
                if (icon != IntPtr.Zero)
                    return TestIcon(Icon.FromHandle(icon));
                else
                    return null;
            }
        }
        /// <summary>
        /// Gets the small icon for the window. Be careful with this property, check IsHung first.
        /// </summary>
        public Icon SmallIcon
        {
            get
            {
                IntPtr icon = SendMessage(this._handle, WM_GETICON, (IntPtr)ICON_SMALL, IntPtr.Zero);
                if (icon != IntPtr.Zero)
                    return TestIcon(Icon.FromHandle(icon));
                else
                {
                    Version osVersion = System.Environment.OSVersion.Version;
                    if (osVersion.Major >= 5 && osVersion.Minor >= 1)//minimum operation system for ICON_SMALL2 is WinXP.
                        icon = SendMessage(this._handle, WM_GETICON, (IntPtr)ICON_SMALL2, IntPtr.Zero);
                    if (icon != IntPtr.Zero)
                        return TestIcon(Icon.FromHandle(icon));
                    return null;
                }
            }
        }
        /// <summary>
        /// Gets an icon associated with the window class. For more information, see GCL_HICON in MSDN.
        /// </summary>
        public Icon ClassIcon
        {
            get
            {
                IntPtr icon = GetClassLongPtr(this._handle, GCL_HICON);
                if (icon != IntPtr.Zero)
                    return TestIcon(Icon.FromHandle(icon));
                return null;
            }
        }
        /// <summary>
        /// Gets a small icon associated with the window class.For more information, see GCL_HICONSM in MSDN.
        /// </summary>
        public Icon SmallClassIcon
        {
            get
            {
                IntPtr icon = GetClassLongPtr(this._handle, GCL_HICONSM);
                if (icon != IntPtr.Zero)
                    return TestIcon(Icon.FromHandle(icon));
                return null;
            }
        }
        #endregion Window Info

        #region Other Functions
        /// <summary>
        /// Closes the window. 
        /// </summary>
        public void Close()
        {
            SysCommand(SC.SC_CLOSE, 0);
        }
        /// <summary>
        /// Activates the form and gives it focus. 
        /// Cann't activate window in some cases, for example, when a popup window is shown. For more information, see SetForegroundWindow in MSDN.
        /// If you want to activate window in any case use ForceActivate();
        /// </summary>
        public bool Activate()
        {
            return SetForegroundWindow(_handle);
        }
        /// <summary>
        /// Activates the window and gives it focus in any case. See Activate() also.
        /// </summary>
        public void ForceActivate()
        {
            if (!SetForegroundWindow(_handle))
            {
                IntPtr tmp = this._handle;
                CWindow topLevel = this.TopLevelWindow;
                bool animation = CWindow.StateChangeAnimation;
                if (animation)
                    CWindow.StateChangeAnimation = false;
                topLevel.Minimize();
                topLevel.Restore();
                if (animation)
                    CWindow.StateChangeAnimation = true;
                SetForegroundWindow(this._handle);
            }
        }
        /// <summary>
        /// Sets input focus to the control.
        /// </summary>
        /// <returns></returns>
        public bool Focus()
        {
            if (CanFocus)
                SetFocus(_handle);
            return this.Focused;
        }
        /// <summary>
        /// Creates the Graphics for the window. 
        /// </summary>
        public Graphics CreateGraphics()
        {
            return Graphics.FromHwnd(this._handle);
        }
        private bool SysCommand(SC wParam, int lParam)
        {
            return PostMessage(_handle, WM_SYSCOMMAND, (int)wParam, lParam);
        }
        #endregion Other Functions


        #region WinApi Functions
        static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return (IntPtr)GetClassLongPtr32(hWnd, nIndex);
        }
        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        static extern int GetClassLongPtr32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(int uiAction, int uiParam, ref ANIMATIONINFO pvParam, int fWinIni);


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetTopWindow", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        static extern IntPtr GetTopWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool SetWindowPos(IntPtr hWnd, WindowPosAfter hWndInsertAfter, int x, int y, int cx, int cy, WinPosFlags flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool ClientToScreen(IntPtr hWnd, ref POINT pt);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref RECT lpPoints, int cPoints);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern int GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT placement);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT placement);
        [DllImport("user32.dll")]
        static extern bool AdjustWindowRect(ref RECT lpRect, int dwStyle, bool bMenu);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool EnableWindow(IntPtr hWnd, bool enable);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        [DllImport("user32.dll", ExactSpelling = true)]
        static extern IntPtr GetAncestor(IntPtr hwnd, AncestorFlags gaFlags);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow_(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool IsWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool IsWindowEnabled(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr GetFocus();
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr GetCapture();
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr SetCapture(IntPtr hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        static extern bool IsWindowUnicode(IntPtr hWnd);


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool UpdateWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool RedrawWindow(IntPtr hwnd, IntPtr rcUpdate, IntPtr hrgnUpdate, RDW flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr rect, bool erase);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, SW nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref MINMAXINFO lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetMenu(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool IsMenu(IntPtr hMenu);
        [DllImport("user32.dll")]
        static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);
        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd,
           IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        static extern IntPtr BeginDeferWindowPos(int nNumWindows);
        [DllImport("user32.dll")]
        static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        [DllImport("user32.dll", EntryPoint = "ChildWindowFromPointEx", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr ChildWindowFromPointEx(IntPtr hwndParent, POINT pt, int uFlags);
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(POINT point);

        /// <summary>
        /// Delegate for the EnumChildWindows method
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
        /// <returns>True to continue enumerating, false to stop.</returns>
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr parameter);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "EnumWindows")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows_(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "EnumThreadWindows")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumThreadWindows_(uint dwThreadId, EnumWindowsProc lpfn, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint nullValue);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        [DllImport("user32.dll")]
        static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
        [DllImport("user32.dll")]
        static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsHungAppWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
        static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return (IntPtr)GetWindowLong32(hWnd, nIndex);
        }
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size > 4)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return (IntPtr)SetWindowLong32(hWnd, nIndex, (int)dwNewLong);
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        static extern uint RealGetWindowClass(IntPtr hwnd, [Out] StringBuilder pszType, uint cchType);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyIcon(IntPtr hIcon);
        [DllImport("gdi32.dll")]
        static extern int GetRgnBox(IntPtr hrgn, out RECT lprc);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetLayeredWindowAttributes(IntPtr hwnd, out int crKey, out byte bAlpha, out int dwFlags);
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);
        static Color ColorFromCOLORREF(int colorref)
        {
            int red = colorref & 0xff;
            int green = (colorref >> 8) & 0xff;
            int blue = (colorref >> 0x10) & 0xff;
            int a = (colorref>>18)&0xff;
            return Color.FromArgb(a,red, green, blue);
        }
        static int ColorToCOLORREF(Color color)
        {
            return ((color.A<<0x18)|color.R | (color.G << 8) | (color.B << 0x10));
        }
        #endregion  WinApi Functions
        #region WinApi Structs
        /// <summary>
        /// ANIMATIONINFO specifies animation effects associated with user actions.
        /// Used with SystemParametersInfo when SPI_GETANIMATION or SPI_SETANIMATION action is specified.
        /// </summary>
        /// <remark>
        /// The uiParam value must be set to (System.UInt32)Marshal.SizeOf(typeof(ANIMATIONINFO)) when using this structure.
        /// </remark>
        [StructLayout(LayoutKind.Sequential)]
        struct ANIMATIONINFO
        {
            /// <summary>
            /// Creates an AMINMATIONINFO structure.
            /// </summary>
            /// <param name="iMinAnimate">If non-zero and SPI_SETANIMATION is specified, enables minimize/restore animation.</param>
            public ANIMATIONINFO(System.Int32 iMinAnimate)
            {
                this.cbSize = Marshal.SizeOf(typeof(ANIMATIONINFO));
                this.iMinAnimate = iMinAnimate;
            }
            /// <summary>
            /// Always must be set to Marshal.SizeOf(typeof(ANIMATIONINFO)).
            /// </summary>
            public int cbSize;
            /// <summary>
            /// If non-zero, minimize/restore animation is enabled, otherwise disabled.
            /// </summary>
            public int iMinAnimate;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public int ptMinPosition_x;
            public int ptMinPosition_y;
            public int ptMaxPosition_x;
            public int ptMaxPosition_y;
            public int rcNormalPosition_left;
            public int rcNormalPosition_top;
            public int rcNormalPosition_right;
            public int rcNormalPosition_bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.x, p.y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(Rectangle r)
            {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }

            public static RECT Empty { get { return new RECT(0, 0, 0, 0); } }
            public int Height { get { return bottom - top; } }
            public int Width { get { return right - left; } }
            public Size Size { get { return new Size(this.right - this.left, this.bottom - this.top); } }
            public Point Location { get { return new Point(left, top); } }

            public static implicit operator RECT(Rectangle p) { return new RECT(p); }
            public static implicit operator Rectangle(RECT p) { return Rectangle.FromLTRB(p.left, p.top, p.right, p.bottom); }

            public static RECT FromXYWH(int x, int y, int width, int height) { return new RECT(x, y, x + width, y + height); }
        }
        #endregion WinApi Structs
        #region WinApi Constants
        const int
            SPI_GETANIMATION = 0x0048,
            SPI_SETANIMATION = 0x0049,
            WM_GETMINMAXINFO = 0x0024,
            LWA_COLORKEY = 0x00000001,
            LWA_ALPHA = 0x00000002,
            WPF_RESTORETOMAXIMIZED = 0x0002,
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_SETTEXT = 0x000C,
            ICON_SMALL = 0,
            ICON_BIG = 1,
            ICON_SMALL2 = 2,
            WM_GETICON = 0x007F,
            WM_SETICON = 0x0080,
            GCL_HICON = (-14),
            GCL_HICONSM = (-34),
            WM_SYSCOMMAND = 0x0112;
        enum AncestorFlags : int
        {
            /// <summary>
            /// Retrieves the parent window. This does not include the owner, as it does with the GetParent function. 
            /// </summary>
            GA_PARENT = 1,
            /// <summary>
            /// Retrieves the root window by walking the chain of parent windows.
            /// </summary>
            GA_ROOT = 2,
            /// <summary>
            /// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent. 
            /// </summary>
            GA_ROOTOWNER = 3
        }
        enum SW: int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }
        enum SC: int
        {
            SC_SIZE = 0xF000,
            SC_MOVE = 0xF010,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
            SC_NEXTWINDOW = 0xF040,
            SC_PREVWINDOW = 0xF050,
            SC_CLOSE = 0xF060,
            SC_VSCROLL = 0xF070,
            SC_HSCROLL = 0xF080,
            SC_MOUSEMENU = 0xF090,
            SC_KEYMENU = 0xF100,
            SC_ARRANGE = 0xF110,
            SC_RESTORE = 0xF120,
            SC_TASKLIST = 0xF130,
            SC_SCREENSAVE = 0xF140,
            SC_HOTKEY = 0xF150,
            SC_DEFAULT = 0xF160,
            SC_MONITORPOWER = 0xF170,
            SC_CONTEXTHELP = 0xF180,
            SC_SEPARATOR = 0xF00F,
            SC_ICON = SC_MINIMIZE,
            SC_ZOOM = SC_MAXIMIZE
        }
        enum RDW: int
        {
            RDW_INVALIDATE = 0x0001,
            RDW_INTERNALPAINT = 0x0002,
            RDW_ERASE = 0x0004,
            RDW_VALIDATE = 0x0008,
            RDW_NOINTERNALPAINT = 0x0010,
            RDW_NOERASE = 0x0020,
            RDW_NOCHILDREN = 0x0040,
            RDW_ALLCHILDREN = 0x0080,
            RDW_UPDATENOW = 0x0100,
            RDW_ERASENOW = 0x0200,
            RDW_FRAME = 0x0400,
            RDW_NOFRAME = 0x0800
        }
        #endregion WinApi Constants
    }

    public enum GetWindow_Cmd : int
    {
        /// <summary>
        /// The retrieved handle identifies the window of the same type that is highest in the Z order. 
        /// If the specified window is a topmost window, the handle identifies the topmost window that is highest in the Z order. 
        /// If the specified window is a top-level window, the handle identifies the top-level 
        /// window that is highest in the Z order. If the specified window is a child window, 
        /// the handle identifies the sibling window that is highest in the Z order.
        /// </summary>
        GW_HWNDFIRST = 0,
        /// <summary>
        /// The retrieved handle identifies the window of the same type that is lowest in the Z order. 
        /// If the specified window is a topmost window, the handle identifies the topmost window that is lowest in the Z order. 
        /// If the specified window is a top-level window, the handle identifies the top-level 
        /// window that is lowest in the Z order. If the specified window is a child window, the handle identifies 
        /// the sibling window that is lowest in the Z order.
        /// </summary>
        GW_HWNDLAST = 1,
        /// <summary>
        /// The retrieved handle identifies the window below the specified window in the Z order. 
        /// If the specified window is a topmost window, the handle identifies the topmost window below 
        /// the specified window. If the specified window is a top-level window, the handle identifies 
        /// the top-level window below the specified window. If the specified window is a child window, 
        /// the handle identifies the sibling window below the specified window.
        /// </summary>
        GW_HWNDNEXT = 2,
        /// <summary>
        /// The retrieved handle identifies the window above the specified window in the Z order. 
        /// If the specified window is a topmost window, the handle identifies the topmost window above the specified window. 
        /// If the specified window is a top-level window, the handle identifies the top-level window above the specified window. 
        /// If the specified window is a child window, the handle identifies the sibling window above the specified window.
        /// </summary>
        GW_HWNDPREV = 3,
        /// <summary>
        /// The retrieved handle identifies the specified window's owner window, if any. For more information, see Owned Windows.
        /// </summary>
        GW_OWNER = 4,
        /// <summary>
        /// The retrieved handle identifies the child window at the top of the Z order, 
        /// if the specified window is a parent window; otherwise, the retrieved handle is NULL. 
        /// The function examines only child windows of the specified window. It does not examine descendant windows.
        /// </summary>
        GW_CHILD = 5,
        /// <summary>
        /// Windows 2000/XP: The retrieved handle identifies the enabled popup window owned by the specified window 
        /// (the search uses the first such window found using GW_HWNDNEXT); otherwise, if there are no enabled popup 
        /// windows, the retrieved handle is that of the specified window. 
        /// </summary>
        GW_ENABLEDPOPUP = 6
    }
    public enum WindowPosAfter:int
    {
        HWND_TOP = (0),
        HWND_BOTTOM = (1),
        HWND_TOPMOST = (-1),
        HWND_NOTOPMOST = (-2)
    }
    enum WinPosFlags : int
    {
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_NOREDRAW = 0x0008,
        SWP_NOACTIVATE = 0x0010,//
        SWP_FRAMECHANGED = 0x0020  /* The frame changed: send WM_NCCALCSIZE */,
        SWP_SHOWWINDOW = 0x0040,
        SWP_HIDEWINDOW = 0x0080,
        SWP_NOCOPYBITS = 0x0100,
        SWP_NOOWNERZORDER = 0x0200  /* Don't do owner Z ordering */,
        SWP_NOSENDCHANGING = 0x0400  /* Don't send WM_WINDOWPOSCHANGING */,
        SWP_DRAWFRAME = SWP_FRAMECHANGED,
        SWP_NOREPOSITION = SWP_NOOWNERZORDER,
        SWP_DEFERERASE = 0x2000,
        SWP_ASYNCWINDOWPOS = 0x4000
    }
    public enum WindowStyles : int
    {
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = (int)(-0x80000000),
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_CAPTION = 0x00C00000,     /* WS_BORDER | WS_DLGFRAME  */
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,
        WS_TILED = WS_OVERLAPPED,
        WS_ICONIC = WS_MINIMIZE,
        WS_SIZEBOX = WS_THICKFRAME,
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
        WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
        WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU),
        WS_CHILDWINDOW = WS_CHILD
    }
    public enum WindowExStyles : int
    {
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_ACCEPTFILES = 0x00000010,
        /// <summary>
        /// Specifies that a window should not be painted until siblings beneath the window (that were created by the same thread) 
        /// have been painted. You can use it for hit testing - the mouse events will be passed to the other windows underneath the layered window.
        /// </summary>
        WS_EX_TRANSPARENT = 0x00000020,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_WINDOWEDGE = 0x00000100,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
        WS_EX_LAYERED = 0x00080000,
        WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
        WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_NOACTIVATE = 0x08000000
    }
    
    /// <summary>
    /// Class for changing several window's properties in a single screen-refreshing cycle.
    /// Specify what you need and call RePosition().
    /// </summary>
    public class CWindowWorker
    {
        public static implicit operator CWindowWorker(CWindow window)
        {
            return new CWindowWorker(window);
        }
        public static implicit operator CWindowWorker(IntPtr handle)
        {
            return new CWindowWorker(handle);
        }

        private IntPtr handle;
        private Flags flags;
        private Point location;
        private bool visible;
        private Size size;
        private WindowPosAfter order;

        public CWindowWorker(CWindow window)
        {
            handle = window.Handle;
        }

        /// <summary>
        /// Sets window location in client coordinates of parent's client area.
        /// </summary>
        public Point Location
        {
            set
            {
                location = value;
                flags |= Flags.Location;
            }
        }
        /// <summary>
        /// Sets size of the window.
        /// </summary>
        public Size Size
        {
            set
            {
                size = value;
                flags |= Flags.Size;
            }
        }
        /// <summary>
        /// Sets visible state of the window.
        /// </summary>
        public bool Visible
        {
            set
            {
                visible = value;
                flags |= Flags.Show;
            }
        }
        /// <summary>
        /// Changes window placement in the Z-order. You can use this function only once for current reposition.
        /// </summary>
        /// <param name="insertAfter">A window to precede the positioned window in the Z order.</param>
        public bool SetOrder(CWindow insertAfter)
        {
            if ((flags & Flags.Order) == Flags.Order)
                throw new Exception("You have already set the order.");
            this.order = (WindowPosAfter)insertAfter.Handle;
            flags |= Flags.Order;
            return true;
        }
        /// <summary>
        /// Changes window placement in the Z-order. You can use this function only once for current reposition.
        /// </summary>
        /// <param name="order">Specific window order positions.</param>
        public bool SetOrder(WindowPosAfter order)
        {
            if ((flags & Flags.Order) == Flags.Order)
                throw new Exception("You already set the order.");
            this.order = order;
            flags |= Flags.Order;
            return true;
        }
        /// <summary>
        /// Sets the specified styles to the specified value. For details see CWindow.SetStyle()
        /// </summary>
        public void SetStyle(WindowStyles styles, bool value)
        {
            CWindow win = handle;
            win.SetStyle(styles, value, false);
            flags |= Flags.Styles;
        }
        /// <summary>
        /// Sets the specified extended styles to the specified value. For details see CWindow.SetExStyle()
        /// </summary>
        public void SetExStyle(WindowExStyles styles, bool value)
        {
            CWindow win = handle;
            win.SetExStyle(styles, value, false);
            flags |= Flags.Styles;
        }
        /// <summary>
        /// Activates the window.
        /// </summary>
        public void Activate()
        {
            flags |= Flags.Activate;
        }
        /// <summary>
        /// Repositions the window. It is recomended to use this function only once for current object.
        /// </summary>
        public bool RePosition()
        {
            IntPtr winPosInfo = BeginDeferWindowPos(1);
            winPosInfo = InsertReposition(winPosInfo);
            if (winPosInfo == IntPtr.Zero)
                return false;
            return EndDeferWindowPos(winPosInfo);
        }
        /// <summary>
        /// Don't use this function. It is for inner use only.
        /// </summary>
        internal IntPtr InsertReposition(IntPtr winPosInfo)
        {
            if (!((CWindow)handle).Exists)
                return winPosInfo;
            WinPosFlags wFlags = 0;
            if ((flags & Flags.Activate) != Flags.Activate)
                wFlags |= WinPosFlags.SWP_NOACTIVATE;
            if ((flags & Flags.Location) != Flags.Location)
                wFlags |= WinPosFlags.SWP_NOMOVE;
            if ((flags & Flags.Order) != Flags.Order)
                wFlags |= WinPosFlags.SWP_NOZORDER;
            if ((flags & Flags.Show) == Flags.Show)
                wFlags |= visible ? WinPosFlags.SWP_SHOWWINDOW : WinPosFlags.SWP_HIDEWINDOW;
            if ((flags & Flags.Size) != Flags.Size)
                wFlags |= WinPosFlags.SWP_NOSIZE;
            if ((flags & Flags.Styles) == Flags.Styles)
                wFlags |= WinPosFlags.SWP_FRAMECHANGED;

            wFlags |= WinPosFlags.SWP_NOOWNERZORDER;
            return DeferWindowPos(winPosInfo, handle, order, location.X, location.Y, size.Width, size.Height, wFlags);
        }
        #region Win32 functions
        [DllImport("user32.dll")]
        static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, WindowPosAfter hWndInsertAfter, int x, int y, int cx, int cy, WinPosFlags uFlags);
        [DllImport("user32.dll")]
        static extern IntPtr BeginDeferWindowPos(int nNumWindows);
        [DllImport("user32.dll")]
        static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);
        #endregion Win32 functions
        private enum Flags : int
        {
            Location = 1,
            Size = 2,
            Order = 4,
            Activate = 8,
            Show = 16,
            Styles = 32
        }
    }
    /// <summary>
    /// Class for changing properties of several windows in a single screen-refreshing cycle.  
    /// Specify what you need and call RePosition().
    /// </summary>
    public class CWindowsWorker
    {
        private Dictionary<IntPtr, CWindowWorker> dic;

        public CWindowsWorker()
        {
            dic = new Dictionary<IntPtr, CWindowWorker>();
        }

        /// <summary>
        /// Gets the window's properties structure in witch you can specify new properties.
        /// </summary>
        /// <param name="window">Specify the window, properties of witch you want to change.</param>
        public CWindowWorker this[CWindow window]
        {
            get
            {
                if (!dic.ContainsKey(window))
                {
                    dic.Add(window, new CWindowWorker(window));
                }
                return dic[window];
            }
        }
        /// <summary>
        /// Makes reposition of all windows, that you have specified.
        /// </summary>
        /// <returns></returns>
        public bool RePosition()
        {
            IntPtr winPosInfo = BeginDeferWindowPos(dic.Keys.Count);
            foreach (IntPtr key in dic.Keys)
            {
                winPosInfo = dic[key].InsertReposition(winPosInfo);
                if (winPosInfo == IntPtr.Zero)
                    return false;
            }
            return EndDeferWindowPos(winPosInfo);
        }
        #region Win32 functions
        [DllImport("user32.dll")]
        static extern IntPtr BeginDeferWindowPos(int nNumWindows);
        [DllImport("user32.dll")]
        static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);
        #endregion Win32 functions
    }
    public class WindowStyle
    {
        WindowStyles styles;
        public WindowStyle(WindowStyles wStyles)
        {
            styles = wStyles;
        }
        public WindowStyles Set(WindowStyles wStyles, bool value)
        {
            if (value)
                styles |= wStyles;
            else
                styles &= ~wStyles;
            return styles;
        }
        public bool Check(WindowStyles wStyles) { return (styles & wStyles) == wStyles ? true : false; }
        public static implicit operator WindowStyle(WindowStyles wStyles) { return new WindowStyle(wStyles); }
        public static implicit operator WindowStyles(WindowStyle wStyles) { return wStyles.styles; }
        public static implicit operator WindowStyle(IntPtr wStyles) { return new WindowStyle((WindowStyles)wStyles); }
        public static implicit operator IntPtr(WindowStyle wStyles) { return (IntPtr)wStyles.styles; }
        public static implicit operator WindowStyle(int wStyles) { return new WindowStyle((WindowStyles)wStyles); }
        public static implicit operator int(WindowStyle wStyles) { return (int)wStyles.styles; }
    }
    public class WindowExStyle
    {
        WindowExStyles exStyles;
        public WindowExStyle(WindowExStyles wStyles)
        {
            exStyles = wStyles;
        }
        public WindowExStyles Set(WindowExStyles wStyles, bool value)
        {
            if (value)
                exStyles |= wStyles;
            else
                exStyles &= ~wStyles;
            return exStyles;
        }
        public bool Check(WindowExStyles wStyles) { return (exStyles & wStyles) == wStyles ? true : false; }
        public static implicit operator WindowExStyle(WindowExStyles wStyles) { return new WindowExStyle(wStyles); }
        public static implicit operator WindowExStyles(WindowExStyle wStyles) { return wStyles.exStyles; }
        public static implicit operator WindowExStyle(IntPtr wStyles) { return new WindowExStyle((WindowExStyles)wStyles); }
        public static implicit operator IntPtr(WindowExStyle wStyles) { return (IntPtr)wStyles.exStyles; }
        public static implicit operator WindowExStyle(int wStyles) { return new WindowExStyle((WindowExStyles)wStyles); }
        public static implicit operator int(WindowExStyle wStyles) { return (int)wStyles.exStyles; }
    }
}