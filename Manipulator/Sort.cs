namespace Extension.Manipulator
{
  public class Sort
  {
    public static int[] BubbleSortByIndex(decimal[] unsortedSet, int setSize, bool isAscending = true) {
      if (unsortedSet == null || setSize <= 0)
        return null;
      int[] sortedSetIndex = new int[setSize];
      bool isSwapped;
      int tempSetIndex = 0;

      for (int i = 0; i < setSize; ++i)
        sortedSetIndex[i] = i;

      do {
        isSwapped = false;
        for (int i = 0; i < setSize - 1; ++i)
          if (isAscending && unsortedSet[sortedSetIndex[i + 1]] < unsortedSet[sortedSetIndex[i]]) { //ascending case
            tempSetIndex = sortedSetIndex[i];
            sortedSetIndex[i] = sortedSetIndex[i + 1];
            sortedSetIndex[i + 1] = tempSetIndex;
            isSwapped = true;
          } else if (!isAscending && unsortedSet[sortedSetIndex[i + 1]] > unsortedSet[sortedSetIndex[i]]) { //descneding case, if next data is higher than this data, swap
            tempSetIndex = sortedSetIndex[i];
            sortedSetIndex[i] = sortedSetIndex[i + 1];
            sortedSetIndex[i + 1] = tempSetIndex;
            isSwapped = true;
          }
      } while (isSwapped);

      return sortedSetIndex;
    }

    public static int[] BubbleSortByIndex(double[] unsortedSet, int setSize, bool isAscending = true) {
      if (unsortedSet == null || setSize <= 0)
        return null;
      int[] sortedSetIndex = new int[setSize];
      bool isSwapped;
      int tempSetIndex = 0;

      for (int i = 0; i < setSize; ++i)
        sortedSetIndex[i] = i;

      do {
        isSwapped = false;
        for (int i = 0; i < setSize - 1; ++i)
          if (isAscending && unsortedSet[sortedSetIndex[i + 1]] < unsortedSet[sortedSetIndex[i]]) { //ascending case
            tempSetIndex = sortedSetIndex[i];
            sortedSetIndex[i] = sortedSetIndex[i + 1];
            sortedSetIndex[i + 1] = tempSetIndex;
            isSwapped = true;
          } else if (!isAscending && unsortedSet[sortedSetIndex[i + 1]] > unsortedSet[sortedSetIndex[i]]) { //descending case
            tempSetIndex = sortedSetIndex[i];
            sortedSetIndex[i] = sortedSetIndex[i + 1];
            sortedSetIndex[i + 1] = tempSetIndex;
            isSwapped = true;
          }
      } while (isSwapped);

      return sortedSetIndex;
    }

    public static int[] BubbleSortByIndex(float[] unsortedSet, int setSize, bool isAscending = true) {
      double[] unsortedSetDouble = new double[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDouble[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDouble, setSize, isAscending);
    }

    public static int[] BubbleSortByIndex(ulong[] unsortedSet, int setSize, bool isAscending = true) {
      decimal[] unsortedSetDecimal = new decimal[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDecimal[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDecimal, setSize, isAscending);
    }

    public static int[] BubbleSortByIndex(long[] unsortedSet, int setSize, bool isAscending = true) {
      decimal[] unsortedSetDecimal = new decimal[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDecimal[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDecimal, setSize, isAscending);
    }

    public static int[] BubbleSortByIndex(uint[] unsortedSet, int setSize, bool isAscending = true) {
      double[] unsortedSetDouble = new double[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDouble[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDouble, setSize, isAscending);
    }

    public static int[] BubbleSortByIndex(int[] unsortedSet, int setSize, bool isAscending = true) {
      double[] unsortedSetDouble = new double[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDouble[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDouble, setSize, isAscending);
    }

    public static int[] BubbleSortByIndex(ushort[] unsortedSet, int setSize, bool isAscending = true) {
      double[] unsortedSetDouble = new double[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDouble[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDouble, setSize, isAscending);
    }

    public static int[] BubbleSortByIndex(short[] unsortedSet, int setSize, bool isAscending = true) {
      double[] unsortedSetDouble = new double[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDouble[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDouble, setSize, isAscending);
    }

    public static int[] BubbleSortByIndex(byte[] unsortedSet, int setSize, bool isAscending = true) {
      double[] unsortedSetDouble = new double[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDouble[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDouble, setSize, isAscending);
    }

    public static int[] BubbleSortByIndex(sbyte[] unsortedSet, int setSize, bool isAscending = true) {
      double[] unsortedSetDouble = new double[setSize];
      for (int i = 0; i < setSize; ++i)
        unsortedSetDouble[i] = unsortedSet[i];
      return BubbleSortByIndex(unsortedSetDouble, setSize, isAscending);
    }

  }
}
