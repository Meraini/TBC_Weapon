using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionSetup : MonoBehaviourPun, IOnEventCallback
{
    private List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField]
    private TextMeshProUGUI questionText;
    [SerializeField]
    private TextMeshProUGUI categoryText;
    [SerializeField]
    private AnswerButton[] answerButtons;

    private int correctAnswerChoice;

    private CVStoSO cvsToSO;
    private CountDown countDown;

    private QuizOwner quizOwner;

    private const byte AddTimeEventCode = 1;

    private void Awake()
    {
        cvsToSO = FindObjectOfType<CVStoSO>();
        countDown = GetComponentInChildren<CountDown>();
        quizOwner = GetComponent<QuizOwner>();
    }

    private void Start()
    {
        if (quizOwner != null)
        {
            if (quizOwner.ownerActorNumber == -1)
            {
                quizOwner.ownerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            }
            if (quizOwner.ownerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} owns {gameObject.name}. Enabling interactivity.");
                // This quiz belongs to the local player
                EnableInteractivity(true);

                if (countDown != null)
                {
                    countDown.OnTimerEnded += OnTimerEnded;
                    countDown.StartTimer();
                }

                StartCoroutine(InitializeQuestions());
            }
            else
            {
                Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} does NOT own {gameObject.name}. Disabling interactivity.");
                // This quiz belongs to another player
                EnableInteractivity(false);
            }

            if (quizOwner.ownerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // This quiz belongs to the local player
                EnableInteractivity(true);

                if (countDown != null)
                {
                    countDown.OnTimerEnded += OnTimerEnded;
                    countDown.StartTimer();
                }

                StartCoroutine(InitializeQuestions());
            }
            else
            {
                Debug.LogError($"QuizOwner component not found on {gameObject.name}");
                // This quiz belongs to another player
                EnableInteractivity(false);
            }
        }
        else
        {
            Debug.LogError("QuizOwner component not found on " + gameObject.name);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (countDown != null)
        {
            countDown.OnTimerEnded -= OnTimerEnded;
        }

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private IEnumerator InitializeQuestions()
    {
        while (cvsToSO == null || cvsToSO.questions == null || cvsToSO.questions.Count == 0)
        {
            yield return null;
        }

        questions = new List<QuestionData>(cvsToSO.questions);

        GenerateNewQuestion();
    }

    public void GenerateNewQuestion()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.Log("No more questions available.");
            OnQuizCompleted();
            return;
        }

        SelectNewQuestion();
        SetQuestionValues();
        SetAnswerValues();
    }

    private void SelectNewQuestion()
    {
        int randomQuestionIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomQuestionIndex];
        questions.RemoveAt(randomQuestionIndex);
    }

    private void SetQuestionValues()
    {
        if (currentQuestion == null)
        {
            Debug.LogError("Current question is null.");
            return;
        }

        questionText.text = currentQuestion.question;
        categoryText.text = currentQuestion.category;
    }

    private void SetAnswerValues()
    {
        List<string> answers = RandomizeAnswers(new List<string>(currentQuestion.answers));

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool isCorrect = (i == correctAnswerChoice);

            answerButtons[i].SetIsCorrect(isCorrect);
            answerButtons[i].SetAnswerText(answers[i]);
        }
    }

    private List<string> RandomizeAnswers(List<string> originalList)
    {
        bool correctAnswerChosen = false;
        List<string> newList = new List<string>();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (originalList.Count == 0)
                break;

            int random = Random.Range(0, originalList.Count);

            if (!correctAnswerChosen && originalList[random] == currentQuestion.answers[0])
            {
                correctAnswerChoice = i;
                correctAnswerChosen = true;
            }

            newList.Add(originalList[random]);
            originalList.RemoveAt(random);
        }

        return newList;
    }

    private void OnTimerEnded()
    {
        Debug.Log("Time's up!");

        // Disable answer buttons
        foreach (var button in answerButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }

        // Notify other players
        photonView.RPC("PlayerFinished", RpcTarget.AllBuffered, PhotonNetwork.NickName, false);
    }

    private void OnQuizCompleted()
    {
        Debug.Log("Quiz Completed!");

        if (countDown != null)
        {
            countDown.StopTimer();
        }

        // Disable answer buttons
        foreach (var button in answerButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }

        // Notify other players
        photonView.RPC("PlayerFinished", RpcTarget.AllBuffered, PhotonNetwork.NickName, true);
    }

    [PunRPC]
    void PlayerFinished(string playerName, bool completedQuiz)
    {
        string status = completedQuiz ? "Completed the quiz" : "Ran out of time";
        Debug.Log($"{playerName} {status}");

        PlayerStatusManager statusManager = FindObjectOfType<PlayerStatusManager>();
        if (statusManager != null)
        {
            statusManager.UpdatePlayerStatus(playerName, status);
        }
    }

    public void EnableInteractivity(bool isInteractable)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.interactable = isInteractable;
            canvasGroup.blocksRaycasts = isInteractable;
        }
        else
        {
            Debug.LogWarning("CanvasGroup component not found on " + gameObject.name);
        }
    }

    // Implement IOnEventCallback to handle network events
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == AddTimeEventCode)
        {
            // This event is for adding time
            float timeToAdd = (float)photonEvent.CustomData;

            // Add time to our timer
            if (countDown != null)
            {
                countDown.AddTime(timeToAdd);
                Debug.Log("Added time to timer: " + timeToAdd);
            }
            else
            {
                Debug.LogError("CountDown script not found in " + gameObject.name);
            }
        }
    }
}
