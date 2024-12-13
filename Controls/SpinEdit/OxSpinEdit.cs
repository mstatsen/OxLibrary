using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxSpinEdit : OxFrame
    {
        private int minimum;
        private int maximum;
        private int LastValue = 0;

        public bool ShowStepButtons { get; set; } = true;
        private readonly OxTextBox TextBox = new()
        {
            Top = 0,
            TextAlign = HorizontalAlignment.Center,
            Dock = OxDock.Fill,
            BorderStyle = BorderStyle.None
        };
        private readonly OxIconButton DecreaseButton = CreateButton(OxIcons.Minus, OxDock.Left);
        private readonly OxIconButton IncreaseButton = CreateButton(OxIcons.Plus, OxDock.Right);

        protected override void PrepareInnerComponents()
        {
            base.PrepareInnerComponents();
            DecreaseButton.Parent = this;
            IncreaseButton.Parent = this;
            DecreaseButton.Click += (s, e) => Value -= Step;
            IncreaseButton.Click += (s, e) => Value += Step;
            PrepareTextBox();
        }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            Minimum = 0;
            Maximum = 100;
            Step = 1;
            EnableButtons();
        }

        private static OxIconButton CreateButton(Bitmap icon, OxDock dock) =>
            new(icon, 14)
            {
                Dock = dock,
                HiddenBorder = false,
                FixedBorderColor = true
            };

        public int Value
        {
            get
            {
                if (!int.TryParse(TextBox.Text, out int currentValue))
                    currentValue = LastValue;

                return currentValue;
            }
            set
            {
                if (!TextBox.Text.Equals(value.ToString()))
                {
                    TextBox.Text = value.ToString();
                    Text = TextBox.Text;
                    CheckValue();
                    LastValue = value;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }

                EnableButtons();
            }
        }

        private void EnableButtons()
        {
            DecreaseButton.SetVisible(ShowStepButtons && !TextBox.ReadOnly && (Value > minimum));
            IncreaseButton.SetVisible(ShowStepButtons && !TextBox.ReadOnly && (Value < maximum));
            Borders[OxDock.Left].SetVisible(!ShowStepButtons || TextBox.ReadOnly || (Value <= minimum));
            Borders[OxDock.Right].SetVisible(!ShowStepButtons || TextBox.ReadOnly || (Value >= maximum));
        }

        public int Minimum
        {
            get => minimum;
            set
            {
                if (!minimum.Equals(value))
                {
                    minimum = value;
                    CheckValue();
                }

                EnableButtons();
            }
        }

        public int Maximum 
        { 
            get => maximum;
            set
            {
                if (!maximum.Equals(value))
                {
                    maximum = value;
                    CheckValue();
                }

                EnableButtons();
            }
        }

        public int Step { get; set; }

        private void CheckValue()
        {
            if (maximum < minimum)
                maximum = minimum;

            if (Value < minimum)
                Value = minimum;

            if (Value > maximum)
                Value = maximum;
        }

        private void PrepareTextBox()
        {
            TextBox.Parent = this;
            TextBox.Left = DecreaseButton.Right;
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            TextBox.TextChanged += (s, e) => SetValue(TextBox.Text);
            TextBox.MouseWheel += (s, e) => IncreaseValue(e.Delta > 0 ? Step : -Step);
            TextBox.KeyDown += TextBoxKeyDownHandler;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            TextBoxKeyDownHandler(this, e);
        }

        private void IncreaseValue(int increase)
        {
            if (TextBox.ReadOnly)
                return;

            Value += increase * (ModifierKeys.HasFlag(Keys.Control) ? 10 : 1);
        }

        private void TextBoxKeyDownHandler(object? sender, KeyEventArgs e) => 
            IncreaseValue(e.KeyData switch
                {
                    Keys.Up => Step,
                    Keys.Down => -Step,
                    _ => 0
                }
            );

        public override void PrepareColors()
        {
            base.PrepareColors();
            DecreaseButton.BaseColor = Colors.BaseColor;
            IncreaseButton.BaseColor = Colors.BaseColor;
            TextBox.BackColor = Colors.Lighter(7);
        }

        private void SetValue(string text)
        {
            if (text.Equals(string.Empty))
                Value = 0;
            else
            if (int.TryParse(text, out int newValue)
                && newValue >= minimum
                && newValue <= maximum)
                Value = newValue;
            else TextBox.Text = LastValue.ToString();

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override Color GetBorderColor() =>
            IsEnabled
                ? Colors.Darker(8)
                : Colors.Lighter(2);

        public EventHandler? ValueChanged;

        public OxSpinEdit() : base(new(80, 22)) { }

        public bool ReadOnly
        {
            get => TextBox.ReadOnly;
            set
            {
                TextBox.ReadOnly = value;
                EnableButtons();
            }
        }
    }
}