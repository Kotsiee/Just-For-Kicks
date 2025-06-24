using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using UnityEngine.Android;

using GoogleMobileAds.Mediation.UnityAds.Api;
using GoogleMobileAds.Mediation.LiftoffMonetize.Api;
using GoogleMobileAds.Mediation.IronSource.Api;

public class Login : MonoBehaviour
{
    [SerializeField] private AuthManager authManager;
    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private GameObject parentMenu, offlineMenu, loadingTxt, spinnerShoe, loginMenu, registerMenu, verificationMenu, resetPasswordMenus, resetEmailMenu, resetResendMenu, termsMenu, termsBtn, agreeTerms;
    [SerializeField] private TMP_Text title, login_Error, register_Error, reset_Error, loginBtn, registerBtn, verification_Email, reset_EmailText;
    [SerializeField] private TMP_InputField login_Email, login_Password, register_Username, register_Email, register_Password, register_repassword, reset_Email;
    [SerializeField] private Sprite ticked, unticked;
    [SerializeField] private Canvas canvas;

    private FirebaseUser user;
    private bool termsAccpted = false;

    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");

        StartCoroutine(CheckInternetConnection());

        UnityAds.SetConsentMetaData("gdpr.consent", true);
        UnityAds.SetConsentMetaData("privacy.consent", true);

        IronSource.SetConsent(true);
        IronSource.SetMetaData("do_not_sell", "true");

