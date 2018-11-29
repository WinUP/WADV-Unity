namespace Core.VisualNovel.Script.Compiler.Tokens {
    public enum TokenType {
        DialogueSpeaker,
        DialogueContent,
        LineBreak,
        String,
        Number,
        CreateScope,
        LeaveScope,
        CallStart,
        CallEnd,
        Variable,
        LeftBracket,
        RightBracket,
        
        PickChild,
        MinusEqual,
        Minus,
        AddEqual,
        Add,
        MultiplyEqual,
        Multiply,
        DivideEqual,
        Divide,
        GreaterEqual,
        Equal,
        Greater,
        LesserEqual,
        Lesser,
        
        LogicNot,
        LogicEqual,

        Scenario,
        If,
        ElseIf,
        Else,
        Loop,
        Language,
        Return
    }
}
