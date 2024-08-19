using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows;
using CoordinateSharp;
using Prism.Commands;
using Prism.Mvvm;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.Converters;
using SharpKml.Base;
using SharpKml.Dom;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems;
using System.Security.Policy;
using Point = SharpKml.Dom.Point;
using System.Diagnostics;

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
        private double highestPeak;
        private double heightBuffer = 300;
        private double homeAltitude;
        private double aglResult;
        private double losResult;

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

        public double HighestPeak
        {
            get => highestPeak;
            set
            {
                SetProperty(ref highestPeak, value);
                CalculateLOS();
                CalculateAGL();
            }
        }

        public bool IsFirstItemHomePoint { get => _isFirstItemHomePoint; set => SetProperty(ref _isFirstItemHomePoint, value); }
        public string HomePoint { get => _homePoint; set => SetProperty(ref _homePoint, value); }
        public string Targets { get => _targets; set => SetProperty(ref _targets, value); }

        public double HeightBuffer
        {
            get => heightBuffer; set
            {
                SetProperty(ref heightBuffer, value);
                CalculateAGL();
                CalculateLOS();
            }
        }

        public double HomeAltitude
        {
            get => homeAltitude; set
            {
                SetProperty(ref homeAltitude, value);
                CalculateAGL();
                CalculateLOS();
            }
        }

        public double AGLResult { get => aglResult; set => SetProperty(ref aglResult, value); }
        public double LOSResult { get => losResult; set => SetProperty(ref losResult, value); }

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

        private DelegateCommand _browseTargetsCommand;

        public DelegateCommand BrowseTargetsCommand
        {
            get { return _browseTargetsCommand ??= new DelegateCommand(OnBrowseTargets); }
        }

        private void OnBrowseTargets()
        {
            var dlg = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            var result = dlg.ShowDialog();
            if (result == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                Targets = dlg.FileName;
            }
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

        private void CalculateAGL()
        {
            AGLResult = HeightBuffer + HighestPeak - HomeAltitude;
        }

        private void CalculateLOS()
        {
            LOSResult = HeightBuffer + HighestPeak;
        }

        private DelegateCommand _generateKmlCommand;
        private string _targets;
        private string _homePoint;
        private bool _isFirstItemHomePoint;

        public DelegateCommand GenerateKmlCommand
        {
            get { return _generateKmlCommand ??= new DelegateCommand(OnGenerateKml); }
        }

        private void OnGenerateKml()
        {
            var dlg = new Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog
            {
                DefaultExtension = ".kml",
            };

            var result = dlg.ShowDialog();
            if (result == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                var mgrs = File.ReadAllText(Targets).Split($"\r\n\t;, ".ToCharArray()).Where(s => !string.IsNullOrWhiteSpace(s.Trim()));

                // CreateCirclesKml(mgrs.ToList(), 0.250, dlg.FileName);

                if (IsFirstItemHomePoint)
                {
                    CreateCirclesKml(mgrs.Skip(1).ToList(), 0.250, dlg.FileName, mgrs.First());
                }
                else
                {
                    CreateCirclesKml(mgrs.ToList(), 0.250, dlg.FileName);
                }
            }
        }

        private Models.Coordinate MgrsToLatLon(string mgrsText)
        {
            var success = Coordinate.TryParse(mgrsText, out var coord);
            if (success)
            {
                coord.FormatOptions.Format = CoordinateFormatType.Decimal;
                coord.FormatOptions.Round = 10;
                var latLong = coord.ToString().Split(" ".ToCharArray());
                return new Models.Coordinate(double.Parse(latLong[0]), double.Parse(latLong[1]));
            }

            return null;
        }



        // KML Generator
        private void CreateCirclesKml(List<string> mgrsCoords, double radiusKm, string kmlFilename, string homePoint = "")
        {
            var kml = new Kml();
            var document = new Document();
            var style = new SharpKml.Dom.Style
            {
                Id = "circleStyle",

                Line = new LineStyle
                {
                    Width = 2.0,
                    Color = new Color32(255, 0, 0, 255)
                },

                Polygon = new PolygonStyle
                {
                    Fill = false
                }
            };

            var homeStyle = new SharpKml.Dom.Style
            {
                Id = "homeStyle",

                Line = new LineStyle
                {
                    Width = 2.5,
                    Color = new Color32(255, 255, 0, 0)
                },

                Polygon = new PolygonStyle
                {
                    Fill = false
                }
            };

            document.AddStyle(style);
            document.AddStyle(homeStyle);


            for (int i = 0; i < mgrsCoords.Count; i++)
            {
                string mgrs = mgrsCoords[i];
                var latLon = MgrsToLatLon(mgrs);

                var circlePlacemark = new Placemark
                {
                    StyleUrl = new Uri("#circleStyle", UriKind.Relative),
                    Name = $"T{i + 1} {mgrs}",
                    Geometry = CreateCircle(latLon, radiusKm),
                };

                var pinPlacemark = new Placemark
                {
                    Name = $"T{i + 1}",
                    Geometry = new Point
                    {
                        Coordinate = new SharpKml.Base.Vector
                        {
                            Latitude = latLon.Latitude,
                            Longitude = latLon.Longitude
                        }
                    },
                };

                document.AddFeature(circlePlacemark);
                document.AddFeature(pinPlacemark);
            }


            if (!string.IsNullOrWhiteSpace(homePoint))
            {
                var latLongHP = MgrsToLatLon(homePoint);
                var circleLimitPlacemark = new Placemark
                {
                    StyleUrl = new Uri("#homeStyle", UriKind.Relative),
                    Name = Guid.NewGuid().ToString().Split('-')[0],
                    Geometry = CreateCircle(latLongHP, 10),
                };

                var homeCirclePlacemark = new Placemark
                {
                    StyleUrl = new Uri("#homeStyle", UriKind.Relative),
                    Name = "HP",
                    Geometry = CreateCircle(latLongHP, 0.300)
                };

                var homePinPlacemark = new Placemark
                {
                    Name = "Home",
                    Geometry = new Point
                    {
                        Coordinate = new SharpKml.Base.Vector
                        {
                            Latitude = latLongHP.Latitude,
                            Longitude = latLongHP.Longitude
                        }
                    },
                };

                document.AddFeature(circleLimitPlacemark);
                document.AddFeature(homeCirclePlacemark);
                document.AddFeature(homePinPlacemark);
            }


            kml.Feature = document;

            using (var stream = File.OpenWrite(kmlFilename))
            {
                var serializer = new Serializer();
                serializer.Serialize(kml, stream);
            }

            Console.WriteLine($"KML file '{kmlFilename}' created successfully.");
            Process.Start(kmlFilename);
        }



        private SharpKml.Dom.Polygon CreateCircle(Models.Coordinate center, double radiusKm, int numPoints = 36)
        {
            var coords = new List<SharpKml.Base.Vector>();
            var earthRadiusKm = 6371.0; // Radius of Earth in kilometers

            for (int i = 0; i < numPoints; i++)
            {
                var angle = 2 * Math.PI * i / numPoints;
                var dx = radiusKm * Math.Cos(angle);
                var dy = radiusKm * Math.Sin(angle);

                var lat = center.Latitude + (dy / earthRadiusKm) * (180 / Math.PI);
                var lon = center.Longitude + (dx / earthRadiusKm) * (180 / Math.PI) / Math.Cos(center.Latitude * Math.PI / 180);

                coords.Add(new SharpKml.Base.Vector(lat, lon));
            }

            // Close the circle by adding the first point again
            coords.Add(coords[0]);

            var linearRing = new LinearRing();
            linearRing.Coordinates = new CoordinateCollection(coords);

            var polygon = new SharpKml.Dom.Polygon();
            polygon.OuterBoundary = new OuterBoundary { LinearRing = linearRing };

            return polygon;
        }
    }
}
