namespace Player
{
    // ILSpyBased#2
    using System;
    using UnityEngine;

    public class TimeManager : MonoBehaviour
    {
        public static long NetworkTime => (long)(Time.time * 1000);
    }



}