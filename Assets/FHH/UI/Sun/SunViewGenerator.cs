using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Sun
{
    public sealed class SunViewGenerator : ViewGeneratorBase<SunPresenter, SunView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            var root = new VisualElement
            {
                name = "SunWindow"
            };
            root.AddToClassList("sun-window");
            root.AddToClassList("col");
            root.AddToClassList("i18n-skip");

            // Title
            var titleLabel = new Label("Sonnenstandsimulation")
            {
                name = "SunWindowTitleLabel"
            };
            titleLabel.AddToClassList("sun-title");
            root.Add(titleLabel);

            // Year input section
            var yearSection = new VisualElement { name = "SunYearSection" };
            yearSection.AddToClassList("sun-section");
            yearSection.AddToClassList("col");
            yearSection.AddToClassList("i18n-skip");

            var yearLabel = new Label("Jahr:")
            {
                name = "SunYearLabel"
            };
            yearLabel.AddToClassList("sun-label");

            var yearRow = new VisualElement { name = "SunYearRow" };
            yearRow.AddToClassList("row");
            yearRow.AddToClassList("i18n-skip");

            var yearField = new IntegerField
            {
                name = "SunYearField"
            };
            yearField.AddToClassList("sun-year-field");
            yearField.AddToClassList("i18n-skip");
            yearField.autoCorrection = true;
            yearField.keyboardType = TouchScreenKeyboardType.NumberPad;
            yearField.isDelayed = true;

            yearRow.Add(yearField);
            yearSection.Add(yearLabel);
            yearSection.Add(yearRow);
            root.Add(yearSection);

            // Date slider section
            var dateSection = new VisualElement { name = "SunDateSection" };
            dateSection.AddToClassList("sun-section");
            dateSection.AddToClassList("col");
            dateSection.AddToClassList("i18n-skip");

            var dateHeaderRow = new VisualElement { name = "SunDateHeaderRow" };
            dateHeaderRow.AddToClassList("row");
            dateHeaderRow.AddToClassList("sun-row");
            dateHeaderRow.AddToClassList("i18n-skip");

            var dateLabel = new Label("Datum:")
            {
                name = "SunDateLabel"
            };
            dateLabel.AddToClassList("sun-label");

            var dateValueLabel = new Label
            {
                name = "SunDateValueLabel"
            };
            dateValueLabel.AddToClassList("sun-value-label");
            dateValueLabel.AddToClassList("i18n-skip");

            dateHeaderRow.Add(dateLabel);
            dateHeaderRow.Add(dateValueLabel);

            var dateSlider = new SliderInt
            {
                name = "SunDateSlider"
            };
            dateSlider.lowValue = 1;
            dateSlider.highValue = 365;
            dateSlider.AddToClassList("sun-slider");
            dateSlider.AddToClassList("i18n-skip");

            var dateTooltipLabel = new Label
            {
                name = "SunDateTooltipLabel"
            };
            dateTooltipLabel.AddToClassList("sun-tooltip-label");
            dateTooltipLabel.AddToClassList("i18n-skip");

            dateSection.Add(dateHeaderRow);
            dateSection.Add(dateSlider);
            dateSection.Add(dateTooltipLabel);
            root.Add(dateSection);

            // Time slider section
            var timeSection = new VisualElement { name = "SunTimeSection" };
            timeSection.AddToClassList("sun-section");
            timeSection.AddToClassList("col");
            timeSection.AddToClassList("i18n-skip");

            var timeHeaderRow = new VisualElement { name = "SunTimeHeaderRow" };
            timeHeaderRow.AddToClassList("row");
            timeHeaderRow.AddToClassList("sun-row");
            timeHeaderRow.AddToClassList("i18n-skip");

            var timeLabel = new Label("Uhrzeit:")
            {
                name = "SunTimeLabel"
            };
            timeLabel.AddToClassList("sun-label");

            var timeValueLabel = new Label
            {
                name = "SunTimeValueLabel"
            };
            timeValueLabel.AddToClassList("sun-value-label");
            timeValueLabel.AddToClassList("i18n-skip");

            timeHeaderRow.Add(timeLabel);
            timeHeaderRow.Add(timeValueLabel);

            var timeSlider = new SliderInt
            {
                name = "SunTimeSlider"
            };
            timeSlider.lowValue = 0 * 60;
            timeSlider.highValue = 24 * 60;
            timeSlider.AddToClassList("sun-slider");
            timeSlider.AddToClassList("i18n-skip");

            var timeTooltipLabel = new Label
            {
                name = "SunTimeTooltipLabel"
            };
            timeTooltipLabel.AddToClassList("sun-tooltip-label");
            timeTooltipLabel.AddToClassList("i18n-skip");

            timeSection.Add(timeHeaderRow);
            timeSection.Add(timeSlider);
            timeSection.Add(timeTooltipLabel);
            root.Add(timeSection);

            // Studies (radio buttons) section
            var studiesSection = new VisualElement { name = "SunStudiesSection" };
            studiesSection.AddToClassList("sun-section");
            studiesSection.AddToClassList("col");
            studiesSection.AddToClassList("i18n-skip");

            var studiesLabel = new Label("Sonnenstudien:")
            {
                name = "SunStudiesLabel"
            };
            studiesLabel.AddToClassList("sun-label");

            var studiesGroup = new RadioButtonGroup
            {
                name = "SunStudiesGroup"
            };
            studiesGroup.choices = new List<string> { "17.01.", "21.03." };
            studiesGroup.AddToClassList("sun-studies-group");
            studiesGroup.AddToClassList("i18n-skip");

            studiesSection.Add(studiesLabel);
            studiesSection.Add(studiesGroup);
            root.Add(studiesSection);

            // Expose controls on the view
            View.YearField = yearField;
            View.DateSlider = dateSlider;
            View.TimeSlider = timeSlider;
            View.DateValueLabel = dateValueLabel;
            View.DateTooltipLabel = dateTooltipLabel;
            View.TimeValueLabel = timeValueLabel;
            View.TimeTooltipLabel = timeTooltipLabel;
            View.StudiesGroup = studiesGroup;

            await UniTask.CompletedTask;
            return root;
        }
    }
}