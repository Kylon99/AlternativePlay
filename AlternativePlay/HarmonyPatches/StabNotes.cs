using Harmony;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(NoteCutInfo.speedOK), MethodType.Getter)]
    [HarmonyPatch(typeof(NoteCutInfo))]
    internal class SpeedPatch
    {
        public SpeedPatch()
        {
        }

        private static void Postfix(ref bool __result)
        {
            Logging.Info($"*************** SpeedPatch Postfix: {__result}");
            if (ConfigOptions.instance.StabNotes)
            {
                Logging.Info("*************** Speed StabNotes On");
                // Set the speed of the cut to be always accepted
                __result = true;
            }
        }
    }

    [HarmonyPatch(nameof(NoteCutInfo.directionOK), MethodType.Getter)]
    [HarmonyPatch(typeof(NoteCutInfo))]
    internal class DirectionPatch
    {
        public DirectionPatch()
        {
        }

        private static void Postfix(ref bool __result)
        {
            Logging.Info($"*************** DirectionPatch Postfix: {__result}");
            if (ConfigOptions.instance.StabNotes)
            {
                Logging.Info("*************** Direction StabNotes On");
                // Set the direction of the note cut to always be accepted
                __result = true;
            }
        }
    }
}
