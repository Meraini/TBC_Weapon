using System;
using UnityEngine;

[Serializable]
public class QuestionData
{
    public string question;
    public string category;
    [Tooltip("The correct answer should always be listed first, they are randomized later")]
    public string[] answers;
}
