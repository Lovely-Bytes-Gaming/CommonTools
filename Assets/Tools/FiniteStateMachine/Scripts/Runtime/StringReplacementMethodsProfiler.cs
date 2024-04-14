using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Profiling;

public class StringReplacementMethodsProfiler : MonoBehaviour
{
#if UNITY_EDITOR
    
    private string manyMatches =
        "Dodge Stich Dodge Dodge Stich Dodge Stich Dodge Dodge Stich Dodge Stich Dodge Dodge Stich Dodge Stich Dodge Dodge Stich Dodge Stich Dodge Dodge Stich";

    private string singleMatch = "Dodge Stich Dodge Dodge";

    private const string pattern = "Stich";
    private const string replacement = "Hieb";
    
    private void Update()
    {
        Profiler.BeginSample("Single Match");
            Profile(singleMatch);
        Profiler.EndSample(); 
        
        Profiler.BeginSample("Many Matches");
            Profile(manyMatches);
        Profiler.EndSample();
    }

    private void Profile(string str)
    {
        Profiler.BeginSample("String Replace");
        str.Replace( pattern, replacement);
        Profiler.EndSample();
        
        Profiler.BeginSample("Regex Replace");
        Regex.Replace(str, pattern, replacement);
        Profiler.EndSample();

        Profiler.BeginSample("String Replace: Stress Test");
        for (int i = 0; i < 1_000; ++i)
            str.Replace( pattern, replacement);
        Profiler.EndSample();
        
        Profiler.BeginSample("Regex Replace: Stress Test");
        for (int i = 0; i < 1_000; ++i)
            Regex.Replace(str, pattern, replacement);
        Profiler.EndSample();
    }
#endif
}
