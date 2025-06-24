using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;

public class LoadBoxes : MonoBehaviour
{
    [SerializeField] private ShopNavigation shopNavigation;
    [SerializeField] private List<BoxScriptable> boxes;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip anim;
    [SerializeField] private GameObject touchToUnlockPanel, boxItems, rarity, backBtn, openBoxOverlay;
    [SerializeField] private Button prev, next;
    [SerializeField] private AudioClip pop;

    private int boxCount;
    private bool opening = false;

    void Update()
    {
        if (!opening)
            shopNavigation.box.transform.rotation = Quaternion.Euler(Mathf.Sin(Time.fixedTime * 8) * 10 - 20, 0, Mathf.Sin(Time.fixedTime * 4) * 10);
    }

    public void loadBox(int index)
    {
        opening = false;
        boxCount = 0;
        rarity.SetActive(false);
        boxes = new List<BoxScriptable>();
        Resources.LoadAll<BoxScriptable>("Scriptable Objects/Boxes").ToList().ForEach(iBox => boxes.Add(iBox));
        boxes.Sort((x, y) => x.id.CompareTo(y.id));

        BoxScriptable box = boxes[index];
        shopNavigation.boxName.text = box.displayName;
        shopNavigation.boxName.color = new Color32(255, 255, 255, 255);
        shopNavigation.box.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = box.lidTexture;
        shopNavigation.box.transform.GetChild(1).GetComponent<MeshRenderer>().material.mainTexture = box.boxTexture;
        shopNavigation.coinImg.SetActive(true);
        shopNavigation.openButton.SetActive(false);
        shopNavigation.openButton.SetActive(false);
        shopNavigation.openButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "OPEN";
        shopNavigation.coinImg.GetComponent<Image>().sprite = shopNavigation.coinSprite;
        shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().text = box.cost.ToString();
        shopNavigation.itemPrice.transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(40, 0);
        shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        prev.onClick.RemoveAllListeners();
        next.onClick.RemoveAllListeners();
        prev.onClick.AddListener(() => prevBox());
        next.onClick.AddListener(() => nextBox());

        for (int i = 0; i < box.rarities.Count; i++)
        {
            shopNavigation.boxRarities[i].text = box.rarities[i].ToString("0.0") + "%";
        }

        StopAllCoroutines();
        DateTime date = new DateTime();
        switch (box.type)
        {
            case BoxType.Free:
                date = Convert.ToDateTime(shopNavigation.databaseManager.playerData.playerAttributesData.boxes[box.uid].Split(":")[2].Replace(";", ":")).AddHours(24);
                shopNavigation.itemPrice.SetActive(false);

                if ((date - DateTime.UtcNow).TotalMilliseconds < 0)
                {
                    boxCount = 1;
                    shopNavigation.openButton.SetActive(true);
                    shopNavigation.openButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    shopNavigation.openButton.GetComponent<Button>().onClick.AddListener(() => openBox(box));
                    shopNavigation.openButton.GetComponent<Image>().color = new Color32(190, 40, 70, 255);

                    shopNavigation.boxDesc.text = "Ready!";
                }
                else
                {
                    shopNavigation.openButton.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
                    StartCoroutine(updateTime(date, 24, box));
                }

                break;
            case BoxType.Ad:
                date = Convert.ToDateTime(shopNavigation.databaseManager.playerData.playerAttributesData.boxes[box.uid].Split(":")[3].Replace(";", ":")).AddHours(12);
                boxCount = int.Parse(shopNavigation.databaseManager.playerData.playerAttributesData.boxes[box.uid].Split(":")[2]);
                shopNavigation.itemPrice.SetActive(false);

                if (boxCount > 0 & (date - DateTime.UtcNow).TotalMilliseconds < 0)
                {
                    shopNavigation.openButton.SetActive(true);
                    shopNavigation.openButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    shopNavigation.openButton.GetComponent<Button>().onClick.AddListener(() => openBox(box));
                    shopNavigation.openButton.GetComponent<Image>().color = new Color32(190, 40, 70, 255);

                    shopNavigation.boxDesc.text = boxCount.ToString() + " boxes";
                }
                else
                {
                    shopNavigation.openButton.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
                    StartCoroutine(updateTime(date, 12, box));
                }

                break;
            case BoxType.Rarity:
                boxCount = 0;
                shopNavigation.itemPrice.SetActive(true);
                if (shopNavigation.databaseManager.playerData.playerAttributesData.boxes.ContainsKey(box.uid))
                {
                    if (checkBoxItemsAvailable(box).Count != 0)
                    {
                        boxCount = int.Parse(shopNavigation.databaseManager.playerData.playerAttributesData.boxes[box.uid].Split(":")[2]);
                        shopNavigation.openButton.SetActive(true);
                        shopNavigation.openButton.GetComponent<Image>().color = new Color32(190, 40, 70, 255);
                        shopNavigation.openButton.GetComponent<Button>().onClick.RemoveAllListeners();
                        //openBoxBtn.GetComponent<Button>().onClick.AddListener(() => uiConsistency.uISounds.pressButton());
                        shopNavigation.openButton.GetComponent<Button>().onClick.AddListener(() => openBox(box));
                    }
                    else
                    {
                        shopNavigation.boxDesc.text = "box unavailable";
                    }
                }
                else
                {
                    shopNavigation.openButton.SetActive(false);
                    shopNavigation.openButton.GetComponent<Button>().onClick.RemoveAllListeners();
                }

                shopNavigation.boxDesc.text = boxCount.ToString() + " boxes";
                buyBox(box);
                break;
        }
    }

