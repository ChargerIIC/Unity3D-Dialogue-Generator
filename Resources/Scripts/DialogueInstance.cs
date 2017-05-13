// Copyright 2010 Royce Kimmons | royce@kimmonsdesign.com
// Released under Creative Commons Attribution 3.0 License
// http://creativecommons.org/licenses/by/3.0/
// Conversion by Vernon Burt

using System;
using UnityEngine;

public class DialohueInstance : MonoBehaviour
{
    public GUISkin dialogueSkin;
    public int startOn = 0;
    public Texture2D defaultImg;
    public AudioClip defaultAudio;
    public DialogueEntry[] dialogue =  {new DialogueEntry()};
    public DialogueEntry display = new DialogueEntry();
    public bool[] bools = new[]{false, false, false, false};
    public int[] ints =  {0, 0, 0, 0};
    public String[] strings =  {"", "", "", ""};


    private float waitTime = 0.2f;
    private int lineCount = 0;
    private float timeStart = 0.0f;
    private float textSpeed = 80.0f;
    private String[] parsedText;
    private String pwEntry = string.Empty;
    private int toLoad = -1;
    private int curItem = 0;
    //private Array history = new Array();
    private int phase = 0;
    private GUIContent curContent = new GUIContent("");
    private float glow = 0;
    private AudioSource aud;
    private int jumpto;
    private Vector2 s = Vector2.zero;

    void Start()
    {
        Restart();
    }

    void OnEnable()
    {
        Restart();
    }

    void OnDisable()
    {
        if (aud)
        {
            if (aud.clip)
            {
                if (aud.isPlaying) aud.Stop();
            }
        }
    }

    void Update()
    {
        var text = parsedText[lineCount];
        var chars = (Time.time - timeStart) * textSpeed;
        if (chars < text.Length) text = text.Substring(0, Convert.ToInt32(chars));
        Texture2D img;
        if (defaultImg)
            img = defaultImg;

        if (display.img)
            img = display.img;

        curContent = new GUIContent(text);
        switch (display.mode)
        {
            case 2:
                if (Input.GetKeyDown("return")) EvaluatePassword();
                break;
        }
        glow = Mathf.PingPong((Time.time / 4.0f), 0.4f) + 0.6f;
    }

    void OnGUI()
    {
        if (dialogue != null)
        {
            if (dialogue.Length > 0)
            {
                if (display!=null)
                {
                    GUI.skin = dialogueSkin;
                    if (curContent!=null)
                    {
                        switch (display.pos)
                        {
                            case 0:
                                GUILayout.BeginHorizontal("textbox", GUILayout.Width(Screen.width));
                                if (display.align == 0 && curContent.image) GUILayout.Label(curContent.image, "img");
                                GUILayout.Label(curContent.text, "text");
                                if (display.align == 1 && curContent.image)
                                    GUILayout.Label(curContent.image, GUILayout.Width(256));
                                GUILayout.EndHorizontal();
                                break;
                            case 1:
                                var p = 0;
                                if (display.align == 1) p = Screen.width - 400;
                                var p2 = 20;
                                if (display.align == 1) p2 = Screen.width - 220;
                                GUI.Label(new Rect(p, Screen.height - 517, 400, 400), display.img);
                                GUI.Box(new Rect(p2, Screen.height - 41 - 117, 200, 41), display.name, "namebar");
                                GUI.Box(new Rect(0, Screen.height - 120, Screen.width, 120), curContent, "textboxplayer");
                                break;
                            case 2:
                                GUI.Box(new Rect(30, Screen.height / 2 - 60, Screen.width - 60, 120), curContent,
                                    "textboxmiddle");
                                break;
                        }
/*	for (var l in display.links) {
		GUILayout.Button(l.shortText);	
	}*/
                        DoNextButton();
                    }
                    if (display.choices != null)
                    {
                        if (display.choices.Length > 0 && lineCount >= parsedText.Length - 1 && display.mode == 1)
                        {
                            if (display.choiceMode == 1)
                            {
                                ShowWheel();
                            }
                            else
                            {
                                ShowList();
                            }
                        }
                    }
                }
            }
        }
    }

    void DoNextButton()
    {
        switch (display.mode)
        {
            case 0:
                if (lineCount < parsedText.Length - 1)
                {
                    if (GUI.Button(new Rect(Screen.width - 84, Screen.height - 84, 64, 64), "Next", "arrow"))
                        ProgressLineCount();
                }
                else
                {
                    if (GUI.Button(new Rect(Screen.width - 84, Screen.height - 84, 64, 64), "Next", "arrow"))
                        LoadDialogue(display.next);
                }
                break;
            case 1:
                if (lineCount < parsedText.Length - 1)
                {
                    if (GUI.Button(new Rect(Screen.width - 84, Screen.height - 84, 64, 64), "Next", "arrow"))
                        ProgressLineCount();
                }
                break;
            case 2:
                if (lineCount < parsedText.Length - 1)
                {
                    if (GUI.Button(new Rect(Screen.width - 84, Screen.height - 84, 64, 64), "Next", "arrow"))
                        ProgressLineCount();
                }
                else
                {
                    ShowPassword();
                }
                break;
            case 3:
                if (lineCount < parsedText.Length - 1)
                {
                    if (GUI.Button(new Rect(Screen.width - 84, Screen.height - 84, 64, 64), "Next", "arrow"))
                        ProgressLineCount();
                }
                else
                {
                    if (GUI.Button(new Rect(Screen.width - 84, Screen.height - 84, 64, 64), "Next", "arrow"))
                    {
                        var d = gameObject.GetComponent("DialogueInstance");

                        //eval(display.script);
                    }
                }
                break;
            case 4:
                if (lineCount < parsedText.Length - 1)
                {
                    if (GUI.Button(new Rect(Screen.width - 84, Screen.height - 84, 64, 64), "Next", "arrow"))
                        ProgressLineCount();
                }
                else
                {
                    if (GUI.Button(new Rect(Screen.width - 84, Screen.height - 84, 64, 64), "Next", "arrow")) EndDialogue();
                }
                break;
        }
    }

