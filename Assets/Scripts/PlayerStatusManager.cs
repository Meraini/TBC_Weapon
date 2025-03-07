using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatusManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private Dictionary<string, string> playerStatuses = new Dictionary<string, string>();

    [SerializeField] private TextMeshProUGUI statusBoardText;

    private void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!playerStatuses.ContainsKey(player.NickName))
            {
                playerStatuses[player.NickName] = "In Progress";
            }
        }

        UpdateStatusBoard();
    }

    public void UpdatePlayerStatus(string playerName, string status)
    {
        playerStatuses[playerName] = status;
        UpdateStatusBoard();
        CheckAllPlayersFinished();
    }

    void UpdateStatusBoard()
    {
        string statusBoard = "Player Statuses:\n";
        foreach (var entry in playerStatuses)
        {
            statusBoard += $"{entry.Key}: {entry.Value}\n";
        }

        statusBoardText.text = statusBoard;
    }

    void CheckAllPlayersFinished()
    {
        foreach (var status in playerStatuses.Values)
        {
            if (status == "In Progress")
            {
                return;
            }
        }

        Debug.Log("All players have finished!");
        // Proceed to end game logic, e.g., load results scene or display a summary
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (!playerStatuses.ContainsKey(newPlayer.NickName))
        {
            playerStatuses[newPlayer.NickName] = "In Progress";
            UpdateStatusBoard();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (playerStatuses.ContainsKey(otherPlayer.NickName))
        {
            playerStatuses.Remove(otherPlayer.NickName);
            UpdateStatusBoard();
        }
    }

    // Synchronize the player statuses across the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the data to other players
            stream.SendNext(playerStatuses.Count);
            foreach (var entry in playerStatuses)
            {
                stream.SendNext(entry.Key);
                stream.SendNext(entry.Value);
            }
        }
        else
        {
            // Receive the data from other players
            int count = (int)stream.ReceiveNext();
            playerStatuses.Clear();
            for (int i = 0; i < count; i++)
            {
                string playerName = (string)stream.ReceiveNext();
                string status = (string)stream.ReceiveNext();
                playerStatuses[playerName] = status;
            }
            UpdateStatusBoard();
        }
    }
}
