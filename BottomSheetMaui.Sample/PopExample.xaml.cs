namespace BottomSheetMaui.Sample;

public partial class PopExample : Popup
{
	public PopExample()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		Close(pgEntry.Text);
    }
}