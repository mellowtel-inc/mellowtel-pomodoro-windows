using System.Windows;

namespace PomodoroTimer.Dialogs;

public partial class ConsentDialog : Window
{
    public ConsentDialog()
    {
        InitializeComponent();
    }

    private void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void DeclineButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
