using System.Windows;
using System.Windows.Input;
using R6ProfileSwitcherWPF.ViewModels;

namespace R6ProfileSwitcherWPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Subscribe to key events
            this.KeyDown += MainWindow_KeyDown;

            // Focus the window so key events are caught
            this.Loaded += (s, e) => Keyboard.Focus(this);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"KEY PRESSED IN APP: {e.Key}");

            if (DataContext is MainWindowViewModel vm)
            {
                vm.HandleKeyPress(e);
            }
        }

    }
}
