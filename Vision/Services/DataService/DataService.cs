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

using Vision.DataManager.MySQL;
using Vision.DataManager.SQLite;

using Vision.Framework.ConsoleFramework;
using Vision.Framework.ModuleLoader;
using Vision.Framework.Modules;
using Vision.Framework.Services;
using Nini.Config;
using System;
using System.Collections.Generic;

namespace Vision.Services.DataService
{
    public class LocalDataService
    {
        private string ConnectionString = "";
        private string StorageProvider = "";

        public void Initialize(IConfigSource source, IRegistryCore simBase)
        {
            IConfig m_config = source.Configs["VisionData"];
            if (m_config != null)
            {
                StorageProvider = m_config.GetString("StorageProvider", StorageProvider);
                ConnectionString = m_config.GetString("ConnectionString", ConnectionString);
            }

            IGenericData DataConnector = null;
            if (StorageProvider == "MySQL")
                //Allow for fallback when VisionData isn't set
            {
                MySQLDataLoader GenericData = new MySQLDataLoader();

                DataConnector = GenericData;
            }
                /*else if (StorageProvider == "MSSQL2008")
            {
                MSSQLDataLoader GenericData = new MSSQLDataLoader();

                DataConnector = GenericData;
            }
            else if (StorageProvider == "MSSQL7")
            {
                MSSQLDataLoader GenericData = new MSSQLDataLoader();

                DataConnector = GenericData;
            }*/
            else if (StorageProvider == "SQLite")
                //Allow for fallback when VisionData isn't set
            {
                SQLiteLoader GenericData = new SQLiteLoader();

                DataConnector = GenericData;
            }

            List<IVisionDataPlugin> Plugins = VisionModuleLoader.PickupModules<IVisionDataPlugin>();
            foreach (IVisionDataPlugin plugin in Plugins)
            {
                try
                {
                    plugin.Initialize(DataConnector == null ? null : DataConnector.Copy(), source, simBase,
                                      ConnectionString);
                }
                catch (Exception ex)
                {
                    if (MainConsole.Instance != null)
                        MainConsole.Instance.Warn("[DataService]: Exception occurred starting data plugin " +
                                                  plugin.Name + ", " + ex.ToString());
                }
            }
        }

        public void Initialize(IConfigSource source, IRegistryCore simBase, List<Type> types)
        {
            IConfig m_config = source.Configs["VisionData"];
            if (m_config != null)
            {
                StorageProvider = m_config.GetString("StorageProvider", StorageProvider);
                ConnectionString = m_config.GetString("ConnectionString", ConnectionString);
            }

            IGenericData DataConnector = null;
            if (StorageProvider == "MySQL")
                //Allow for fallback when VisionData isn't set
            {
                MySQLDataLoader GenericData = new MySQLDataLoader();

                DataConnector = GenericData;
            }
                /*else if (StorageProvider == "MSSQL2008")
            {
                MSSQLDataLoader GenericData = new MSSQLDataLoader();

                DataConnector = GenericData;
            }
            else if (StorageProvider == "MSSQL7")
            {
                MSSQLDataLoader GenericData = new MSSQLDataLoader();

                DataConnector = GenericData;
            }*/
            else if (StorageProvider == "SQLite")
                //Allow for fallback when VisionData isn't set
            {
                SQLiteLoader GenericData = new SQLiteLoader();

                DataConnector = GenericData;
            }

            foreach (Type t in types)
            {
                List<dynamic> Plugins = VisionModuleLoader.PickupModules(t);
                foreach (dynamic plugin in Plugins)
                {
                    try
                    {
                        plugin.Initialize(DataConnector.Copy(), source, simBase, ConnectionString);
                    }
                    catch (Exception ex)
                    {
                        if (MainConsole.Instance != null)
                            MainConsole.Instance.Warn("[DataService]: Exception occurred starting data plugin " +
                                                      plugin.Name + ", " + ex.ToString());
                    }
                }
            }
        }
    }
}