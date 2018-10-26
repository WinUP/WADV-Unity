namespace Assets.Core.VisualNovel.Script {
    public enum TokenizerState {
        InDialogue,
        InDialogueName,
        InDialogueContent,
        InCommand,
        InCommandName,
        InCommandFunction,
        InCommandParameterName,
        InCommandParameterValue,
        InVariableName
    }
}
