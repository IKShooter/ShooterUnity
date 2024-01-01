using System;

namespace Events
{
   public delegate void ErrorEvent(string tag, Exception exception, bool isCritical);
}