    public IEnumerator updateTime(DateTime openedTime, int timeDisparity, BoxScriptable box)
    {
        string formatedDate;
        formatedDate = (openedTime - DateTime.UtcNow).ToString("hh':'mm':'ss''");
        formatedDate = ((openedTime - DateTime.UtcNow).TotalHours <= 1) ? string.Format("{0}m {1}s", formatedDate.Split(":")[1], formatedDate.Split(":")[2]) : string.Format("{0}h {1}m {2}s", formatedDate.Split(":")[0], formatedDate.Split(":")[1], formatedDate.Split(":")[2]);
        formatedDate = ((openedTime - DateTime.UtcNow).TotalHours >= timeDisparity | (openedTime - DateTime.UtcNow).TotalHours <= 0) ? "Ready" : formatedDate;
        shopNavigation.boxDesc.text = formatedDate;

        if (formatedDate == "Ready")
        {
            loadBox(box.id);
            yield break;
        }

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(updateTime(openedTime, timeDisparity, box));
    }

    public void buyBox(BoxScriptable box)
    {
        shopNavigation.itemPrice.GetComponent<Button>().onClick.RemoveAllListeners();

        if (box.cost <= shopNavigation.databaseManager.playerData.playerAttributesData.coins)
        {
            shopNavigation.itemPrice.GetComponent<Image>().color = new Color32(255, 135, 0, 255);
            shopNavigation.itemPrice.GetComponent<Button>().onClick.AddListener(() =>
            {
                shopNavigation.openLoading();
                shopNavigation.uISoundFX.playCoinChange();
                shopNavigation.databaseManager.updateUserCollection("boxes", new Dictionary<string, object>()
                {
                    [box.uid] = box.path + ":" + box.id + ":" + (boxCount + 1)
                });

                loadBox(box.id);
                shopNavigation.closeLoading();
                shopNavigation.takeCoins((int)box.cost);
            });
        }
        else
            shopNavigation.itemPrice.GetComponent<Image>().color = new Color32(135, 135, 135, 255);

        shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().text = box.cost.ToString();
    }

