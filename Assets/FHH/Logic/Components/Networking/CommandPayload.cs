using System;
using UnityEngine;

namespace FHH.Logic.Components.Networking
{
    [Serializable]
    public class CommandPayload
    {
        public string Type;
        public bool Value; // unused
        public double X;
        public double Y;
        public double Z;
        public string ProjectId; // For guided mode
        public string Variant;
        public bool Enabled;
        public Quaternion Rotation;
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
    }
}