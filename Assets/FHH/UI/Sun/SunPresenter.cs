using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Components.Networking;
using FHH.Logic.Components.Sunlight;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System;
using FHH.Logic.Components.Collaboration;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Sun
{
    public sealed class SunPresenter : PresenterBase<SunPresenter, SunView, SunModel>
    {
        private const int MinHourMinutes = 0 * 60;
        private const int MaxHourMinutes = 24 * 60;
        private const int MinuteStep = 1;
        private CollaborationService _collab;
        private SunCalculator _sunCalculator;

        public SunPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            SunModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model ?? new SunModel(), perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();

            _sunCalculator = GameObject.FindGameObjectWithTag("Sun").GetComponent<SunCalculator>();
            if (_sunCalculator == null)
            {
                Debug.LogWarning($"{nameof(SunPresenter)}: No {nameof(SunCalculator)} found.");
            }
            if (_sunCalculator != null)
            {
                var current = _sunCalculator.GetDateTime();
                Model.SetFromDateTime(current);
                ClampModelToUiRange();
            }
            _collab = ServiceLocator.GetService<CollaborationService>();
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await base.InitializeAfterUiAsync();

            if (View == null) return;

            HookUpEvents();
            SyncViewFromModel();
            ApplyToSun();
        }

        private void HookUpEvents()
        {
            if (View.YearField != null)
            {
                View.YearField.RegisterValueChangedCallback(evt =>
                {
                    OnYearChanged(evt.newValue);
                });
            }

            if (View.DateSlider != null)
            {
                View.DateSlider.RegisterValueChangedCallback(evt =>
                {
                    OnDateSliderChanged(evt.newValue);
                });
            }

            if (View.TimeSlider != null)
            {
                View.TimeSlider.RegisterValueChangedCallback(evt =>
                {
                    OnTimeSliderChanged(evt.newValue);
                });
            }

            if (View.StudiesGroup != null)
            {
                View.StudiesGroup.RegisterValueChangedCallback(evt =>
                {
                    OnStudiesChanged(evt.newValue);
                });
            }
        }

        private void SyncViewFromModel()
        {
            var dateTime = Model.GetDateTime();

            // Year
            if (View.YearField != null)
            {
                View.YearField.SetValueWithoutNotify(dateTime.Year);
            }

            // Date slider
            if (View.DateSlider != null)
            {
                var daysInYear = DateTime.IsLeapYear(dateTime.Year) ? 366 : 365;
                View.DateSlider.lowValue = 1;
                View.DateSlider.highValue = daysInYear;
                View.DateSlider.SetValueWithoutNotify(dateTime.DayOfYear);
            }

            // Time slider (stored as minutes)
            if (View.TimeSlider != null)
            {
                var minutes = dateTime.Hour * 60 + dateTime.Minute;
                minutes = SnapMinutes(Math.Clamp(minutes, MinHourMinutes, MaxHourMinutes));

                View.TimeSlider.lowValue = MinHourMinutes;
                View.TimeSlider.highValue = MaxHourMinutes;
                View.TimeSlider.SetValueWithoutNotify(minutes);

                Model.TimeMinutes = minutes;
            }

            UpdateDateLabels();
            UpdateTimeLabels();
            SyncStudiesSelection();
        }

        private void OnYearChanged(int newYear)
        {
            var oldDateTime = Model.GetDateTime();
            var clampedYear = Math.Clamp(newYear, 1900, 2100);

            DateTime newDate;
            try
            {
                newDate = new DateTime(clampedYear, oldDateTime.Month, oldDateTime.Day);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Fallback for 29.02 on non-leap years
                newDate = new DateTime(clampedYear, oldDateTime.Month, 1).AddDays(-1);
            }

            Model.Year = clampedYear;
            Model.DayOfYear = newDate.DayOfYear;

            ClampModelToUiRange();

            if (View.YearField != null && View.YearField.value != clampedYear)
            {
                View.YearField.SetValueWithoutNotify(clampedYear);
            }

            if (View.DateSlider != null)
            {
                var daysInYear = DateTime.IsLeapYear(Model.Year) ? 366 : 365;
                View.DateSlider.lowValue = 1;
                View.DateSlider.highValue = daysInYear;
                View.DateSlider.SetValueWithoutNotify(Model.DayOfYear);
            }

            UpdateDateLabels();
            SyncStudiesSelection();
            ApplyToSun();
        }

        private void OnDateSliderChanged(int newDayOfYear)
        {
            var daysInYear = DateTime.IsLeapYear(Model.Year) ? 366 : 365;
            var clamped = Math.Clamp(newDayOfYear, 1, daysInYear);
            Model.DayOfYear = clamped;

            if (View.DateSlider != null && View.DateSlider.value != clamped)
            {
                View.DateSlider.SetValueWithoutNotify(clamped);
            }

            UpdateDateLabels();
            SyncStudiesSelection();
            ApplyToSun();
        }

        private void OnTimeSliderChanged(int newMinutes)
        {
            var snapped = SnapMinutes(Math.Clamp(newMinutes, MinHourMinutes, MaxHourMinutes));
            Model.TimeMinutes = snapped;

            if (View.TimeSlider != null && View.TimeSlider.value != snapped)
            {
                View.TimeSlider.SetValueWithoutNotify(snapped);
            }

            UpdateTimeLabels();
            ApplyToSun();
        }

        private void OnStudiesChanged(int index)
        {
            if (index < 0) return;

            DateTime date;
            switch (index)
            {
                case 0:
                    date = new DateTime(Model.Year, 1, 17);
                    break;
                case 1:
                    date = new DateTime(Model.Year, 3, 21);
                    break;
                default:
                    return;
            }

            Model.DayOfYear = date.DayOfYear;

            if (View.DateSlider != null)
            {
                View.DateSlider.SetValueWithoutNotify(Model.DayOfYear);
            }

            UpdateDateLabels();
            ApplyToSun();
        }

        private void UpdateDateLabels()
        {
            var dateTime = Model.GetDateTime();
            var dateString = dateTime.ToString("dd.MM.yyyy");

            if (View.DateValueLabel != null)
            {
                View.DateValueLabel.text = dateString;
            }

            if (View.DateTooltipLabel != null)
            {
                View.DateTooltipLabel.text = dateString;
            }
        }

        private void UpdateTimeLabels()
        {
            var minutes = Model.TimeMinutes;
            var hours = minutes / 60;
            var mins = minutes % 60;
            var timeString = $"{hours:00}:{mins:00}";

            if (View.TimeValueLabel != null)
            {
                View.TimeValueLabel.text = timeString;
            }

            if (View.TimeTooltipLabel != null)
            {
                View.TimeTooltipLabel.text = timeString;
            }
        }

        private void SyncStudiesSelection()
        {
            if (View.StudiesGroup == null) return;

            var dateTime = Model.GetDateTime();
            var index = -1;

            if (dateTime.Month == 1 && dateTime.Day == 17)
            {
                index = 0;
            }
            else if (dateTime.Month == 3 && dateTime.Day == 21)
            {
                index = 1;
            }

            View.StudiesGroup.SetValueWithoutNotify(index);
        }

        private void ApplyToSun()
        {
            if (_sunCalculator == null) return;

            var dateTime = Model.GetDateTime();
            _sunCalculator.SetTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute);
            if (!_collab.IsGuidedModeEnabled) return;
            if (!ServiceLocator.GetService<PermissionService>().IsModerator()) return;
            SendCommandAsync("Sun", year:dateTime.Year, month:dateTime.Month, 
                day:dateTime.Day, hour:dateTime.Hour, minute:dateTime.Minute).Forget();
        }

        private static int SnapMinutes(int minutes)
        {
            var snapped = ((minutes + MinuteStep / 2) / MinuteStep) * MinuteStep;
            return snapped;
        }

        private void ClampModelToUiRange()
        {
            var dt = Model.GetDateTime();
            var minutes = dt.Hour * 60 + dt.Minute;
            minutes = Math.Clamp(minutes, MinHourMinutes, MaxHourMinutes);
            minutes = SnapMinutes(minutes);

            Model.TimeMinutes = minutes;
        }

        private async UniTask SendCommandAsync(string type,
            bool value = false,
            double x = 0, double y = 0, double z = 0,
            string projectId = null,
            string variant = null,
            bool enabled = false,
            Quaternion rotation = default,
            int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0
        )
        {
            var json = JsonUtility.ToJson(new CommandPayload
            {
                Type = type,
                Value = value,
                X = x,
                Y = y,
                Z = z,
                ProjectId = projectId,
                Variant = variant,
                Enabled = enabled,
                Rotation = rotation,
                Year = year,
                Month = month,
                Day = day,
                Hour = hour,
                Minute = minute
            });

            var vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();
            vivoxHandler.SendCommandMessageAsync(json, _collab.CommandChannelName).Forget();
            await UniTask.CompletedTask;
        }
    }
}