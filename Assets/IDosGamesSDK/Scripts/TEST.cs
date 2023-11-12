using PlayFab;
using PlayFab.EconomyModels;
using System;
using UnityEngine;
using WalletConnectSharp.Unity;

namespace IDosGames
{
    public class TEST : WalletConnectActions
    {

        /* private void Awake()
         {
             PlayerPrefs.DeleteAll();
         }*/
        public void GetInventoryV2()
        {
            PlayFabEconomyAPI.GetInventoryItems(new GetInventoryItemsRequest(),
            result =>
            {
                foreach (var item in result.Items)
                {
                    if (item.Type != "subscription")
                    {
                        continue;
                    }

                    DateTime.TryParse($"{item.ExpirationDate}", out DateTime expirationDate);

                    if (expirationDate <= DateTime.UtcNow)
                    {
                        continue;
                    }

                    Debug.Log(item.Type);
                    Debug.Log(item.Id);
                    Debug.Log(item.StackId);
                    Debug.Log(item.ExpirationDate);
                }


            },
            error => Debug.Log("Error"));

        }

        public void AddToken()
        {

        }

        public async void GetTokenBalance()
        {
            var balance = await WalletConnectBlockchainService.GetTokenBalance(WalletConnect.ActiveSession.Accounts[0], VirtualCurrencyID.IG);

            Debug.Log(" --- " + balance);
        }
    }
}