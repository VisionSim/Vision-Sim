﻿/*
 * Copyright (c) Contributors, http://vision-sim.org/, http://whitecore-sim.org/, http://aurora-sim.org/, http://opensimulator.org/
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

using Nini.Config;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using System;
using System.IO;
using Vision.Framework.Modules;
using Vision.Framework.SceneInfo;
using Vision.Framework.Servers.HttpServer;
using Vision.Framework.Servers.HttpServer.Implementation;
using Vision.Framework.Servers.HttpServer.Interfaces;
using Vision.Framework.Utilities;

namespace Vision.Modules.Caps
{
    public class GroupAPIv1 : INonSharedRegionModule
    {
        private IScene m_scene;

        #region INonSharedRegionModule Members

        public void Initialize(IConfigSource pSource)
        {
        }

        public void AddRegion(IScene scene)
        {
            m_scene = scene;
            m_scene.EventManager.OnRegisterCaps += RegisterCaps;
        }

        public void RemoveRegion(IScene scene)
        {
            m_scene.EventManager.OnRegisterCaps -= RegisterCaps;
        }

        public void RegionLoaded(IScene scene)
        {
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }

        public void Close()
        {
        }

        public string Name
        {
            get { return "GroupAPIv1"; }
        }

        #endregion

        public OSDMap RegisterCaps(UUID agentID, IHttpServer server)
        {
            OSDMap retVal = new OSDMap();
            retVal["GroupAPIv1"] = CapsUtil.CreateCAPS("GroupAPIv1", "");

            server.AddStreamHandler(new GenericStreamHandler("POST", retVal["GroupAPIv1"],
                                                 delegate(string path, Stream request,
                                                          OSHttpRequest httpRequest,
                                                          OSHttpResponse httpResponse)
                                                 { return ProcessGroupAPI(request, httpResponse, agentID); }));

            return retVal;
        }

        private byte[] ProcessGroupAPI(Stream request, OSHttpResponse httpResponse, UUID agentID)
        {
            //
            // This Caps is called with the following context:
            //
            // https://serverIP:serverport/cap/16d22b69-9754-446e-a7a5-8d7144c712dd?group_id=<groupUUID>
            //
            // It gets 2 POSTS send to it
            //
            // 'ban_action':i1 // Ban the user
            // 'ban_action':i2 // Unban the user
            //
            // When there's no action, it returns the values of the banned user and the time that they are banned
            //
            // This needs a new table in groups that keeps track of these
            // Suggestion would be: group_bans
            //
            // Fly-Man- 17 Jul 2015
            throw new NotImplementedException();
        }
    }
}
