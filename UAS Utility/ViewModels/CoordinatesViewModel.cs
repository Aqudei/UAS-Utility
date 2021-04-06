using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CoordinateSharp;

namespace UAS_Utility.ViewModels
{
    public class CoordinatesViewModel : BindableBase
    {
        private DelegateCommand _copyMgrsCommand;
        private DelegateCommand _copyDegreesCommand;

        private string _mgrs;

        public string Mgrs
        {
            get => _mgrs;
            set => SetProperty(ref _mgrs, value);
        }

        private double _latitude;

        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        private double _longitude;

        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }


        private string _mgrsResult;

        public string MgrsResult
        {
            get => _mgrsResult;
            set => SetProperty(ref _mgrsResult, value);
        }

        private string _degreesResult = "0,0";

        public string DegreesResult
        {
            get => _degreesResult;
            set => SetProperty(ref _degreesResult, value);
        }


        public CoordinatesViewModel()
        {
            PropertyChanged += CoordinatesViewModel_PropertyChanged;
        }

        private void CoordinatesViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (nameof(Longitude) == e.PropertyName || nameof(Latitude) == e.PropertyName)
            {
                ComputeMgrs();
            }

            if (nameof(Mgrs) == e.PropertyName)
            {
                ComputeDegrees();
            }
        }

        private void ComputeDegrees()
        {
            var success = Coordinate.TryParse(Mgrs, out var coord);
            if (success)
            {
                coord.FormatOptions.Format = CoordinateFormatType.Decimal;
                coord.FormatOptions.Round = 10;
                DegreesResult = coord.ToString().Replace(" ", ",");
            }
        }

        private void ComputeMgrs()
        {
            var coord = new Coordinate(Latitude, Longitude);
            coord.FormatOptions.Round = 10;
            MgrsResult = coord.MGRS.ToString();
        }

        public DelegateCommand CopyDegreesCommand => _copyDegreesCommand ??= new DelegateCommand(CopyDegrees);

        public DelegateCommand CopyMgrsCommand => _copyMgrsCommand ??= new DelegateCommand(CopyMgrs);

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