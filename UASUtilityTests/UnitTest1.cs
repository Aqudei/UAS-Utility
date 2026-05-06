using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpKml.Dom;
using System;
using System.Diagnostics;
using System.Linq;

namespace UASUtilityTests
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }


        void ModifyAltitude(SharpKml.Dom.Feature feature)
        {
            if (feature == null) return;

            // Print current feature name
            Debug.WriteLine($"{feature.GetType().Name} - {feature.Name}");

            if (feature is SharpKml.Dom.Placemark placemark)
            {
                if (placemark.Geometry != null)
                {
                    if (placemark.Geometry is LineString line)
                    {
                        line.AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute;
                        var coords = line.Coordinates.ToList();

                        coords[0].Altitude = 100;
                        coords[1].Altitude = 200;

                        line.Coordinates = new SharpKml.Dom.CoordinateCollection(coords);

                    }
                }
            }

            // If it's a container (Document or Folder), recurse into children
            if (feature is SharpKml.Dom.Container container)
            {
                foreach (var child in container.Features)
                {
                    ModifyAltitude(child);
                }
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var kmlPath = @"D:\Downloads\Sample.kml";

            using (var steam = System.IO.File.OpenRead(kmlPath))
            {
                var file = SharpKml.Engine.KmlFile.Load(steam);
                if (file.Root is SharpKml.Dom.Kml kml && kml.Feature is SharpKml.Dom.Document doc)
                {
                    ModifyAltitude(doc);
                }

            }

        }
    }
}
