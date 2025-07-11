﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

using R6ProfileSwitcherWPF.Models;

namespace R6ProfileSwitcherWPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Operator> AllOperators { get; set; } = [];
        public ObservableCollection<Operator> FilteredOperators { get; set; } = [];

        private OperatorRole _selectedRole = OperatorRole.Attacker;
        public OperatorRole SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (_selectedRole != value)
                {
                    _selectedRole = value;
                    OnPropertyChanged(nameof(SelectedRole));
                    FilterOperators();
                }
            }
        }

        private Operator? _selectedOperator;
        public Operator? SelectedOperator
        {
            get => _selectedOperator;
            set
            {
                if (_selectedOperator != value)
                {
                    _selectedOperator = value;
                    OnPropertyChanged(nameof(SelectedOperator));
                    OnPropertyChanged(nameof(WindowTitle));
                }
            }
        }

        public string WindowTitle =>
            IsHotkeyModeActive
                ? "Enter The Operator Number - HOTKEY MODE ACTIVE"
                : SelectedOperator != null
                    ? $"Selected Operator - {SelectedOperator.Name}"
                    : "No Operators Selected";

        public ICommand SelectOperatorCommand { get; }

        public MainWindowViewModel()
        {
            LoadOperators();
            SelectOperatorCommand = new RelayCommand(SelectOperator);
        }

        private void SelectOperator(object? parameter)
        {
            if (parameter is Operator selected)
            {
                foreach (var op in AllOperators)
                    op.IsSelected = false;

                selected.IsSelected = true;
                SelectedOperator = selected;

                FilterOperators();

                Debug.WriteLine($"Operator selected: {selected.Name}");

                // Later: navigate to operator detail page instead of sending keystrokes
            }
        }

        #region Hotkey Handling

        private bool _isHotkeyModeActive;
        public bool IsHotkeyModeActive
        {
            get => _isHotkeyModeActive;
            set
            {
                if (_isHotkeyModeActive != value)
                {
                    _isHotkeyModeActive = value;
                    OnPropertyChanged(nameof(IsHotkeyModeActive));
                    OnPropertyChanged(nameof(WindowTitle));
                }
            }
        }

        private string _digitBuffer = "";
        public void HandleKeyPress(KeyEventArgs e)
        {
            Debug.WriteLine($"Key pressed: {e.Key}");

            if (!IsHotkeyModeActive)
            {
                if (e.Key == Key.F12 && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    IsHotkeyModeActive = true;
                    _digitBuffer = "";
                    Debug.WriteLine("Hotkey mode activated.");
                }
                return;
            }

            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                AddDigit((e.Key - Key.D0).ToString());
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                AddDigit((e.Key - Key.NumPad0).ToString());
            }
            else if (e.Key == Key.Enter)
            {
                ParseBuffer();
            }
            else if (e.Key == Key.Escape)
            {
                CancelHotkeyMode();
            }
        }

        private void AddDigit(string digit)
        {
            if (_digitBuffer.Length < 2)
            {
                _digitBuffer += digit;
                Debug.WriteLine($"Digit added: {digit}. Buffer: {_digitBuffer}");
            }
        }

        private void ParseBuffer()
        {
            if (int.TryParse(_digitBuffer, out int opNumber))
            {
                var op = AllOperators.FirstOrDefault(o => o.Number == opNumber);
                if (op != null)
                {
                    SelectOperator(op);
                    Debug.WriteLine($"Operator #{opNumber} selected: {op.Name}");
                }
                else
                {
                    Debug.WriteLine($"No operator found for #{opNumber}");
                }
            }
            else
            {
                Debug.WriteLine($"Invalid number entered: {_digitBuffer}");
            }

            ExitHotkeyMode();
        }

        private void CancelHotkeyMode()
        {
            Debug.WriteLine("Hotkey mode cancelled.");
            _digitBuffer = "";
            IsHotkeyModeActive = false;
        }

        private void ExitHotkeyMode()
        {
            _digitBuffer = "";
            IsHotkeyModeActive = false;
        }

        #endregion

        private void LoadOperators()
        {
            var allOperators = new List<Operator>();

            string[] attackerNames =
            [
                "Sledge", "Thatcher", "Ash", "Thermite", "Twitch", "Montagne", "Glaz", "Fuze", "Blitz", "IQ",
                "Buck", "Blackbeard", "Capitao", "Hibana", "Jackal", "Ying", "Zofia", "Dokkaebi", "Lion", "Finka",
                "Maverick", "Nomad", "Gridlock", "Nokk", "Amaru", "Kali", "Iana", "Ace", "Zero", "Flores",
                "Osa", "Sens", "Brava"
            ];

            string[] defenderNames =
            [
                "Smoke", "Mute", "Castle", "Pulse", "Doc", "Rook", "Kapkan", "Tachanka", "Jäger", "Bandit",
                "Frost", "Valkyrie", "Caveira", "Echo", "Mira", "Lesion", "Ela", "Vigil", "Maestro", "Alibi",
                "Clash", "Kaid", "Mozzie", "Warden", "Goyo", "Wamai", "Aruni", "Thunderbird", "Thorn",
                "Azami", "Solis", "Fenrir"
            ];

            int number = 1;

            foreach (var name in attackerNames)
            {
                allOperators.Add(new Operator(
                    name,
                    number++,
                    OperatorRole.Attacker,
                    GetOperatorImagePath(name)
                ));
            }

            foreach (var name in defenderNames)
            {
                allOperators.Add(new Operator(
                    name,
                    number++,
                    OperatorRole.Defender,
                    GetOperatorImagePath(name)
                ));
            }

            AllOperators = new ObservableCollection<Operator>(allOperators);
            FilterOperators();
        }

        private static string GetOperatorImagePath(string name)
        {
            string[] extensions = [".png", ".jpg", ".jpeg"];
            foreach (var ext in extensions)
            {
                try
                {
                    var uri = new Uri($"pack://application:,,,/Resources/Images/{name}{ext}", UriKind.Absolute);
                    var info = System.Windows.Application.GetResourceStream(uri);
                    if (info != null)
                    {
                        info.Stream.Dispose();
                        return uri.ToString();
                    }
                }
                catch
                {
                    // Ignore errors
                }
            }

            return $"pack://application:,,,/Resources/Images/Placeholder.png";
        }

        private void FilterOperators()
        {
            var filtered = AllOperators
                .Where(op => op.Role == SelectedRole)
                .ToList();

            FilteredOperators = new ObservableCollection<Operator>(filtered);
            OnPropertyChanged(nameof(FilteredOperators));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RelayCommand(Action<object?> execute) : ICommand
    {
        private readonly Action<object?> _execute = execute;

        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged;
    }
}
