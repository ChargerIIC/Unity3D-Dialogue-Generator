public class DialogueChoice {
	public String shortText;
	public int next;
	public boolean editor;

	DialogueChoice () {
		shortText = "New Choice";
		editor = false;
	}

	DialogueChoice (s:String) {
		shortText = s;
		editor = false;
	}
}
