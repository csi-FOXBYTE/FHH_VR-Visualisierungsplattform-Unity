using System;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;

namespace FHH.UI.Sun
{
    public sealed class SunModel : PresenterModelBase
    {
        public int Year { get; set; }
        public int DayOfYear { get; set; }
        public int TimeMinutes { get; set; } // minutes since midnight

        public SunModel()
        {
            var now = DateTime.Now;
            Year = now.Year;
            DayOfYear = now.DayOfYear;
            TimeMinutes = now.Hour * 60 + now.Minute;
        }

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
        }

        public DateTime GetDateTime()
        {
            var year = Math.Clamp(Year, 1, 9999);
            var daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            var dayOfYear = Math.Clamp(DayOfYear, 1, daysInYear);
            var minutes = Math.Clamp(TimeMinutes, 0, 24 * 60 - 1);

            var date = new DateTime(year, 1, 1).AddDays(dayOfYear - 1);
            return date.Date + TimeSpan.FromMinutes(minutes);
        }

        public void SetFromDateTime(DateTime dateTime)
        {
            Year = dateTime.Year;
            DayOfYear = dateTime.DayOfYear;
            TimeMinutes = dateTime.Hour * 60 + dateTime.Minute;
        }
    }
}