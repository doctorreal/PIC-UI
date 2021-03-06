﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class V_RoomModal : MonoBehaviour 
{
	// #revision
	V_UIController UIController;
	V_CustomLobbyManager LobbyManager;
	
	// buttons
	[HeaderAttribute("RoomModalPanel Buttons")]
	[SpaceAttribute(10f)]

	[SerializeField]
	Button createButton;
	[SerializeField]
	Button cancelButton;
	public Button goForwardInMaps;
	public Button goBackwardInMaps;

	// room vars
	[HeaderAttribute("RoomModalPanel variables")]
	[SpaceAttribute(10f)]
	public InputField roomName;
	public Dropdown playerMode;
	public Dropdown gameMode;
	public InputField password;
	public Image map;
	public Text mapName;

	
	void Awake()
	{
		// #revision
		UIController = FindObjectOfType<V_UIController>();
		LobbyManager = FindObjectOfType<V_CustomLobbyManager>();

		UIController.IfClick_GoTo(createButton, OnCreateRoom);
		UIController.IfClick_GoTo(cancelButton, OnCancel);
		UIController.IfClick_GoTo(goForwardInMaps, ()=> ChangeMap(goToNextMap: true));
		UIController.IfClick_GoTo(goBackwardInMaps,()=> ChangeMap(goToNextMap: false));

		if (UIController.thumbnailMaps == null)
		{
			UIController.ThrowError("Thumbnails are not set for maps", ()=> 
			{
				UIController.GoFrom_To(UIController.genericErrorModal, this.gameObject);
				return;
			});
		}
		map.sprite = UIController.thumbnailMaps[0];
		mapName.text = UIController.ReturnMap(0);
	}


    void OnEnable()
	{
		UIController.GetItemInDropDown(gameMode, UIController.ReturnGameMode(LobbyManager.currentRoom.gameMode));
	}
	void OnCreateRoom()
	{

		if (LobbyManager.currentRoom == null)
		{
			UIController.ThrowError("V_CustomLobbyManager: currentRoom is not set", ()=>
			{
				UIController.GoFrom_To(UIController.genericErrorModal, this.gameObject);
			});
			// do something about it!!! and then:
			return;
		}
		LobbyManager.currentRoom.roomName = roomName.text;

		LobbyManager.currentRoom.map = UIController.ReturnMap(mapName.text);

		LobbyManager.currentRoom.playerMode = UIController.ReturnPlayerMode(playerMode.options[playerMode.value].text);

		LobbyManager.currentRoom.gameMode = UIController.ReturnGameMode(gameMode.options[gameMode.value].text);

		LobbyManager.currentRoom.password = password.text;
		
		LobbyManager.SaveRoom(LobbyManager.currentRoom.ID, LobbyManager.currentRoom);
		
		UIController.GoFrom_To(UIController.RoomModalPanel, UIController.RoomPanel);
	}
	void OnCancel()
	{
		UIController.AskYesNoQ("Are u sure u want to discard the room?", 
		() => // yes answer
		{
			UIController.GoFrom_To(UIController.genericYesNoModal, UIController.LobbyPanel); 
			LobbyManager.RemoveRoom(LobbyManager.currentRoom.ID);
			this.gameObject.SetActive(false);
		}, 
		() => // no answer
		{
			UIController.GoFrom_To(UIController.genericYesNoModal, UIController.RoomModalPanel);
		});
	}
    private void ChangeMap(bool goToNextMap)
    {
		// print("changing map");
		if (UIController.thumbnailMaps == null)
		{
			UIController.ThrowError("V_UIController: thumbnailMaps is null", ()=> UIController.GoFrom_To(UIController.genericErrorModal, this.gameObject));
		}

		for(int i = 0; i < UIController.thumbnailMaps.Length; i++)
		{
			if (map.sprite == UIController.thumbnailMaps[i])
			{
				// if we are pressing go to forward map
				if (goToNextMap)
				{
					//  if we are not skipping the last index of the array
					if (i+1 < UIController.thumbnailMaps.Length)
					{
						map.sprite = UIController.thumbnailMaps[i+1];
						mapName.text = UIController.ReturnMap(i+1);
						break;
					}
					//  if we are proceeding, we just go back to first index!
					else
					{
						map.sprite = UIController.thumbnailMaps[0];
						mapName.text = UIController.ReturnMap(0);
						break;	 
					}

				}
				// if we are going backward in maps
				else
				{
					if (i-1 >= 0)
					{
						map.sprite = UIController.thumbnailMaps[i-1];
						mapName.text = UIController.ReturnMap(i-1);
						break;
					}
					else
					{
						map.sprite = UIController.thumbnailMaps[UIController.thumbnailMaps.Length-1];
						mapName.text = UIController.ReturnMap(UIController.thumbnailMaps.Length-1);
						break;
					}
					
				}
			}
		}
    }
}
