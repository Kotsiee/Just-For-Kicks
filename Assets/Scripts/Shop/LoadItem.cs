using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadItem : MonoBehaviour
{
    [SerializeField]
    private GameObject shelfItem, content, brand, grid, pageDot;

    [SerializeField]
    private ShopNavigation shopNavigation;

    [SerializeField]
    private TMP_Text rarityText;

    public void nextShoeBrand()
    {
        shopNavigation.uISoundFX.changeMenu();
        switch (shopNavigation.state)
        {
            case ShopNavigation.pageState.shoes:
                shopNavigation.selectedBrandIndex++;
                if (shopNavigation.selectedBrandIndex > shopNavigation.items.shoes.Count - 1)
                    shopNavigation.selectedBrandIndex = 0;
                loadShoes(shopNavigation.selectedBrandIndex);
                break;
            case ShopNavigation.pageState.balls:
                shopNavigation.selectedBallBrandIndex++;
                if (shopNavigation.selectedBallBrandIndex > shopNavigation.items.balls.Count - 1)
                    shopNavigation.selectedBallBrandIndex = 0;
                loadBalls(shopNavigation.selectedBallBrandIndex);
                break;
        }
    }

    public void prevShoeBrand()
    {
        shopNavigation.uISoundFX.changeMenu();
        switch (shopNavigation.state)
        {
            case ShopNavigation.pageState.shoes:
                shopNavigation.selectedBrandIndex--;
                if (shopNavigation.selectedBrandIndex < 0)
                    shopNavigation.selectedBrandIndex = shopNavigation.items.shoes.Count - 1;
                loadShoes(shopNavigation.selectedBrandIndex);
                break;
            case ShopNavigation.pageState.balls:
                shopNavigation.selectedBallBrandIndex--;
                if (shopNavigation.selectedBallBrandIndex < 0)
                    shopNavigation.selectedBallBrandIndex = shopNavigation.items.balls.Count - 1;
                loadBalls(shopNavigation.selectedBallBrandIndex);
                break;
        }
    }

    public void loadShoes(string value)
    {
        int i = 0, val = 0;
        foreach (var item in shopNavigation.items.shoes.Keys)
        {
            if (item == value)
                val = i;

            i++;
        }

        shopNavigation.selectedBrandIndex = val;
        loadShoes(val);
    }

    public void loadShoes(int index)
    {
        string directory = shopNavigation.items.shoes.ElementAt(index).Key;

        foreach (Transform child in shopNavigation.pagination.transform)
            Destroy(child.gameObject);

        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        foreach (var item in shopNavigation.items.shoes.Keys)
        {
            GameObject page = Instantiate(pageDot, shopNavigation.pagination.transform);
            if (directory == item)
                page.GetComponent<Image>().color = new Color32(200, 65, 32, 255);

            page.GetComponent<Button>().onClick.AddListener(() => loadShoes((string)item));
        }

        shopNavigation.itemBrand.text = directory;
        List<string> paths = shopNavigation.items.shoes[directory].ToList();

        float yPos = 100;

        paths.ForEach(path =>
        {
            GameObject brandTitle = Instantiate(brand, content.transform);
            brandTitle.transform.GetChild(1).GetComponent<TMP_Text>().text = path;
            brandTitle.GetComponent<RectTransform>().offsetMin = new Vector2(0, -yPos);
            brandTitle.GetComponent<RectTransform>().offsetMax = new Vector2(0, -yPos);
            brandTitle.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            brandTitle.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

            GameObject rack = Instantiate(grid, content.transform);
            rack.GetComponent<RectTransform>().offsetMin = new Vector2(0, -yPos - 150);
            rack.GetComponent<RectTransform>().offsetMax = new Vector2(0, -yPos - 150);
            rack.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            rack.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            Debug.Log("Scriptable Objects/Shoe/" + directory + "/" + path);
            List<ShoeScriptable> shoes = Resources.LoadAll<ShoeScriptable>("Scriptable Objects/Shoe/" + directory + "/" + path).ToList();
            shoes.Sort((x, y) => x.id.CompareTo(y.id));
            brandTitle.transform.GetChild(2).GetComponent<TMP_Text>().text = shoes[0].rarity.ToString();
            brandTitle.transform.GetChild(0).GetComponent<Image>().color = shopNavigation.rarityColours[(int)shoes[0].rarity];

            shoes.ForEach(shoe =>
            {
                GameObject shelf = Instantiate(shelfItem, rack.transform);
                shelf.transform.GetChild(0).GetComponent<MeshFilter>().mesh = shoe.prefab.GetComponent<MeshFilter>().sharedMesh;
                if (shopNavigation.ownedItems.Contains(shoe.uid))
                    shelf.transform.GetChild(0).GetComponent<MeshRenderer>().material = shopNavigation.pallete;
                else
                    shelf.transform.GetChild(0).GetComponent<MeshRenderer>().material = shopNavigation.locked;

                shelf.GetComponent<Image>().color = shopNavigation.rarityColours[(int)shoe.rarity];
                shelf.GetComponent<Button>().onClick.RemoveAllListeners();
                shelf.GetComponent<Button>().onClick.AddListener(() => { displayItem(shoe); LeanTween.scale(shopNavigation.player, new Vector3(10, 10, shopNavigation.shoeSide * 10), 0.15f).setEaseInBack(); shopNavigation.uISoundFX.pressButton(); });
                shelf.tag = "Shoes";
                shelf.layer = 7;
                shelf.transform.GetChild(0).gameObject.layer = 7;
            });

            yPos += (Mathf.Ceil(shoes.Count / Mathf.Floor(rack.GetComponent<RectTransform>().rect.width / rack.GetComponent<GridLayoutGroup>().cellSize.x)) + 1) * rack.GetComponent<GridLayoutGroup>().cellSize.y + brandTitle.GetComponent<RectTransform>().rect.height + 300;
        });

        content.GetComponent<RectTransform>().offsetMin = new Vector2(0, -yPos - 500);
        content.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        content.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        content.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
    }


    public void loadBalls(string value)
    {
        int i = 0, val = 0;
        foreach (var item in shopNavigation.items.balls)
        {
            if (item == value)
                val = i;
            i++;
        }

        shopNavigation.selectedBallBrandIndex = val;
        loadBalls(val);
    }

    public void loadBalls(int index)
    {
        string directory = shopNavigation.items.balls[index];

        foreach (Transform child in shopNavigation.pagination.transform)
            Destroy(child.gameObject);

        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        foreach (var item in shopNavigation.items.balls)
        {
            GameObject page = Instantiate(pageDot, shopNavigation.pagination.transform);
            if (directory == item)
                page.GetComponent<Image>().color = new Color32(200, 65, 32, 255);

            page.GetComponent<Button>().onClick.AddListener(() => loadBalls(item));
        }

        shopNavigation.itemBrand.text = directory;
        float yPos = 0;
        GameObject rack = Instantiate(grid, content.transform);
        rack.GetComponent<RectTransform>().offsetMin = new Vector2(0, -yPos - 150);
        rack.GetComponent<RectTransform>().offsetMax = new Vector2(0, -yPos - 150);
        rack.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        rack.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        List<BallScriptable> balls = Resources.LoadAll<BallScriptable>("Scriptable Objects/Balls/" + directory).ToList();
        balls.Sort((x, y) => x.id.CompareTo(y.id));

        balls.ForEach(ball =>
        {
            GameObject shelf = Instantiate(shelfItem, rack.transform);
            shelf.transform.GetChild(0).GetComponent<MeshFilter>().mesh = ball.prefab.GetComponent<MeshFilter>().sharedMesh;
            shelf.transform.GetChild(0).transform.localScale = new Vector3(6500, 6500, 6500);
            shelf.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, shelf.transform.GetChild(0).transform.localPosition.z);
            if (shopNavigation.ownedItems.Contains(ball.uid))
                shelf.transform.GetChild(0).GetComponent<MeshRenderer>().material = shopNavigation.pallete;
            else
                shelf.transform.GetChild(0).GetComponent<MeshRenderer>().material = shopNavigation.locked;

            shelf.GetComponent<Image>().color = shopNavigation.rarityColours[(int)ball.rarity];
            shelf.GetComponent<Button>().onClick.RemoveAllListeners();
            shelf.GetComponent<Button>().onClick.AddListener(() => { displayItem(ball); LeanTween.scale(shopNavigation.player, new Vector3(10, 10, shopNavigation.shoeSide * 10), 0.15f).setEaseInBack(); shopNavigation.uISoundFX.pressButton(); });
            shelf.tag = "Balls";
            shelf.layer = 7;
            shelf.transform.GetChild(0).gameObject.layer = 7;
        });
        yPos += (Mathf.Ceil(balls.Count / Mathf.Floor(rack.GetComponent<RectTransform>().rect.width / rack.GetComponent<GridLayoutGroup>().cellSize.x)) + 1) * rack.GetComponent<GridLayoutGroup>().cellSize.y + 250;

        content.GetComponent<RectTransform>().offsetMin = new Vector2(0, -yPos);
        content.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        content.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        content.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
    }

    public async void displayItem(IScriptableObject item)
    {
        shopNavigation.selectedShoe = shopNavigation.databaseManager.playerData.playerAttributesData.selectedShoe;
        shopNavigation.selectedBall = shopNavigation.databaseManager.playerData.playerAttributesData.selectedBall;
        shopNavigation.itemName.text = item.displayName;
        shopNavigation.itemDesc.text = item.colour;
        shopNavigation.player.GetComponent<MeshFilter>().mesh = item.prefab.GetComponent<MeshFilter>().sharedMesh;
        if (shopNavigation.ownedItems.Contains(item.uid))
        {
            shopNavigation.player.GetComponent<MeshRenderer>().material = shopNavigation.pallete;
            shopNavigation.selectedColor = Color.white;
            //Item is already selected
            if ((item.path + ":" + item.id) == shopNavigation.selectedShoe | (item.path + ":" + item.id) == shopNavigation.selectedBall)
            {
                shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().text = "Selected";
                shopNavigation.itemPrice.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
            }
            //Item has been bought before
            else
            {
                shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().text = "Select";
                shopNavigation.itemPrice.GetComponent<Image>().color = new Color32(0, 153, 153, 255);
                shopNavigation.itemPrice.GetComponent<Button>().onClick.RemoveAllListeners();

                if (item.GetType() == typeof(ShoeScriptable))
                    shopNavigation.itemPrice.GetComponent<Button>().onClick.AddListener(() => selectShoe(item));
                if (item.GetType() == typeof(BallScriptable))
                    shopNavigation.itemPrice.GetComponent<Button>().onClick.AddListener(() => selectBall(item));
            }


            shopNavigation.coinImg.SetActive(false);
            shopNavigation.itemPrice.SetActive(true);
            shopNavigation.itemPrice.transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        }
        else
        {
            shopNavigation.itemPrice.GetComponent<Button>().onClick.RemoveAllListeners();

            if (shopNavigation.databaseManager.playerData.playerAttributesData.gems >= item.cost & !shopNavigation.databaseManager.playerPrefs.offline)
            {
                shopNavigation.itemPrice.GetComponent<Image>().color = new Color32(255, 135, 0, 255);

                if (item.GetType() == typeof(ShoeScriptable))
                    shopNavigation.itemPrice.GetComponent<Button>().onClick.AddListener(() => buyShoe(item));
                if (item.GetType() == typeof(BallScriptable))
                    shopNavigation.itemPrice.GetComponent<Button>().onClick.AddListener(() => buyBall(item));
            }
            else
            {
                shopNavigation.itemPrice.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
                if (shopNavigation.databaseManager.playerPrefs.offline)
                {
                    shopNavigation.itemPrice.GetComponent<Button>().onClick.AddListener(() => shopNavigation.openOffline());
                }
            }

            shopNavigation.player.GetComponent<MeshRenderer>().material = shopNavigation.locked;
            shopNavigation.selectedColor = shopNavigation.player.GetComponent<MeshRenderer>().material.color;

            shopNavigation.coinImg.SetActive(true);
            shopNavigation.itemPrice.SetActive(true);
            shopNavigation.coinImg.GetComponent<Image>().sprite = shopNavigation.gemSprite;
            shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().text = item.cost.ToString();
            shopNavigation.itemPrice.transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(40, 0);
            shopNavigation.itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
        }

        await Task.Delay(150);

        LeanTween.scale(shopNavigation.player, new Vector3(50, 50, shopNavigation.shoeSide * 50), 0.15f).setEaseInBack();
    }

    public void selectBall(IScriptableObject item)
    {
        string info = item.path + ":" + item.id;

        shopNavigation.databaseManager.setSelectedBall(info);
        displayItem(item);

        shopNavigation.databaseManager.fileHandler.Save(shopNavigation.databaseManager.playerData);
    }

    public void selectShoe(IScriptableObject item)
    {
        string info = item.path + ":" + item.id;

        shopNavigation.databaseManager.setSelectedShoe(info);
        displayItem(item);

        shopNavigation.databaseManager.fileHandler.Save(shopNavigation.databaseManager.playerData);
    }

    public void buyBall(IScriptableObject item)
    {
        shopNavigation.databaseManager.updateUserCollection("balls", new Dictionary<string, object>()
        {
            [item.uid] = item.path + ":" + item.id
        });
        shopNavigation.takeGems((int)item.cost);
        shopNavigation.ownedItems.Add(item.uid);

        displayItem(item);
        loadBalls(item.path);
        LeanTween.scale(shopNavigation.player, new Vector3(10, 10, shopNavigation.shoeSide * 10), 0.15f).setEaseInBack();
    }
    public void buyShoe(IScriptableObject item)
    {
        shopNavigation.databaseManager.updateUserCollection("shoes", new Dictionary<string, object>()
        {
            [item.uid] = item.path + ":" + item.id
        });
        shopNavigation.takeGems((int)item.cost);
        shopNavigation.ownedItems.Add(item.uid);

        displayItem(item);
        loadShoes(item.path.Split("/")[0]);
        LeanTween.scale(shopNavigation.player, new Vector3(10, 10, shopNavigation.shoeSide * 10), 0.15f).setEaseInBack();
    }
}
