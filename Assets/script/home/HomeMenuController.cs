using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenuController : MonoBehaviour
{
    public void LoadGameplayScene()
    {
        SceneManager.LoadScene("gameplay"); 
    }
}