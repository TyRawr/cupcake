using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;

public static class StoreManager {

    public class ProductDescription
    {
        public ProductDescription(string _icon , string _displayName , string _price)
        {
            this.icon = _icon;
            this.display_name = _displayName;
            this.price = _price;
        }
        public string icon;
        public string display_name;
        public string price;
    }

    public static List<string> ProductIDs = new List<string>();
    public static Dictionary<string,ProductDescription> ProductInfoMap = new Dictionary<string,ProductDescription> ();


    public static void Init()
    {
        // initiate some things, buttons?
        // in app purchase login somethings?
        ProductIDs.Add(Constants.PRODUCT_GOLD_10);
        ProductIDs.Add(Constants.PRODUCT_GOLD_50);
        ProductIDs.Add(Constants.PRODUCT_GOLD_110);
        ProductIDs.Add(Constants.PRODUCT_GOLD_250);
        ProductIDs.Add(Constants.PRODUCT_GOLD_750);
        // Will there be some localization thing?
        // This info needs to be the exact same as on the store

        ProductInfoMap.Add(Constants.PRODUCT_GOLD_10, new ProductDescription(Constants.PRODUCT_GOLD_10 , "Gold (10)" , "$0.99"));

        ProductInfoMap.Add(Constants.PRODUCT_GOLD_50, new ProductDescription(Constants.PRODUCT_GOLD_50, "Gold (50)", "$4.99"));

        ProductInfoMap.Add(Constants.PRODUCT_GOLD_110, new ProductDescription(Constants.PRODUCT_GOLD_110, "Gold (110)", "$9.99")); // 10% free, most popular

        ProductInfoMap.Add(Constants.PRODUCT_GOLD_250, new ProductDescription(Constants.PRODUCT_GOLD_250, "Gold (250)", "$19.99")); // 25% free
        
        ProductInfoMap.Add(Constants.PRODUCT_GOLD_750, new ProductDescription(Constants.PRODUCT_GOLD_750, "Gold (750)", "$49.99")); // 50% free, best value
    }

    public static void OpenStore(Transform store)
    {
        //Initialize the app store bindings etc..
        //possibly need to setup some stuff before this?
        //bring up some items to buy
        store.gameObject.SetActive(true);
    }

    public static void BuyThisThing(string thingID = "")
    {
        if("".Equals("blah"))
        {
            // buy the thing
        } else if ("".Equals("bleh"))
        {
            //buy this other thing
        }
    }
}
