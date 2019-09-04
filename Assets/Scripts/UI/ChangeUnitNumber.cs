using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeUnitNumber : MonoBehaviour
{
    public TMP_InputField InputField;
    
    private void Awake()
    {
        var storedNumberOfEnemies = PlayerPrefs.GetInt("NumberOfEnemies");
        if (storedNumberOfEnemies == 0)
        {
            storedNumberOfEnemies = 5000;
        }
        PlayerPrefs.SetInt("NumberOfEnemies", storedNumberOfEnemies);

        InputField.text = storedNumberOfEnemies.ToString();
    }

    public void ApplyNumberOfEnemies()
    {
        var inputResult = int.Parse(InputField.text);
        if (inputResult > 100000)
        {
            return;
        }
        
        PlayerPrefs.SetInt("NumberOfEnemies", inputResult);
        
        SceneManager.LoadScene("EmptyScene");
    }
}
