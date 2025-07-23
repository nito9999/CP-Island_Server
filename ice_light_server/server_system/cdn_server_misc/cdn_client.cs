using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static ice_light_server.server_system.route_system;
using static System.Net.Mime.MediaTypeNames;

namespace ice_light_server.server_system.cdn_server_misc
{
    internal class cdn_client
    {
        public class SessionRequest
        {
            public string PublicKeyModulus { get; set; }
            public string PublicKeyExponent { get; set; }
            public string UserId { get; set; }
        }

        [Route("/mix/state", port: 2021)]
        public static object mix_state()
        {
            return new
            {
                Users = new List<object>(),
                Friendships = new List<object>(),
                FriendshipInvitations = new List<object>(),
                Alerts = new List<object>(),
                Status = "OK"
            };
        }

        [Route("/mix/registration/text", port: 2021)]
        public static string mix_registration_text()
        {
            //return "{\"Status\":\"OK\",\"RegistrationText\":[{\"TextCode\":\"gtou_ppv2_proxy_create\",\"Text\":\"By creating an account, I agree to the <a target=\\\"_blank\\\" href=\\\"https://disneytermsofuse.com/english/\\\">Terms of Use</a> and acknowledge the <a target=\\\"_blank\\\" href=\\\"https://privacy.thewaltdisneycompany.com/en/current-privacy-policy/\\\">Privacy Policy</a>.\"}]}";
            
            return JsonConvert.SerializeObject(new
            {
                RegistrationText = new List<object>
                {
                    new
                    {
                        TextCode = "gtou_ppv2_proxy_create",
                        Text = $"Welcome to ice_light_server ver {Program.Version}."
                    }
                },
                Status = "OK"
            });
        }
        private static Random random = new Random();

        [Route("/mix/session/user", port: 2021)]
        public static string mix_session_user(SessionRequest session)
        {
            api.server.Rsa_Encrytion rsa = new api.server.Rsa_Encrytion();
            byte[] publicKeyModulus = Convert.FromBase64String(session.PublicKeyModulus);
            byte[] publicKeyExponent = Convert.FromBase64String(session.PublicKeyExponent);
            RSAParameters publicKey = new RSAParameters
            {
                Modulus = publicKeyModulus,
                Exponent = publicKeyExponent
            };
            string tmp = "ice_light-abcdzyxw-data-12345678";
            byte[] plaintext = rsa.Encrypt(Encoding.UTF8.GetBytes(tmp), publicKey);
            return JsonConvert.SerializeObject(new
            {
                HashedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(session.UserId)),
                EncryptedSymmetricKey = plaintext,
                SessionId = random.NextInt64(),
                Status = "OK"
            });
        }

        [Route("/cdn/jgc/v5/client/*/configuration/site", port: 2021)]
        public static string jgc_v5_client_configuration_site()
        {
            var tmp = new
            {
                data = new
                {
                    compliance = new
                    {
                        defaultAgeBand = "ADULT",
                        defaultCountryCode = "US",
                        ageBands = new Dictionary<string, object>
                        {
                            {
                                "ADULT",
                                new 
                                {
                                    country = "US",
                                    minAge = 0,
                                    maxAge = 9999,
                                    legalRequirements = new 
                                    {
                                        cookiePermissions = new
                                        {
                                            FUNCTIONALITY = "ALLOWED",
                                            PERFORMANCE = "ALLOWED",
                                            STRICTLY_NECESSARY = "ALLOWED",
                                            TARGETING_ADVERTISING = "OPT_IN"
                                        },
                                        marketingPermissions = new
                                        {
                                            LOB = "OPT_IN",
                                            FOB = "OPT_IN",
                                            EMAIL_NEWSLETTER = "OPT_IN",

                                        }
                                    },
                                    UPDATE = new Dictionary<string, object>
                                    {
                                        {
                                            "username",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "password",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "dateOfBirth",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "firstName",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "lastName",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "email",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "parentEmail",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        }
                                    },
                                    CREATE = new Dictionary<string, object>
                                    {
                                        {
                                            "username",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "password",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "dateOfBirth",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "firstName",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "lastName",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "email",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        },
                                        {
                                            "parentEmail",
                                            new
                                            {
                                                editable = "EDITABLE",
                                                required = true
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        embargoedCountries = new List<string>
                        {
                            "CU",
                            "IR",
                            "KP",
                            "SD",
                            "SY"
                        },
                    },
                    marketing = new Dictionary<string, object>
                    {
                        {
                            "ADULT",
                            new
                            {
                                CREATE = new Dictionary<string, object>(),
                                UPDATE = new Dictionary<string, object>(),
                                PARTIAL = new Dictionary<string, object>(),
                            }
                        }
                    },
                    legal = new Dictionary<string, object>
                    {
                        {
                            "ADULT",
                            new
                            {
                                documentTypeOrder = new List<string>
                                {
                                    "combined",
                                    "tou",
                                    "privacy",
                                    "cookie"
                                },
                                documents = new Dictionary<string, object>
                                {
                                    {
                                        "GTOU",
                                        new
                                        {
                                            type = "tou",
                                            displayCheckbox = false,
                                            displayStyle = "separate"
                                        }
                                    },
                                    {
                                        "ppV2",
                                        new
                                        {
                                            type = "privacy",
                                            displayCheckbox = false,
                                            displayStyle = "separate"
                                        }
                                    }
                                },
                                CREATE = new List<object>
                                {
                                    new
                                    {
                                        key = "gtou_ppv2_proxy",
                                        displayCheckbox = false,
                                        type = "grouping",
                                        groupingRule = "GENERIC",
                                        sortingTag = "tou",
                                    }
                                }
                            }
                        }
                    }
                },
                error = (object)null
            };
            string tmp2 = api.files.ReadfronResource("ice_light_server.Resources.data.configuration.site.json");
            return tmp2;
        }
    }
}
