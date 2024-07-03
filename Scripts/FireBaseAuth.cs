using Proyecto26;
using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FullSerializer;
using UnityEngine.Serialization;
using Newtonsoft.Json;

[Serializable]
public class UserInfo
{
    public bool emailVerified;
}
[Serializable]
public class EmailConfirmationInfo
{
    public UserInfo[] users;
}

[Serializable]
public class SignUpResponse
{
    public string localId;
    public string idToken;
    public string UserName;
    public string PSWD;
}

[Serializable]
public class PlayerData
{
    public string GameName;
}

public class FireBaseAuth : MonoBehaviour
{
    public string Account;
    //Authentication
    private string AuthKey = "AIzaSyDWk_4agsPXi4EfMBVy3FzvEMftB6V3GNU";
    public string localId;
    public string idToken;
    public static FireBaseAuth Instance;

    public PlayerData Data;
    private SignUpResponse signUpResponse;
    private static readonly string firebaseLink = "https://authentication-34a12-default-rtdb.firebaseio.com/";

    public static fsSerializer serializer = new fsSerializer();

    public delegate void PostItemCallback();
    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerDataInFirebase(string GameName)
    {
        Data.GameName = GameName;

        PostToDatabase(Data, false);
    }

    public void PostToDatabase(PlayerData playerData, bool signingUp = false)
    {
        if (signingUp)
            RestClient.Put<SignUpResponse>($"{firebaseLink}" + "/" + localId /*+ "/" + Account */+ "/" + "Credentials" + ".json?auth=" + idToken, signUpResponse).Then(response =>
            {
                if (Setup.instance != null)
                    Setup.instance.LoggedInSuccess();
            });
        else
        {
            RestClient.Put<PlayerData>($"{firebaseLink}" + "/" + localId + ".json?auth=" + idToken, playerData).Then(response =>
            {
                callback();
            });
        }
    }

    private void callback()
    {
    }

    #region Firebase Auth
    public void SignUpUserButton(string email, string ownerName, string password)
    {
        SignUpUser(email, ownerName, password);
    }

    public void SignInUserButton(string email, string password)
    {
        SignInUser(email, password);
    }
    private void SignUpUser(string email, string username, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignUpResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + AuthKey, userData).Then(
            response =>
            {
                string emailVerification = "{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"" + response.idToken + "\"}";
                RestClient.Post(
                    "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + AuthKey,
                    emailVerification);
                idToken = response.idToken;
                localId = response.localId;
                response.PSWD = PlayerPrefs.GetString("RealPassword");
                response.UserName = username;
                if (Setup.instance != null)
                {
                    Setup.instance.SignUpScreen.SetActive(false);
                    Setup.instance.RegisterButton.interactable = false;
                    Setup.instance.NotifyUser("You have successfully signed up. Please verify your email to complete the registration.");

                }


            }).Catch(error =>
            {
                Setup.instance.LogInFailed();
                Debug.Log(error);
            });
    }

    private void SignInUser(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignUpResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + AuthKey, userData).Then(
            response =>
            {
                string emailVerification = "{\"idToken\":\"" + response.idToken + "\"}";
                RestClient.Post(
                   "https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=" + AuthKey,
                   emailVerification).Then(
                   emailResponse =>
                   {
                       try
                       {
                           // Deserialize the JSON string directly into EmailConfirmationInfo object
                           EmailConfirmationInfo emailConfirmationInfo = JsonConvert.DeserializeObject<EmailConfirmationInfo>(emailResponse.Text);
                           Debug.Log("Deserialized JSON: " + emailConfirmationInfo);

                           if (emailConfirmationInfo != null && emailConfirmationInfo.users != null && emailConfirmationInfo.users.Length > 0)
                           {
                               bool emailVerified = emailConfirmationInfo.users[0].emailVerified;
                               Debug.Log("Email Verified: " + emailVerified);

                               if (emailVerified)
                               {
                                   Debug.Log("Email verified");
                                   Setup.instance.NotifyUser("Email verified, Logging in!");
                                   idToken = response.idToken;
                                   localId = response.localId;
                                   if (Setup.instance != null)
                                   {
                                       Setup.instance.LoggedInSuccess();
                                   }
                                  
                               }
                               else
                               {
                                   Debug.Log("Email not verified");
                                   Setup.instance.NotifyUser("Please verify email first");
                               }
                           }
                           else
                           {
                               Debug.Log("No user information found in response.");
                           }
                       }
                       catch (JsonSerializationException ex)
                       {
                           Debug.LogError("Error during JSON deserialization: " + ex.Message);
                       }
                   }).Catch(error =>
                   {
                       Debug.Log("Email Response Error: " + error);
                   });

                //idToken = response.idToken;
                //localId = response.localId;
                //if (Setup.instance != null)
                //    Setup.instance.LoggedInSuccess();
                //Debug.Log("Signed in Account called: " + localId);
                //GetUsername();
            }).Catch(error =>
            {
                Setup.instance.LogInFailed();
                Debug.Log(error);
            });
    }

    public void GetUsername()
    {
        RestClient.Get<SignUpResponse>($"{firebaseLink}" + "/" + localId /*+ "/" + Account */+ "/" + "Credentials" + ".json?auth=" + idToken).Then(response =>
        {
            //Setup.instance.mainMenu.UserNameText.text = response.UserName;
            //OwnerProperties.instance.mainMenu.PasswordText.text = response.PSWD;
            //OwnerProperties.instance.mainMenu.PasswordRetrieveScreen.SetActive(true);
        });
    }

    #endregion
}
