/*
 * Copyright (c) Contributors, http://vision-sim.org/, http://whitecore-sim.org/, http://aurora-sim.org, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
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


using Vision.Framework.Modules;
using Vision.Framework.SceneInfo;

namespace Vision.Modules.Monitoring.Monitors
{
    public class AgentUpdateMonitor : IAgentUpdateMonitor
    {
        int agentTime;
        int agentUpdates;

        public AgentUpdateMonitor (IScene scene)
        {
        }

        #region Implementation of IMonitor

        public double GetValue ()
        {
            return agentUpdates;
        }

        public string GetName ()
        {
            return "Agent Update Count";
        }

        public string GetInterfaceName ()
        {
            return "IAgentUpdateMonitor";
        }

        public string GetFriendlyValue ()
        {
            return (int)GetValue () + " updates/sec";
        }

        #endregion

        #region IAgentUpdateMonitor Members

        public int AgentFrameTime {
            get { return agentTime; }
        }

        public int AgentUpdates {
            get { return agentUpdates; }
        }

        public void AddAgentUpdates (int value)
        {
            agentUpdates += value;
        }

        public void AddAgentTime (int value)
        {
            agentTime += value;
        }

        #endregion

        #region IMonitor Members

        public void ResetStats ()
        {
            agentUpdates = 0;
            agentTime = 0;
        }

        #endregion
    }
}
