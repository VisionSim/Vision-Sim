/*
 * Copyright (c) Contributors, http://vision-sim.org/, http://whitecore-sim.org/, http://aurora-sim.org, http://opensimulator.org/, http://whitecore-sim.org/, http://aurora-sim.org, http://opensimulator.org/
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
using System.IO;
using Vision.Framework.Servers.HttpServer.Implementation;
using Vision.Framework.Servers.HttpServer.Interfaces;

namespace Vision.Framework.Servers.HttpServer
{
    public abstract class BaseRequestHandler : IStreamedRequestHandler
    {
        protected readonly string m_httpMethod;

        protected readonly string m_path;

        protected BaseRequestHandler(string httpMethod, string path)
        {
            m_httpMethod = httpMethod;
            m_path = path;
        }

        public virtual string ContentType
        {
            get { return "application/xml"; }
        }

        public virtual string HttpMethod
        {
            get { return m_httpMethod; }
        }

        public virtual string Path
        {
            get { return m_path; }
        }

        public string GetBodyAsString(Stream request)
        {
            StreamReader sr = new StreamReader(request);
            string body = sr.ReadToEnd();
            sr.Close();
            body = body.Trim();
            return body;
        }

        protected string GetParam(string path)
        {
            if (CheckParam(path))
            {
                return path.Substring(m_path.Length);
            }

            return String.Empty;
        }

        protected bool CheckParam(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return false;
            }

            return path.StartsWith(Path);
        }

        protected string[] SplitParams(string path)
        {
            string param = GetParam(path);

            return param.Split(new char[] {'/', '?', '&'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public abstract byte[] Handle(string path, Stream request,
                                      OSHttpRequest httpRequest, OSHttpResponse httpResponse);
    }
}