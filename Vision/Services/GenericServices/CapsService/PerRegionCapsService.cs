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

using System.Collections.Generic;
using OpenMetaverse;
using Vision.Framework.Modules;
using Vision.Framework.Services;
using GridRegion = Vision.Framework.Services.GridRegion;

namespace Vision.Services
{
	/// <summary>
	///     This keeps track of what clients are in the given region
	/// </summary>
	public class PerRegionCapsService : IRegionCapsService
	{
		#region Declares

		UUID m_RegionID;
		GridRegion m_cachedRegion;

		protected Dictionary<UUID, IRegionClientCapsService> m_clientsInThisRegion =
			new Dictionary<UUID, IRegionClientCapsService> ();

		IRegistryCore m_registry;

		public IRegistryCore Registry {
			get { return m_registry; }
		}

		public ulong RegionHandle {
			get { return Region.RegionHandle; }
		}

		public int RegionX {
			get { return Region.RegionLocX; }
		}

		public int RegionY {
			get { return Region.RegionLocY; }
		}

		public GridRegion Region {
			get {
				if (m_cachedRegion != null)
					return m_cachedRegion;
                
				m_cachedRegion = Registry.RequestModuleInterface<IGridService> ().GetRegionByUUID (null, m_RegionID);
				return m_cachedRegion;
			}
		}

		#endregion

		#region Initialize

		/// <summary>
		///     Initialize the service
		/// </summary>
		/// <param name="regionID"></param>
		/// <param name="registry"></param>
		public void Initialize (UUID regionID, IRegistryCore registry)
		{
			m_RegionID = regionID;
			m_registry = registry;
		}

		public void Close ()
		{
			foreach (IRegionClientCapsService regionC in m_clientsInThisRegion.Values) {
				regionC.ClientCaps.RemoveCAPS (m_RegionID);
			}
			m_clientsInThisRegion.Clear ();
		}

		#endregion

		#region Add/Get/Remove clients

		/// <summary>
		///     Add this client to the region
		/// </summary>
		/// <param name="service"></param>
		public void AddClientToRegion (IRegionClientCapsService service)
		{
			if (!m_clientsInThisRegion.ContainsKey (service.AgentID))
				m_clientsInThisRegion.Add (service.AgentID, service);
			else //Update the client then... this shouldn't ever happen!
                m_clientsInThisRegion [service.AgentID] = service;
		}

		/// <summary>
		///     Remove the client from this region
		/// </summary>
		/// <param name="service"></param>
		public void RemoveClientFromRegion (IRegionClientCapsService service)
		{
			if (m_clientsInThisRegion.ContainsKey (service.AgentID))
				m_clientsInThisRegion.Remove (service.AgentID);
		}

		/// <summary>
		///     Get an agent's Caps by UUID
		/// </summary>
		/// <param name="agentID"></param>
		/// <returns></returns>
		public IRegionClientCapsService GetClient (UUID agentID)
		{
			if (m_clientsInThisRegion.ContainsKey (agentID))
				return m_clientsInThisRegion [agentID];
			return null;
		}

		/// <summary>
		///     Get all clients in this region
		/// </summary>
		/// <returns></returns>
		public List<IRegionClientCapsService> GetClients ()
		{
			return new List<IRegionClientCapsService> (m_clientsInThisRegion.Values);
		}

		#endregion
	}
}