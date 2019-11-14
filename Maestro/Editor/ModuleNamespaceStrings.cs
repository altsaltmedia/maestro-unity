namespace AltSalt.Maestro
{
    public class ModuleNamespaceStrings
    {
        private string _name;

        public string name
        {
            get => _name;
            set => _name = value;
        }

        private string _path;

        public string path
        {
            get => _path;
            set => _path = value;
        }
            
        private string _editorPath;

        public string editorPath
        {
            get => _editorPath;
            set => _editorPath = value;
        }

        public ModuleNamespaceStrings(string name, string path)
        {
            this.name = name;
            this.path = path;
            this.editorPath = path + "Editor/";
        }
    }
}