        LiftoffMonetize.UpdateConsentStatus(VungleConsentStatus.OPTED_IN, "1.0.0");
        LiftoffMonetize.UpdateCCPAStatus(VungleCCPAStatus.OPTED_IN);
    }

    public void loading(bool isLoading)
    {
        loadingTxt.SetActive(isLoading);
        spinnerShoe.SetActive(isLoading);
    }

    IEnumerator CheckInternetConnection()
    {
        loading(true);
        hideMenus();
        UnityWebRequest request = new UnityWebRequest("https://just-for-kick-default-rtdb.firebaseio.com/");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            openOfflineMenu();
            databaseManager.playerPrefs.offline = true;
            databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
        }
        else
        {
            user = authManager.user;
            databaseManager.playerPrefs.offline = false;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(async task =>
            {
                if (authManager.status == DependencyStatus.Available)
                {
                    // If they are available, initialize Firebase
                    if (user != null)
                    {
                        if ((user.IsEmailVerified | user.IsAnonymous) & databaseManager.playerPrefs.loggedIn)
                        {
                            databaseManager.playerPrefs.loggedIn = true;

                            if (user.IsAnonymous)
                                databaseManager.playerPrefs.anonymous = true;
                            else
                                databaseManager.playerPrefs.anonymous = false;

                            databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
                            await databaseManager.loadPlayerDataFromDatabaseIE().ContinueWithOnMainThread(loadTask =>
                            {
                                if (loadTask.IsCompleted)
                                {
                                    SceneManager.LoadScene("Game");
                                }
                            });
                        }
                        else
                        {
                            loading(false);
                            login_Email.text = user.Email;
                            parentMenu.SetActive(true);
                            parentMenu.transform.localScale = Vector3.zero;
                            LeanTween.scale(parentMenu, Vector3.one, 0.25f).setEaseOutBack();
                            openLoginMenu();
                        }
                    }
                    else
                    {
                        loading(false);
                        databaseManager.playerPrefs.loggedIn = false;
                        databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
                        parentMenu.SetActive(true);
                        parentMenu.transform.localScale = Vector3.zero;
                        LeanTween.scale(parentMenu, Vector3.one, 0.25f).setEaseOutBack();
                        openLoginMenu();
                    }
                }
            });
        }
    }

    public void tryAgain()
    {
        StartCoroutine(CheckInternetConnection());
    }



    #region Open Menus
    private void hideMenus()
    {
        parentMenu.SetActive(false);
        offlineMenu.SetActive(false);
        resetPasswordMenus.SetActive(false);
        termsMenu.SetActive(false);
    }

    private void hideLoginMenus()
    {
        parentMenu.SetActive(true);
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);
        verificationMenu.SetActive(false);
        resetEmailMenu.SetActive(false);
        resetResendMenu.SetActive(false);

        login_Error.gameObject.SetActive(false);
        register_Error.gameObject.SetActive(false);
        reset_Error.gameObject.SetActive(false);
    }

    public void openOfflineMenu()
    {
        hideMenus();
        loading(false);
        offlineMenu.transform.localScale = Vector3.zero;
        LeanTween.scale(offlineMenu, Vector3.one, 0.15f).setEaseInBack();
        offlineMenu.SetActive(true);
    }

    public void openLoginMenu()
    {
        hideLoginMenus();
        loginBtn.color = new Color32(255, 255, 255, 255);
        registerBtn.color = new Color32(153, 153, 153, 255);
        title.text = "LOGIN";

        loginMenu.SetActive(true);
        LeanTween.value(gameObject, Vector2.zero, new Vector2(canvas.pixelRect.width, 0), 0.25f).setOnUpdate((Vector2 val) => registerMenu.GetComponent<RectTransform>().anchoredPosition = val).setEaseInCirc();
        LeanTween.value(gameObject, new Vector2(-canvas.pixelRect.width, 0), Vector2.zero, 0.25f).setOnUpdate((Vector2 val) => loginMenu.GetComponent<RectTransform>().anchoredPosition = val).setEaseOutCirc();
        registerMenu.SetActive(false);
        registerMenu.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void openRegisterMenu()
    {
        hideLoginMenus();
        registerBtn.color = new Color32(255, 255, 255, 255);
        loginBtn.color = new Color32(153, 153, 153, 255);
        title.text = "REGISTER";

        registerMenu.SetActive(true);
        LeanTween.value(gameObject, Vector2.zero, new Vector2(-canvas.pixelRect.width, 0), 0.25f).setOnUpdate((Vector2 val) => loginMenu.GetComponent<RectTransform>().anchoredPosition = val).setEaseInCirc();
        LeanTween.value(gameObject, new Vector2(canvas.pixelRect.width, 0), Vector2.zero, 0.25f).setOnUpdate((Vector2 val) => registerMenu.GetComponent<RectTransform>().anchoredPosition = val).setEaseOutCirc();
        loginMenu.SetActive(false);
        loginMenu.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void openVerificationMenu(FirebaseUser user)
    {
        hideLoginMenus();
        loginBtn.color = new Color32(153, 153, 153, 255);
        registerBtn.color = new Color32(153, 153, 153, 255);
        title.text = "VERIFICATION";
        verificationMenu.SetActive(true);

        verificationMenu.SetActive(true);
        verification_Email.text = "We sent an email to \n" + user.Email;

        user.SendEmailVerificationAsync();
    }

    public void openVerificationMenu()
    {
        user.SendEmailVerificationAsync();
    }

    public void openResetPasswordMenu_1()
    {
        hideLoginMenus();
        resetPasswordMenus.SetActive(true);
        loginBtn.color = new Color32(153, 153, 153, 255);
        registerBtn.color = new Color32(153, 153, 153, 255);
        title.text = "RESET PASSWORD";

        resetEmailMenu.SetActive(true);
        LeanTween.value(gameObject, Vector2.zero, new Vector2(canvas.pixelRect.width, 0), 0.25f).setOnUpdate((Vector2 val) => loginMenu.GetComponent<RectTransform>().anchoredPosition = val).setEaseInCirc();
        LeanTween.value(gameObject, new Vector2(-canvas.pixelRect.width, 0), Vector2.zero, 0.25f).setOnUpdate((Vector2 val) => resetEmailMenu.GetComponent<RectTransform>().anchoredPosition = val).setEaseOutCirc();
        loginMenu.SetActive(false);
        loginMenu.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void openResetPasswordMenu_2()
    {
        loading(true);
        LeanTween.scale(parentMenu, Vector3.zero, 0.15f).setEaseInBack();
        hideLoginMenus();
        resetPasswordMenus.SetActive(true);
        resetEmailMenu.SetActive(true);

        authManager.resetPassword(reset_Email.text, val =>
        {
            LeanTween.scale(parentMenu, Vector3.one, 0.15f).setEaseInBack();

            switch (val)
            {
                case AuthError.MissingEmail:
                    showError(login_Error, "Missing Email");
                    break;
                case AuthError.InvalidEmail:
                    showError(login_Error, "Incorrect Email");
                    break;
                case AuthError.UserNotFound:
                    showError(login_Error, "Account does not exist");
                    break;
                case AuthError.None:
                    reset_EmailText.text = "We sent an email to \n" + user.Email;
                    resetPasswordMenus.SetActive(true);
                    resetEmailMenu.SetActive(false);
                    resetResendMenu.SetActive(true);
                    break;
            }
        });
    }

    public async void openTermsMenu(bool state)
    {
        LeanTween.scale(termsMenu, state ? Vector3.one : Vector3.zero, 0.25f).setEaseInOutBack();
        await Task.Delay(state ? 0 : 250);
        termsMenu.SetActive(state);
    }

    #endregion

    #region Buttons
    public void login()
    {
        login_Error.gameObject.SetActive(false);
        loading(true);
        LeanTween.scale(parentMenu, Vector3.zero, 0.15f).setEaseInBack();

        authManager.LoginWithEmail(login_Email.text, login_Password.text, val =>
        {
            loading(false);
            LeanTween.scale(parentMenu, Vector3.one, 0.15f).setEaseInBack();

            switch (val)
            {
                case AuthError.MissingEmail:
                    showError(login_Error, "Missing Email");
                    break;
                case AuthError.MissingPassword:
                    showError(login_Error, "Missing Password");
                    break;
                case AuthError.WrongPassword:
                    showError(login_Error, "Incorrect Password");
                    break;
                case AuthError.InvalidEmail:
                    showError(login_Error, "Incorrect Email");
                    break;
                case AuthError.UserNotFound:
                    showError(login_Error, "Account does not exist");
                    break;
            }
        }, async user =>
        {
            if (user != null)
            {
                if (user.IsEmailVerified)
                {
                    databaseManager.playerPrefs.loggedIn = true;
                    databaseManager.playerPrefs.anonymous = false;
                    databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
                    await databaseManager.loadPlayerDataFromDatabaseIE().ContinueWithOnMainThread(loadTask =>
                    {
                        if (loadTask.IsCompleted)
                        {
                            SceneManager.LoadScene("Game");
                        }
                    });
                }
                else
                {
                    openVerificationMenu(user);
                }
            }
        });
    }

    public void register()
    {
        loading(true);
        LeanTween.scale(parentMenu, Vector3.zero, 0.15f).setEaseInBack();

        string pattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        Regex regex = new Regex(pattern);

        register_Error.gameObject.SetActive(false);
        if (register_Username.text.IsNullOrEmpty() | register_Email.text.IsNullOrEmpty() | register_Password.text.IsNullOrEmpty() | register_repassword.text.IsNullOrEmpty())
            showError(register_Error, "Some fields are empty");
        else if (!regex.IsMatch(register_Password.text) | register_Password.text.Length < 8)
            showError(register_Error, "Invalid Password. Must be 8 characters or more and contain special characters (eg. !@#%*)");
        else if (register_repassword.text != register_Password.text)
            showError(register_Error, "Passwords are not the same");
        else if (!termsAccpted)
            showError(register_Error, "You have not accepted the Terms and Conditions");
        else
        {
            if (databaseManager.playerPrefs.anonymous == true)
            {
                authManager.RegisterFromAnonymous(register_Username.text, register_Email.text, register_Password.text,
                    val =>
                    {
                        loading(false);
                        LeanTween.scale(parentMenu, Vector3.one, 0.15f).setEaseInBack();

                        switch (val)
                        {
                            case AuthError.MissingEmail:
                                showError(register_Error, "Missing Email");
                                break;
                            case AuthError.MissingPassword:
                                showError(register_Error, "Missing Password");
                                break;
                            case AuthError.WeakPassword:
                                showError(register_Error, "Invalid Password. Must be 8 characters or more and contain special characters (eg. !@#%*)");
                                break;
                            case AuthError.InvalidEmail:
                            case AuthError.AccountExistsWithDifferentCredentials:
                            case AuthError.EmailAlreadyInUse:
                                showError(register_Error, "Account Already Exists");
                                break;
                        }
                    },
                    user => openVerificationMenu(user));
            }
            else
            {
                authManager.RegisterWithEmail(register_Username.text, register_Email.text, register_Password.text,
                    val =>
                    {
                        loading(false);
                        LeanTween.scale(parentMenu, Vector3.one, 0.15f).setEaseInBack();

                        switch (val)
                        {
                            case AuthError.MissingEmail:
                                showError(register_Error, "Missing Email");
                                break;
                            case AuthError.MissingPassword:
                                showError(register_Error, "Missing Password");
                                break;
                            case AuthError.WeakPassword:
                                showError(register_Error, "Invalid Password. Must be 8 characters or more and contain special characters (eg. !@#%*)");
                                break;
                            case AuthError.InvalidEmail:
                            case AuthError.AccountExistsWithDifferentCredentials:
                            case AuthError.EmailAlreadyInUse:
                                showError(register_Error, "Account Already Exists");
                                break;
                        }
                    },
                    user => openVerificationMenu(user));
            }
        }
    }

    public async void playAsGuest()
    {
        loading(true);
        LeanTween.scale(parentMenu, Vector3.zero, 0.15f).setEaseInBack();
        await Task.Delay(150);
        await authManager.LoginAnonymously().ContinueWithOnMainThread(async task =>
        {
            if (task.IsCompleted)
            {
                databaseManager.playerPrefs.loggedIn = true;
                databaseManager.playerPrefs.anonymous = true;
                databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
                await databaseManager.loadPlayerDataFromDatabaseIE().ContinueWithOnMainThread(loadTask =>
                {
                    if (loadTask.IsCompleted)
                    {
                        SceneManager.LoadScene("Game");
                    }
                });
            }
        });
    }

    public async void playOffline()
    {
        loading(true);
        LeanTween.scale(offlineMenu, Vector3.zero, 0.15f).setEaseInBack();
        await Task.Delay(150);
        SceneManager.LoadScene("Game");
    }

    public async void exitGame()
    {
        loading(true);
        LeanTween.scale(offlineMenu, Vector3.zero, 0.15f).setEaseInBack();
        await Task.Delay(150);
        Application.Quit();
    }

    public void acceptTnC()
    {
        if (termsAccpted)
        {
            termsBtn.GetComponent<Image>().sprite = unticked;
            agreeTerms.GetComponent<Image>().color = new Color32(90, 184, 64, 255);
            agreeTerms.transform.GetChild(0).GetComponent<TMP_Text>().text = "Agree";

            termsAccpted = false;
        }
        else
        {
            termsBtn.GetComponent<Image>().sprite = ticked;
            agreeTerms.GetComponent<Image>().color = new Color32(190, 40, 70, 255);
            agreeTerms.transform.GetChild(0).GetComponent<TMP_Text>().text = "Disagree";

            termsAccpted = true;
        }
    }

    #endregion

    private void showError(TMP_Text obj, string message)
    {
        obj.gameObject.SetActive(true);
        obj.text = "- Error: " + message;
    }
}