    void Restart()
    {
        LoadDialogue(startOn);
        timeStart = Time.time;
        lineCount = 0;
    }

    void LoadDialogue(int i)
    {
        curItem = i;
        timeStart = Time.time;
        display = new DialogueEntry();
        display = dialogue[i];
        lineCount = 0;
        ParseText(display.longText);
        if (!aud)
        {
            gameObject.AddComponent<AudioSource>();
            aud = gameObject.GetComponent<AudioSource>();
        }
        PlayClip(i);
    }

    void ParseText(String s)
    {
        parsedText = s.Split("|"[0]);
    }

    void EndDialogue()
    {
        this.enabled = false;
    }

    void ShowPassword()
    {
        pwEntry = GUI.TextField(new Rect(Screen.width / 2 - 200, Screen.height / 2 + 30, 380, 60), pwEntry, "inputbox");
        if (pwEntry != "")
        {
            StartGlow();
            if (GUI.Button(new Rect(Screen.width / 2 + 170, Screen.height / 2 + 30, 64, 64), "",
                "inputboxbutton")) EvaluatePassword();
            EndGlow();
        }
        if (GUI.Button(new Rect(20, Screen.height - 84, Screen.width - 40, 64), display.exit.shortText, "exit"))
            LoadDialogue(display.exit.next);
    }

    void EvaluatePassword()
    {
        if (pwEntry != "")
        {
            bool validate = false;
            foreach (var p in display.passwords)
            {
                if (pwEntry == p.shortText && validate == false)
                {
                    LoadDialogue(p.next);
                    validate = true;
                }
            }
            if (validate == false)
            {
                LoadDialogue(display.incorrect);
            }
            pwEntry = "";
        }
    }

    void ShowWheel()
    {
        GUI.Label(new Rect(Screen.width / 2 - 140, Screen.height - 163, 280, 133), "", "wheelbase");
        if (display.choices.Length > 0)
        {
            if (GUI.Button(new Rect(0, Screen.height - 163, Screen.width / 2, 43), display.choices[0].shortText,
                "r1c1active")) toLoad = display.choices[0].next;
        }
        else
        {
            GUI.Label(new Rect(0, Screen.height - 163, Screen.width / 2, 43), "");
        }
        if (display.choices.Length > 1)
        {
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 163, Screen.width / 2, 43),
                display.choices[1].shortText, "r1c2active")) toLoad = display.choices[1].next;
        }
        else
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 163, Screen.width / 2, 43), "");
        }
        if (display.choices.Length > 2)
        {
            if (GUI.Button(new Rect(0, Screen.height - 120, Screen.width / 2, 40), display.choices[2].shortText,
                "r2c1active")) toLoad = display.choices[2].next;
        }
        else
        {
            GUI.Label(new Rect(0, Screen.height - 120, Screen.width / 2, 40), "");
        }
        if (display.choices.Length > 3)
        {
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 120, Screen.width / 2, 40),
                display.choices[3].shortText, "r2c2active")) toLoad = display.choices[3].next;
        }
        else
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 120, Screen.width / 2, 40), "");
        }

        if (display.choices.Length > 4)
        {
            if (GUI.Button(new Rect(0, Screen.height - 80, Screen.width / 2, 51), display.choices[4].shortText,
                "r3c1active")) toLoad = display.choices[4].next;
        }
        else
        {
            GUI.Label(new Rect(0, Screen.height - 80, Screen.width / 2, 51), "");
        }
        if (display.choices.Length > 5)
        {
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 80, Screen.width / 2, 51),
                display.choices[5].shortText, "r3c2active")) toLoad = display.choices[5].next;
        }
        else
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 80, Screen.width / 2, 59), "");
        }

    }

    void ShowList()
    {
        GUILayout.BeginArea(new Rect(0, Screen.height - 150, Screen.width, 150), "", "box");
        s = GUILayout.BeginScrollView(s);
        foreach (var c in display.choices) {
            if (GUILayout.Button(c.shortText, GUILayout.Height(30))) toLoad = c.next;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    void ProgressLineCount()
    {
        timeStart = Time.time;
        lineCount++;
        PlayClip(curItem);
    }

    void FixedUpdate()
    {
        if (toLoad != -1)
        {
            LoadDialogue(toLoad);
            s = Vector2.zero;
        }
        toLoad = -1;
    }

    void StartGlow()
    {
        var color = GUI.color;
        color.a = glow;
        GUI.color = color;
    }

    void EndGlow()
    {
        var color = GUI.color;
        color.a = 1.0f;
        GUI.color = color;
    }

    void PlayClip(int i)
    {
        if (aud.clip)
        {
            if (aud.isPlaying) aud.Stop();
        }
        if (dialogue[i].narration != null)
        {
            if (dialogue[i].narration.Length > lineCount)
            {
                if (dialogue[i].narration[lineCount])
                {
                    aud.clip = dialogue[i].narration[lineCount];
                    aud.loop = false;
                    aud.Play();
                }
            }
        }
    }
}
