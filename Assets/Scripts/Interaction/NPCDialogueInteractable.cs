using UnityEngine;

namespace Scripts.Interaction
{
    public class NPCDialogueInteractable : Interactable
    {
        [Header("NPC Dialogue Settings")]
        [SerializeField] private string npcName = "NPC";
        [SerializeField] private string[] dialogueLines = { "Hello!" };
        [SerializeField] private bool randomizeDialogue = false;
        
        private int currentDialogueIndex = 0;
        
        public override void Interact()
        {
            string messageToShow = GetDialogueLine();
            Debug.Log($"[{npcName}]: {messageToShow}");
            
            // Move to next dialogue line for next interaction
            if (!randomizeDialogue)
            {
                currentDialogueIndex = (currentDialogueIndex + 1) % dialogueLines.Length;
            }
        }
        
        private string GetDialogueLine()
        {
            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                return "...";
            }
            
            if (randomizeDialogue)
            {
                int randomIndex = Random.Range(0, dialogueLines.Length);
                return dialogueLines[randomIndex];
            }
            else
            {
                return dialogueLines[currentDialogueIndex];
            }
        }
        
        // Optional: Reset dialogue to beginning
        public void ResetDialogue()
        {
            currentDialogueIndex = 0;
        }
        
        // Optional: Set specific dialogue line
        public void SetDialogueLine(int index)
        {
            if (index >= 0 && index < dialogueLines.Length)
            {
                currentDialogueIndex = index;
            }
        }
        
        // Optional: Add new dialogue line at runtime
        public void AddDialogueLine(string newLine)
        {
            var newDialogueArray = new string[dialogueLines.Length + 1];
            for (int i = 0; i < dialogueLines.Length; i++)
            {
                newDialogueArray[i] = dialogueLines[i];
            }
            newDialogueArray[dialogueLines.Length] = newLine;
            dialogueLines = newDialogueArray;
        }
    }
}