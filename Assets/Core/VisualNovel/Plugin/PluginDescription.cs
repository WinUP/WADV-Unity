namespace Core.VisualNovel.Plugin {
    public struct PluginDescription {
        public string Name { get; }
        public IVisualNovelPlugin Plugin { get; }

        public PluginDescription(string name, PluginIdentifier identifier, IVisualNovelPlugin plugin) {
            Name = name;
            Plugin = plugin;
        }
    }
}