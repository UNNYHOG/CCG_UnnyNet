using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Text loadingText;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(string text)
    {
        loadingText.text = text;
        gameObject.SetActive(true);
    }
}
