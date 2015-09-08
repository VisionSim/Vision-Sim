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
 *     * Neither the name of the Vision Sim Project nor the
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


using Vision.Framework.DatabaseInterfaces;
using Vision.Framework.Modules;
using Vision.Framework.Services;
using Vision.Framework.Utilities;
using Nini.Config;
using System.Collections.Generic;

namespace Vision.Services.DataService
{
    public class LocalAssetConnector : ConnectorBase, IAssetConnector
    {
        private IGenericData GD;

        #region IAssetConnector Members

        public void Initialize(IGenericData GenericData, IConfigSource source, IRegistryCore simBase,
                               string defaultConnectionString)
        {
            GD = GenericData;

            if (source.Configs[Name] != null)
                defaultConnectionString = source.Configs[Name].GetString("ConnectionString", defaultConnectionString);

            if (GD != null)
                GD.ConnectToDatabase(defaultConnectionString, "Asset",
                                     source.Configs["UniverseConnectors"].GetBoolean("ValidateTables", true));

            Framework.Utilities.DataManager.RegisterPlugin(Name + "Local", this);

            if (source.Configs["UniverseConnectors"].GetString("AssetConnector", "LocalConnector") == "LocalConnector")
            {
                Framework.Utilities.DataManager.RegisterPlugin(this);
            }
        }

        public string Name
        {
            get { return "IAssetConnector"; }
        }

        [CanBeReflected(ThreatLevel = ThreatLevel.Low)]
        public void UpdateLSLData(string token, string key, string value)
        {
            object remoteValue = DoRemote(token, key, value);
            if (remoteValue != null || m_doRemoteOnly)
                return;
            if (FindLSLData(token, key).Count == 0)
            {
                GD.Insert("lslgenericdata", new[] {token, key, value});
            }
            else
            {
                Dictionary<string, object> values = new Dictionary<string, object>(1);
                values["ValueSetting"] = value;

                QueryFilter filter = new QueryFilter();
                filter.andFilters["KeySetting"] = key;

                GD.Update("lslgenericdata", values, null, filter, null, null);
            }
        }

        [CanBeReflected(ThreatLevel = ThreatLevel.Low)]
        public List<string> FindLSLData(string token, string key)
        {
            object remoteValue = DoRemote(token, key);
            if (remoteValue != null || m_doRemoteOnly)
                return (List<string>) remoteValue;

            QueryFilter filter = new QueryFilter();
            filter.andFilters["Token"] = token;
            filter.andFilters["KeySetting"] = key;
            return GD.Query(new string[1] {"*"}, "lslgenericdata", filter, null, null, null);
        }

        #endregion

        public void Dispose()
        {
        }
    }
}