namespace DcsTranslateTool.Core.Models;
public class RepoEntry( string name, string absolutePath, bool isDirectory ) {
    public string Name => name;
    public string AbsolutePath => absolutePath;
    public bool IsDirectory => isDirectory;
    //Mode = mode;
    //Type = type;
    //Size = size;
    //Sha = sha;
    //Url = url;
}
