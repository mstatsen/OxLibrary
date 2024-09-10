namespace OxLibrary.Dialogs
{
    public delegate string GetEmptyMandatoryFieldName();

    public class OxDialog : OxForm
    {
        public OxDialog() : base()
        {
            Sizeble = false;
            CanMaximize = false;
            CanMinimize = false;
        }

        protected override void SetUpForm()
        {
            base.SetUpForm();
            MinimizeBox = false;
            MaximizeBox = false;
            KeyPreview = true;
        }

        public new OxDialogMainPanel MainPanel
        {
            get => (OxDialogMainPanel)base.MainPanel;
            set => base.MainPanel = value;
        }

        public OxDialogButton DialogButtons
        {
            get => MainPanel.DialogButtons;
            set => MainPanel.DialogButtons = value;
        }

        protected override OxFormMainPanel CreateMainPanel() =>
            new OxDialogMainPanel(this);

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!e.Handled && e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            e.Cancel = DialogResult switch
            {
                DialogResult.OK or
                DialogResult.Yes or
                DialogResult.Continue => 
                    !CanOKClose(),
                DialogResult.Cancel => 
                    !CanCancelClose(),
                _ => 
                    false,
            };
        }

        public GetEmptyMandatoryFieldName? GetEmptyMandatoryFieldName;

        private bool CheckMandatoryFields()
        {
            string? emptyMandatoryField = GetEmptyMandatoryFieldName?.Invoke();
            emptyMandatoryField ??= EmptyMandatoryField();

            if (emptyMandatoryField == string.Empty)
                return true;

            OxMessage.ShowError($"{emptyMandatoryField} is mandatory");
            return false;
        }

        protected virtual string EmptyMandatoryField() =>
            string.Empty;

        public virtual bool CanOKClose() =>
            CheckMandatoryFields();

        public virtual bool CanCancelClose() => true;
    }
}