using UnityEngine;
using TMPro;
using System.Collections;

public class Text : MonoBehaviour
{
    public float delay = 0.3f;
    public float wait = 2f;
    private TMP_Text textComponent;
    private string fullMessage;
    private string[] words;
    public GameObject exit;
    public GameObject teleporter;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        fullMessage = textComponent.text;
        words = fullMessage.Split(' ');
        textComponent.text = "";
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShowWords());
    }

    IEnumerator ShowWords()
    {
        foreach (string word in words)
        {
            textComponent.text += word + " ";
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(wait);
        exit.SetActive(false);
        Collider2D col = teleporter.GetComponent<Collider2D>();
        col.enabled = true;
        gameObject.SetActive(false);
    }

    
}
