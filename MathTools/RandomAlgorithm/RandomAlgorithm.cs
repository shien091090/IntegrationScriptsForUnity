using System.Collections.Generic;

namespace SNShien.Common.MathTools
{
    public static class RandomAlgorithm
    {
        public static int GetRandomNumberByWeight(Dictionary<int, int> weightSetting)
        {
            if (weightSetting == null || weightSetting.Count == 0)
                return 0;

            int totalWeight = 0;
            foreach (int weight in weightSetting.Values)
            {
                totalWeight += weight;
            }

            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int currentWeight = 0;
            foreach (KeyValuePair<int, int> weight in weightSetting)
            {
                currentWeight += weight.Value;
                if (randomValue < currentWeight)
                {
                    return weight.Key;
                }
            }

            return 0;
        }
    }
}