using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class AnswerButton : MonoBehaviourPun
{
    private bool isCorrect;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private float timeIncreaseAmount = 5f; // Adjust as needed

    private QuestionSetup questionSetup;
    private QuizOwner quizOwner;

    [Header("Target Player Settings")]
    [Tooltip("Select how to choose the target player whose timer will be increased.")]
    public TargetPlayerSelection targetPlayerSelection;

    [Tooltip("Specify the target player's index (starting from 1).")]
    public int targetPlayerIndex = 1;

    [Tooltip("Specify the target player's NickName.")]
    public string targetPlayerNickName;

    [Tooltip("Specify the target layer name (e.g., 'Quiz_Player1').")]
    public string targetLayerName;

    public enum TargetPlayerSelection
    {
        ByIndex,
        ByNickName,
        ByLayer,
        RandomPlayer,
        AllOtherPlayers
    }

    private const byte AddTimeEventCode = 1;

    private void Awake()
    {
        questionSetup = GetComponentInParent<QuestionSetup>();
        quizOwner = GetComponentInParent<QuizOwner>();
    }

    private void Start()
    {
        if (quizOwner != null)
        {
            if (quizOwner.ownerActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // Disable the button if not owned by the local player
                GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            Debug.LogError("QuizOwner component not found in parent of " + gameObject.name);
        }
    }

    public void SetAnswerText(string newText)
    {
        answerText.text = newText;
    }

    public void SetIsCorrect(bool newBool)
    {
        isCorrect = newBool;
    }

    public void OnClick()
    {
        if (quizOwner != null && quizOwner.ownerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            if (isCorrect)
            {
                Debug.Log("CORRECT ANSWER");

                // Determine the target player(s) based on the selection
                List<int> targetActorNumbers = GetTargetActorNumbers();

                if (targetActorNumbers.Count > 0)
                {
                    // Send an event to the target player(s) to add time
                    object content = timeIncreaseAmount; // Amount of time to add
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                    {
                        TargetActors = targetActorNumbers.ToArray()
                    };
                    SendOptions sendOptions = new SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(AddTimeEventCode, content, raiseEventOptions, sendOptions);
                }
                else
                {
                    Debug.LogWarning("No valid target players found.");
                }
            }
            else
            {
                Debug.Log("WRONG ANSWER");
            }

            if (questionSetup != null)
            {
                questionSetup.GenerateNewQuestion();
            }
            else
            {
                Debug.LogError("QuestionSetup script not found in parent.");
            }
        }
        else
        {
            Debug.LogWarning("Attempted to click answer button not owned by local player.");
        }
    }

    private List<int> GetTargetActorNumbers()
    {
        List<int> targetActorNumbers = new List<int>();

        switch (targetPlayerSelection)
        {
            case TargetPlayerSelection.ByIndex:
                {
                    // Find the player with the specified index
                    if (targetPlayerIndex >= 1 && targetPlayerIndex <= PhotonNetwork.PlayerList.Length)
                    {
                        Photon.Realtime.Player targetPlayer = PhotonNetwork.PlayerList[targetPlayerIndex - 1];
                        if (targetPlayer.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            targetActorNumbers.Add(targetPlayer.ActorNumber);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Target player index is out of range.");
                    }
                    break;
                }
            case TargetPlayerSelection.ByNickName:
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (player.NickName == targetPlayerNickName && player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            targetActorNumbers.Add(player.ActorNumber);
                            break;
                        }
                    }
                    break;
                }
            case TargetPlayerSelection.ByLayer:
                {
                    // Find the quiz with the specified layer
                    GameObject[] allQuizzes = GameObject.FindGameObjectsWithTag("QuizUI");

                    foreach (GameObject quiz in allQuizzes)
                    {
                        if (quiz.layer == LayerMask.NameToLayer(targetLayerName))
                        {
                            QuizOwner targetQuizOwner = quiz.GetComponent<QuizOwner>();
                            if (targetQuizOwner != null && targetQuizOwner.ownerActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                            {
                                targetActorNumbers.Add(targetQuizOwner.ownerActorNumber);
                                break;
                            }
                        }
                    }

                    if (targetActorNumbers.Count == 0)
                    {
                        Debug.LogWarning("No quiz found with the specified layer or it belongs to the local player.");
                    }
                    break;
                }
            case TargetPlayerSelection.RandomPlayer:
                {
                    List<Photon.Realtime.Player> otherPlayers = new List<Photon.Realtime.Player>(PhotonNetwork.PlayerList);
                    otherPlayers.Remove(PhotonNetwork.LocalPlayer);

                    if (otherPlayers.Count > 0)
                    {
                        int randomIndex = Random.Range(0, otherPlayers.Count);
                        targetActorNumbers.Add(otherPlayers[randomIndex].ActorNumber);
                    }
                    break;
                }
            case TargetPlayerSelection.AllOtherPlayers:
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            targetActorNumbers.Add(player.ActorNumber);
                        }
                    }
                    break;
                }
        }

        return targetActorNumbers;
    }
}
