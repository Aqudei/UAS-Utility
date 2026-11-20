using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAS_Utility.ViewModels
{
    public class FormulaViewModel : BindableBase
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        private string _losFormula;
        private string _aglFormula;

        private string _rtlAltitudeFormula;

        public string RTLAltitudeFormula
        {
            get { return _rtlAltitudeFormula; }
            set { SetProperty(ref _rtlAltitudeFormula , value); }
        }

        public string LOSFormula
        {
            get => _losFormula;
            set => SetProperty(ref _losFormula, value);
        }

        public string AGLFormula
        {
            get => _aglFormula;
            set => SetProperty(ref _aglFormula, value);
        }

        public DelegateCommand SaveCommand { get; }

        public FormulaViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator
                ?? throw new ArgumentNullException(nameof(dialogCoordinator));

            LoadSettings();

            SaveCommand = new DelegateCommand(
                async () => await SaveAsync(),
                CanSave
            )
            .ObservesProperty(() => LOSFormula)
            .ObservesProperty(() => AGLFormula)
            .ObservesProperty(() => RTLAltitudeFormula);
        }

        private void LoadSettings()
        {
            LOSFormula = Properties.Settings.Default.LOSFormula;
            AGLFormula = Properties.Settings.Default.AGLFormula;
            RTLAltitudeFormula = Properties.Settings.Default.RTLAltitudeFormula;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(LOSFormula)
                && !string.IsNullOrWhiteSpace(AGLFormula)
                && !string.IsNullOrWhiteSpace(RTLAltitudeFormula);
        }

        private async Task SaveAsync()
        {
            try
            {
                Properties.Settings.Default.LOSFormula = LOSFormula.Trim();
                Properties.Settings.Default.AGLFormula = AGLFormula.Trim();
                Properties.Settings.Default.RTLAltitudeFormula = RTLAltitudeFormula.Trim();

                Properties.Settings.Default.Save();

                await _dialogCoordinator.ShowMessageAsync(
                    this,
                    "Settings Saved",
                    "Your calculation formulas have been successfully saved."
                );
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(
                    this,
                    "Save Failed",
                    $"Unable to save your formula settings.\n\nError: {ex.Message}",
                    MessageDialogStyle.Affirmative
                );
            }
        }
    }
}
