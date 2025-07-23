using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static ice_light_server.server_system.route_system;

namespace ice_light_server.server_system.api_server_misc
{
    internal class api_client
    {
        [Route("/register/jgc/v5/client/*/validate", port: 2020)]
        public static object register_jgc_v5_client_validate()
        {
            var tmp = new
            {
                error = (object)null
            };
            return tmp;
        }
        [Route("/register/jgc/v5/client/*/api-key", port: 2020)]
        public static object register_jgc_v5_client_api_key(HttpListenerContext context)
        {
            var tmp = new
            {
                error = (object)null
            };
            context.Response.AddHeader("api-key", "ffffffffff");
            return tmp;
        }
        public class login_json
        {
            public string loginValue { get; set; }
            public string password { get; set; }
        }
        [Route("/register/jgc/v5/client/*/guest/login", port: 2020)]
        public static object register_jgc_v5_client_guest_login(login_json login)
        {
            var tmp = new
            {
                data = new
                {
                    etag = "ddddddddd", // idk what is this for
                    profile = new
                    {
                        ageBand = "ADULT",
                        ageBandAssumed = true,
                        dateOfBirth = DateTime.MinValue.ToString(),
                        email = "no@no.no",
                        parentEmail = "no@no.no",
                        parentEmailVerified = true,
                        swid = "no@no.no",
                        emailVerified = true,
                        firstName = "ice_light",
                        middleName = "ice_light",
                        lastName = "ice_light",
                        username = login.loginValue,
                        languagePreference = "en",
                        region = "us",
                        status = "ACTIVE",
                    },
                    displayName = new
                    {
                        displayName = login.loginValue,
                        proposedDisplayName = login.loginValue,
                        moderatedStatusDate = "",
                        proposedStatus = "NONE",
                    },
                    token = new
                    {
                        access_token = "dddddddd",
                        refresh_token = "dddddddd",
                        ttl = 0,
                        swid = "dddddddd",
                    } 
                },
                error = (object)null
            };
            return tmp;
        }
        [Route("/register/jgc/v5/client/*/guest/register", port: 2020)]
        public static object register_jgc_v5_client_guest_register()
        {
            string loginValue = "fffffffff";
            var tmp = new
            {
                data = new
                {
                    etag = "ddddddddd", // idk what is this for
                    profile = new
                    {
                        ageBand = "ADULT",
                        ageBandAssumed = true,
                        dateOfBirth = DateTime.MinValue.ToString(),
                        email = "no@no.no",
                        parentEmail = "no@no.no",
                        parentEmailVerified = true,
                        swid = "no@no.no",
                        emailVerified = true,
                        firstName = "ice_light",
                        middleName = "ice_light",
                        lastName = "ice_light",
                        username = loginValue,
                        languagePreference = "en",
                        region = "us",
                        status = "ACTIVE",
                    },
                    displayName = new
                    {
                        displayName = loginValue,
                        proposedDisplayName = loginValue,
                        moderatedStatusDate = "",
                        proposedStatus = "NONE",
                    },
                    token = new
                    {
                        access_token = "dddddddd",
                        refresh_token = "dddddddd",
                        ttl = 0,
                        swid = "dddddddd",
                    }
                },
                error = (object)null
            };
            return tmp;
        }
    }
}
