namespace Core.VisualNovel.Plugin {
    public struct PluginDescription {
        public string Name { get; }
        public string[] Parameters { get; }
        public PluginIdentifier Identifier { get; }

        public PluginDescription(string name, string[] parameters, PluginIdentifier identifier) {
            Name = name;
            Parameters = parameters;
            Identifier = identifier;
        }
    }
}