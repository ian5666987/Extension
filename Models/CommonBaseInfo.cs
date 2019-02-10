namespace Extension.Models {
  public class CommonBaseInfo : BaseInfo {
    public string Name { get; protected set; }
    public string RightSide { get; private set; }
    public bool HasRightSide { get; private set; }
    public CommonBaseInfo(string desc) : base(desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      int index = desc.IndexOf('=');
      if (index <= 0) {//equal to zero is not allowed
        Name = desc; //take the whole description as Name
        IsValid = true; //will be true at this point
        return;
      }
      Name = desc.Substring(0, index).Trim(); //people may use extra space: time = go, by
      if (string.IsNullOrWhiteSpace(Name)) //at this point, if the name returned is null or whitespace, then return without saying that it is valid
        return;
      IsValid = true; //At this point, since name exists, it is a valid item
      if (desc.Length <= index + 1) //time= (index is 4, length is 5), if length is not greater than index + 1, no need to proceed
        return;
      RightSide = desc.Substring(index + 1).Trim();
      HasRightSide = !string.IsNullOrWhiteSpace(RightSide);
    }
  }
}
