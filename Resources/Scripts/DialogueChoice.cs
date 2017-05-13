using System;

public class DialogueChoice
{
    public String shortText;
    public int next;
    public bool editor;

    public DialogueChoice()
    {
        shortText = "New Choice";
        editor = false;
    }

    public DialogueChoice(string s)
    {
        shortText = s;
        editor = false;
    }
}
