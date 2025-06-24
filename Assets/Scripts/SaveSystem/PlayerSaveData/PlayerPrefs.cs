[System.Serializable]
public class PlayerPref
{
    public float music, soundFX;
    public bool loggedIn, anonymous, offline;
    public int shoeSide;

    public PlayerPref()
    {
        music = 0.5f;
        soundFX = 0.5f;
        loggedIn = false;
        anonymous = false;
        offline = true;
        shoeSide = 1;
    }
}
