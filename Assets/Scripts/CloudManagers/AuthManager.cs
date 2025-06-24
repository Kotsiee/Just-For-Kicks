using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using GooglePlayGames;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    public DependencyStatus status;
    public FirebaseAuth auth;
    public FirebaseDatabase databaseRef;
    public FirebaseUser user;
    //public PlayGamesLocalUser PGUser;

    public DatabaseManager databaseManager;

    public bool LoginComplete = false, regComplete = false;

    public void LogOut()
    {
        if (user != null)
        {
            user = null;

            FirebaseAuth.DefaultInstance.SignOut();
            auth = FirebaseAuth.DefaultInstance;
        }

        databaseManager.playerPrefs.loggedIn = false;
        databaseManager.playerPrefsFileHander.Save(databaseManager.playerPrefs);
        SceneManager.LoadScene("Login");
    }

    public void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        databaseManager.databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void deleteUser()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            user.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("User deleted successfully.");
                    databaseManager.databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetValueAsync(null);
                }
            });
        }
    }

    public void LoginWithEmail(string _email, string _password, Action<AuthError> onCallback, Action<FirebaseUser> getUser)
    {
        auth.SignInWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(loginTask =>
        {
            if (loginTask.IsFaulted)
            {
                FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
                onCallback.Invoke((AuthError)firebaseEx.ErrorCode);
            }

            if (loginTask.IsCompleted)
            {
                AuthResult result = loginTask.Result;

                user = result.User;
                getUser.Invoke(user);
            }
        });
    }



    public async Task LoginAnonymously()
    {
        // Assuming LoginMenu and auth are class variables

        await auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(signInTask =>
        {
            if (signInTask.IsCompleted)
            {
                AuthResult result = signInTask.Result;
                user = result.User;

                databaseManager.getID(exists =>
                {
                    Debug.Log(exists);
                    if (!exists)
                        databaseManager.addUser("");
                });
            }
        });

        await databaseManager.loadPlayerDataFromDatabaseIE();

        return;
    }

    public void RegisterFromAnonymous(string _username, string _email, string _password, Action<AuthError> onCallback, Action<FirebaseUser> getUser)
    {
        Credential credential = EmailAuthProvider.GetCredential(_email, _password);

        auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWithOnMainThread(registerTask =>
        {
            if (registerTask.IsFaulted)
            {
                FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
                onCallback.Invoke((AuthError)firebaseEx.ErrorCode);
            }

            if (registerTask.IsCompleted)
            {
                AuthResult result = registerTask.Result;
                Debug.LogFormat("User registered successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

                user = result.User;
                getUser.Invoke(user);

                if (user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(profileTask =>
                    {
                        if (profileTask.IsFaulted)
                        {
                            FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
                            onCallback.Invoke((AuthError)firebaseEx.ErrorCode);
                        }

                        if (profileTask.IsCompleted)
                        {
                            regComplete = true;
                            databaseManager.setUsername(_username);
                        }
                    });
                }
            }
        });
    }


    public void RegisterWithEmail(string _username, string _email, string _password, Action<AuthError> onCallback, Action<FirebaseUser> getUser)
    {
        auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(registerTask =>
        {
            if (registerTask.IsFaulted)
            {
                FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
                onCallback.Invoke((AuthError)firebaseEx.ErrorCode);
            }

            if (registerTask.IsCompleted)
            {
                AuthResult result = registerTask.Result;
                Debug.LogFormat("User registered successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

                user = result.User;
                getUser.Invoke(user);

                if (user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(profileTask =>
                    {
                        if (profileTask.IsFaulted)
                        {
                            FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
                            onCallback.Invoke((AuthError)firebaseEx.ErrorCode);
                        }

                        if (profileTask.IsCompleted)
                        {
                            regComplete = true;
                            databaseManager.addUser(_username);
                        }
                    });
                }
            }
        });
    }


    public Task resetPassword(string _email, Action<AuthError> onCallback)
    {
        return auth.SendPasswordResetEmailAsync(_email).ContinueWithOnMainThread(resetPasswordTask =>
        {
            if (resetPasswordTask.IsFaulted)
            {
                FirebaseException firebaseEx = resetPasswordTask.Exception.GetBaseException() as FirebaseException;
                onCallback.Invoke((AuthError)firebaseEx.ErrorCode);
            }

            if (resetPasswordTask.IsCompleted)
            {
                onCallback.Invoke(AuthError.None);
            }
        });
    }

}
