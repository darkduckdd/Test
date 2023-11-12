using Game.Domain;
using IDosGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicUpdate : MonoBehaviour
{
    [SerializeField] private float uploadMinute = 5f;

    public static Action UploadCash;

    private float uploadSecond = 0;

    private void OnEnable()
    {
        UploadCash += SendToServer;
    }
    private void OnDisable()
    {
        UploadCash -= SendToServer;
    }
    private void Awake()
    {

        StartCoroutine(Upload());
    }

    private void SendToServer()
    {
        var data = PlayerPrefs.GetString("model");
        GameModel gameModel = JsonUtility.FromJson<GameModel>(data);

        var parameter = new Dictionary<string, object>()
            {
                {

                    "VirtualCurrency","MO"
                },
                {
                    "Amount",gameModel.Cash.ToString()
                }
            };

        PlayFabService.ExecuteCloudFunction("AddAnyVirtualCurrency", functionParameter: parameter);
        uploadSecond = 0;

        StartCoroutine(Upload());
    }
    private IEnumerator Upload()
    {
        while (uploadSecond < uploadMinute * 60)
        {
            uploadSecond++;
            yield return new WaitForSeconds(1f);

        }

        SendToServer();
    }
}
