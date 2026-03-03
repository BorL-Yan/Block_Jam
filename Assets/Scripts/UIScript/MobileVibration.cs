using UnityEngine;

public static class MobileVibration 
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject _vibrator;
    
    // Ленивая инициализация вибратора для безопасности
    private static AndroidJavaObject Vibrator
    {
        get
        {
            if (_vibrator == null)
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        _vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                    }
                }
            }
            return _vibrator;
        }
    }
#endif

    public static void Vibrate(VibrationType type)
    { 
        if (Application.platform == RuntimePlatform.Android)
        {
            long milliseconds = 0;
            // Амплитуда в Android может быть от 1 до 255
            int amplitude = 0; 
            
            switch (type)
            {
                case VibrationType.weak:
                    milliseconds = 50;
                    amplitude = 50;
                    break;
                case VibrationType.average:
                    milliseconds = 100;
                    amplitude = 120;
                    break;
                case VibrationType.strong:
                    milliseconds = 200;
                    amplitude = 255; 
                    break;
            }
            
#if UNITY_ANDROID && !UNITY_EDITOR
            // ИСПРАВЛЕНО: hasVibrator вместо hasVibrate
            if (Vibrator != null && Vibrator.Call<bool>("hasVibrator")) 
            {
                // Проверяем версию Android
                using (AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    int sdkInt = versionClass.GetStatic<int>("SDK_INT");
                    
                    if (sdkInt >= 26) // Android 8.0 (Oreo) и выше
                    {
                        using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                        {
                            using (AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude))
                            {
                                Vibrator.Call("vibrate", vibrationEffect);
                            }
                        }
                    }
                    else // Старые версии Android (без контроля амплитуды)
                    {
                        Vibrator.Call("vibrate", milliseconds);
                    }
                }
            }
#endif
        }
    }
}

public enum VibrationType
{
    weak,
    average,
    strong
}