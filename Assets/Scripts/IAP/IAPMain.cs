using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

public class IAPMain : MonoBehaviour, IDetailedStoreListener
{
    /*
     * add coins - consumable
     * add gems - consumable
     * remove ads - non-consumable
     */

    public IAPItems cItem;
    public List<IAPItems> cItems;
    public nonConsumable nonConsumable;
    public List<TMP_Text> costs;
    public TMP_Text coinsTxt, gemsTxt;

    IStoreController storeController;

    public Data data;
    public Payload payload;
    public PayloadData payloadData;

    public bool removeAds;
    public DatabaseManager databaseManager;
    public GameObject removeAdsButton;

    private int coins, gems;

    private void Start()
    {
        setupBuilder();
        removeAds = (databaseManager.playerData.playerAttributesData.removeAds) ? true : CheckNonConsumable(nonConsumable.GPID);

        coins = databaseManager.playerData.playerAttributesData.coins;
        gems = databaseManager.playerData.playerAttributesData.gems;

        if (!removeAds)
        {
            removeAdsButton.GetComponent<Image>().color = Color.white;
            removeAdsButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            removeAdsButton.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
            removeAdsButton.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.white;
        }
        else
        {
            removeAdsButton.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
            removeAdsButton.transform.GetChild(0).GetComponent<Image>().color = new Color32(135, 135, 135, 255);
            removeAdsButton.transform.GetChild(1).GetComponent<TMP_Text>().color = new Color32(135, 135, 135, 255);
            removeAdsButton.transform.GetChild(2).GetComponent<TMP_Text>().color = new Color32(135, 135, 135, 255);
        }
    }

    public void buyCoins(Consumable coin)
    {
        cItem = coin;
        storeController.InitiatePurchase(coin.GPID);
    }

    public void removeAdsBtn()
    {
        if (!removeAds)
        {
            cItem = new nonConsumable();
            storeController.InitiatePurchase(nonConsumable.GPID);
        }
    }

    #region IAP Controllers
    //Process purchase
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        if (product.definition.id == cItem.GPID)
        {
            try
            {
                string receipt = product.receipt;
                data = JsonUtility.FromJson<Data>(receipt);
                payload = JsonUtility.FromJson<Payload>(data.Payload);
                payloadData = JsonUtility.FromJson<PayloadData>(payload.json);

                int quantity = payloadData.quantity;
                Consumable consumable = (Consumable)cItem;
                addCoins(consumable.noCoins);
                addGems(consumable.noGems);
            }
            catch
            {
                Consumable consumable = (Consumable)cItem;
                addCoins(consumable.noCoins);
                addGems(consumable.noGems);
            }
        }
        else if (product.definition.id == nonConsumable.GPID)
        {
            removeAds = true;
            databaseManager.setRemoveAds(true);
            if (!removeAds)
            {
                removeAdsButton.GetComponent<Image>().color = new Color32(90, 185, 64, 255);
                removeAdsButton.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color32(90, 185, 64, 255);
            }
            else
            {
                removeAdsButton.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
                removeAdsButton.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color32(135, 135, 135, 255);
            }
        }

        return PurchaseProcessingResult.Complete;
    }

    void setupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        cItems = new List<IAPItems>();
        Resources.LoadAll<Consumable>("Scriptable Objects/IAP").ToList().ForEach(iCoins => cItems.Add(iCoins));

        cItems.ForEach(cItem =>
        {
            builder.AddProduct(cItem.GPID, ProductType.Consumable);
        });

        builder.AddProduct(nonConsumable.GPID, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    bool CheckNonConsumable(string id)
    {
        if (storeController != null)
        {
            var product = storeController.products.WithID(id);
            if (product != null)
            {
                if (product.hasReceipt)//purchased
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;

        costs[0].text = storeController.products.WithID("remove_ads").metadata.localizedPriceString;
        costs[1].text = storeController.products.WithID("item_1").metadata.localizedPriceString;
        costs[2].text = storeController.products.WithID("item_2").metadata.localizedPriceString;
        costs[3].text = storeController.products.WithID("item_3").metadata.localizedPriceString;
        costs[4].text = storeController.products.WithID("item_4").metadata.localizedPriceString;
        costs[5].text = storeController.products.WithID("item_5").metadata.localizedPriceString;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    private async void wealthChange(TMP_Text txt, int start, int end, int startAmount, int amount)
    {
        LeanTween.value(txt.gameObject, start, end, 0.2f).setOnUpdate(val => txt.fontSize = val).setEaseOutBack();
        await Task.Delay(250);
        txt.text = (startAmount + amount).ToString();
        LeanTween.value(txt.gameObject, end, start, 0.2f).setOnUpdate(val => txt.fontSize = val).setEaseInBack();
    }

    public void addCoins(int amount)
    {
        wealthChange(coinsTxt, 60, 80, coins, amount);
        coins += amount;

        databaseManager.setCoins(coins);
    }

    public void addGems(int amount)
    {
        wealthChange(gemsTxt, 60, 80, gems, amount);
        gems += amount;

        databaseManager.setGems(gems);
    }
}
