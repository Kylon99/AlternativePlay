using AlternativePlay.HarmonyPatches;
using AlternativePlay.Installers;
using AlternativePlay.Models;
using BS_Utils.Utilities;
using HarmonyLib;
using IPA;
using System.Reflection;
using Zenject;

namespace AlternativePlay
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class AlternativePlay
    {
        public const string assemblyName = "AlternativePlay";

        public static IPA.Logging.Logger Logger { get; private set; }

        [Init]
        public AlternativePlay(IPA.Logging.Logger logger)
        {
            AlternativePlay.Logger = logger;
            ProjectContext.Instance.Container.Install<AlternativePlayInstaller>();
            ProjectContext.Instance.Container.Resolve<Configuration>().LoadConfiguration();
        }

        [OnStart]
        public void Start()
        {
            var harmonyInstance = new Harmony("com.kylon99.beatsaber.alternativeplay");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
