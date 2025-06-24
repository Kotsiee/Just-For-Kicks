using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Settings : MonoBehaviour
{
    public List<GameObject> closePanels;
    [SerializeField] private DatabaseManager databaseManager;

    [SerializeField] private Slider musicSlider, sfxSlider;
    [SerializeField] private GameObject settingsPanel, termsMenu, leftBtn, rightBtn, mainSettingsPanel, deleteAccountPanel, resetPasswordPanel, logoutPanel, deleteButton, resetButton, editBtn, saveBtn;
    [SerializeField] private TMP_Text username, email, reset_email, username_error;
    [SerializeField] private TMP_InputField emailInput, passwordInput, usernameInput;

    [SerializeField] private GameObject loading;
    [SerializeField] private Canvas canvas;

    void Start()
    {
        if (databaseManager.playerPrefs.shoeSide == 1) rightShoe();
        else leftShoe();

        musicSlider.value = databaseManager.playerPrefs.music;
        sfxSlider.value = databaseManager.playerPrefs.soundFX;
        username.text = databaseManager.playerData.name;
        email.text = databaseManager.playerData.email;
    }

    public void editUsername()
    {
        usernameInput.gameObject.SetActive(true);
        saveBtn.SetActive(true);
        editBtn.SetActive(false);
    }

    public void saveUsername()
    {
        if (usernameInput.text.Length < 3 || usernameInput.text.Length > 12)
        {
            username_error.gameObject.SetActive(true);
            username_error.text = "- Error: Username length must be between 3 and 12 characters.";
        }
        else
        {
            username_error.gameObject.SetActive(false);
            databaseManager.setUsername(usernameInput.text);
            usernameInput.gameObject.SetActive(false);
            saveBtn.SetActive(false);
            editBtn.SetActive(true);
        }
    }

    public void openSettings()
    {
        closePanels.ForEach(panel => panel.SetActive(false));
        settingsPanel.gameObject.SetActive(true);

        resetButton.SetActive(!databaseManager.authManager.user.IsAnonymous & !databaseManager.playerPrefs.offline);
        deleteButton.SetActive(!databaseManager.authManager.user.IsAnonymous & !databaseManager.playerPrefs.offline);
        editBtn.SetActive(!databaseManager.playerPrefs.offline);
    }

    public void openLink(string link)
    {
        Application.OpenURL(link);
    }

    public void leftShoe()
    {
        leftBtn.GetComponent<Image>().color = new Color32(30, 160, 225, 255);
        rightBtn.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
        databaseManager.playerPrefs.shoeSide = -1;
        databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
    }

    public void rightShoe()
    {
        rightBtn.GetComponent<Image>().color = new Color32(30, 160, 225, 255);
        leftBtn.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
        databaseManager.playerPrefs.shoeSide = 1;
        databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
    }

    public void changeMusicVolume()
    {
        if (musicSlider.value <= 0.01f)
        {
            databaseManager.musicSource.volume = 0;
            databaseManager.music.audioMixer.SetFloat("MusicVolume", 0);
        }
        else
        {
            databaseManager.musicSource.volume = 1;
            databaseManager.music.audioMixer.SetFloat("MusicVolume", 20f * Mathf.Log10(musicSlider.value) + 6.02f);
        }
    }

    public void changeSFXVolume()
    {
        if (sfxSlider.value <= 0.01f)
        {
            databaseManager.sfxSources.ForEach(source => source.volume = 0);
            databaseManager.sfx.audioMixer.SetFloat("SFXVolume", 0);
        }
        else
        {
            databaseManager.sfxSources.ForEach(source => source.volume = 1);
            databaseManager.sfx.audioMixer.SetFloat("SFXVolume", 20f * Mathf.Log10(sfxSlider.value) + 6.02f);
        }
    }

    public void buttonUpSave()
    {
        databaseManager.playerPrefs.music = musicSlider.value;
        databaseManager.playerPrefs.soundFX = sfxSlider.value;

        databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
    }

    public async void openTermsMenu(bool state)
    {
        LeanTween.scale(termsMenu, state ? Vector3.one : Vector3.zero, 0.25f).setEaseInOutBack();
        await Task.Delay(state ? 0 : 250);
        termsMenu.SetActive(state);
    }



    public async void logout()
    {
        if (!databaseManager.playerPrefs.offline)
        {
            loading.SetActive(true);
            if (databaseManager.authManager.user.IsAnonymous)
            {
                await databaseManager.databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetValueAsync(null);
                databaseManager.authManager.deleteUser();
                databaseManager.authManager.LogOut();
            }
            else
                databaseManager.authManager.LogOut();
        }
        else
        {
            databaseManager.playerPrefs.loggedIn = false;
            databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
            SceneManager.LoadScene("Login");
        }
    }

    public void resetPassword()
    {
        loading.SetActive(true);
        databaseManager.authManager.resetPassword(databaseManager.authManager.user.Email, null).ContinueWithOnMainThread(resetPasswordTask =>
        {
            loading.SetActive(false);
            if (resetPasswordTask.IsCompleted)
            {
                reset_email.text = "We sent an email to \n" + databaseManager.authManager.user.Email;
            }
        });
    }

    public async void deleteAccount()
    {
        loading.SetActive(true);
        if (databaseManager.authManager.user.IsAnonymous)
        {
            await databaseManager.databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetValueAsync(null);
            databaseManager.authManager.deleteUser();
            databaseManager.authManager.LogOut();
        }
        else
        {
            FirebaseUser user = databaseManager.authManager.auth.CurrentUser;

            Credential credential =
                EmailAuthProvider.GetCredential(emailInput.text, passwordInput.text);

            if (databaseManager.authManager.user != null)
            {
                await databaseManager.authManager.user.ReauthenticateAsync(credential).ContinueWith(async task =>
                {
                    if (task.IsFaulted)
                    {

                    }

                    if (task.IsCompletedSuccessfully)
                    {
                        await databaseManager.databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetValueAsync(null);
                        databaseManager.authManager.deleteUser();
                        databaseManager.authManager.LogOut();
                    }
                });
            }
        }
    }

    private void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        LeanTween.value(gameObject, new Vector2(0, panel.GetComponent<RectTransform>().anchoredPosition.y), new Vector2(canvas.pixelRect.width, panel.GetComponent<RectTransform>().anchoredPosition.y), 0.15f)
            .setOnUpdate((Vector2 val) => mainSettingsPanel.GetComponent<RectTransform>().anchoredPosition = val)
            .setEaseInBack();
        LeanTween.value(gameObject, new Vector2(-canvas.pixelRect.width, panel.GetComponent<RectTransform>().anchoredPosition.y), new Vector2(0, panel.GetComponent<RectTransform>().anchoredPosition.y), 0.15f)
            .setOnUpdate((Vector2 val) => panel.GetComponent<RectTransform>().anchoredPosition = val)
            .setEaseOutBack();
        mainSettingsPanel.SetActive(false);
        mainSettingsPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, panel.GetComponent<RectTransform>().anchoredPosition.y);
    }

    private void BackToMainPanel(GameObject openedTab)
    {
        mainSettingsPanel.SetActive(true);
        LeanTween.value(gameObject, new Vector2(0, openedTab.GetComponent<RectTransform>().anchoredPosition.y), new Vector2(-canvas.pixelRect.width, openedTab.GetComponent<RectTransform>().anchoredPosition.y), 0.15f)
            .setOnUpdate((Vector2 val) => openedTab.GetComponent<RectTransform>().anchoredPosition = val)
            .setEaseInBack();
        LeanTween.value(gameObject, new Vector2(canvas.pixelRect.width, openedTab.GetComponent<RectTransform>().anchoredPosition.y), new Vector2(0, openedTab.GetComponent<RectTransform>().anchoredPosition.y), 0.15f)
            .setOnUpdate((Vector2 val) => mainSettingsPanel.GetComponent<RectTransform>().anchoredPosition = val)
            .setEaseOutBack();
        openedTab.SetActive(false);
        openedTab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, openedTab.GetComponent<RectTransform>().anchoredPosition.y);
    }

    public void openLogoutMenu()
    {
        OpenPanel(logoutPanel);
    }

    public void openResetPassword()
    {
        OpenPanel(resetPasswordPanel);
    }

    public void openDeleteAccount()
    {
        OpenPanel(deleteAccountPanel);
    }

    public void back()
    {
        GameObject openedTab = null;

        if (logoutPanel.activeSelf)
            openedTab = logoutPanel;

        if (resetPasswordPanel.activeSelf)
            openedTab = resetPasswordPanel;

        if (deleteAccountPanel.activeSelf)
            openedTab = deleteAccountPanel;

        BackToMainPanel(openedTab);
    }
}
