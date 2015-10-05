/*
 * Copyright (c) Contributors, http://vision-sim.org/, http://whitecore-sim.org/, http://aurora-sim.org, http://opensimulator.org/, http://aurora-sim.org/
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
using Vision.DataManager.Migration;
using Vision.Framework.Utilities;

namespace Base.Currency
{
    public class CurrencyMigrator_3 : Migrator
    {
        public CurrencyMigrator_3()
        {
            Version = new Version(0, 0, 3);
            MigrationName = "BaseCurrency";

            schema = new List<SchemaDefinition>();

            AddSchema("currency", ColDefs(
                ColDef("PrincipalID", ColumnTypes.String50),
                ColDef("Amount", ColumnTypes.Integer30),
                ColDef("LandInUse", ColumnTypes.Integer30),
                ColDef("Tier", ColumnTypes.Integer30),
                ColDef("IsGroup", ColumnTypes.TinyInt1),
                new ColumnDefinition
                    {
                        Name = "StipendsBalance",
                        Type = new ColumnTypeDef
                                   {
                                       Type = ColumnType.Integer,
                                       Size = 11,
                                       defaultValue = "0"
                                   }
                    }
                                             ),
                      IndexDefs(
                          IndexDef(new string[1] {"PrincipalID"}, IndexType.Primary)
                          ));

            // Currency Transaction Logs
            AddSchema("currency_history", ColDefs(
                ColDef("TransactionID", ColumnTypes.String36),
                ColDef("Description", ColumnTypes.String128),
                ColDef("FromPrincipalID", ColumnTypes.String36),
                ColDef("FromName", ColumnTypes.String128),
                ColDef("ToPrincipalID", ColumnTypes.String36),
                ColDef("ToName", ColumnTypes.String128),
                ColDef("Amount", ColumnTypes.Integer30),
                ColDef("TransType", ColumnTypes.Integer11),
                ColDef("Created", ColumnTypes.Integer30),
                ColDef("ToBalance", ColumnTypes.Integer30),
                ColDef("FromBalance", ColumnTypes.Integer30),
                ColDef("FromObjectName", ColumnTypes.String50),
                ColDef("ToObjectName", ColumnTypes.String50),
                ColDef("RegionID", ColumnTypes.Char36)),
                IndexDefs(
                    IndexDef(new string[1] { "TransactionID" }, IndexType.Primary)
                    ));

            // this is actually used for all purchases now.. a better name would be _purchased
            AddSchema("currency_purchased", ColDefs(
                ColDef("PurchaseID", ColumnTypes.String36),
                ColDef("PrincipalID", ColumnTypes.String36),
                ColDef("IP", ColumnTypes.String64),
                ColDef("Amount", ColumnTypes.Integer30),
                ColDef("RealAmount", ColumnTypes.Integer30),
                ColDef("Created", ColumnTypes.Integer30),
                ColDef("Updated", ColumnTypes.Integer30)),
                IndexDefs(
                    IndexDef(new string[1] { "PurchaseID" }, IndexType.Primary)
                ));
        }

        protected override void DoCreateDefaults(IDataConnector genericData)
        {
            EnsureAllTablesInSchemaExist(genericData);
        }

        protected override bool DoValidate(IDataConnector genericData)
        {
            return TestThatAllTablesValidate(genericData);
        }

        protected override void DoMigrate(IDataConnector genericData)
        {
            DoCreateDefaults(genericData);
        }

        protected override void DoPrepareRestorePoint(IDataConnector genericData)
        {
            CopyAllTablesToTempVersions(genericData);
        }
    }
}