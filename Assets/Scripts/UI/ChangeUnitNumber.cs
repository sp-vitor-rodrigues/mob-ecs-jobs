using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeUnitNumber : MonoBehaviour
{
    public TMP_InputField DefendersInputField;
    public TMP_InputField AttackerInputField;
    public TextMeshProUGUI DefendersAmount;
    public TextMeshProUGUI AttackersAmount;

    private void Awake()
    {
        var storedNumberOfDefenders = PlayerPrefs.GetInt("NumberOfDefenders", 1);

        PlayerPrefs.SetInt("NumberOfDefenders", storedNumberOfDefenders);

        DefendersInputField.text = storedNumberOfDefenders.ToString();

        var storedNumberOfEnemies = PlayerPrefs.GetInt("NumberOfAttackers", 1);

        PlayerPrefs.SetInt("NumberOfAttackers", storedNumberOfEnemies);

        AttackerInputField.text = storedNumberOfEnemies.ToString();
    }

    public void ApplyNumberOfDefenders()
    {
        var inputResult = int.Parse(DefendersInputField.text);
        if (inputResult > 10000)
        {
            return;
        }

        PlayerPrefs.SetInt("NumberOfDefenders", inputResult);

        SceneManager.LoadScene("EmptyScene");
    }

    public void ApplyNumberOfEnemies()
    {
        var inputResult = int.Parse(AttackerInputField.text);
        if (inputResult > 10000)
        {
            return;
        }

        PlayerPrefs.SetInt("NumberOfAttackers", inputResult);

        SceneManager.LoadScene("EmptyScene");
    }

    public void SetNumberOfDefenders(int amount)
    {
        DefendersAmount.text = amount.ToString();
    }

    public void SetNumberOfAttackers(int amount)
    {
        AttackersAmount.text = amount.ToString();
    }
}
