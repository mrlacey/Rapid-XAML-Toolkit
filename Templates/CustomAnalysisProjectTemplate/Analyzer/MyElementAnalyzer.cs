using System;
using System.ComponentModel.Composition;
using RapidXaml;

namespace $ext_safeprojectname$
{
    [Serializable]
    [Export(typeof(ICustomAnalyzer))]
    public class MyElementAnalyzer : MarshalByRefObject, ICustomAnalyzer
    {
        // TODO: set the name of the element/type this analyzer will analyze.
        public string TargetType() => "MyElement";

        public AnalysisActions Analyze(RapidXamlElement element)
        {
            // TODO: Implement this analyzer.
            // More details at [LINK GOES HERE]
            return AnalysisActions.None;
        }
    }
}
