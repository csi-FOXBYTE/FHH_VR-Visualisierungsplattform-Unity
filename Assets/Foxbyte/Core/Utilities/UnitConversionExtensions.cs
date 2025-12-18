namespace Foxbyte.Core
{
    public static class UnitConversionExtensions
    {
         // Conversion constants
        private const double MetersPerInch = 0.0254;
        private const double MetersPerFoot = 0.3048;
        private const double PoundsPerKilogram = 2.20462;
        private const double LitersPerGallon = 3.78541;

        // Length conversions
        public static float MillimetersToInches(this float millimeters)
        {
            return (float)(millimeters * 0.001 / MetersPerInch);
        }

        public static float InchesToMillimeters(this float inches)
        {
            return (float)(inches * MetersPerInch * 1000);
        }

        public static float MillimetersToFeet(this float millimeters)
        {
            return (float)(millimeters * 0.001 / MetersPerFoot);
        }

        public static float FeetToMillimeters(this float feet)
        {
            return (float)(feet * MetersPerFoot * 1000);
        }

        public static float CentimetersToInches(this float centimeters)
        {
            return (float)(centimeters * 0.01 / MetersPerInch);
        }

        public static float InchesToCentimeters(this float inches)
        {
            return (float)(inches * MetersPerInch * 100);
        }

        public static float CentimetersToFeet(this float centimeters)
        {
            return (float)(centimeters * 0.01 / MetersPerFoot);
        }

        public static float FeetToCentimeters(this float feet)
        {
            return (float)(feet * MetersPerFoot * 100);
        }

        public static float MetersToFeet(this float meters)
        {
            return (float)(meters / MetersPerFoot);
        }

        public static float FeetToMeters(this float feet)
        {
            return (float)(feet * MetersPerFoot);
        }

        public static float MetersToInches(this float meters)
        {
            return (float)(meters / MetersPerInch);
        }

        public static float InchesToMeters(this float inches)
        {
            return (float)(inches * MetersPerInch);
        }

        // Weight conversions
        public static float KilogramsToPounds(this float kilograms)
        {
            return (float)(kilograms * PoundsPerKilogram);
        }

        public static float PoundsToKilograms(this float pounds)
        {
            return (float)(pounds / PoundsPerKilogram);
        }
        
        // Temperature conversions
        public static float CelsiusToFahrenheit(this float celsius)
        {
            return (float)(celsius * (9.0 / 5.0) + 32.0);
        }

        public static float FahrenheitToCelsius(this float fahrenheit)
        {
            return (float)((fahrenheit - 32.0) * (5.0 / 9.0));
        }
        
        // Volume conversions
        public static float LitersToGallons(this float liters)
        {
            return (float)(liters / LitersPerGallon);
        }

        public static float GallonsToLiters(this float gallons)
        {
            return (float)(gallons * LitersPerGallon);
        }
    }
}