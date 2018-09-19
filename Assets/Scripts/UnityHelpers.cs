using System;

public static class UnityHelpers
{
    public static bool IsIPhoneX()
    {
        return UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX;
    }
}
