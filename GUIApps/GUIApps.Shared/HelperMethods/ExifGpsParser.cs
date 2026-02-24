using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace GUIApps.Shared.HelperMethods;

internal static class ExifGpsParser
{
    public static (double Lat, double Lon)? TryGetGps(byte[] data)
    {
        try
        {
            using var stream = new MemoryStream(data);
            var directories = ImageMetadataReader.ReadMetadata(stream);
            var gps = directories.OfType<GpsDirectory>().FirstOrDefault() ?? null;
            GeoLocation location = new GeoLocation();
            bool? locationFound = gps?.TryGetGeoLocation(out location);

            if ((bool)locationFound!)
            {
                return (location.Latitude, location.Longitude);
            }
        }
        catch
        {
            //Ignored
        }

        return null;
    }
}
