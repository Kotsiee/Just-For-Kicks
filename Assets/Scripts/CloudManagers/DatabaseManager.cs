using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using GooglePlayGames.OurUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class DatabaseManager : MonoBehaviour
{
    public AuthManager authManager;

    public DatabaseReference databaseReference;

    public PlayerData playerData;
    public PlayerPref playerPrefs;
    public FileHandler fileHandler, playerPrefsFileHander;
    public AudioSource musicSource;
    public List<AudioSource> sfxSources;
    public AudioMixerGroup music, sfx;

    public Canvas canvas;

    private void Awake()
    {
        fileHandler = new FileHandler("playerData.save", true);
        playerPrefsFileHander = new FileHandler("playerPrefs.save", true);
        if (fileHandler.Load<PlayerData>(true) != null)
        {
            try
            {
                playerData = fileHandler.Load<PlayerData>(true);
            }
            catch
            {
                playerData = new PlayerData();
                try
                {
                    playerData.userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
                }
                catch
                {
                    playerData.userID = "";
                }
                fileHandler.Save(playerData);
            }
        }
        else
        {
            playerData = new PlayerData();
            try
            {
                playerData.userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            }
            catch
            {
                playerData.userID = "";
            }
            fileHandler.Save(playerData);
        }

        if (playerPrefsFileHander.Load<PlayerPref>(true) != null)
        {
            try
            {
                playerPrefs = playerPrefsFileHander.Load<PlayerPref>(true);
            }
            catch
            {
                playerPrefs = new PlayerPref();
                playerPrefsFileHander.Save(playerPrefs);
            }

            if (playerPrefs.music <= 0.01f)
            {
                musicSource.volume = 0;
                music.audioMixer.SetFloat("MusicVolume", 0);
            }
            else
            {
                musicSource.volume = 1;
                music.audioMixer.SetFloat("MusicVolume", 20f * Mathf.Log10(playerPrefs.music) + 6.02f);
            }

            if (playerPrefs.soundFX <= 0.01f)
            {
                sfxSources.ForEach(source => source.volume = 0);
                sfx.audioMixer.SetFloat("SFXVolume", 0);
            }
            else
            {
                sfxSources.ForEach(source => source.volume = 1);
                sfx.audioMixer.SetFloat("SFXVolume", 20f * Mathf.Log10(playerPrefs.soundFX) + 6.02f);
            }
        }
        else
        {
            playerPrefs = new PlayerPref();
            playerPrefsFileHander.Save(playerPrefs);
        }

        Debug.Log(Application.persistentDataPath);
        authManager.InitializeFirebase();
    }

    void Start()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Adjust the RectTransform position of the canvas based on the safe area
            Rect safeArea = Screen.safeArea;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.offsetMin = new Vector2(safeArea.xMin, safeArea.yMin);
            canvasRect.offsetMax = new Vector2(-safeArea.xMax, -safeArea.yMax);
        }
    }

    public int GetStatusBarHeight()
    {
        int statusBarHeight = 0;
        AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        int resourceId = unityActivity.Call<AndroidJavaObject>("getResources").Call<int>("getIdentifier", "status_bar_height", "dimen", "android");

        if (resourceId > 0)
        {
            statusBarHeight = unityActivity.Call<AndroidJavaObject>("getResources").Call<int>("getDimensionPixelSize", resourceId);
        }

        return statusBarHeight;
    }

    #region User

    public void addUser(string user)
    {
        Debug.Log(user);
        User newUser = new User(user);
        Debug.Log(user);
        string json = JsonConvert.SerializeObject(newUser);
        Debug.Log(user);
        try
        {
            fileHandler = new FileHandler("playerData.save", true);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        Debug.Log(user);
        playerData = new PlayerData();
        Debug.Log(user);
        playerData.userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        fileHandler.Save(playerData);
        Debug.Log(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetRawJsonValueAsync(json);
        Debug.Log(user);
    }

    public void resetPassword()
    {
        if (authManager.user != null)
        {
            authManager.auth.SendPasswordResetEmailAsync(authManager.user.Email).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Password reset email sent successfully.");
            });
        }
    }

    public void reauthenticateUser(string email, string password)
    {
        FirebaseUser user = authManager.auth.CurrentUser;

        // Get authManager.auth credentials from the user for re-authentication. The example below shows
        // email and password credentials but there are multiple possible providers,
        // such as GoogleAuthProvider or FacebookAuthProvider.
        Credential credential =
            EmailAuthProvider.GetCredential(email, password);

        if (authManager.user != null)
        {
            authManager.user.ReauthenticateAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("ReauthenticateAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("ReauthenticateAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User reauthenticated successfully.");
            });
        }
    }

    #endregion

    #region Get user items

    public async void updateUserCollection(string collection, Dictionary<string, object> itemCollection)
    {
        switch (collection)
        {
            case "shoes":
                foreach (var child in itemCollection)
                {
                    playerData.playerAttributesData.shoes[child.Key] = (string)child.Value;
                }
                break;

            case "balls":
                foreach (var child in itemCollection)
                {
                    playerData.playerAttributesData.balls[child.Key] = (string)child.Value;
                }
                break;

            case "boxes":
                foreach (var child in itemCollection)
                {
                    playerData.playerAttributesData.boxes[child.Key] = (string)child.Value;
                }
                break;
        }

        fileHandler.Save(playerData);
        await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child(collection).UpdateChildrenAsync(itemCollection);
    }

    public async void removeUserCollectionItem(string collection, Dictionary<string, object> itemCollection, string item)
    {
        switch (collection)
        {
            case "shoes":
                playerData.playerAttributesData.shoes.Remove(item);
                break;

            case "balls":
                playerData.playerAttributesData.balls.Remove(item);
                break;

            case "boxes":
                playerData.playerAttributesData.boxes.Remove(item);
                break;
        }

        fileHandler.Save(playerData);
        await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child(collection).Child(item).SetValueAsync(null);
        await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child(collection).UpdateChildrenAsync(itemCollection);
    }

    private Task GetData(string childName, Action<string> onCallback)
    {
        var databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var userReference = databaseReference.Child("users").Child(userId).Child(childName);

        return userReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                onCallback.Invoke(snapshot.Value.ToString());
            }
        });
    }

    private Task GetDictionaryData(string childName, Action<Dictionary<string, object>> onCallback)
    {
        var databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var userReference = databaseReference.Child("users").Child(userId).Child(childName);

        return userReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                DataSnapshot snapshot = task.Result;
                foreach (var child in snapshot.Children)
                {
                    data.Add(child.Key, child.Value);
                }
                onCallback.Invoke(data);
            }
        });
    }

    public Task getID(Action<bool> onCallback)
    {
        var databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var userReference = databaseReference.Child("users").Child(userId);

        return userReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                    onCallback.Invoke(false);
                else
                    onCallback.Invoke(true);
            }
        });
    }

    public Task getUsername(Action<string> onCallback)
    {
        return GetData("name", onCallback);
    }

    public Task getRemoveAds(Action<bool> onCallback)
    {
        return GetData("removeAds", value => onCallback.Invoke(bool.Parse(value)));
    }

    public Task getCoins(Action<int> onCallback)
    {
        return GetData("coins", value => onCallback.Invoke(int.Parse(value)));
    }

    public Task getGems(Action<int> onCallback)
    {
        return GetData("gems", value => onCallback.Invoke(int.Parse(value)));
    }

    public Task getHighscore(Action<int> onCallback)
    {
        return GetData("highscore", value => onCallback.Invoke(int.Parse(value)));
    }

    public Task getGift(Action<string> onCallback)
    {
        return GetData("gift", onCallback);
    }

    public Task getSelectedShoe(Action<string> onCallback)
    {
        return GetData("selectedShoe", onCallback);
    }

    public Task getSelectedBall(Action<string> onCallback)
    {
        return GetData("selectedBall", onCallback);
    }

    public Task getShoes(Action<Dictionary<string, object>> onCallback)
    {
        return GetDictionaryData("shoes", onCallback);
    }

    public Task getBalls(Action<Dictionary<string, object>> onCallback)
    {
        return GetDictionaryData("balls", onCallback);
    }

    public Task getBoxes(Action<Dictionary<string, object>> onCallback)
    {
        return GetDictionaryData("boxes", onCallback);
    }

    #endregion

    public async Task<PlayerData> loadPlayerDataFromDatabaseIE()
    {
        playerData.userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        playerData.email = FirebaseAuth.DefaultInstance.CurrentUser.Email;
        await getUsername((val) => { playerData.name = val; playerData.playerAttributesData.name = val; }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getRemoveAds((val) => playerData.playerAttributesData.removeAds = val).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getCoins((val) => { playerData.playerAttributesData.coins = val; }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getGems((val) => { playerData.playerAttributesData.gems = val; }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getHighscore((val) => { playerData.playerAttributesData.highscore = val; }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getGift((val) => { playerData.playerAttributesData.gift = val; }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getSelectedShoe((val) => { playerData.playerAttributesData.selectedShoe = val; }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getSelectedBall((val) => { playerData.playerAttributesData.selectedBall = val; }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getShoes((val) =>
        {
            playerData.playerAttributesData.shoes = new Dictionary<string, string>();
            foreach (var shoe in val)
            {
                playerData.playerAttributesData.shoes[shoe.Key] = (string)shoe.Value;
            }
        }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getBalls((val) =>
        {
            playerData.playerAttributesData.balls = new Dictionary<string, string>();
            foreach (var ball in val)
            {
                playerData.playerAttributesData.balls[ball.Key] = (string)ball.Value;
            }
        }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });
        await getBoxes((val) =>
        {
            playerData.playerAttributesData.boxes = new Dictionary<string, string>();
            foreach (var box in val)
            {
                playerData.playerAttributesData.boxes[box.Key] = (string)box.Value;
            }
        }).ContinueWithOnMainThread(task => { if (task.IsCompleted) { fileHandler.Save(playerData); } });

        return playerData;
    }

    #region Set user items
    public async void setUsername(string name)
    {
        playerData.name = name;
        fileHandler.Save(playerData);
        if(!playerPrefs.offline)
            await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("name").SetValueAsync(name);
    }

    public async void setRemoveAds(bool val)
    {
        playerData.playerAttributesData.removeAds = val;
        fileHandler.Save(playerData);
        if (!playerPrefs.offline)
            await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("removeAds").SetValueAsync(val);
    }

    public async void setCoins(int coins)
    {
        playerData.playerAttributesData.coins = coins;
        fileHandler.Save(playerData);
        if (!playerPrefs.offline)
            await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("coins").SetValueAsync(coins);
    }

    public async void setGems(int gems)
    {
        playerData.playerAttributesData.gems = gems;
        fileHandler.Save(playerData);
        if (!playerPrefs.offline)
            await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("gems").SetValueAsync(gems);
    }

    public async void setHignscore(int highscore)
    {
        playerData.playerAttributesData.highscore = highscore;
        fileHandler.Save(playerData);
        if (!playerPrefs.offline)
            await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("highscore").SetValueAsync(highscore);
    }

    public async void setSelectedShoe(string shoe)
    {
        playerData.playerAttributesData.selectedShoe = shoe;
        fileHandler.Save(playerData);
        if (!playerPrefs.offline)
            await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("selectedShoe").SetValueAsync(shoe);
    }

    public async void setSelectedBall(string ball)
    {
        playerData.playerAttributesData.selectedBall = ball;
        fileHandler.Save(playerData);
        if (!playerPrefs.offline)
            await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("selectedBall").SetValueAsync(ball);
    }

    public async void setGift(string time)
    {
        playerData.playerAttributesData.gift = time;
        fileHandler.Save(playerData);
        if (!playerPrefs.offline)
            await databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("gift").SetValueAsync(time);
    }
    #endregion
}
