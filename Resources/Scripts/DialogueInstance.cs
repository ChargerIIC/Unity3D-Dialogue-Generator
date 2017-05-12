// Copyright 2010 Royce Kimmons | royce@kimmonsdesign.com
// Released under Creative Commons Attribution 3.0 License
// http://creativecommons.org/licenses/by/3.0/
// Conversion by Vernon Burt
public GUISkin dialogueSkin;
public int startOn  = 0;
public Texture2D defaultImg;
public AudioClip defaultAudio;
public DialogueEntry[] dialogue = [new DialogueEntry()];
public DialogueEntry display = new DialogueEntry();
public bool[] bools  = [false,false,false,false];
public int[] ints = [0,0,0,0];
public String[] strings = ["","","",""];


private float waitTime = 0.2;
private int lineCount = 0;
private float timeStart = 0.0;
private float textSpeed = 80.0;
private String[] parsedText;
private String pwEntry = string.Empty;
private int toLoad = -1;
private int curItem = 0;
private Array history = new Array();
private int phase = 0;
private GUIContent curContent = GUIContent("");
private float glow = 0;
private AudioSource aud;
private var jumpto : int;
private var s : Vector2 = Vector2.zero;

void Start () {
	Restart ();
}

void OnEnable () {
	Restart();
}

void OnDisable () {
	if (aud) {
	if (aud.clip) {
		if (aud.isPlaying) aud.Stop();
	}
	}
}

void Update () {
	var text = parsedText[lineCount];
	var chars = (Time.time-timeStart)*textSpeed;
	if (chars<text.length) text = text.Substring(0,chars);
	var img : Texture2D;
	if (defaultImg) img = defaultImg;
	if (display.img) img = display.img;
	curContent = GUIContent(text);
	switch (display.mode) {
		case 2:
		if (Input.GetKeyDown("return")) EvaluatePassword();
		break;
	}
	glow = Mathf.PingPong(Time.time/4,.4)+.6;
}

void OnGUI () {
if (dialogue) {
if (dialogue.length>0) {
if (display) {
GUI.skin = dialogueSkin;
if (curContent) {
	switch (display.pos) {
		case 0:
			GUILayout.BeginHorizontal("textbox",GUILayout.Width(Screen.width));
			if (display.align==0&&curContent.image) GUILayout.Label(curContent.image,"img");
			GUILayout.Label(curContent.text,"text");
			if (display.align==1&&curContent.image) GUILayout.Label(curContent.image,GUILayout.Width(256));
			GUILayout.EndHorizontal();
		break;
		case 1:
		var p = 0;
		if (display.align == 1) p = Screen.width-400;
		var p2 = 20;
		if (display.align == 1) p2 = Screen.width-220;
			GUI.Label(Rect(p,Screen.height-517,400,400),display.img);
			GUI.Box(Rect(p2,Screen.height-41-117,200,41),display.name,"namebar");
			GUI.Box(Rect(0,Screen.height-120,Screen.width,120),curContent,"textboxplayer");
		break;
		case 2:
			GUI.Box(Rect(30,Screen.height/2-60,Screen.width-60,120),curContent,"textboxmiddle");		
		break;
	}
/*	for (var l in display.links) {
		GUILayout.Button(l.shortText);	
	}*/
	DoNextButton();
}
	if (display.choices) {
		if (display.choices.length>0&&lineCount>=parsedText.length-1&&display.mode==1) {
			if (display.choiceMode==1) {
				ShowWheel();
			} else {
				ShowList();	
			}
		}
	}
	}
	}
	}
}

void DoNextButton () {
	switch(display.mode) {
	case 0:
	if (lineCount<parsedText.length-1) {
		if (GUI.Button(Rect(Screen.width-84,Screen.height-84,64,64),"Next","arrow")) ProgressLineCount();			
		} else {
		if (GUI.Button(Rect(Screen.width-84,Screen.height-84,64,64),"Next","arrow")) LoadDialogue(display.next);
		}
	break;	
	case 1:
	if (lineCount<parsedText.length-1) {
		if (GUI.Button(Rect(Screen.width-84,Screen.height-84,64,64),"Next","arrow")) ProgressLineCount();
		}
	break;
	case 2:
		if (lineCount<parsedText.length-1) {
		if (GUI.Button(Rect(Screen.width-84,Screen.height-84,64,64),"Next","arrow")) ProgressLineCount();
		} else {
		ShowPassword();
		}	
	break;
	case 3:
		if (lineCount<parsedText.length-1) {
		if (GUI.Button(Rect(Screen.width-84,Screen.height-84,64,64),"Next","arrow")) ProgressLineCount();
		} else {
		if (GUI.Button(Rect(Screen.width-84,Screen.height-84,64,64),"Next","arrow")) {
			var d = gameObject.GetComponent("DialogueInstance");
			eval(display.script);
		}
		}
	break;
	case 4:
		if (lineCount<parsedText.length-1) {
		if (GUI.Button(Rect(Screen.width-84,Screen.height-84,64,64),"Next","arrow")) ProgressLineCount();
		} else {
		if (GUI.Button(Rect(Screen.width-84,Screen.height-84,64,64),"Next","arrow")) EndDialogue();
		}
	break;
	}
}

