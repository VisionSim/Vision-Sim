/*
 * Copyright (c) Contributors, http://vision-sim.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 * For an explanation of the license of each contributor and the content it 
 * covers please see the Licenses directory.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Vision-Sim Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.Linq;
using Vision.Framework.Modules;
using Vision.Framework.SceneInfo;

namespace Vision.Modules.Monitoring.Monitors
{
    class ScriptCountMonitor : IScriptCountMonitor
    {
        readonly IScene m_scene;

        public ScriptCountMonitor (IScene scene)
        {
            m_scene = scene;
        }

        #region Implementation of IMonitor

        public double GetValue ()
        {
            return 0;
        }

        public string GetName ()
        {
            return "Total Script Count";
        }

        public string GetInterfaceName ()
        {
            return "IScriptCountMonitor";
        }

        public string GetFriendlyValue ()
        {
            return ActiveScripts + " active script(s), " + ScriptEPS + " event(s) per second";
        }

        public void ResetStats ()
        {
        }

        #endregion

        #region IScriptCountMonitor Members

        public int ActiveScripts {
            get {
                IScriptModule [] modules = m_scene.RequestModuleInterfaces<IScriptModule> ();
                return modules.Where (module => module != null).Sum (module => module.GetActiveScripts ());
            }
        }

        public int ScriptEPS {
            get {
                IScriptModule [] modules = m_scene.RequestModuleInterfaces<IScriptModule> ();
                return modules.Where (module => module != null).Sum (module => module.GetScriptEPS ());
            }
        }

        #endregion
    }
}