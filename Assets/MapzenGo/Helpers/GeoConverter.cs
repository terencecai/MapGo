using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapzenGo.Helpers.VectorD;
using UnityEngine;


namespace MapzenGo.Helpers
{
    //SOURCE: http://stackoverflow.com/questions/12896139/geographic-coordinates-converter
    public static class GM
    {
        private const int TileSize = 200;
        private const int EarthRadius = 6378137;
        private const double InitialResolution = 2 * Math.PI * EarthRadius / TileSize;
        private const double OriginShift = 2 * Math.PI * EarthRadius / 2;

        private const double THREE_PI = Math.PI * 3;
        private const double TWO_PI = Math.PI * 2;

        public static Vector2d LatLonToMeters(Vector2d v)
        {
            return LatLonToMeters(v.x, v.y);
        }

        //Converts given lat/lon in WGS84 Datum to XY in Spherical Mercator EPSG:900913
        public static Vector2d LatLonToMeters(double lat, double lon)
        {
            var p = new Vector2d();
            p.x = (lon * OriginShift / 180);
            p.y = (Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180));
            p.y = (p.y * OriginShift / 180);
            return new Vector2d(p.x, p.y);
        }

        public static Vector2d MetersToLatLon(Vector2d meters)
        {

            var latitude = (1.5707963267948966 - (2.0 * Math.Atan(Math.Exp((-1.0 * meters.y) / EarthRadius)))) * (180 / Math.PI);
            var longitude = ((meters.x / EarthRadius) * 57.295779513082323) - ((Math.Floor((((meters.x / EarthRadius) * 57.295779513082323) + 180.0) / 360.0)) * 360.0);

            return new Vector2d(latitude, longitude);
        }

        //Converts pixel coordinates in given zoom level of pyramid to EPSG:900913
        public static Vector2d PixelsToMeters(Vector2d p, int zoom)
        {
            var res = Resolution(zoom);
            var met = new Vector2d();
            met.x = (p.x * res - OriginShift);
            met.y = -(p.y * res - OriginShift);
            return met;
        }

        //Converts EPSG:900913 to pyramid pixel coordinates in given zoom level
        public static Vector2d MetersToPixels(Vector2d m, int zoom)
        {
            var res = Resolution(zoom);
            var pix = new Vector2d();
            pix.x = ((m.x + OriginShift) / res);
            pix.y = ((-m.y + OriginShift) / res);
            return pix;
        }

        //Returns a TMS (NOT Google!) tile covering region in given pixel coordinates
        public static Vector2d PixelsToTile(Vector2d p)
        {
            var t = new Vector2d();
            t.x = (int)Math.Ceiling(p.x / (double)TileSize) - 1;
            t.y = (int)Math.Ceiling(p.y / (double)TileSize) - 1;
            return t;
        }

        public static Vector2d PixelsToRaster(Vector2d p, int zoom)
        {
            var mapSize = TileSize << zoom;
            return new Vector2d(p.x, mapSize - p.y);
        }

        //Returns tile for given mercator coordinates
        public static Vector2d MetersToTile(Vector2d m, int zoom)
        {
            var p = MetersToPixels(m, zoom);
            return PixelsToTile(p);
        }

        //Returns bounds of the given tile in EPSG:900913 coordinates
        public static RectD TileBounds(Vector2d t, int zoom)
        {
            var min = PixelsToMeters(new Vector2d(t.x * TileSize, t.y * TileSize), zoom);
            var max = PixelsToMeters(new Vector2d((t.x + 1) * TileSize, (t.y + 1) * TileSize), zoom);
            return new RectD(min, max - min);
        }

        //Returns bounds of the given tile in latutude/longitude using WGS84 datum
        // public static Rect TileLatLonBounds(Vector2d t, int zoom)
        // {
        //    var bound = TileBounds(t, zoom);
        //    var min = MetersToLatLon(new Vector2d(bound.xMin, bound.yMin));
        //    var max = MetersToLatLon(new Vector2d(bound.xMax, bound.yMax));
        //    return new Rect(min.x, min.y, Math.Abs(max.x - min.x), Math.Abs(max.y - min.y));
        // }

        //Resolution (meters/pixel) for given zoom level (measured at Equator)
        public static double Resolution(int zoom)
        {
            return InitialResolution / (Math.Pow(2, zoom));
        }

        public static double ZoomForPixelSize(double pixelSize)
        {
            for (var i = 0; i < 30; i++)
                if (pixelSize > Resolution(i))
                    return i != 0 ? i - 1 : 0;
            throw new InvalidOperationException();
        }

        // Switch to Google Tile representation from TMS
        public static Vector2d ToGoogleTile(Vector2d t, int zoom)
        {
            return new Vector2d(t.x, ((int)Math.Pow(2, zoom) - 1) - t.y);
        }

        // Switch to TMS Tile representation from Google
        public static Vector2d ToTmsTile(Vector2d t, int zoom)
        {
            return new Vector2d(t.x, ((int)Math.Pow(2, zoom) - 1) - t.y);
        }

