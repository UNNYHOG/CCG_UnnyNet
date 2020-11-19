using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnnyNet.API.Auth;

public class LoginWindow : MonoBehaviour
{
    [SerializeField]
    private LoadingScreen loadingScreen;

    [SerializeField]
    private InputField userName;
    [SerializeField]
    private InputField password;
    
    [SerializeField]
    private Button loginButton;
    
    [SerializeField]
    private Button loginAsGuestButton;
    
    [SerializeField]
    private Text errorText;

    private void Awake()
    {
        loginButton.onClick.AddListener(LoginClicked);   
        loginAsGuestButton.onClick.AddListener(LoginAsGuestClicked);
    }

    private void LoginAsGuestClicked()
    {
        RemoveError();
        loadingScreen.Show("Authorizing...");
        UnnyNet.Auth.AsGuest(CheckAuthResponse);
    }

    private void CheckAuthResponse(AuthResponseData response)
    {
        loadingScreen.Hide();
        if (response.Success)
        {
            SceneManager.LoadScene("Store");
        }
        else
        {
            ShowError($"[{response.Error.Code}] {response.Error.Message}");
        }
    }

    private void LoginClicked()
    {
        RemoveError();
        loadingScreen.Show("Authorizing...");
        UnnyNet.Auth.WithName(userName.text, password.text, CheckAuthResponse);
    }

    private void Start()
    {
        UnnyNet.Main.Init(new UnnyNet.AppConfig {
            ApiGameId = "7546c2f2-2739-11eb-a5d4-0684909fddca",
            PublicKey = "M2IxNzQwOWNjNDYxMjg0MDI0YzVjOD",
            Environment = UnnyNet.Constants.Environment.Development,
            OnReadyCallback = responseData =>
            {
                Debug.Log("UnnyNet Initialized: " + responseData.Success);
                loadingScreen.Hide();
            }
        });
    }

    private void RemoveError()
    {
        errorText.text = string.Empty;
    }

    private void ShowError(string text)
    {
        errorText.text = text;
    }
}
