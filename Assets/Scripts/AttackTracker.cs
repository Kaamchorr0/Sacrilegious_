using UnityEngine;

public class AttackTracker : MonoBehaviour
{
    private const string Close_Atk_Key = "CloseAtk";
    private const string Long_Atk_Key = "LongAtk";
    public void LogCloseAtk()
    {
        int currentAtk = PlayerPrefs.GetInt(Close_Atk_Key, 0);
        currentAtk++;
        PlayerPrefs.SetInt(Close_Atk_Key, currentAtk);
    }
    public void LogLongAtk()
    {
        int currentAtk = PlayerPrefs.GetInt(Long_Atk_Key, 0);
        currentAtk++;
        PlayerPrefs.SetInt(Long_Atk_Key, currentAtk);
    }   
    public static void ResetTrackers()
    {
        PlayerPrefs.SetInt(Close_Atk_Key, 0);
        PlayerPrefs.SetInt(Long_Atk_Key, 0);
        Debug.Log("Reset");
    }
}
