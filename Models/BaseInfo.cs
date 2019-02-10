namespace Extension.Models {
  public abstract class BaseInfo {
    public bool IsValid { get; protected set; } //This does not mean to be checked here, only to be provided
    public string UntrimmedOriginalDesc { get; protected set; }
    public string OriginalDesc { get; protected set; } //So does this

    public BaseInfo(string desc) {
      UntrimmedOriginalDesc = desc;
      OriginalDesc = desc?.Trim(); //Note that original description is trimmed while the untrimmed is named untrimmed
    }
  }
}
