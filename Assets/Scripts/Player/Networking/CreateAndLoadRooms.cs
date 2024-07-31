using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class CreateAndLoadRooms : MonoBehaviourPun
{
    public GameObject chooseScreen;
    public GameObject createRoomScreen;
    public GameObject nameScreen;
    public GameObject createButton;
    public GameObject joinButton;
    public TMP_InputField createRoomName;
    public TMP_InputField joinRoomName;
    public TMP_InputField nameField;
    public TMP_Text mapChoice;
    public TMP_Text notifier;
    string map;
    bool IsCreating;


    public void creatingANewRoom()
    {
        IsCreating = true;
        chooseScreen.SetActive(false);
        createRoomScreen.SetActive(true);
        createButton.SetActive(true);
    }

    public void joiningARoom()
    {
        IsCreating = false;
        chooseScreen.SetActive(false);
        createRoomScreen.SetActive(true);
        joinButton.SetActive(true);
    }

    public void NameScreen()
    {
        createRoomScreen.SetActive(false);
        nameScreen.SetActive(true);
    }

    public void EnterName()
    {
        PhotonNetwork.NickName = nameField.text;
        print(PhotonNetwork.NickName);
        CreateRoomOrJoinRoom(IsCreating);
    }

    public void CreateRoomOrJoinRoom(bool isCreating)
    {
        if (isCreating)
        {
            PhotonNetwork.CreateRoom(createRoomName.text);
            PhotonNetwork.LoadLevel(map);
        }
        else
        {
            PhotonNetwork.JoinRoom(joinRoomName.text);
            PhotonNetwork.LoadLevel(map);
        }
    }    

    public void Check()
    {
        if (map != null) 
        {
            if (createRoomName != null) 
            {
                NameScreen();
            }
            else
            {
                StartCoroutine(notify(3, "No room name!"));
            }
        }
        else
        {
            StartCoroutine(notify(3,"Map Not Selected!"));
        }
    }

    IEnumerator notify(float time, string issue)
    {
        notifier.text = issue;
        yield return new WaitForSeconds(time);
        notifier.text = "";
        yield return null;
    }

    public void setMap(string choice)
    {
        map = choice;
        mapChoice.text = "Current Map: " +map;
    }

}
