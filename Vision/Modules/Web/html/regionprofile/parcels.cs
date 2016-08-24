﻿/*
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
using System.IO;
using OpenMetaverse;
using Vision.Framework.DatabaseInterfaces;
using Vision.Framework.Modules;
using Vision.Framework.SceneInfo;
using Vision.Framework.Servers.HttpServer.Implementation;
using Vision.Framework.Services;
using Vision.Framework.Utilities;
using RegionFlags = Vision.Framework.Services.RegionFlags;

namespace Vision.Modules.Web
{
    public class RegionParcelsPage : IWebInterfacePage
    {
        public string [] FilePath {
            get {
                return new []
                {
                    "html/regionprofile/parcels.html"
                };
            }
        }

        public bool RequiresAuthentication {
            get { return false; }
        }

        public bool RequiresAdminAuthentication {
            get { return false; }
        }

        public Dictionary<string, object> Fill (WebInterface webInterface, string filename, OSHttpRequest httpRequest,
                                               OSHttpResponse httpResponse, Dictionary<string, object> requestParameters,
                                               ITranslator translator, out string response)
        {
            response = null;
            var vars = new Dictionary<string, object> ();
            if (httpRequest.Query.ContainsKey ("regionid")) {
                var regionService = webInterface.Registry.RequestModuleInterface<IGridService> ();
                var region = regionService.GetRegionByUUID (null, UUID.Parse (httpRequest.Query ["regionid"].ToString ()));

                IEstateConnector estateConnector = Framework.Utilities.DataManager.RequestPlugin<IEstateConnector> ();
                var ownerUUID = UUID.Zero;
                string ownerName = "Unknown";
                if (estateConnector != null) {

                    EstateSettings estate = estateConnector.GetEstateSettings (region.RegionID);
                    if (estate != null) {
                        ownerUUID = estate.EstateOwner;
                        UserAccount estateOwnerAccount = null;
                        var accountService = webInterface.Registry.RequestModuleInterface<IUserAccountService> ();
                        if (accountService != null)
                            estateOwnerAccount = accountService.GetUserAccount (null, estate.EstateOwner);
                        ownerName = estateOwnerAccount == null ? "No account found" : estateOwnerAccount.Name;
                    }
                } else {
                    ownerUUID = UUID.Zero;
                    ownerName = "Unknown";
                }

                vars.Add ("OwnerUUID", ownerUUID);
                vars.Add ("OwnerName", ownerName);
                vars.Add ("RegionName", region.RegionName);
                vars.Add ("RegionLocX", region.RegionLocX / Constants.RegionSize);
                vars.Add ("RegionLocY", region.RegionLocY / Constants.RegionSize);
                vars.Add ("RegionSizeX", region.RegionSizeX);
                vars.Add ("RegionSizeY", region.RegionSizeY);
                vars.Add ("RegionType", region.RegionType);
                vars.Add ("RegionTerrain", region.RegionTerrain);
                vars.Add ("RegionOnline",
                    (region.Flags & (int)RegionFlags.RegionOnline) ==
                    (int)RegionFlags.RegionOnline
                    ? translator.GetTranslatedString ("Online")
                    : translator.GetTranslatedString ("Offline"));

                IDirectoryServiceConnector directoryConnector =
                    Framework.Utilities.DataManager.RequestPlugin<IDirectoryServiceConnector> ();
                if (directoryConnector != null) {
                    IUserAccountService accountService =
                        webInterface.Registry.RequestModuleInterface<IUserAccountService> ();
                    List<LandData> data = directoryConnector.GetParcelsByRegion (0, 10, region.RegionID, UUID.Zero,
                        ParcelFlags.None, ParcelCategory.Any);
                    List<Dictionary<string, object>> parcels = new List<Dictionary<string, object>> ();
                    string url = "../images/icons/no_parcel.jpg";

                    if (data != null) {
                        foreach (var p in data) {
                            Dictionary<string, object> parcel = new Dictionary<string, object> ();
                            parcel.Add ("ParcelNameText", translator.GetTranslatedString ("ParcelNameText"));
                            parcel.Add ("ParcelOwnerText", translator.GetTranslatedString ("ParcelOwnerText"));
                            parcel.Add ("ParcelUUID", p.GlobalID);
                            parcel.Add ("ParcelName", p.Name);
                            parcel.Add ("ParcelOwnerUUID", p.OwnerID);
                            parcel.Add ("ParcelSnapshotURL", url);
                            if (accountService != null) {
                                var account = accountService.GetUserAccount (null, p.OwnerID);
                                if (account != null)
                                    parcel.Add ("ParcelOwnerName", account.Name);
                                else
                                    parcel.Add ("ParcelOwnerName", translator.GetTranslatedString ("NoAccountFound"));
                            }

                            parcels.Add (parcel);
                        }
                    }

                    vars.Add ("ParcelInRegion", parcels);
                    vars.Add ("NumberOfParcelsInRegion", parcels.Count);
                }

                IWebHttpTextureService webTextureService = webInterface.Registry.
                    RequestModuleInterface<IWebHttpTextureService> ();
                if (webTextureService != null && region.TerrainMapImage != UUID.Zero)
                    vars.Add ("RegionImageURL", webTextureService.GetTextureURL (region.TerrainMapImage));
                else
                    vars.Add ("RegionImageURL", "../images/icons/no_terrain.jpg");

                vars.Add ("RegionInformationText", translator.GetTranslatedString ("RegionInformationText"));
                vars.Add ("OwnerNameText", translator.GetTranslatedString ("OwnerNameText"));
                vars.Add ("RegionLocationText", translator.GetTranslatedString ("RegionLocationText"));
                vars.Add ("RegionSizeText", translator.GetTranslatedString ("RegionSizeText"));
                vars.Add ("RegionNameText", translator.GetTranslatedString ("RegionNameText"));
                vars.Add ("RegionTypeText", translator.GetTranslatedString ("RegionTypeText"));
                vars.Add ("RegionTerrainText", translator.GetTranslatedString ("RegionTerrainText"));
                vars.Add ("RegionInfoText", translator.GetTranslatedString ("RegionInfoText"));
                vars.Add ("RegionOnlineText", translator.GetTranslatedString ("RegionOnlineText"));
                vars.Add ("NumberOfUsersInRegionText", translator.GetTranslatedString ("NumberOfUsersInRegionText"));
                vars.Add ("ParcelsInRegionText", translator.GetTranslatedString ("ParcelsInRegionText"));
                vars.Add ("MainServerURL", webInterface.GridURL);
            }

            return vars;
        }

        public bool AttemptFindPage (string filename, ref OSHttpResponse httpResponse, out string text)
        {
            httpResponse.ContentType = "text/html";
            text = File.ReadAllText ("html/regionprofile/parcels.html");
            return true;
        }
    }
}