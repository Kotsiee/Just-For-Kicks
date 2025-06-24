[System.Serializable]
public class PlayerData
{
    public string userID, name, email;
    public User playerAttributesData;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public PlayerData()
    {
        playerAttributesData = new User(name);
    }
}