        static float GD_semiMajorAxis = 6378137.000000f;
        static float GD_TranMercB = 6356752.314245f;
        static float GD_geocentF = 0.003352810664f;

        public static void geodeticOffsetInv(float refLat, float refLon,
            float lat, float lon,
            out float xOffset, out float yOffset)
        {
            float a = GD_semiMajorAxis;
            float b = GD_TranMercB;
            float f = GD_geocentF;

            float L = lon - refLon;
            float U1 = Mathf.Atan((1 - f) * Mathf.Tan(refLat));
            float U2 = Mathf.Atan((1 - f) * Mathf.Tan(lat));
            float sinU1 = Mathf.Sin(U1);
            float cosU1 = Mathf.Cos(U1);
            float sinU2 = Mathf.Sin(U2);
            float cosU2 = Mathf.Cos(U2);

            float lambda = L;
            float lambdaP;
            float sinSigma;
            float sigma;
            float cosSigma;
            float cosSqAlpha;
            float cos2SigmaM;
            float sinLambda;
            float cosLambda;
            float sinAlpha;
            int iterLimit = 100;
            do
            {
                sinLambda = Mathf.Sin(lambda);
                cosLambda = Mathf.Cos(lambda);
                sinSigma = Mathf.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda) +
                    (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) *
                    (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
                if (sinSigma == 0)
                {
                    xOffset = 0.0f;
                    yOffset = 0.0f;
                    return;  // co-incident points
                }
                cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
                sigma = Mathf.Atan2(sinSigma, cosSigma);
                sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
                cosSqAlpha = 1 - sinAlpha * sinAlpha;
                cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;
                if (cos2SigmaM != cos2SigmaM) //isNaN
                {
                    cos2SigmaM = 0;  // equatorial line: cosSqAlpha=0 (§6)
                }
                float C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
                lambdaP = lambda;
                lambda = L + (1 - C) * f * sinAlpha *
                    (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
            } while (Mathf.Abs(lambda - lambdaP) > 1e-12 && --iterLimit > 0);

            if (iterLimit == 0)
            {
                xOffset = 0.0f;
                yOffset = 0.0f;
                return;  // formula failed to converge
            }

            float uSq = cosSqAlpha * (a * a - b * b) / (b * b);
            float A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            float B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            float deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
                B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));
            float s = b * A * (sigma - deltaSigma);

            float bearing = Mathf.Atan2(cosU2 * sinLambda, cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
            xOffset = Mathf.Sin(bearing) * s;
            yOffset = Mathf.Cos(bearing) * s;
        }


        public static double distFrom(double lat1, double lng1, double lat2, double lng2)
        {
            double earthRadius = 6371.0; // miles (or 6371.0 kilometers)
            double dLat = toRadians(lat2 - lat1);
            double dLng = toRadians(lng2 - lng1);
            double sindLat = Math.Sin(dLat / 2);
            double sindLng = Math.Sin(dLng / 2);
            double a = Math.Pow(sindLat, 2) + Math.Pow(sindLng, 2)
                    * Math.Cos(toRadians(lat1)) * Math.Cos(toRadians(lat2));
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double dist = earthRadius * c;

            return Math.Round(dist, 3) * 1000;
        }

        public static Location PointAtDistance(Location inputCoords, double distance)
        {
            Location result = new Location(0, 0);
            var coords = inputCoords.toRadians();
            var sinLat = Math.Sin(coords.GetLatitude());
            var cosLat = Math.Cos(coords.GetLongitude());
            var bearing = UnityEngine.Random.value * 5;
            var theta = distance / EarthRadius;
            var sinBearing = Math.Sin(bearing);
            var cosBearing = Math.Cos(bearing);
            var sinTheta = Math.Sin(theta);
            var cosTheta = Math.Cos(theta);

            result.SetLatitude(Math.Asin(sinLat * cosTheta + cosLat * sinTheta * cosBearing));
            result.SetLongitude(coords.GetLongitude() + Math.Atan2(
              sinBearing * sinTheta * cosLat,
              cosTheta - sinLat * Math.Sin(result.GetLatitude())
            ));
            result.SetLongitude(((result.GetLongitude() + THREE_PI) % TWO_PI) - Math.PI);


            return result.toDegrees();
        }

        public static Location PointInCircle(Location coords, double distance)
        {
            var rnd = UnityEngine.Random.value * 5;
            var randomDist = Math.Sqrt(rnd) * distance;
            Location result = null;
            var d = new System.Random().Next(1, 10);
            for (int i = 0; i < d; i++) { result = PointAtDistance(coords, randomDist); }
            return result;
        }

        public static double toRadians(double degree)
        {
            return (Math.PI / 180) * degree;
        }

        public static double toDegrees(double radian)
        {
            return radian / (Math.PI / 180);
        }

    }


}
