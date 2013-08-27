/*
Copyright (C) 2010-2013 David Mitchell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;

namespace MCForge {

    public static class Heart {

        /// <summary>
        /// The max number of retries it runs for a beat
        /// </summary>
        public const int MAX_RETRIES = 3;

        /// <summary>
        /// Gets or sets a value indicating whether this instance can beat.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can beat; otherwise, <c>false</c>.
        /// </value>
        public static bool CanBeat { get; set; }

        static Timer Timer;
        static object Lock = new object();


        private readonly static IBeat[] Beats = {

            //Keep in this order.
            new MinecraftBeat(),
            new WOMBeat(),
            new MCForgeBeat(),
        };




        static Heart() {
            new Thread(new ThreadStart(() => {
                Timer = new Timer(OnBeat, null,
#if DEBUG
                6000, 6000
#else
                45000, 45000
#endif
                );
            })).Start();
        }

        private static void OnBeat(object state) {
            for ( int i = 0; i < Beats.Length; i++ ) {
                if ( Beats[i].Persistance )
                    Pump(Beats[i]);
            }
        }



        /// <summary>
        /// Inits this instance.
        /// </summary>
        public static void Init() {
            if ( Server.logbeat ) {
                if ( !File.Exists("heartbeat.log") ) {
                    using ( File.Create("heartbeat.log") ) { }
                }
            }
            
            CanBeat = true;
            
            for ( int i = 0; i < Beats.Length; i++ )
                Pump(Beats[i]);
        }

        /// <summary>
        /// Pumps the specified beat.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <returns></returns>
        public static void Pump(IBeat beat) {
            
            if(!CanBeat)
                return;

            byte[] data = Encoding.ASCII.GetBytes(beat.Prepare());

            for ( int i = 0; i < MAX_RETRIES; i++ ) {
                try {
                    var request = WebRequest.Create(beat.URL) as HttpWebRequest;
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    request.Timeout = 15000;
                    request.ContentLength = data.Length;

                    using ( var writer = request.GetRequestStream() ) {
                        writer.Write(data, 0, data.Length);

                        if ( Server.logbeat )
                            Server.s.Log("Beat " + beat.ToString() + " was sent");
                    }

                    using ( var reader = new StreamReader(request.GetResponse().GetResponseStream()) ) {
                        string read = reader.ReadToEnd().Trim();
                        beat.OnResponse(read);

                        if ( Server.logbeat )
                            Server.s.Log("Beat: \"" + read + "\" was recieved");
                    }
                    return;
                }
                catch {
                    continue;
                }
            }

            if ( Server.logbeat )
                Server.s.Log("Beat: " + beat.ToString() + " failed.");
        }

        /// <summary>
        /// Encodes the URL.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>An encoded url</returns>
        public static string EncodeUrl(string input) {
            StringBuilder output = new StringBuilder();
            for ( int i = 0; i < input.Length; i++ ) {
                if ( ( input[i] >= '0' && input[i] <= '9' ) ||
                    ( input[i] >= 'a' && input[i] <= 'z' ) ||
                    ( input[i] >= 'A' && input[i] <= 'Z' ) ||
                    input[i] == '-' || input[i] == '_' || input[i] == '.' || input[i] == '~' ) {
                    output.Append(input[i]);
                }
                else if ( Array.IndexOf<char>(ReservedChars, input[i]) != -1 ) {
                    output.Append('%').Append(( (int)input[i] ).ToString("X"));
                }
            }
            return output.ToString();
        }

        public static readonly char[] ReservedChars = { ' ', '!', '*', '\'', '(', ')', ';', ':', '@', '&', '=', '+', '$', ',', '/', '?', '%', '#', '[', ']' };
    }

}
