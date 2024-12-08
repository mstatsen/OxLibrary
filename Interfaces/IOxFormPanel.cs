namespace OxLibrary.Interfaces;

public interface IOxFormPanel : IOxFormPanelBase
{
    IOxForm? Form { get; set; }
}