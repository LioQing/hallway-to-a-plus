using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class AnomalySettings
{
    public static AnomalyManager.Environment Environment;
    public static AnomalyManager.AnomalyType? Anomaly;
    public static int? StoryLevel;
    public static bool PrevTrapped;
    public static HashSet<AnomalyManager.AnomalyType> PastAnomalies = new();
    
    public static void NextEnvironment()
    {
        Environment = Environment switch
        {
            AnomalyManager.Environment.Normal => AnomalyManager.Environment.DistanceFade,
            AnomalyManager.Environment.DistanceFade => AnomalyManager.Environment.Torch,
            AnomalyManager.Environment.Torch => AnomalyManager.Environment.Lidar,
            AnomalyManager.Environment.Lidar => AnomalyManager.Environment.Normal,
            _ => throw new ArgumentOutOfRangeException(nameof(Environment), Environment, null)
        };
    }

    public static void NextAnomaly()
    {
        var availableAnomalies = Enum
            .GetValues(typeof(AnomalyManager.AnomalyType))
            .Cast<AnomalyManager.AnomalyType>()
            .Where(anomaly => !PastAnomalies.Contains(anomaly))
            .ToHashSet();
        
        if (Environment == AnomalyManager.Environment.Torch)
        {
            // Mannequin anomalies do not work in torch environment
            availableAnomalies.Remove(AnomalyManager.AnomalyType.LeftMannequin);
            availableAnomalies.Remove(AnomalyManager.AnomalyType.RightMannequin);
        }

        if (Environment == AnomalyManager.Environment.Lidar)
        {
            // TwChim anomalies do not work in lidar environment
            availableAnomalies.Remove(AnomalyManager.AnomalyType.LeftTwChim);
            availableAnomalies.Remove(AnomalyManager.AnomalyType.RightTwChim);
            
            // Shrink anomalies do not work in lidar environment
            availableAnomalies.Remove(AnomalyManager.AnomalyType.LeftShrink);
            availableAnomalies.Remove(AnomalyManager.AnomalyType.RightShrink);
        }
        
        var randomIndex = Random.Range(0, availableAnomalies.Count);
        var anomaly = availableAnomalies.ElementAt(randomIndex);
        
        PastAnomalies.Add(anomaly);
        
        if (PastAnomalies.Count == Enum.GetValues(typeof(AnomalyManager.AnomalyType)).Length)
        {
            PastAnomalies.Clear();
        }
        
        Anomaly = anomaly;
    }
}
