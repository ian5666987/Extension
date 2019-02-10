namespace Extension.Checker {
  class TreeNode {
		public static bool HasGrandChild(System.Windows.Forms.TreeNode treeNode) {
      if (treeNode == null)
        return false;
      int noOfDirectChildren = treeNode.GetNodeCount(false);
      int noOfDescendants = treeNode.GetNodeCount(true);
      return noOfDirectChildren < noOfDescendants && noOfDirectChildren > 0;
    }

		public static bool HasChild(System.Windows.Forms.TreeNode treeNode) {
      return treeNode != null && treeNode.GetNodeCount(false) > 0;
    }

		public static bool IsAFileExtension(System.Windows.Forms.TreeNode treeNode, string fileExtensionString) {
      return treeNode != null && !string.IsNullOrWhiteSpace(fileExtensionString)
        && treeNode.Text.Length > (fileExtensionString.Length + 1) //there must be at least fileFormat.Length + 1 (dot) + 1 char (filename) characters
        && (treeNode.Text.Substring(treeNode.Text.Length - fileExtensionString.Length) == fileExtensionString); //if the extension is correct
    }

		public static bool HasAFileExtensionChild(System.Windows.Forms.TreeNode treeNode, string fileExtensionString) {
      if (treeNode == null || treeNode.GetNodeCount(false) <= 0)
        return false;
			foreach (System.Windows.Forms.TreeNode tn in treeNode.Nodes)
        if (IsAFileExtension(tn, fileExtensionString))
          return true;
      return false;
    }
  }
}

    //public static bool HasGrandChild(TreeNode treeNode) {
    //  if (treeNode != null) {
    //    int noOfDirectChildren = treeNode.GetNodeCount(false);
    //    int noOfDescendants = treeNode.GetNodeCount(true);
    //    if (noOfDirectChildren < noOfDescendants && noOfDirectChildren > 0)
    //      return true;
    //  }
    //  return false;
    //}

    //public static bool HasChild(TreeNode treeNode) {
    //  if (treeNode != null)
    //    if (treeNode.GetNodeCount(false) > 0)
    //      return true;
    //  return false;
    //}

    //public static bool IsAFileExtension(TreeNode treeNode, string fileExtensionString) {
    //  if (treeNode != null && fileExtensionString != "")
    //    if (treeNode.Text.Length > (fileExtensionString.Length + 1)) //there must be at least fileFormat.Length + 1 (dot) + 1 char (filename) characters
    //      if (treeNode.Text.Substring(treeNode.Text.Length - fileExtensionString.Length) == fileExtensionString) //if the extension is correct
    //        return true;
    //  return false;
    //}

    //public static bool HasAFileExtensionChild(TreeNode treeNode, string fileExtensionString) {
    //  if (treeNode != null)
    //    if (treeNode.GetNodeCount(false) > 0)
    //      foreach (TreeNode tn in treeNode.Nodes)
    //        if (IsAFileExtension(tn, fileExtensionString))
    //          return true;        
    //  return false;
    //}

