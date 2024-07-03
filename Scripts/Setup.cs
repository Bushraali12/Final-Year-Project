using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Setup : MonoBehaviour
{
    public InputField SignInEmail, OwnerPassword, GameName;
    public InputField SignUpEmail, SignUpUserName, SignUpMobileNumber, SignUpPassword, SignUpConfirmPassword;
    public GameObject LogInFailureScreen, LogInSuccessScreen, SignUpButton, SignUpScreen;
    public Button RegisterButton;
    public GameObject NotificationAlert;
    public Text NotifyText;
    public string Email, Username;
    public GameObject Signinpanel;
    public GameObject signuppanel;
    public Button signup;
    public Button CrossButton;
    public Button Back;
    public Button Back1;

    public static Setup instance;
    private void Awake()
    {
        instance = this;
        //SignUpButton.SetActive(false);
    }
    private void Start()
    {
        // Add listeners to buttons
        signup.onClick.AddListener(HideSignUpPanel);
        CrossButton.onClick.AddListener(ShowSignInPanel);
        // Back.onClick.AddListener(ShowSignInPanel);
        // Back1.onClick.AddListener(ShowSignUPanel);
    }
    public void HideSignUpPanel()
    {
        Signinpanel.SetActive(false);
    }
    public void ShowSignInPanel()
    {
        Signinpanel.SetActive(true);
    }
    public void ShowSignUPanel()
    {
        Signinpanel.SetActive(false);
        signuppanel.SetActive(true);
    }

    public void SignIn()
    {
        Email = SignInEmail.textComponent.text;
        FireBaseAuth.Instance.Account = Email;
        string Password = OwnerPassword.textComponent.text;
        if (!(string.IsNullOrEmpty(Email)) && !(string.IsNullOrEmpty(Password)))
        {
            PlayerPrefs.SetString("OwnerName", this.SignInEmail.textComponent.text.ToString());
            PlayerPrefs.SetString("RealPassword", this.OwnerPassword.text);
            PlayerPrefs.SetString("OwnerPassword", this.OwnerPassword.textComponent.text.ToString());
            Debug.Log("Password: " + OwnerPassword.text);
            LoginViaFirebase();
        }
        
    }

    public void CheckPassword()
    {
        string name = SignUpUserName.textComponent.text;
        FireBaseAuth.Instance.Account = name;
        string Password = SignUpPassword.textComponent.text;
        string ConfirmPassord = SignUpConfirmPassword.textComponent.text;
        if (!(string.IsNullOrEmpty(Password)) && !(string.IsNullOrEmpty(ConfirmPassord)) && Password == ConfirmPassord)
        {
            RegisterButton.interactable = true;
        }
        else
        {
            RegisterButton.interactable = false;
        }
    }

    public void Regiser()
    {
        Username = SignUpUserName.textComponent.text;
        Email = SignUpEmail.textComponent.text;
        FireBaseAuth.Instance.Account = Username;
        string Password = SignUpPassword.textComponent.text;
        string ConfirmPassord = SignUpConfirmPassword.textComponent.text;
        if (!(string.IsNullOrEmpty(Password)) && !(string.IsNullOrEmpty(ConfirmPassord)) && Password == ConfirmPassord)
        {
            
            PlayerPrefs.SetString("OwnerEmail", Email);
            PlayerPrefs.SetString("OwnerName", Username);
            PlayerPrefs.SetString("RealPassword", this.SignUpPassword.text);
            PlayerPrefs.SetString("OwnerPassword", this.SignUpPassword.textComponent.text.ToString());
            SignUpToFirebase();

        }
    }

    public void SetGameName()
    {
        string name = GameName.textComponent.text;
        if (!(string.IsNullOrEmpty(name)))
        {
            PlayerPrefs.SetString("GameName", this.GameName.textComponent.text.ToString());
            FireBaseAuth.Instance.SetPlayerDataInFirebase(name);
        }
    }

    public void SignUpToFirebase()
    {
        if (FireBaseAuth.Instance != null)
        {
            FireBaseAuth.Instance.SignUpUserButton(Email,Username, PlayerPrefs.GetString("RealPassword"));
            Debug.Log("Loaded account retrieved");
            // Signinpanel.SetActive(false);
        }
    }

    public void LoginViaFirebase()
    {
        if (FireBaseAuth.Instance != null)
        {
            FireBaseAuth.Instance.SignInUserButton(Email, PlayerPrefs.GetString("RealPassword"));
            Debug.Log("Loaded account retrieved");
            // Signinpanel.SetActive(false);
        }
    }

    public void LoggedInSuccess()
    {
        SignUpScreen.SetActive(false);
        RegisterButton.interactable = false;
        //LogInSuccessScreen.SetActive(true);
        NotifyUser(Username + " Logged In Successfully");
        LoadNextScene();
        PlayerPrefs.SetInt("LoggedIn", 1);
    }

    public void NotifyUser(string Message)
    {
        NotificationAlert.SetActive(true);
        NotifyText.text = Message;
    }

    public void LogInFailed()
    {
        //PlayerPrefs.SetInt("LoggedIn", 0);
        SignUpButton.SetActive(true);
        LogInFailureScreen.SetActive(true);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
