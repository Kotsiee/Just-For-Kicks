using Firebase.Auth;
using UnityEngine;

[CreateAssetMenu]
public class DataPersistance : ScriptableObject
{
    private FirebaseUser _user;
    public FirebaseUser user
    {
        get { return _user; }
        set { _user = value; }
    }

    private bool _IsLogged;
    public bool IsLogged
    {
        get { return _IsLogged; }
        set { _IsLogged = value; }
    }

    private ShoeScriptable _shoe;
    public ShoeScriptable selectedShoe
    {
        get { return _shoe; }
        set { _shoe = value; }
    }

    private BallScriptable _ball;
    public BallScriptable selectedBall
    {
        get { return _ball; }
        set { _ball = value; }
    }

    private float _music;
    public float music
    {
        get { return _music; }
        set { _music = value; }
    }

    private float _soundFX;
    public float soundFX
    {
        get { return _soundFX; }
        set { _soundFX = value; }
    }
}
