namespace OxLibrary.Controls
{
    /// <remarks>
    /// The Control interface that is supposed to be used in the OxDaoEngine component system.
    /// In addition to implementing IOxControl, it is also necessary 
    /// to implement the parent interface IOxControlManager.<br/><br/>
    /// <b>Firstly:</b> you need registry manager as OxControlManager&lt;BaseControl&gt; by call 
    /// <code>manager = OxControlManager.RegisterControl&lt;BaseControlClass&gt;(this,OnSizeChanged);</code>
    /// <b>Secondly:</b> add to your class code, like following 
    /// (<comment>for all code, see comments below ther IOxControl description in IOxControl.cs</comment>):<br/><br/>
    /// <code>
    /// public new OxWidth Width { get =&gt; manager.Width; set =&gt; manager.Width = value;}<br/>
    /// ...<br/>
    /// public virtual bool OnSizeChanged(SizeChangedEventArgs e)<br/>
    /// {<br/>
    ///     if (!SizeChanging &amp;&amp; e.Changed)<br/>
    ///         base.OnSizeChanged(e);<br/>
    ///     return e.Changed;<br/>
    /// }<br/><br/>
    /// protected override sealed void OnSizeChanged(EventArgs e) =&gt;<br/>
    ///     base.OnSizeChanged(e);
    /// </code>
    /// </remarks>
    public interface IOxControl<TBaseControl> : IOxControlManager
        where TBaseControl : Control
    {
        OxControlManager<TBaseControl> Manager { get; }
        bool Visible { get; set; }
        bool Enabled { get; set; }
        string Text { get; set; }
        string Name { get; set; }
        Color BackColor { get; set; }
        Control? Parent { get; set; }
        object Tag { get; set; }
        void Update();
        void BringToFront();
        void SendToBack();
        void Dispose();

        event EventHandler? VisibleChanged;
        event EventHandler? SizeChanged;
        event EventHandler? Click;
        event EventHandler? DoubleClick;
        event EventHandler? Enter;
        event EventHandler? GotFocus;
        event KeyEventHandler? KeyDown;
        event KeyPressEventHandler? KeyPress;
        event KeyEventHandler? KeyUp;
    }

    /**
       IOxControlManager Code implementation example for IOxControl<BaseControlClass>
       private readonly OxControlManager<BaseControlClass> manager;

       public OxControlManager<BaseControlClass> Manager => manager;

       public ... ()
       {
          maneger OxControlManager.RegisterControl<BaseControlClass>(this,OnSizeChanged);
       }

        public new OxWidth Width
        {
            get => manager.Width;
            set => manager.Width = value;
        }

        public new OxWidth Height
        {
            get => manager.Height;
            set => manager.Height = value;
        }

        public new OxWidth Top
        {
            get => manager.Top;
            set => manager.Top = value;
        }

        public new OxWidth Left
        {
            get => manager.Left;
            set => manager.Left = value;
        }

        public new OxWidth Bottom => manager.Bottom;

        public new OxWidth Right => manager.Right;

        public new OxSize Size
        {
            get => manager.Size;
            set => manager.Size = value;
        }

        public new OxSize ClientSize
        {
            get => manager.ClientSize;
            set => manager.ClientSize = value;
        }

        public new OxPoint Location 
        {
            get => manager.Location;
            set => manager.Location = value;
        }


        public new OxSize MinimumSize
        {
            get => manager.MinimumSize;
            set => manager.MinimumSize = value;
        }

        public new OxSize MaximumSize
        {
            get => manager.MaximumSize;
            set => manager.MaximumSize = value;
        }

        public new OxDock Dock
        {
            get => manager.Dock;
            set => manager.Dock = value;
        }

        private new void SetBounds(int x, int y, int width, int height) { }

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            manager.SetBounds(x, y, width, height);

        public virtual bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (!SizeChanging && e.Changed)
                base.OnSizeChanged(e);

            return e.Changed;
        }

        protected override sealed void OnSizeChanged(EventArgs e) =>
            base.OnSizeChanged(e);
     */
}