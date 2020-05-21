using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager current;
    
    [SerializeField] private TMP_Text nameTextUI = null;
    [SerializeField] private TMP_Text dialogueTextUI = null;
    [SerializeField] private Image characterImageUI = null;
    [SerializeField] private Image BGImageUI = null;

    private Queue<string> sentences;
    private Queue<Sprite> sprites;
    private Coroutine typeCoroutine;

    private void Awake() 
    {
        current = this;
    }

    void Start()
    {
        BGImageUI.gameObject.SetActive(false);
        sentences = new Queue<string>();
        sprites = new Queue<Sprite>();
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        GameManager.Instance.isDialogueEnabled = true;
        GameManager.Instance.ActivateDialogueCamera();

        BGImageUI.gameObject.SetActive(true);

        //characterImageUI.sprite = dialogue.sprite;

        nameTextUI.text = dialogue.name;

        sentences.Clear();
        sprites.Clear();
        
        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            sentences.Enqueue(dialogue.sentences[i].sentence);
        }
        
        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            sprites.Enqueue(dialogue.sentences[i].sprite);
        }
        

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Sprite sprite = sprites.Dequeue();
        
        if(typeCoroutine != null)
            StopCoroutine(typeCoroutine);

        typeCoroutine = StartCoroutine(TypeSentence(sentence, sprite));
    }

    IEnumerator TypeSentence(string sentence, Sprite sprite)
    {
        characterImageUI.sprite = sprite;
        dialogueTextUI.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueTextUI.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        BGImageUI.gameObject.SetActive(false);

        GameManager.Instance.isDialogueEnabled = false;
        GameManager.Instance.ActivateNormalCamera();
    }

}
