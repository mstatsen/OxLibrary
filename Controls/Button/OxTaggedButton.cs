namespace OxLibrary.Controls;

public class OxTaggedButton : OxButton
{
    public OxTaggedButton(short tag) : base(tag.ToString(), null) { }

    private short tag = 0;
    public new short Tag 
    { 
        get => tag; 
        set => SetTag(value); 
    }

    private void SetTag(short value)
    {
        tag = value;
        Text = tag.ToString();
    }
}