void Restart() {
	LoadDialogue(startOn);
	timeStart = Time.time;
	lineCount = 0;
}

void LoadDialogue (int i) {
	curItem = i;
	timeStart = Time.time;
	display = new DialogueEntry();
	display = dialogue[i];
	lineCount = 0;
	ParseText(display.longText);
	if (!aud) {
		gameObject.AddComponent(AudioSource);
		aud = gameObject.GetComponent(AudioSource);
	}
	PlayClip(i);
}

void ParseText (String s) {
	parsedText = s.Split("|"[0]);
}

void EndDialogue() {
	this.enabled = false;
}

void ShowPassword () {
	pwEntry = GUI.TextField(Rect(Screen.width/2-200,Screen.height/2+30,380,60),pwEntry,"inputbox");
	if (pwEntry!="") {
		StartGlow();
		if (GUI.Button(Rect(Screen.width/2+170,Screen.height/2+30,64,64),"","inputboxbutton")) EvaluatePassword();
		EndGlow();
	}
	if (GUI.Button(Rect(20,Screen.height-84,Screen.width-40,64),display.exit.shortText,"exit")) LoadDialogue(display.exit.next);
}

void EvaluatePassword () {
if (pwEntry!="") {
	var validate : boolean = false;
	for (var p in display.passwords) {
		if (pwEntry==p.shortText&&validate==false) {
		LoadDialogue(p.next);
		validate = true;
		}
	}
	if (validate == false) {
		LoadDialogue(display.incorrect);		
	}
	pwEntry = "";
	}
}

void ShowWheel () {
GUI.Label(Rect(Screen.width/2-140,Screen.height-163,280,133),"","wheelbase");
		if (display.choices.length>0) {
			if (GUI.Button(Rect(0,Screen.height-163,Screen.width/2,43),display.choices[0].shortText,"r1c1active")) toLoad=display.choices[0].next;	
		} else {
			GUI.Label(Rect(0,Screen.height-163,Screen.width/2,43),"");
		}		
		if (display.choices.length>1) {
			if (GUI.Button(Rect(Screen.width/2,Screen.height-163,Screen.width/2,43),display.choices[1].shortText,"r1c2active")) toLoad=display.choices[1].next;	
		} else {
			GUI.Label(Rect(Screen.width/2,Screen.height-163,Screen.width/2,43),"");
		}
		if (display.choices.length>2) {
			if (GUI.Button(Rect(0,Screen.height-120,Screen.width/2,40),display.choices[2].shortText,"r2c1active")) toLoad=display.choices[2].next;	
		} else {
			GUI.Label(Rect(0,Screen.height-120,Screen.width/2,40),"");
		}
		if (display.choices.length>3) {
			if (GUI.Button(Rect(Screen.width/2,Screen.height-120,Screen.width/2,40),display.choices[3].shortText,"r2c2active")) toLoad=display.choices[3].next;	
		} else {
			GUI.Label(Rect(Screen.width/2,Screen.height-120,Screen.width/2,40),"");
		}
		
		if (display.choices.length>4) {
			if (GUI.Button(Rect(0,Screen.height-80,Screen.width/2,51),display.choices[4].shortText,"r3c1active")) toLoad=display.choices[4].next;	
		} else {
			GUI.Label(Rect(0,Screen.height-80,Screen.width/2,51),"");
		}
		if (display.choices.length>5) {
			if (GUI.Button(Rect(Screen.width/2,Screen.height-80,Screen.width/2,51),display.choices[5].shortText,"r3c2active")) toLoad=display.choices[5].next;	
		} else {
			GUI.Label(Rect(Screen.width/2,Screen.height-80,Screen.width/2,59),"");
		}

}

void ShowList () {
	GUILayout.BeginArea(Rect(0,Screen.height-150,Screen.width,150),"","box");
	s = GUILayout.BeginScrollView(s);
	for (var c in display.choices) {
		if (GUILayout.Button(c.shortText,GUILayout.Height(30))) toLoad = c.next;	
	}
	GUILayout.EndScrollView();
	GUILayout.EndArea();
}

void ProgressLineCount () {
	timeStart = Time.time;
	lineCount++;
	PlayClip (curItem);
}

void FixedUpdate () {
	if (toLoad!=-1) {
		LoadDialogue(toLoad);
		s = Vector2.zero;
	}
	toLoad = -1;
}

void StartGlow () {
	GUI.color.a = glow;
}

void EndGlow () {
	GUI.color.a = 1.0;
}

void PlayClip (i:int) {
	if (aud.clip) {
		if (aud.isPlaying) aud.Stop();
	}
	if (dialogue[i].narration) {
		if (dialogue[i].narration.length>lineCount) {
			if (dialogue[i].narration[lineCount]) {
				aud.clip = dialogue[i].narration[lineCount];
				aud.loop = false;
				aud.Play();
			}
		}
	}
}