    public void openBox(BoxScriptable box)
    {
        shopNavigation.openLoading();
        switch (box.type)
        {
            case BoxType.Free:
                shopNavigation.databaseManager.updateUserCollection("boxes", new Dictionary<string, object>
                {
                    [box.uid] = box.path + ":" + box.id + ":" + DateTime.UtcNow.ToString("MM-dd-yyyy HH;mm;ss"),
                });
                openBoxFunction(box);


                AndroidNotificationCenter.CancelNotification(shopNavigation.notificationID);
                shopNavigation.notification.FireTime = System.DateTime.UtcNow.AddHours(24);

                shopNavigation.notificationID = AndroidNotificationCenter.SendNotification(shopNavigation.notification, "channel_id");

                var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(shopNavigation.notificationID);


                if (notificationStatus == NotificationStatus.Scheduled)
                {
                    // Replace the scheduled notification with a new notification.
                    AndroidNotificationCenter.UpdateScheduledNotification(shopNavigation.notificationID, shopNavigation.notification, "channel_id");
                }
                else if (notificationStatus == NotificationStatus.Delivered)
                {
                    // Remove the previously shown notification from the status bar.
                    AndroidNotificationCenter.CancelNotification(shopNavigation.notificationID);
                }
                else if (notificationStatus == NotificationStatus.Unknown)
                {
                    AndroidNotificationCenter.SendNotification(shopNavigation.notification, "channel_id");
                }

                break;
            case BoxType.Ad:
                if (boxCount - 1 > 0)
                    shopNavigation.databaseManager.updateUserCollection("boxes", new Dictionary<string, object>
                    {
                        [box.uid] = box.path + ":" + box.id + ":" + (boxCount - 1) + ":" + shopNavigation.databaseManager.playerData.playerAttributesData.boxes[box.uid].Split(":")[3]
                    });
                else
                    shopNavigation.databaseManager.updateUserCollection("boxes", new Dictionary<string, object>
                    {
                        [box.uid] = box.path + ":" + box.id + ":3:" + DateTime.UtcNow.ToString("MM-dd-yyyy HH;mm;ss"),
                    });
                openBoxOverlay.SetActive(true);
                shopNavigation.rewardedAd.LoadAd(() => { }, () => shopNavigation.closeLoading(), () => { });
                break;
            case BoxType.Rarity or BoxType.Branded:
                if (boxCount > 1)
                    shopNavigation.databaseManager.updateUserCollection("boxes", new Dictionary<string, object>
                    {
                        [box.uid] = box.path + ":" + box.id + ":" + (boxCount - 1)
                    });
                else
                    shopNavigation.databaseManager.removeUserCollectionItem("boxes", new Dictionary<string, object>(), box.uid);
                openBoxFunction(box);
                break;
        }
    }

    int run = 0, tempRun = 0;
    private List<IScriptableObject> itemsOpened;
    private int itemOn;
    public void openBoxFunction(BoxScriptable box)
    {
        openBoxOverlay.SetActive(false);
        shopNavigation.uISoundFX.playOpenBox();
        opening = true;

        prev.gameObject.SetActive(false);
        next.gameObject.SetActive(false);
        shopNavigation.box.LeanMoveLocalY(-1.8f, 0.25f);
        shopNavigation.box.LeanRotate(new Vector3(0, 0, 0), 0.25f);

        shopNavigation.boxName.gameObject.SetActive(false);
        shopNavigation.boxDesc.gameObject.SetActive(false);
        touchToUnlockPanel.SetActive(true);
        shopNavigation.itemPrice.SetActive(false);
        shopNavigation.openButton.SetActive(false);

        tempRun = Mathf.Min(UnityEngine.Random.Range(2, box.rangeUpper), checkBoxItemsAvailable(box).Count);
        run = tempRun;
        itemsOpened = new List<IScriptableObject>();

        touchToUnlockPanel.GetComponent<Button>().onClick.RemoveAllListeners();
        touchToUnlockPanel.GetComponent<Button>().onClick.AddListener(() => getNewItem(box));

        shopNavigation.closeLoading();
    }

