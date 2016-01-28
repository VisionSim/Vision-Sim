/*
 * Copyright (c) Contributors, http://vision-sim.org/, http://whitecore-sim.org/, http://aurora-sim.org
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


using System;
using System.Collections.Generic;
using System.IO;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using Vision.Framework.ClientInterfaces;
using Vision.Framework.ConsoleFramework;
using Vision.Framework.DatabaseInterfaces;
using Vision.Framework.Servers.HttpServer;
using Vision.Framework.Servers.HttpServer.Implementation;
using Vision.Framework.Services;
using Vision.Framework.Utilities;

namespace Vision.Services
{
    public class GroupCAPS : ICapsServiceConnector
    {
        protected IGroupsServiceConnector m_groupService;
        protected IRegionClientCapsService m_service;

        public void RegisterCaps(IRegionClientCapsService service)
        {
            m_service = service;
            m_groupService = Framework.Utilities.DataManager.RequestPlugin<IGroupsServiceConnector>();

            service.AddStreamHandler("GroupMemberData",
                                     new GenericStreamHandler("POST", service.CreateCAPS("GroupMemberData", ""),
                                                              GroupMemberData));


            var apiUri = service.CreateCAPS("GroupAPIv1", "");
            service.AddStreamHandler ("GroupAPIv1", new GenericStreamHandler ("GET", apiUri, ProcessGetGroupAPI));
            service.AddStreamHandler ("GroupAPIv1", new GenericStreamHandler ("POST", apiUri, ProcessPostGroupAPI));

        }

        public void EnteringRegion()
        {
        }

        public void DeregisterCaps()
        {
            m_service.RemoveStreamHandler ("GroupMemberData", "POST");
            m_service.RemoveStreamHandler ("GroupAPIv1", "GET");
            m_service.RemoveStreamHandler ("GroupAPIv1", "POST");
        }

        #region Group Members

        public byte[] GroupMemberData(string path, Stream request, OSHttpRequest httpRequest,
                                      OSHttpResponse httpResponse)
        {
            try
            {
                OSDMap rm = (OSDMap) OSDParser.DeserializeLLSDXml(HttpServerHandlerHelpers.ReadFully(request));
                UUID groupID = rm["group_id"].AsUUID();

                OSDMap defaults = new OSDMap();
                ulong EveryonePowers = (ulong) (GroupPowers.Accountable |
                                                GroupPowers.AllowSetHome |
                                                GroupPowers.ReceiveNotices |
                                                GroupPowers.JoinChat |
                                                GroupPowers.AllowVoiceChat);
                defaults["default_powers"] = EveryonePowers;

                List<string> titles = new List<string>();
                OSDMap members = new OSDMap();
                int count = 0;
                foreach (GroupMembersData gmd in m_groupService.GetGroupMembers(m_service.AgentID, groupID))
                {
                    OSDMap member = new OSDMap();
                    member["donated_square_meters"] = gmd.Contribution;
                    member["owner"] = (gmd.IsOwner ? "Y" : "N");
                    member["last_login"] = gmd.OnlineStatus;
                    if (titles.Contains(gmd.Title))
                    {
                        member["title"] = titles.FindIndex((s) => s == gmd.Title);
                    }
                    else
                    {
                        titles.Add(gmd.Title);
                        member["title"] = titles.Count-1;
                    }
                    member["powers"] = gmd.AgentPowers;
                    count++;
                    members[gmd.AgentID.ToString()] = member;
                }

                OSDMap map = new OSDMap();
                map["member_count"] = count;
                map["group_id"] = groupID;
                map["defaults"] = defaults;
                map["titles"] = titles.ToOSDArray();
                map["members"] = members;
                return OSDParser.SerializeLLSDXmlBytes(map);
            }
            catch (Exception e)
            {
                MainConsole.Instance.Error("[Caps]: " + e);
            }

            return null;
        }


        public byte[] ProcessGetGroupAPI (string path, Stream request, OSHttpRequest httpRequest,
            OSHttpResponse httpResponse)
        {
            string groupID;
            if (httpRequest.QueryString["group_id"] != null)
            {
                groupID = httpRequest.QueryString["group_id"];
                MainConsole.Instance.Debug("[GroupAPIv1] Requesting groups bans for group_id: " + groupID);

                // Get group banned member list
                OSDMap bannedUsers = new OSDMap();

                foreach (GroupBannedAgentsData user in m_groupService.GetGroupBannedMembers(m_service.AgentID, (UUID) groupID))
                {
                    OSDMap banned = new OSDMap();
                    banned["ban_date"] = user.BanDate;  
                    bannedUsers[user.AgentID.ToString()] = banned;
                }

                OSDMap map = new OSDMap();
                map["group_id"] = groupID;
                map["ban_list"] = bannedUsers;
                return OSDParser.SerializeLLSDXmlBytes(map);

            }

            return null;
        }

        public byte[] ProcessPostGroupAPI (string path, Stream request, OSHttpRequest httpRequest,
            OSHttpResponse httpResponse)
        {
            string groupID;

            if (httpRequest.QueryString["group_id"] != null)
            {
                List<UUID> banUsers = new List<UUID>();

                groupID = httpRequest.QueryString["group_id"];

                string body = HttpServerHandlerHelpers.ReadString(request).Trim();
                OSDMap map = (OSDMap)OSDParser.DeserializeLLSDXml(body);

                MainConsole.Instance.Debug("[GroupAPIv1] Requesting a POST for group_id: " + groupID);

                if (map.ContainsKey ("ban_ids"))
                    banUsers = ((OSDArray) map["ban_ids"]).ConvertAll<UUID>(o => o);

                if (map.ContainsKey ("ban_action"))
                {
                    if (map ["ban_action"].AsInteger () == 1)
                    {
                        m_groupService.AddGroupBannedAgent (m_service.AgentID, (UUID) groupID, banUsers);
                        return null;
                    }
                    if (map ["ban_action"].AsInteger () == 2)
                    {
                        m_groupService.RemoveGroupBannedAgent (m_service.AgentID, (UUID) groupID, banUsers);
                        return null;
                    }
                }

                // get banned agent details
                var banUser = m_groupService.GetGroupBannedUser(m_service.AgentID, (UUID) groupID, banUsers[0]);
                OSDMap retMap = new OSDMap();
                retMap["group_id"] = groupID;

                OSDMap banned = new OSDMap();
                banned["ban_date"] = banUser.BanDate;               
                banned[banUser.AgentID.ToString()] = banned;

                retMap["ban_list"] = banned;

                return OSDParser.SerializeLLSDXmlBytes(retMap);

            }
            return null;
        }

        #endregion
    }
}
