using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanAndReloadScene : MonoBehaviour
{
    void Start()
    {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene("SCN_Map");
    }
}
