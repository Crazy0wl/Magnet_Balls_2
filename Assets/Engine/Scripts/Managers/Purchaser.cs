using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace MB_Engine
{
    public class Purchaser : Singleton<Purchaser>, IStoreListener
    {
        public string[] ConsumableProducts;
        public string[] NoConsumableProducts;
        private List<string> consumableProductsList;
        private List<string> noConsumableProductsList;

        static IStoreController m_StoreController;          // The Unity Purchasing system.
        static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
        public bool IsTest;

        protected Purchaser() { }

        void Start()
        {
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                return;
            }
            // Create a builder, first passing in a suite of Unity provided stores.
            //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            //builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = IsTest;

            //// Add a product to sell / restore by way of its identifier, associating the general identifier
            //// with its store-specific identifiers.
            //consumableProductsList = new List<string>(ConsumableProducts);
            //noConsumableProductsList = new List<string>(NoConsumableProducts);
            //foreach (string productName in ConsumableProducts)
            //{
            //    builder.AddProduct(productName, ProductType.Consumable);
            //}
            //foreach (string productName in NoConsumableProducts)
            //{
            //    builder.AddProduct(productName, ProductType.NonConsumable);
            //}
            //UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        private Product GetProduct(string productId, bool consumable = false)
        {
            if (IsInitialized())
            {
                if (consumable)
                {
                    if (consumableProductsList.Contains(productId))
                    {
                        return m_StoreController.products.WithID(productId);
                    }
                }
                else
                {
                    if (noConsumableProductsList.Contains(productId))
                    {
                        return m_StoreController.products.WithID(productId);
                    }
                }
            }
            return null;
        }

        public bool ProductActive(string productId, bool consumable = false)
        {
            Product product = GetProduct(productId, consumable);
            return product != null && !product.hasReceipt && product.availableToPurchase;
        }

        public void PurchaseProduct(string productId, bool consumable = false)
        {
            Product product = GetProduct(productId, consumable);
            if (product != null && !product.hasReceipt && product.availableToPurchase)
            {
                m_StoreController.InitiatePurchase(product);
            }
        }
        #region commented
        //public void PurchaseProduct()
        //{
        //    // If Purchasing has been initialized ...
        //    if (IsInitialized())
        //    {
        //        // ... look up the Product reference with the general product identifier and the Purchasing 
        //        // system's products collection.
        //        Product product = m_StoreController.products.WithID(NoConsumableProducts[0]);
        //        //    product.hasReceipt
        //        //product.
        //        // If the look up found a product for this device's store and that product is ready to be sold ... 
        //        if (product != null && !product.hasReceipt && product.availableToPurchase)
        //        {
        //            Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
        //            // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
        //            // asynchronously.
        //            m_StoreController.InitiatePurchase(product);
        //        }
        //        // Otherwise ...
        //        else
        //        {
        //            // ... report the product look-up failure situation  
        //            Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
        //        }
        //    }
        //    // Otherwise ...
        //    else
        //    {
        //        // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
        //        // retrying initiailization.
        //        Debug.Log("BuyProductID FAIL. Not initialized.");
        //    }
        //}
        #endregion

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        //public void RestorePurchases()
        //{
        //    // If Purchasing has not yet been set up ...
        //    if (!IsInitialized())
        //    {
        //        // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
        //        Debug.Log("RestorePurchases FAIL. Not initialized.");
        //        return;
        //    }

        //    // If we are running on an Apple device ... 
        //    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        //    {
        //        // ... begin restoring purchases
        //        //Debug.Log("RestorePurchases started ...");
        //        //// Fetch the Apple store-specific subsystem.
        //        //var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
        //        //// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
        //        //// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
        //        //apple.RestoreTransactions((result) =>
        //        //{
        //        //    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
        //        //    // no purchases are available to be restored.
        //        //    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
        //        //});
        //    }
        //    // Otherwise ...
        //    else
        //    {
        //        // We are not running on an Apple device. No work is necessary to restore purchases.
        //        Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        //    }
        //}

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");
            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
            //GameManager.main.WriteDebug("OnInitialized: PASS");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
            //GameManager.main.WriteDebug("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            ////A consumable product has been purchased by this user.
            //if (string.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal))
            //{
            //    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            //    // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            //    //ScoreManager.score += 100;
            //}
            // Or ... a non-consumable product has been purchased by this user.
            //return args.purchasedProduct.hasReceipt ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
            //if (string.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal))
            //{
            //    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            //    // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
            //}
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
            //GameManager.main.WriteDebug(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }
}
