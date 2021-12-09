using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Clear : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button tweetButton;
    [SerializeField] private Button titleButton;
    [SerializeField] private Text resultTimeText;

    private void Start()
    {
        retryButton.onClick.AddListener(() =>
        {
            var currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        });

        titleButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Title");
        });
        gameObject.transform.localScale = Vector3.zero;
    }

    public void GameClear(int time)
    {
        retryButton.interactable = false;
        tweetButton.interactable = false;
        titleButton.interactable = false;
        gameObject.transform.DOScale(Vector3.one, 0.5f).OnComplete(() =>
        {
            retryButton.interactable = true;
            tweetButton.interactable = true;
            titleButton.interactable = true;
        });
        tweetButton.onClick.AddListener(() =>
        {
            Debug.Log("tweet");
            Twitter.Tweet($"ぷらそにか神経衰弱を{CardManager.FormatTime(time)}でクリアしました！",
                "https://plusonica-fan.github.io/memory-game/", "ぷらそにかファンサイト,ぷらそにか神経衰弱");
        });
        resultTimeText.text = CardManager.FormatTime(time) + "でクリア！";
    }
}
