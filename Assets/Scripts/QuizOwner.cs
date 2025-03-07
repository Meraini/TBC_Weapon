using Photon.Pun;
using UnityEngine;

public class QuizOwner : MonoBehaviour
{
    public int ownerActorNumber = -1; // Default to unassigned

    private void Awake()
    {
        // If ownerActorNumber is unassigned, assign it to the local player
        if (ownerActorNumber == -1)
        {
            ownerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            Debug.Log($"Assigned ownership to Player {ownerActorNumber}");
        }
    }
}
