using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    private void Start()
    {
        var button = GameObject.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync("Game");
        });
    }
}