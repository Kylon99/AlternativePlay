using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace AlternativePlay.Models
{
    public class OpenVRManager
    {
        public CVRSystem System { get; private set; }

        public OpenVRManager()
        {
            var error = EVRInitError.None;
            this.System = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Other);

            if (error != EVRInitError.None)
            {
                AlternativePlay.Logger.Error($"Unable to initialize OpenVR with error: {error.ToString()}");
            }
        }
    }
}
