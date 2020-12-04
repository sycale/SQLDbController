using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ConfigManager {
    public class Config {

        static string CurrentDirectory = Directory.GetCurrentDirectory ();
        static string ConfigDirectory = Path.Combine (CurrentDirectory, "ConfigManager", "Config");
        static string jsonFilePath = @"appsettings.json";
        static string xmlFilePath = @"config.xml";

        public class JsonConfig {
            public string dest { get; set; }
            public string password { get; set; }
            public string dataSource { get; set; }
        }

        [Serializable, XmlRoot ("config")]
        public class XMLConfig {
            [System.Xml.Serialization.XmlElementAttribute ("password")]
            public string password { get; set; }

            [System.Xml.Serialization.XmlElementAttribute ("dest")]
            public string dest { get; set; }

            [System.Xml.Serialization.XmlElementAttribute ("dataSource")]
            public string dataSource { get; set; }

            public XMLConfig () { }

        }

        public static XMLConfig Deserialize () {
            XMLConfig conf = null;
            XmlSerializer serializer = new XmlSerializer (typeof (XMLConfig));
            StreamReader reader = new StreamReader (Path.Combine (ConfigDirectory, xmlFilePath));
            conf = (XMLConfig) serializer.Deserialize (reader);
            reader.Close ();

            return conf;

        }
        public static dynamic DetectConfig () {
            bool JSON = Convert.ToBoolean (Directory.GetFiles (ConfigDirectory, "*.json").Length);
            bool XML = Convert.ToBoolean (Directory.GetFiles (ConfigDirectory, "*.xml").Length);

            if (JSON) {
                return JsonConvert.DeserializeObject<JsonConfig> (File.ReadAllText (Path.Combine (ConfigDirectory, jsonFilePath)));
            } else if (XML) {
                return Deserialize ();
            } else throw new Exception ("no config file has been found");
        }
    }
}