using System;
using System.ComponentModel;
using System.Windows;
using CoordinateSharp;
using Prism.Commands;
using Prism.Mvvm;

namespace UAS_Utility.ViewModels
{
    public class CoordinatesViewModel : BindableBase
    {
        private DelegateCommand _copyDegreesCommand;
        private DelegateCommand _copyMgrsCommand;

        private string _degreesResult = "0;0";

        private string _latitude;

        private string _longitude;

        private string _mgrs;


        private string _mgrsResult;
        private string _prefix;

        public string Prefix
        {
            get => _prefix;
            set => SetProperty(ref _prefix, value);
        }

        public CoordinatesViewModel()
        {
            PropertyChanged += CoordinatesViewModel_PropertyChanged;
        }

        public string Mgrs
        {
            get => _mgrs;
            set => SetProperty(ref _mgrs, value);
        }

        public string Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public string Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public string MgrsResult
        {
            get => _mgrsResult;
            set => SetProperty(ref _mgrsResult, value);
        }

        public string DegreesResult
        {
            get => _degreesResult;
            set => SetProperty(ref _degreesResult, value);
        }

        public DelegateCommand CopyDegreesCommand => _copyDegreesCommand ??=
            new DelegateCommand(CopyDegrees, () => CanCopyDegrees).ObservesProperty(() => DegreesResult);

        public DelegateCommand CopyMgrsCommand => _copyMgrsCommand ??=
            new DelegateCommand(CopyMgrs, () => CanCopyMgrs).ObservesProperty(() => MgrsResult);

        public bool CanCopyMgrs => !string.IsNullOrWhiteSpace(MgrsResult);

        public bool CanCopyDegrees => !string.IsNullOrWhiteSpace(DegreesResult);

        private void CoordinatesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (nameof(Longitude) == e.PropertyName || nameof(Latitude) == e.PropertyName) ComputeMgrs();

            if (nameof(Mgrs) == e.PropertyName) ComputeDegrees();
        }

        private void ComputeDegrees()
        {
            var mgrsText = Mgrs;

            if (!string.IsNullOrWhiteSpace(Prefix))
            {
                mgrsText = Prefix + Mgrs;
            }

            var success = Coordinate.TryParse(mgrsText, out var coord);
            if (success)
            {
                coord.FormatOptions.Format = CoordinateFormatType.Decimal;
                coord.FormatOptions.Round = 10;
                DegreesResult = coord.ToString().Replace(" ", ";");
            }
            else
            {
                DegreesResult = "";
            }
        }

        private void ComputeMgrs()
        {
            try
            {
                var coord = new Coordinate(double.Parse(Latitude), double.Parse(Longitude));
                coord.FormatOptions.Round = 10;
                MgrsResult = coord.MGRS.ToString();
            }
            catch (Exception e)
            {
                MgrsResult = "";
            }
        }

        private void CopyMgrs()
        {
            Clipboard.SetText(MgrsResult);
        }

        private void CopyDegrees()
        {
            Clipboard.SetText(DegreesResult);
        }
    }
}