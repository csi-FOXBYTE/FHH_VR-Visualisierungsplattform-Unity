using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FHH.Logic.Components.Sunlight
{
    public class SunCalculator : MonoBehaviour
    {
        // Animation Parameters
        [SerializeField]
        [Tooltip("Duration of day-night cycle in hours")]
        [Range(1f, 24f)]
        private float timeRangeHours = 24f;

        [SerializeField]
        [Tooltip("Starting hour of the day (0-23)")]
        [Range(0f, 23f)]
        private float startTimeHour = 6f;

        [SerializeField]
        [Tooltip("Duration of animation in seconds")]
        [Range(1f, 300f)]
        private float animationTimeSeconds = 60f;

        // Time Parameters
        [SerializeField]
        [Range(0, 23)]
        private int m_Hour = 0;
        
        [SerializeField]
        [Range(0, 59)]
        private int m_Minute = 0;
        
        [SerializeField]
        [Range(0, 59)]
        private int m_Second = 0;

        // Location and Date Parameters
        [SerializeField]
        private float m_Latitude = 0f;
        
        [SerializeField]
        private float m_Longitude = 0f;
        
        [SerializeField]
        private int m_Year = 2025;
        
        [SerializeField]
        [Range(1, 12)]
        private int m_Month = 3;
        
        [SerializeField]
        [Range(1, 31)]
        private int m_Day = 21;

        // Private fields
        private bool isAnimating = false;
        private float animationProgress = 0f;
        private float startTime;

        private void OnValidate()
        {
            // Update position immediately when inspector values change in editor
            UpdateStaticPosition();
        }

        private void OnEnable()
        {
            // Update position when component is enabled
            UpdateStaticPosition();
        }

        private void Update()
        {
            // Only handle animation at runtime
            HandleAnimation();
        }

        public void SetTime(int year, int month, int day, int hour, int minute)
        {
            m_Year = year;
            m_Month = month;
            m_Day = day;
            m_Hour = hour;
            m_Minute = minute;
            UpdateStaticPosition();
        }

        public void StartSunSimulation()
        {
            isAnimating = true;
            animationProgress = 0f;
            startTime = Time.time;
        }

        public void StopSunSimulation()
        {
            isAnimating = false;
            UpdatePositionForTime((startTimeHour + timeRangeHours) % 24f);
        }

        private void HandleAnimation()
        {
            //if (Keyboard.current.spaceKey.wasPressedThisFrame && !isAnimating)
            //{
            //    isAnimating = true;
            //    animationProgress = 0f;
            //    startTime = Time.time;
            //}

            if (isAnimating)
            {
                animationProgress = (Time.time - startTime) / animationTimeSeconds;
                
                if (animationProgress <= 1f)
                {
                    float currentHour = Mathf.Lerp(startTimeHour, 
                        startTimeHour + timeRangeHours, 
                        animationProgress);
                    currentHour = currentHour % 24f;
                    UpdatePositionForTime(currentHour);
                }
                else
                {
                    isAnimating = false;
                    UpdatePositionForTime((startTimeHour + timeRangeHours) % 24f);
                }
            }
        }

        private void UpdateStaticPosition()
        {
            UpdatePosition(m_Latitude, m_Longitude, m_Year, m_Month, m_Day, 
                m_Hour, m_Minute, m_Second);
        }

        private void UpdatePositionForTime(float hour)
        {
            try
            {
                Observer observer = new Observer(m_Latitude, m_Longitude, 0);
                int hours = Mathf.FloorToInt(hour);
                int minutes = Mathf.FloorToInt((hour - hours) * 60);
                AstroTime time = new AstroTime(new DateTime(m_Year, m_Month, m_Day, hours, minutes, 0));

                Equatorial equ_ofdate = Astronomy.Equator(Body.Sun, time, observer, EquatorEpoch.OfDate, Aberration.Corrected);
                Topocentric hor = Astronomy.Horizon(time, observer, equ_ofdate.ra, equ_ofdate.dec, Refraction.Normal);

                Vector3 sunDirection = -CalculateObjectPosition((float)hor.azimuth, (float)hor.altitude);
                transform.rotation = Quaternion.LookRotation(sunDirection, Vector3.up);
            }
            catch (Exception ex)
            {
                Debug.Log($"UpdatePositionForTime error: {ex.ToString()}");
            }
        }

        private void UpdatePosition(float latitude, float longitude, int year, int month, 
            int day, int hour, int minute, int second)
        {
            try
            {
                Observer observer = new Observer(latitude, longitude, 0);
                AstroTime time = new AstroTime(new DateTime(year, month, day, hour, minute, second));
                
                Equatorial equ_ofdate = Astronomy.Equator(Body.Sun, time, observer, EquatorEpoch.OfDate, Aberration.Corrected);
                Topocentric hor = Astronomy.Horizon(time, observer, equ_ofdate.ra, equ_ofdate.dec, Refraction.Normal);

                Vector3 sunDirection = -CalculateObjectPosition((float)hor.azimuth, (float)hor.altitude);
                transform.rotation = Quaternion.LookRotation(sunDirection, Vector3.up);
            }
            catch (Exception ex)
            {
                Debug.Log($"UpdatePosition error: {ex.ToString()}");
            }
        }

        private Vector3 CalculateObjectPosition(float azimuth, float altitude)
        {
            float azimuthRad = Mathf.Deg2Rad * azimuth;
            float altitudeRad = Mathf.Deg2Rad * altitude;

            float x = Mathf.Cos(altitudeRad) * Mathf.Sin(azimuthRad);
            float y = Mathf.Sin(altitudeRad);
            float z = Mathf.Cos(altitudeRad) * Mathf.Cos(azimuthRad);

            return new Vector3(x, y, z);
        }

        // Getter methods
        public float GetLatitude() { return m_Latitude; }
        public float GetLongitude() { return m_Longitude; }
        public DateTime GetDateTime()
        {
            return new DateTime(m_Year, m_Month, m_Day, m_Hour, m_Minute, m_Second);
        }
    }
}