    public async void displayItem(IScriptableObject item)
    {
        LeanTween.value(touchToUnlockPanel, new Color(0, 0, 0, 0.3f), new Color(0, 0, 0, 0), 0.15f).setOnUpdate(val =>
        {
            touchToUnlockPanel.GetComponent<Image>().color = val;
        });
        await Task.Delay(150);
        touchToUnlockPanel.transform.GetChild(0).gameObject.SetActive(false);

        if (run == tempRun)
        {
            animator.enabled = true;
            animator.Play("Box Opening");
            await Task.Delay((int)(anim.length - 0.1f) * 1000);
            animator.enabled = false;
            shopNavigation.box.transform.GetChild(0).gameObject.SetActive(false);
            boxItems.SetActive(true);
        }
        shopNavigation.uISoundFX.playNextItem();

        itemInfo(item);

        if (boxItems.transform.childCount > 0)
        {
            boxItems.gameObject.LeanMoveLocalX(boxItems.transform.localPosition.x - 2, 0.25f);
        }

        GameObject newItem = Instantiate(item.prefab, boxItems.transform);

        if (item.GetType() == typeof(ShoeScriptable))
            newItem.transform.localScale = new Vector3(30, 30, 30);
        else if (item.GetType() == typeof(BallScriptable))
            newItem.transform.localScale = new Vector3(50, 50, shopNavigation.shoeSide * 50);
        else if (item.GetType() == typeof(CoinScriptable) | item.GetType() == typeof(GemScriptable))
            newItem.transform.localScale = new Vector3(30, 30, 30);

        //SoundFXSource.clip = nextItemClip;
        //SoundFXSource.Play();

        run--;
        newItem.transform.localPosition = new Vector3((boxItems.transform.childCount - 1) * 2, 0);
        LeanTween.moveLocal(newItem, new Vector3((boxItems.transform.childCount - 1) * 2, 0), 0.5f).setEaseOutBack();

        await Task.Delay(500);
        touchToUnlockPanel.transform.GetChild(0).gameObject.SetActive(true);

        LeanTween.value(touchToUnlockPanel, new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.3f), 0.15f).setOnUpdate(val =>
        {
            touchToUnlockPanel.GetComponent<Image>().color = val;
        });
    }

    public void getNewItem(BoxScriptable box)
    {
        if (run > 0)
        {
            backBtn.SetActive(false);
            List<IScriptableObject> tempList = checkBoxItemsAvailable(box);
            List<IScriptableObject> newItems = new List<IScriptableObject>();

            tempList.ForEach((item) =>
            {
                if (!shopNavigation.ownedItems.Contains(item.uid))
                    newItems.Add(item);
            });

            if (newItems.Count > 0)
            {
                IScriptableObject newItem = newItems[UnityEngine.Random.Range(0, newItems.Count)];
                displayItem(newItem);

                if (newItem.GetType() == typeof(ShoeScriptable))
                {
                    shopNavigation.databaseManager.updateUserCollection("shoes", new Dictionary<string, object>()
                    {
                        [newItem.uid] = newItem.path + ":" + newItem.id
                    });
                    shopNavigation.ownedItems.Add(newItem.uid);
                }
                else if (newItem.GetType() == typeof(BallScriptable))
                {
                    shopNavigation.databaseManager.updateUserCollection("balls", new Dictionary<string, object>()
                    {
                        [newItem.uid] = newItem.path + ":" + newItem.id
                    });
                    shopNavigation.ownedItems.Add(newItem.uid);
                }
                else if (newItem.GetType() == typeof(CoinScriptable))
                {
                    shopNavigation.addCoins(((CoinScriptable)newItem).noCoins, 1);
                }
                else if (newItem.GetType() == typeof(GemScriptable))
                {
                    shopNavigation.addGems(((GemScriptable)newItem).noCoins, 1);
                }

                itemsOpened.Add(newItem);
            }
            else
            {
                run--;
                getNewItem(box);
            }
        }
        else if (run == 0)
        {
            touchToUnlockPanel.SetActive(false);

            Quaternion lidRotation = Quaternion.identity;
            lidRotation.eulerAngles = new Vector3(-90, 0, 0);
            shopNavigation.box.transform.GetChild(0).gameObject.SetActive(true);
            shopNavigation.box.transform.GetChild(0).localRotation = lidRotation;
            shopNavigation.box.transform.GetChild(0).localPosition = new Vector3(0, 0.8f, 0);
            LeanTween.scale(shopNavigation.box.gameObject, new Vector3(0, 0, 0), 0.5f).setEaseInBack();
            itemOn = 0;
            itemInfo(itemsOpened[itemOn]);
            itemActions(itemsOpened[itemOn]);

            prev.gameObject.SetActive(true);
            next.gameObject.SetActive(true);

            prev.onClick.RemoveAllListeners();
            next.onClick.RemoveAllListeners();
            prev.onClick.AddListener(() => prevItem());
            next.onClick.AddListener(() => nextItem());

            shopNavigation.coinImg.SetActive(false);
            shopNavigation.openButton.SetActive(true);
            shopNavigation.openButton.GetComponent<Image>().color = new Color32(255, 135, 0, 255);
            shopNavigation.openButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "DONE";
            shopNavigation.openButton.transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            shopNavigation.openButton.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
            shopNavigation.openButton.GetComponent<Button>().onClick.RemoveAllListeners();
            shopNavigation.openButton.GetComponent<Button>().onClick.AddListener(() => viewingDone(box));

            LeanTween.moveLocalX(boxItems, 0, 0.5f).setEaseOutExpo();
        }
    }

    public async void viewingDone(BoxScriptable box)
    {
        foreach (Transform child in boxItems.transform)
        {
            shopNavigation.uISoundFX.m_AudioSource.clip = pop;
            shopNavigation.uISoundFX.m_AudioSource.Play();
            LeanTween.scale(child.gameObject, new Vector3(0, 0, 0), 0.5f).setEaseInBack();
            await Task.Delay(125);
        }

        await Task.Delay((boxItems.transform.childCount * 125) + 500);
        LeanTween.color(shopNavigation.shadow.gameObject, Color.white, 0.15f);
        LeanTween.scale(shopNavigation.box.gameObject, new Vector3(0.5f, 0.5f, 0.5f), 0.5f).setEaseOutBack();
        shopNavigation.box.transform.localPosition = new Vector3(0, -0.8f, 0);
        backBtn.SetActive(true);
        boxItems.transform.localPosition = new Vector3(0,-0.8f,0);

        shopNavigation.uISoundFX.m_AudioSource.clip = pop;
        shopNavigation.uISoundFX.m_AudioSource.Play();
        foreach (Transform child in boxItems.transform)
            Destroy(child.gameObject);
        loadBox(box.id);
    }

    public void itemInfo(IScriptableObject item)
    {
        shopNavigation.boxName.gameObject.SetActive(true);
        shopNavigation.boxDesc.gameObject.SetActive(true);
        shopNavigation.boxName.text = item.displayName;
        shopNavigation.boxDesc.text = item.colour;
        rarity.SetActive(true);
        rarity.GetComponent<TMP_Text>().text = item.rarity.ToString();

        LeanTween.color(shopNavigation.glow.gameObject, shopNavigation.rarityColours[(int)item.rarity], 0.1f);
    }

    public void itemActions(IScriptableObject item)
    {
        shopNavigation.itemPrice.SetActive(false);
        shopNavigation.itemPrice.transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        shopNavigation.itemPrice.GetComponent<Button>().onClick.RemoveAllListeners();
        if ((item.path + ":" + item.id) == shopNavigation.selectedShoe | (item.path + ":" + item.id) == shopNavigation.selectedBall)
        {
            shopNavigation.itemPrice.SetActive(true);
            shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().text = "Selected";
            shopNavigation.itemPrice.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
        }
        else
        {
            shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().text = "Select";
            shopNavigation.itemPrice.GetComponent<Image>().color = new Color32(0, 153, 153, 255);
            shopNavigation.itemPrice.GetComponent<Button>().onClick.RemoveAllListeners();

            if (item.GetType() == typeof(ShoeScriptable))
            {
                shopNavigation.itemPrice.SetActive(true);
                shopNavigation.itemPrice.GetComponent<Button>().onClick.AddListener(() => shopNavigation.loadItems.selectShoe(item));
            }
            if (item.GetType() == typeof(BallScriptable))
            {
                shopNavigation.itemPrice.SetActive(true);
                shopNavigation.itemPrice.GetComponent<Button>().onClick.AddListener(() => shopNavigation.loadItems.selectBall(item));
            }
        }
    }

    public void prevItem()
    {
        shopNavigation.uISoundFX.pressButton();
        if (itemOn != 0)
        {
            LeanTween.moveLocalX(boxItems, boxItems.transform.localPosition.x + 2, 0.5f).setEaseOutExpo();
            itemOn--;
            itemInfo(itemsOpened[itemOn]);
            itemActions(itemsOpened[itemOn]);
        }
    }

    public void nextItem()
    {
        shopNavigation.uISoundFX.pressButton();
        if (itemOn != itemsOpened.Count - 1)
        {
            LeanTween.moveLocalX(boxItems, boxItems.transform.localPosition.x - 2, 0.5f).setEaseOutExpo();
            itemOn++;
            itemInfo(itemsOpened[itemOn]);
            itemActions(itemsOpened[itemOn]);
        }
    }

    public void nextBox()
    {
        shopNavigation.uISoundFX.changeMenu();
        shopNavigation.boxesIndex++;
        if (shopNavigation.boxesIndex > boxes.Count - 1)
            shopNavigation.boxesIndex = 0;

        loadBox(shopNavigation.boxesIndex);
    }

    public void prevBox()
    {
        shopNavigation.uISoundFX.changeMenu();
        shopNavigation.boxesIndex--;
        if (shopNavigation.boxesIndex < 0)
            shopNavigation.boxesIndex = boxes.Count - 1;
        loadBox(shopNavigation.boxesIndex);
    }


    public List<IScriptableObject> getItems(ItemType type)
    {
        // Load all scriptable objects of type IScriptableObject from the "Scriptable Objects" folder
        object[] loadedObjects = Resources.LoadAll("Scriptable Objects", typeof(IScriptableObject));
        List<IScriptableObject> items = new List<IScriptableObject>();

        // Filter objects based on the provided type
        foreach (object obj in loadedObjects)
        {
            if (obj.GetType().ToString() == type.ToString())
                items.Add((IScriptableObject)obj);
        }

        return items;
    }

    public List<IScriptableObject> getItemsByRarity(ItemType type, BoxScriptable box, int ttl)
    {
        List<IScriptableObject> items = new List<IScriptableObject>();

        // Generate a weighted list for random rarity selection
        WeightedList<Rarity, float> randomRarity = generateRandomRarityList(box.rarities);

        // Randomly select a rarity
        Rarity rarity = randomRarity.getRandomItem();

        // Filter items based on the selected rarity
        foreach (IScriptableObject item in getItems(type))
            if (item.rarity == rarity)
                items.Add(item);

        // If no items found for the specified rarity and rarity is not the lowest, try with a lower rarity
        if (items.Count == 0 && ttl != 0)
            items = getItemsByRarity(type, box, ttl - 1);

        return items;
    }

    public List<IScriptableObject> getShoesByBrand(string brand)
    {
        // Retrieve scriptable objects of type ShoeScriptable and filter by brand
        return getItems(ItemType.ShoeScriptable)
            .Where(shoe => shoe.path.Split(":")[0] == brand)
            .ToList();
    }

    public List<IScriptableObject> getBallsByBrand(string brand)
    {
        // Retrieve scriptable objects of type BallScriptable and filter by brand
        return getItems(ItemType.BallScriptable)
            .Where(ball => ball.path.Split(":")[0] == brand)
            .ToList();
    }

    public List<IScriptableObject> getItemsByMaxCost(ItemType type, float cost)
    {
        // Retrieve scriptable objects of the specified type and filter by cost
        return getItems(type)
            .Where(item => item.cost <= cost)
            .ToList();
    }

    private WeightedList<Rarity, float> generateRandomRarityList(List<float> rarities)
    {
        // Generate a weighted list for random rarity selection
        WeightedList<Rarity, float> randomRarity = new WeightedList<Rarity, float>();

        for (int i = 0; i < rarities.Count; i++)
            randomRarity.add((Rarity)i, rarities[i]);

        return randomRarity;
    }

    private List<IScriptableObject> checkBoxItemsAvailable(BoxScriptable box)
    {
        List<IScriptableObject> itemList = new List<IScriptableObject>();

        switch (box.type)
        {
            case BoxType.Ad or BoxType.Free:
                getItemsByRarity(ItemType.ShoeScriptable, box, 6).ForEach((item) => { if (!shopNavigation.ownedItems.Contains(item.uid)) itemList.Add(item); });
                getItemsByRarity(ItemType.BallScriptable, box, 6).ForEach((item) => { if (!shopNavigation.ownedItems.Contains(item.uid)) itemList.Add(item); });
                break;
            case BoxType.Rarity:
                getItemsByRarity(ItemType.ShoeScriptable, box, 6).ForEach((item) => { if (!shopNavigation.ownedItems.Contains(item.uid)) itemList.Add(item); });
                getItemsByRarity(ItemType.BallScriptable, box, 6).ForEach((item) => { if (!shopNavigation.ownedItems.Contains(item.uid)) itemList.Add(item); });
                break;
            case BoxType.Branded:
                getShoesByBrand(box.brand).ForEach((item) => { if (!shopNavigation.ownedItems.Contains(item.uid)) itemList.Add(item); });
                getBallsByBrand(box.brand).ForEach((item) => { if (!shopNavigation.ownedItems.Contains(item.uid)) itemList.Add(item); });
                break;
            default:
                itemList = new List<IScriptableObject>();
                break;
        }
        itemList.Add(getItemsByRarity(ItemType.CoinScriptable, box, 6)[0]);
        itemList.Add(getItemsByRarity(ItemType.GemScriptable, box, 6)[0]);

        return itemList;
    }
}
