
using UnityEngine;
// using RuntimeInspectorNamespace;

public class PrefsWrapper : MonoBehaviour
{

    [SerializeField]
    private string _crystalKey = "Crystal";

    public int Crystal
    {
        get
        {
            return PlayerPrefs.GetInt(_crystalKey, 0);
        }
        set 
        {
            PlayerPrefs.SetInt(_crystalKey, value);
            PlayerPrefs.Save();
        }
    }

    [SerializeField]
    private string _coinKey = "Coin";

    public int Coin
    {
        get
        {
            return PlayerPrefs.GetInt(_coinKey, 0);
        }
        set 
        {
            PlayerPrefs.SetInt(_coinKey, value);
            PlayerPrefs.Save();
        }
    }

}
