using System;
using System.Collections.Generic;

namespace Extension.Math {
  public class Probability {
    public static int GetGeneratedRandomIndex(Random rand, List<decimal> probability) {
      int randNo = 1 + rand.Next(100000); //to generate 1-100000
      decimal accummulatedProbability = 0;
      for (int i = 0; i < probability.Count; ++i) {
        int minVal = (int)(accummulatedProbability * 1000);
        accummulatedProbability += probability[i]; //so, the accummulated probability
        int accVal = (int)(accummulatedProbability * 1000);
        if (randNo > minVal && randNo <= accVal) //first check 1-25000, second one 25001 to 50000 and so on
          return i;
      }
      return -1; //not found
    }

    public static int GetGeneratedRandomIndex(Random rand, List<int> probability) {
      int randNo = 1 + rand.Next(100000); //to generate 1-100000
      int accummulatedProbability = 0;
      for (int i = 0; i < probability.Count; ++i) {
        int minVal = (int)(accummulatedProbability * 1000);
        accummulatedProbability += probability[i]; //so, the accummulated probability
        int accVal = (int)(accummulatedProbability * 1000);
        if (randNo > minVal && randNo <= accVal) //first check 1-25000, second one 25001 to 50000 and so on
          return i;
      }
      return -1; //not found
    }

    public static int GetGeneratedRandomIndex(Random rand, List<double> probability) {
      int randNo = 1 + rand.Next(100000); //to generate 1-100000
      double accummulatedProbability = 0;
      for (int i = 0; i < probability.Count; ++i) {
        int minVal = (int)(accummulatedProbability * 1000);
        accummulatedProbability += probability[i]; //so, the accummulated probability
        int accVal = (int)(accummulatedProbability * 1000);
        if (randNo > minVal && randNo <= accVal) //first check 1-25000, second one 25001 to 50000 and so on
          return i;
      }
      return -1; //not found
    }
  }
}
