using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class Tactics : MonoBehaviour
{
    public string API_KEY = "AIzaSyAVDYQAj1kz0-bLU8xQP0sH6POgnpwF__Q";
    private string uri = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key=";

    public BossController bossController;
    public GameObject loadingScrn;
    public TMP_Text loadText;
    public GameObject bossHealthBarObject;

    private string aiVocab = "'Melee', 'Dash', 'Guard', 'Heal'";
    private List<string> attackPlan = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bossController == null)
        {
            bossController = GetComponent<BossController>();
        }
        StartCoroutine(RunEncounter());
    }

    IEnumerator RunEncounter()
    {
        loadingScrn.SetActive(true);
        loadText.text = "This will take few seconds..";
        yield return StartCoroutine(AtkAPI());
        yield return new WaitForSeconds(1.0f);
        loadingScrn.SetActive(false);

    }
    public void StartFight()
    {
        bossController.isFight = true;
        if (bossHealthBarObject != null)
        {
            bossHealthBarObject.SetActive(true);
        }

        StartCoroutine(ExecuteAtk());
    }
    public IEnumerator ResetAndRethink()
    {
        StopAllCoroutines();
        
        attackPlan.Clear();

        yield return StartCoroutine(RunEncounter());
        yield return new WaitForSeconds(4.0f);
        StartFight();
    }

    IEnumerator AtkAPI()
    {
        int closeAtk = PlayerPrefs.GetInt("CloseAtk", 0);
        int longAtk = PlayerPrefs.GetInt("LongAtk", 0);

        string playerStyle;
        if (closeAtk > longAtk + 3)
        {
            playerStyle = "The player is an aggressive brawler who loves close-range attacks.";
        }
        else if (longAtk > closeAtk + 3)
        {
            playerStyle = "The player is a cautious sniper who loves long-range attacks.";
        }
        else
        {
            playerStyle = "The player has a balanced attack style. ";
        }

        AttackTracker.ResetTrackers();
        string prompt = $"You are a cunning, sentient boss in a 2D fighting game. {playerStyle} " + "Generate a 10-step attack sequence to COUNTER this style. " +
        $"Use ONLY these attack words: {aiVocab}. " + "Respond with ONLY the comma-separated list of 10 attack words. " + "Example: Dash,Melee,Guard,Heal,Melee,Dash,Dash,Guard,Melee,Heal";

        string jsonBody = $@"{{'contents': [{{ 'parts': [{{ 'text': '{prompt}' }}] }}],
            'generationConfig': {{ 'temperature': 1.0, 'maxOutputTokens': 150 }}}}";

        using (UnityWebRequest request = new UnityWebRequest(uri + API_KEY, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                BackupPlan();
            }
            else
            {
                string responseText = request.downloadHandler.text;
                try
                {
                    string start = "\"text\": \"";
                    int startIndex = responseText.IndexOf(start) + start.Length;
                    int endIndex = responseText.IndexOf("\"", startIndex);
                    string rawPlan = responseText.Substring(startIndex, endIndex - startIndex);

                    rawPlan = rawPlan.Replace("\\n", "").Replace(" ", "");
                    attackPlan.AddRange(rawPlan.Split(','));
                    Debug.Log("AI Generated Plan: " + rawPlan);
                }
                catch
                {
                    BackupPlan();
                }
            }
        }
    }
    void BackupPlan()
    {
        attackPlan.AddRange(new string[] { "Dash", "Melee", "Guard", "Heal", "Melee", "Dash", "Melee", "Guard" });
    }

    IEnumerator ExecuteAtk()
    {
        while(true) 
        {
            yield return new WaitUntil(() => bossController.isBusy == false);
            yield return new WaitForSeconds(0.5f); 
            if (attackPlan.Count == 0) BackupPlan(); 
            string move = attackPlan[Random.Range(0, attackPlan.Count)];
            switch (move.Trim())
            {
                case "Melee":
                    bossController.DoMeleeAtk();
                    break;
                case "Dash":
                    bossController.DoDash();
                    break;
                case "Guard":
                    bossController.DoGuard();
                    break;
                case "Heal":
                    bossController.DoHeal();
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
    }
}
