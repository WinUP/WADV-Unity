namespace WADV.VisualNovel.Plugin {
    public struct PluginDescription {
        public string Name { get; }
        public VisualNovelPlugin Plugin { get; }

        public PluginDescription(string name, PluginIdentifier identifier, VisualNovelPlugin plugin) {
            Name = name;
            Plugin = plugin;
        }
    }
}