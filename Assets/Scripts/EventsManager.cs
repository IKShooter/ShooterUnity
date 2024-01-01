using System;

public static class EventsManager<T> where T : Delegate
{
    private static T _handle;
 
    public static void Register(T callback)
    {
        _handle = Delegate.Combine(_handle, callback) as T;
    }
 
    public static void Unregister(T callback)
    {
        _handle = Delegate.Remove(_handle, callback) as T;
    }
 
    public static T Trigger
    {
        get { return _handle; }
    }
}