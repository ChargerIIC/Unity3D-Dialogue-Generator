using System;
using UnityEngine;

public class DialogueEntry
{
	public String name;
	public String longText;
	public Texture2D img;
	public DialogueChoice[] choices;
	public int choiceMode = 1;
	public Link[] links;
	public int next;
	public bool editor;
	public int pos = 0;
	// 0 - top, 1 - bottom, 2 - middle
	public DialogueChoice[] passwords;
	public int incorrect;
	public DialogueChoice exit;
	public int align;
	public int mode;
	public AudioClip[] narration;
	public String script;
	// 0 - next, 1 - choice, 2 - password, 3 - event, 4 - end
	public DialogueEntry () {
		name = "Name";
		longText = "New entry";
		next = 0;
		editor = false;
		incorrect = 0;
		align = 0;
		exit = new DialogueChoice("Exit");
		mode = 0;
	}
}
