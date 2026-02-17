using UnityEngine;

[System.Serializable]
public struct IntentData
{
    public Sprite Icon;
    public bool ShowValue;
    public int Value;

    public static IntentData IconOnly(Sprite icon)
        => new IntentData { Icon = icon, ShowValue = false, Value = 0 };

    public static IntentData IconWithValue(Sprite icon, int value)
        => new IntentData { Icon = icon, ShowValue = true, Value = value };
}
