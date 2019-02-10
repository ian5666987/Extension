using System;
using System.Collections.Generic;

namespace Extension.Algorithms {
  public class EloCalculator {
    public const int DefaultKFactor = 40;
    public const int DefaultDivider = 400;
    public const int DefaultScaleValue = 10;
    public int KFactor { get; private set; }
    public int Divider { get; private set; }
    public int ScaleValue { get; private set; }
    public int AddMemberEloBase { get; set; } = 100;

    public EloCalculator() : this(DefaultKFactor, DefaultDivider, DefaultScaleValue) { }
    public EloCalculator(int kFactor) : this(kFactor, DefaultDivider, DefaultScaleValue) { }
    public EloCalculator(int kFactor, int divider) : this(kFactor, divider, DefaultScaleValue) { }
    public EloCalculator(int kFactor, int divider, int scaleValue) {
      KFactor = kFactor;
      Divider = divider;
      ScaleValue = scaleValue;
    }

    public double GetScoreEstimation(double elo1, double elo2) {
      return 1.0 / (1 + Math.Pow(ScaleValue, (elo2 - elo1) / DefaultDivider));
    }

    public double GetWinElo(double elo1, double elo2) {
      return KFactor * (1 - GetScoreEstimation(elo1, elo2));
    }

    public double GetDrawElo(double elo1, double elo2) {
      return GetWinElo(elo1, elo2) + GetLoseElo(elo1, elo2);
    }

    public double GetLoseElo(double elo1, double elo2) {
      return KFactor * -GetScoreEstimation(elo1, elo2);
    }

    public double GetAdditionalTeamElo(int member) {
      if (member < 2)
        return 0;
      //member = 2 -> 1, 3 -> 3, 4 -> 6
      //2 -> 1*1 = 1
      //3 -> 1.5*2 = 3
      //4 -> 2*3 = 6
      //5 -> 2.5*4 = 10
      return member / (0.5 * member * (member - 1)) * AddMemberEloBase;
    }

    public List<double> GetAdditionalTeamEloList(int member) {
      List<double> results = new List<double> { 0, 0 };
      if (member < 2)
        return results;
      for (int i = 2; i <= member; ++i) {
        double prevElo = results[i - 1];
        double addElo = GetAdditionalTeamElo(i);
        results.Add(prevElo + addElo);
      }
      return results;
    }
  }
}
