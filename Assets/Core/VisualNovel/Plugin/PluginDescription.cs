namespace Core.VisualNovel.Plugin {
    public struct PluginDescription {
        public string Name { get; }
        public PluginIdentifier Identifier { get; }

        public PluginDescription(string name, PluginIdentifier identifier) {
            Name = name;
            Identifier = identifier;
        }
    }
}