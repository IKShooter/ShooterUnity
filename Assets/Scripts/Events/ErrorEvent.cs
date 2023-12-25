using System;

namespace Events
{
   public delegate void ErrorEvent(Exception exception, bool isCritical);
}