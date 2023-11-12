using TMPro;
using UnityEngine;
using WalletConnectSharp.Unity;

public class WalletView : MonoBehaviour
{
	[SerializeField] private TMP_Text _walletAddress;
	[SerializeField] private GameObject _connectedView;
	[SerializeField] private GameObject _disconnectedView;

	private void OnEnable()
	{
		UpdateView();
	}

	public void UpdateWalletAddress()
	{
		var address = WalletConnect.Instance.Session.Accounts[0];

		_walletAddress.text = $"{address[..6]}...{address[^4..]}";
	}

	public void UpdateView()
	{
		Debug.Log("SwitchView");

		if (WalletConnect.Instance == null)
		{
			return;
		}

		var isConnected = WalletConnect.Instance.Connected;

		_connectedView.SetActive(isConnected);
		_disconnectedView.SetActive(!isConnected);

		if (isConnected)
		{
			UpdateWalletAddress();
		}
	}
}
