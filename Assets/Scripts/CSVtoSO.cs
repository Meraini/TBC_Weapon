using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class CVStoSO : MonoBehaviour
{
    private static string googleSheetsUrl = "https://sheets.googleapis.com/v4/spreadsheets/1kwZVfMttl9NCYvVPZf4X8V6yeoDWnQZqr_4MT7I5yhI/values/Sheet1?key=AIzaSyA9pisedM82CEOzSFqFmf0sA82d5QJma5w";
    private static int numberOfAnswers = 4;

    public List<QuestionData> questions;

    private void Awake()
    {
        // Start fetching data as soon as the game starts
        StartCoroutine(FetchQuestions());
    }

    private IEnumerator FetchQuestions()
    {
        Debug.Log("Starting to generate questions from Google Sheets...");

        // Clear existing questions
        questions = new List<QuestionData>();

        // Start the async task
        var task = GetGoogleSheetsData();

        // Wait until the task is complete
        while (!task.IsCompleted)
        {
            yield return null;
        }

        string json = task.Result;

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Failed to get data from Google Sheets.");
            yield break;
        }

        JObject jsonObject = JObject.Parse(json);
        JArray rows = (JArray)jsonObject["values"];

        if (rows == null || rows.Count == 0)
        {
            Debug.LogError("No data found in Google Sheets.");
            yield break;
        }

        foreach (JArray row in rows)
        {
            // Skip the header row
            if (row[0].ToString() == "Question")
                continue;

            // Create a new QuestionData object
            QuestionData questionData = new QuestionData
            {
                question = row[0].ToString(),
                category = row[1].ToString(),
                answers = new string[numberOfAnswers]
            };

            for (int i = 0; i < numberOfAnswers; i++)
            {
                questionData.answers[i] = row[2 + i].ToString();
            }

            questions.Add(questionData);
        }

        Debug.Log("Generated Questions from Google Sheets.");
    }

    private static async Task<string> GetGoogleSheetsData()
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(googleSheetsUrl);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            Debug.LogError($"Failed to fetch Google Sheets data: {response.ReasonPhrase}");
            return null;
        }
    }
}
