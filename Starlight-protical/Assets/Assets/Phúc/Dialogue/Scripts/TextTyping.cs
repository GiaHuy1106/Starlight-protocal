using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using TMPro;
using System.Collections.Generic;
public class TextTyping : MonoBehaviour
{
    public Dialogue dialogue; // Tham chiếu đến ScriptableObject Dialogue
    public float typingSpeed = 0.05f; // Tốc độ gõ chữ
    public bool LookAtTarget;
    [ShowIf("LookAtTarget")]
    public Transform target;
    [ShowIf("LookAtTarget")]
    public Transform Canvas;
    [ShowIf("LookAtTarget")]
    public bool ReverseDirect;
    public TextMeshProUGUI text;
    public float durationPerLine = 2f;
    int currentLineIndex = 0;
    public enum TypingStyle 
    {
        Char,
        Word
    }
    public TypingStyle typingStyle;


    private void Start()
    {
        ResetTyping();
    }


    private void Update()
    {
        if(LookAtTarget && target != null)
        {
            var direct = target.position - this.transform.position;
            if(ReverseDirect)
             direct = this.transform.position - target.position;           
            Canvas.LookAt(target, direct);
        }
    }


    public void TypingByIndex(int index)
    {
        string line = dialogue.dialogs[index];
        text.text = "";
        if(typingStyle == TypingStyle.Char)
            StartTypingChar(line);
        else
            StartTypingWord(line);
    }

    public void TypingByCurrentLine()
    {
        text.text = "";
        if(currentLineIndex < dialogue.dialogs.Count)
        {
            string line = dialogue.dialogs[currentLineIndex];
            if(typingStyle == TypingStyle.Char)
                StartTypingChar(line);
            else
                StartTypingWord(line);
            currentLineIndex++;
        }
    }

    public void SkipTyping()
    {
        StopAllCoroutines();
        if(currentLineIndex > 0 && currentLineIndex <= dialogue.dialogs.Count)
        {
            text.text = dialogue.dialogs[currentLineIndex - 1];
        }
    }

    public void TypingFull(float duration)
    {
        if(text != null)
        StartCoroutine(TypingFull( dialogue.dialogs, duration: durationPerLine));
    }




    public void ResetTyping()
    {
        text.text = "";
        currentLineIndex = 0;
    }

    public void StartTypingChar(string s)
    {
        if(text != null){
            text.text = "";
            StartCoroutine(TypingChar(s));
        }
    }
    public void StartTypingWord(string s)
    {
        if(text != null){
            text.text = "";
            StartCoroutine(TypingWord(s));
        }
    }

    IEnumerator TypingChar(string sequence)
    {      
            foreach (var c in sequence)
            {
                text.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }             
    }
    IEnumerator TypingWord(string sequence)
    {
        foreach (var s in sequence.Split(' '))
        {
            text.text += s + " ";            
            yield return new WaitForSeconds(typingSpeed);   
        }
        text.text = text.text.TrimEnd(); // Loại bỏ khoảng trắng cuối cùng
    }

    IEnumerator TypingFull(IEnumerable<string> conversation, float duration)
    {

        if(typingStyle == TypingStyle.Char)
        {
            foreach (var s in conversation)
            {
                float timing  = typingSpeed * s.Length;
                StartTypingChar(s);
                yield return new WaitForSeconds(durationPerLine + timing);
            }
        }
        else
        {
            foreach (var s in conversation)
            {
                float timing = typingSpeed * s.Split(' ').Length;
                StartTypingWord(s);
                yield return new WaitForSeconds(durationPerLine + timing);
            }

        }


    }

}
